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

# KitX.Contract.Cpp library linker

macro(cr_KitX_Contract)
    link_directories("${PROJECT_SOURCE_DIR}/lib/KitX-Contracts/KitX.Contract.Cpp")
    include_directories("${PROJECT_SOURCE_DIR}/lib/KitX-Contracts/KitX.Contract.Cpp/include")
endmacro()
