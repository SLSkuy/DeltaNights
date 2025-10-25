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
## 项目目录结构

### Assets
项目文件夹
- Animation：动画相关文件
  - AnimatorController：动画控制器
  - CharacterAnim：角色动画文件
- Meshes：模型网格文件（模型需单独下载，Git对于二进制文件支持欠佳，若跟踪会导致.git文件增大）
- Prefabs：存储预制体
  - UI：单个UI界面预制体
- Resources：动态加载资源文件
- Scenes：游戏场景文件
- ScriptableObjects：数据配置文件
  - UISettings：UI配置文件
- Scripts：脚本文件
  - FiniteStateMachine：有限状态机
  - InputProcess：输入处理封装
  - PlayerControl：玩家控制器
    - PlayerFSM：玩家状态机
      - PlayerStates：玩家所有状态
  - UIFramework：UI框架
    - Core：框架核心代码，管理所有UI界面
    - Panel：面板类UI控制代码
    - UIAnimation：UI界面过渡动画
    - Window：窗口类UI控制代码
- Settings：渲染管线设置
- Shader：着色器相关文件
- InputSystem_Actions.inputactions：输入配置文件

### Docs
开发文档文件夹
- 涉众分析：存放各版本涉众分析文档
- 特性分析：存放各版本特性分析文档
- 问题陈述：存放各版本问题陈述报告
- 愿景文档：存放各版本愿景文档

### Packages
Unity内置包管理器

### ProjectSettings
项目全局设置