#include "ToDoManager.h"

ToDoItem ToDoManager::AddItem(const std::string& description) {
    std::lock_guard<std::mutex> lock(mtx_);
    ToDoItem item{ nextId_++, description, "Pending" };
    items_[item.id] = item;
    return item;
}

ToDoItem ToDoManager::UpdateStatus(int id) {
    std::lock_guard<std::mutex> lock(mtx_);
    auto it = items_.find(id);
    if (it != items_.end()) {
        it->second.status = (it->second.status == "Pending") ? "Completed" : "Pending";
        return it->second;
    }
    return { -1, "", "" }; // invalid
}

std::vector<ToDoItem> ToDoManager::GetList() {
    std::lock_guard<std::mutex> lock(mtx_);
    std::vector<ToDoItem> list;
    for (auto& kv : items_) {
        list.push_back(kv.second);
    }
    return list;
}