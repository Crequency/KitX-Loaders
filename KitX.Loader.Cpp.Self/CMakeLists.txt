# CMakeLists.txt
# Copyright (C) 2023 Crequency.
# This file is part of the KitX.Loader.Cpp.Self.
#
# The KitX.Loader.Cpp.Self is free software; you can redistribute it and/or
# modify it under the terms of the GNU Affero General Public License.
#
# The KitX.Loader.Cpp.Self is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
# Affero General Public License for more details.
#
# You should have received a copy of the GNU Affero General Public License
# along with the KitX.Loader.Cpp.Self; see the file LICENSE.  If not,
# see <https://www.gnu.org/licenses/>.

# This is KitX.Loader.Cpp.Self CMake build script.

cmake_minimum_required(VERSION 3.1)
project(KitX.Loader.Cpp.Self LANGUAGES CXX
        VERSION 1.0.0
        DESCRIPTION "KitX Self Loader for C++"
        HOMEPAGE_URL "https://github.com/Crequency/KitX" )

# Set C++ standard
set(CMAKE_CXX_STANDARD 20)

# Init CMake build
include("${CMAKE_CURRENT_SOURCE_DIR}/cmake/cmake_init.cmake")

# Include requirements
include("${srcdir}/cmake/cpp_enhanced.cmake")
include("${srcdir}/cmake/kitx_contract.cmake")

#add_subdirectory(lib/KitX-Contracts/KitX.Contract.Cpp)

# Link requirements
cr_KitX_Contract()
cr_CppEnhanced()

# Add includes
include_directories("${srcdir}/include")
include_directories("${srcdir}/include/modules")

# Add sources
file(GLOB SOURCE_FILES "${srcdir}/src/*.cpp")
file(GLOB MODULES_FILES "${srcdir}/src/modules/*.cpp")

add_library(KitX.Loader.Cpp.Self SHARED
        ${SOURCE_FILES} ${MODULES_FILES}
)
