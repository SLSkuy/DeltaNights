cmake_minimum_required(VERSION 3.30)

project(DeltaNights-Server LANGUAGES CXX)

set(CMAKE_AUTOUIC ON)
set(CMAKE_AUTOMOC ON)
set(CMAKE_AUTORCC ON)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

find_package(QT NAMES Qt6 REQUIRED COMPONENTS Core Network)
find_package(Qt${QT_VERSION_MAJOR} REQUIRED COMPONENTS Core Network)

# 指定protobuf头文件位置
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/protobuf/include)

add_executable(DeltaNights-Server
  main.cpp
  dnserver.h dnserver.cpp
  logger.h logger.cpp
  msgsender.h msgsender.cpp
)

# 指定protobuf库位置
target_link_directories(DeltaNights-Server PRIVATE ${CMAKE_CURRENT_SOURCE_DIR}/protobuf/lib)

target_link_libraries(DeltaNights-Server
  Qt${QT_VERSION_MAJOR}::Core
  Qt${QT_VERSION_MAJOR}::Network
  protobuf
)

include(GNUInstallDirs)
install(TARGETS DeltaNights-Server
    LIBRARY DESTINATION ${CMAKE_INSTALL_LIBDIR}
    RUNTIME DESTINATION ${CMAKE_INSTALL_BINDIR}
)
