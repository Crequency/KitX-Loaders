cmake_minimum_required(VERSION 3.24)
project(KitX_Loader_Cpp_Self)

set(CMAKE_CXX_STANDARD 20)

add_subdirectory(lib/KitX-Contracts/KitX.Contract.Cpp)

link_directories(${PROJECT_SOURCE_DIR}/lib/KitX-Contracts/KitX.Contract.Cpp)
include_directories(${PROJECT_SOURCE_DIR}/lib/KitX-Contracts/KitX.Contract.Cpp)

link_directories(${PROJECT_SOURCE_DIR}/lib/KitX-Contracts/KitX.Contract.Cpp/lib/CppEnhanced)
include_directories(${PROJECT_SOURCE_DIR}/lib/KitX-Contracts/KitX.Contract.Cpp/lib/CppEnhanced/include)

add_library(
        KitX_Loader_Cpp_Self
        include/loader.h source/loader.cpp
        include/modules/ip_address/ip_address.h source/modules/ip_address/ip_address.cpp
)
