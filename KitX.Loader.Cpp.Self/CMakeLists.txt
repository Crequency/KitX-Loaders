cmake_minimum_required(VERSION 3.23)
project(KitX_Loader_Cpp_Self)

set(CMAKE_CXX_STANDARD 20)

add_library(KitX_Loader_Cpp_Self loader.cpp kitx_contract.h)
