// ToDoListServerApp.cpp : Defines the entry point for the application.
//

#include "ToDoListServerApp.h"
#include <grpcpp/grpcpp.h>
#include "../Server/ToDoServiceImpl.h"

using namespace std;

int main() {
    std::string server_address("0.0.0.0:50051");
    ToDoServiceImpl service;

    grpc::ServerBuilder builder;
    builder.AddListeningPort(server_address, grpc::InsecureServerCredentials());
    builder.RegisterService(&service);

    std::unique_ptr<grpc::Server> server(builder.BuildAndStart());
    std::cout << "Server listening on " << server_address << std::endl;

    server->Wait();
    return 0;
}