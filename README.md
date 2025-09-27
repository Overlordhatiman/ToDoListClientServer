
# ToDo List Client-Server Application (C++ gRPC Server + C# WPF Client)

This project is a simple client-server ToDo list application:

* Server: Written in C++20, exposing APIs via gRPC.

* Client: Written in C# WPF (MVVM), consuming the server API over gRPC.

* Protocol: Real-time updates are pushed from the server to all connected clients.

## 📦 Dependencies

You will need the following installed:

[vcpkg](https://github.com/microsoft/vcpkg) (C++ dependency manager)

[CMake](https://cmake.org/) (build system)

[Protobuf & gRPC](https://grpc.io/) (installed via vcpkg)

⚙️ Build & Run Instructions
### 1. Clone the repository
```
git clone https://github.com/Overlordhatiman/ToDoListClientServer.git
cd ToDoListClientServer
```

### 2. Install dependencies via vcpkg

If you don’t already have vcpkg installed:

```
git clone https://github.com/microsoft/vcpkg.git
cd vcpkg
.\bootstrap-vcpkg.bat
```

Integrate vcpkg with CMake:
```
vcpkg integrate install
```

Install gRPC + Protobuf:
```
vcpkg install grpc protobuf
```

### 3. Build the C++ Server

Go to the ToDoListServerApp folder:
```
cd ToDoListServerApp
```

Configure build:
```
cmake -B build -S . -DCMAKE_TOOLCHAIN_FILE=D:/vcpkg/scripts/buildsystems/vcpkg.cmake
```

Compile:
```
cmake --build build
```

Run the server:
Move to the folder
```
\build\Debug
```
And run
```
ToDoListServerApp.exe
```

You should see:
```
Server listening on 0.0.0.0:50051
```

### 4. Run the C# WPF Client

 #### 1. Open ToDoListClientApp.sln in Visual Studio.

 #### 2. Press F5 to run.

 #### 3. You can also open bin/Debug/netX/ToDoListClientApp.exe to start multiple clients simultaneously.

## ✅ Usage

#### 1. Start the C++ server (ToDoListServerApp.exe).

#### 2. Start one or more C# clients.

#### 3. Add tasks via one client → they appear in all other clients in real time.

#### 4. Toggle task completion via checkbox → status is updated across all clients.

## 🏗️ Design Explanation
Why gRPC?
 I chose gRPC over raw TCP/UDP because:

 * Strongly typed contract (.proto file) → avoids protocol mismatches.

 * Cross-language support → C++ server, C# client, and could easily extend to Java, Python, etc.

 * Built-in streaming → perfect for pushing real-time updates from server to all clients.

 * HTTP/2 based → multiplexing, efficient transport, bidirectional streaming.

Compared to other RPC solutions (like REST, SOAP, or custom JSON over TCP), gRPC provides:
 * Auto-generated stubs for both C++ and C#.

 * Much less boilerplate code.

 * High performance close to raw sockets, with less complexity.

Why vcpkg?

Initially tried to manage dependencies manually → ran into context/versioning issues with Protobuf and gRPC.

With vcpkg, setup became much faster and more consistent:

 * One command installed Protobuf + gRPC with correct configuration.

 * Automatically integrated into CMake build.

 * Avoids manual downloading, building, and linking.

Why CMake?

 * CMake is the standard build tool for C++ cross-platform projects.

 * Already used in this project → so it was the natural choice.

 * Makes it easy to integrate gRPC/Protobuf auto-generation.

 * Works seamlessly with Visual Studio and vcpkg.

## 🚀 Future Improvements

 * Add persistent storage (SQLite/Postgres) instead of in-memory data.

 * Authentication and user-specific task lists.

 * Docker containerization for server + client deployment.
