#pragma once
#include <string>

struct ToDoItem {
    int id;
    std::string description;
    std::string status; // "Pending" or "Completed"
};
