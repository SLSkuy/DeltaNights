# DeltaNights


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

DeltaNights 是一款采用 Unity 开发、风格偏向 二次元动作 + 搜打撤（Search-Fight-Extract） 的多人在线游戏项目。
服务器由 C++ / Qt 构建，客户端以模块化结构设计，便于扩展与多人功能接入。

> 本仓库包含客户端和服务端核心代码。模型等大型资源需单独下载，下载链接：`xxx.xxx`（待定）
## 📖 使用文档

- 服务器事件处理：[查看文档](Assets/Scripts/Network/README.md)
- UI框架使用： [查看文档](Assets/Scripts/UIFramework/README.md)
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
│   │   ├── CharacterMesh/ # 角色模型
│   │   ├── MainSceneUI/   # UI 主界面预制体
│   │   └── UIFramework/
│   ├── Resources/         # 动态加载资源
│   ├── Scenes/            # 游戏场景
│   ├── ScriptableObjects/ # 数据配置
│   │   └── UISettings/
│   ├── Scripts/           # 游戏逻辑脚本
│   │   ├── CameraManager/
│   │   ├── EventProcess/
│   │   ├── FiniteStateMachine/
│   │   ├── GameSetting/
│   │   ├── GenerationSystem/
│   │   ├── InputProcess/
│   │   ├── Network/
│   │   ├── PlayerControl/
│   │   │   ├── PlayerFSM/
│   │   │   └── PlayerStates/
│   │   ├── SceneUI/
│   │   └── UIFramework/
│   │       ├── Core/
│   │       ├── Panel/
│   │       ├── UIAnimation/
│   │       └── Window/
│   │   
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

### Client（Unity） 使用Unity 6000.2.10f1

1. 克隆仓库：
```bash
git clone https://github.com/SLSkuy/DeltaNights.git
```
2. 在 Unity Hub 中选择 从本地磁盘添加项目 → 选择项目根目录

3. 打开 Unity 即可编辑与运行

---

### Server（C++/Qt） 使用 Qt 6.9.3

#### 运行环境

* 本项目使用Protobuf作为通信协议，服务端需要使用Protobuf-C++库
* Protobuf仓库链接：[访问 Protobuf仓库](https://github.com/protocolbuffers/protobuf)
* 官方文档：https://protobuf.dev/
* 跟随教程编译后将库文件夹重命名为```protobuf```放入DeltaNights-Server文件夹中并配置cmake即可

> 默认cmake已配置好了protobuf库，需确保protobuf文件夹下有include和lib文件夹

#### 运行方式

1. 使用 Qt Creator 打开：

```
DeltaNights/DeltaNights-Server/CMakeList.txt
```
2. 选择构建并运行
