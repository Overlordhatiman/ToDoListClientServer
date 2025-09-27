#pragma once
#include "todo.grpc.pb.h"
#include "Models/ToDoManager.h"

class ToDoServiceImpl final : public todo::ToDoService::Service {
public:
    grpc::Status AddItem(grpc::ServerContext* context,
        const todo::AddItemRequest* request,
        todo::AddItemResponse* response) override;

    grpc::Status UpdateStatus(grpc::ServerContext* context,
        const todo::UpdateStatusRequest* request,
        todo::UpdateStatusResponse* response) override;

    grpc::Status GetList(grpc::ServerContext* context,
        const todo::Empty* request,
        grpc::ServerWriter<todo::ToDoItem>* writer) override;

private:
    ToDoManager manager_;

    struct Subscriber {
        std::mutex m;
        std::condition_variable cv;
        std::deque<todo::ToDoItem> queue;
        bool active = true;
    };

    std::mutex subscribers_mtx_;
    std::vector<std::shared_ptr<Subscriber>> subscribers_;

    void BroadcastItem(const todo::ToDoItem& item);
};