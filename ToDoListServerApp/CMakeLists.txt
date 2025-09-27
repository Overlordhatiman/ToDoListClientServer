cmake_minimum_required(VERSION 3.15)

project(ToDoListServerApp LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

find_package(protobuf CONFIG REQUIRED)
find_package(gRPC CONFIG REQUIRED)

# Proto file
set(PROTO_FILE ${CMAKE_CURRENT_SOURCE_DIR}/todo.proto)

# Where generated files will go
set(GENERATED_PROTO_DIR ${CMAKE_CURRENT_BINARY_DIR}/generated)
file(MAKE_DIRECTORY ${GENERATED_PROTO_DIR})

# Run protoc manually from CMake
add_custom_command(
    OUTPUT ${GENERATED_PROTO_DIR}/todo.pb.cc ${GENERATED_PROTO_DIR}/todo.pb.h
           ${GENERATED_PROTO_DIR}/todo.grpc.pb.cc ${GENERATED_PROTO_DIR}/todo.grpc.pb.h
    COMMAND protobuf::protoc
    ARGS --proto_path=${CMAKE_CURRENT_SOURCE_DIR}
         --cpp_out=${GENERATED_PROTO_DIR}
         --grpc_out=${GENERATED_PROTO_DIR}
         --plugin=protoc-gen-grpc=$<TARGET_FILE:gRPC::grpc_cpp_plugin>
         ${PROTO_FILE}
    DEPENDS ${PROTO_FILE}
    COMMENT "Generating gRPC and Protobuf sources from ${PROTO_FILE}"
    VERBATIM
)

set(SOURCES
    ToDoListServerApp/ToDoListServerApp.cpp
    Server/ToDoServiceImpl.cpp
    Models/ToDoManager.cpp
    ${GENERATED_PROTO_DIR}/todo.pb.cc
    ${GENERATED_PROTO_DIR}/todo.grpc.pb.cc
)

set(HEADERS
    ToDoListServerApp/ToDoListServerApp.h
    Server/ToDoServiceImpl.h
    Models/ToDoManager.h
    Models/ToDoItem.h
    ${GENERATED_PROTO_DIR}/todo.pb.h
    ${GENERATED_PROTO_DIR}/todo.grpc.pb.h
)

add_executable(ToDoListServerApp ${SOURCES} ${HEADERS})

target_include_directories(ToDoListServerApp PRIVATE
    ${CMAKE_CURRENT_SOURCE_DIR}
    ${GENERATED_PROTO_DIR}
)

target_link_libraries(ToDoListServerApp PRIVATE
    gRPC::grpc++
    protobuf::libprotobuf
)