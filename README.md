# DeltaNights

三脚猫行动：卡丘方舟

> 搜不到、打不赢、撤不了的二次元搜打撤

<div align="center">
  <figure>
    <img src="https://github.com/user-attachments/assets/c437d848-e1e8-4cf1-983b-a0ff06563e1e" 
         width="60%" 
         style="border-radius: 12px; border: 3px solid #ddd;"
         alt="哈基米哦南北绿豆">
    <figcaption><em>欸嘿，洗大锅</em></figcaption>
  </figure>
</div>

---
## 🧩 项目简介（Overview）

DeltaNights：三脚猫行动：卡丘方舟 是一款采用 Unity 开发、风格偏向 二次元动作 + 搜打撤（Search-Fight-Extract） 的多人在线游戏项目。
服务器由 C++ / Qt 构建，客户端以模块化结构设计，便于扩展与多人功能接入。

本仓库包含客户端和服务端的所有核心内容 ，模型等资产需网盘下载导入，链接：xxx.xxx(待定，后续传网盘)

## 📁 项目结构（Project Structure）

### Assets
```
DeltaNights/
├── Assets/                # Unity 主资源目录
│   ├── Animation/         # 动画资源
│   │   ├── AnimatorController/
│   │   └── CharacterAnim/
│   ├── Meshes/            # 模型网格（模型需单独下载避免 Git 体积膨胀）
│   ├── Prefabs/           # 预制体
│   │   └── UI/            # UI 单界面预制体
│   ├── Resources/         # 动态加载资源
│   ├── Scenes/            # 游戏场景
│   ├── ScriptableObjects/ # 数据配置
│   │   └── UISettings/
│   ├── Scripts/           # 游戏逻辑脚本
│   │   ├── FiniteStateMachine/
│   │   ├── InputProcess/
│   │   ├── PlayerControl/
│   │   │   ├── PlayerFSM/
│   │   │   └── PlayerStates/
│   │   └── UIFramework/
│   │       ├── Core/
│   │       ├── Panel/
│   │       ├── UIAnimation/
│   │       └── Window/
│   ├── Settings/          # 渲染管线等设置
│   ├── Shader/            # 着色器
│   └── InputSystem_Actions.inputactions
│
├── DeltaNights-Server/    # 服务器代码
│   ├── protobuf/          # Protobuf库
│   └── CMakeList.txt/
│
├── Docs/                  # 游戏开发文档
│   ├── 涉众分析/
│   ├── 特性分析/
│   ├── 问题陈述/
│   └── 愿景文档/
│
├── Packages/              # Unity Package 管理器内容
└── ProjectSettings/       # Unity 全局项目设置
```
## 🔧 构建与运行 (Build & Run)
Client（Unity） 使用Unity 6000.2.10f1

克隆仓库后从UnityHub -> 从本地磁盘添加项目 -> 选择项目根目录即可

---

Server（C++/Qt） 使用 Qt 6.9.3

克隆仓库后使用Qt Creator打开DeltaNights\DeltaNights-Server\CMakeList.txt构建运行