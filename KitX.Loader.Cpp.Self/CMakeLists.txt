cmake_minimum_required(VERSION 3.24)
project(KitX_Loader_Cpp_Self)

set(CMAKE_CXX_STANDARD 20)

file(GLOB SOURCE_FILES "${CMAKE_CURRENT_SOURCE_DIR}/source/*.cpp")
file(GLOB MODULES_FILES "${CMAKE_CURRENT_SOURCE_DIR}/source/modules/*.cpp")

include_directories(${CMAKE_CURRENT_SOURCE_DIR}/include)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/include/modules)

#add_subdirectory(lib/KitX-Contracts/KitX.Contract.Cpp)

link_directories(${PROJECT_SOURCE_DIR}/lib/KitX-Contracts/KitX.Contract.Cpp)
include_directories(${PROJECT_SOURCE_DIR}/lib/KitX-Contracts/KitX.Contract.Cpp/include)

link_directories(${PROJECT_SOURCE_DIR}/lib/KitX-Contracts/KitX.Contract.Cpp/lib/CppEnhanced)
include_directories(${PROJECT_SOURCE_DIR}/lib/KitX-Contracts/KitX.Contract.Cpp/lib/CppEnhanced/include)

add_library(
        KitX_Loader_Cpp_Self
        SHARED
        ${SOURCE_FILES}
        ${MODULES_FILES}
)
