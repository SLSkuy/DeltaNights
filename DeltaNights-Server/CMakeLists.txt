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
  Logger/logger.cpp Logger/logger.h
  NetworkProcess/udpendpoint.cpp NetworkProcess/udpendpoint.h
  NetworkProcess/networkdispatcher.cpp NetworkProcess/networkdispatcher.h
  BattleProcess/gameroom.cpp BattleProcess/gameroom.h
  BattleProcess/gameroommanager.cpp BattleProcess/gameroommanager.h
  BattleProcess/playerentity.cpp BattleProcess/playerentity.h
  GameData/characterprops.cpp GameData/characterprops.h
  NetworkProcess/tcpendpoint.h NetworkProcess/tcpendpoint.cpp
  gameserver.h gameserver.cpp
  GameData/building.h GameData/building.cpp
  GameData/gamemap.h GameData/gamemap.cpp
  CollisionSystem/collider.h CollisionSystem/collider.cpp
  CollisionSystem/collisionsystem.h CollisionSystem/collisionsystem.cpp
  ClientManage/clientinfo.cpp ClientManage/clientinfo.h
  ClientManage/clientmanager.cpp ClientManage/clientmanager.h
  ClientManage/playerinfo.cpp ClientManage/playerinfo.h
  ClientManage/playerinfomanager.cpp ClientManage/playerinfomanager.h
  GameEvent/AckPackage.pb.cc GameEvent/AckPackage.pb.h
  GameEvent/BattleSyncPackage.pb.cc GameEvent/BattleSyncPackage.pb.h
  GameEvent/LobbySyncPackage.pb.cc GameEvent/LobbySyncPackage.pb.h
  GameEvent/SyncPackage.pb.cc GameEvent/SyncPackage.pb.h
  GameEvent/UnityMath.pb.cc GameEvent/UnityMath.pb.h
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
