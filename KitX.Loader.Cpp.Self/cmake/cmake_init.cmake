# cmake_init.cmake
# Copyright (C) 2023 ChenPi11.
# This file is part of the cppp reiconv library, it is based on GNU LIBICONV.
#
# The cppp reiconv is free software; you can redistribute it and/or
# modify it under the terms of the GNU Lesser General Public License as
# published by the Free Software Foundation; either version 2 of the
# License, or (at your option) any later version.
#
# The cppp reiconv is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
# Lesser General Public License for more details.
#
# You should have received a copy of the GNU Lesser General Public License
# along with the cppp reiconv; see the file COPYING.LIB.  If not,
# see <https://www.gnu.org/licenses/>.

# defines like autoconf.

set(srcdir ${CMAKE_CURRENT_SOURCE_DIR})
set(auxdir ${srcdir}/cmake)
set(outdir ${CMAKE_BINARY_DIR})

set(prefix ${CMAKE_INSTALL_PREFIX})
set(bindir ${prefix}/bin)
set(libdir ${prefix}/lib)
set(includedir ${prefix}/include)
