#pragma once
#include "ToDoItem.h"
#include <unordered_map>
#include <vector>
#include <mutex>

class ToDoManager {
public:
    ToDoItem AddItem(const std::string& description);
    ToDoItem UpdateStatus(int id);
    std::vector<ToDoItem> GetList();

private:
    std::unordered_map<int, ToDoItem> items_;
    int nextId_ = 1;
    std::mutex mtx_;
};