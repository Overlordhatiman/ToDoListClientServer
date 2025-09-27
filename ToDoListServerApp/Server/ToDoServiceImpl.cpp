#include "ToDoServiceImpl.h"
#include <algorithm>
#include <chrono>
#include <iostream>

grpc::Status ToDoServiceImpl::AddItem(grpc::ServerContext* context,
    const todo::AddItemRequest* request,
    todo::AddItemResponse* response) {
    auto itemModel = manager_.AddItem(request->description());

    todo::ToDoItem protoItem;
    protoItem.set_id(itemModel.id);
    protoItem.set_description(itemModel.description);
    protoItem.set_status(itemModel.status);

    auto* respItem = response->mutable_item();
    respItem->CopyFrom(protoItem);

    BroadcastItem(protoItem);

    return grpc::Status::OK;
}

grpc::Status ToDoServiceImpl::UpdateStatus(grpc::ServerContext* context,
    const todo::UpdateStatusRequest* request,
    todo::UpdateStatusResponse* response) {
    auto itemModel = manager_.UpdateStatus(request->id());

    if (itemModel.id == -1) {
        return grpc::Status(grpc::StatusCode::NOT_FOUND, "Item not found");
    }

    todo::ToDoItem protoItem;
    protoItem.set_id(itemModel.id);
    protoItem.set_description(itemModel.description);
    protoItem.set_status(itemModel.status);

    auto* respItem = response->mutable_item();
    respItem->CopyFrom(protoItem);

    BroadcastItem(protoItem);

    return grpc::Status::OK;
}

grpc::Status ToDoServiceImpl::GetList(grpc::ServerContext* context,
    const todo::Empty* request,
    grpc::ServerWriter<todo::ToDoItem>* writer) {
    auto subscriber = std::make_shared<Subscriber>();

    {
        std::lock_guard<std::mutex> lock(subscribers_mtx_);
        subscribers_.push_back(subscriber);
    }

    {
        auto current = manager_.GetList();
        for (const auto& m : current) {
            todo::ToDoItem msg;
            msg.set_id(m.id);
            msg.set_description(m.description);
            msg.set_status(m.status);
            if (!writer->Write(msg)) {
                goto cleanup;
            }
        }
    }

    while (!context->IsCancelled()) {
        std::unique_lock<std::mutex> lk(subscriber->m);
        subscriber->cv.wait_for(lk, std::chrono::milliseconds(200), [&] {
            return !subscriber->queue.empty() || context->IsCancelled();
            });

        while (!subscriber->queue.empty()) {
            todo::ToDoItem msg = std::move(subscriber->queue.front());
            subscriber->queue.pop_front();
            lk.unlock();

            if (!writer->Write(msg)) {
                goto cleanup;
            }

            lk.lock();
        }
    }

cleanup:
    {
        std::lock_guard<std::mutex> lock(subscribers_mtx_);
        subscribers_.erase(std::remove_if(subscribers_.begin(), subscribers_.end(),
            [&](const std::shared_ptr<Subscriber>& s) { return s == subscriber; }),
            subscribers_.end());
    }

    return grpc::Status::OK;
}

void ToDoServiceImpl::BroadcastItem(const todo::ToDoItem& item) {
    std::lock_guard<std::mutex> lock(subscribers_mtx_);
    for (auto& subPtr : subscribers_) {
        if (!subPtr) continue;
        {
            std::lock_guard<std::mutex> lk(subPtr->m);
            subPtr->queue.push_back(item);
        }
        subPtr->cv.notify_one();
    }
}