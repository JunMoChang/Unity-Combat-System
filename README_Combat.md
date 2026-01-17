# Unity 第三人称动作游戏核心系统

> 包含玩家系统和敌人AI 
![Unity](https://img.shields.io/badge/Unity-2021.3+-blue)
![C#](https://img.shields.io/badge/C%23-9.0-purple)
![License](https://img.shields.io/badge/License-MIT-green)

## 功能特性

### 玩家系统
  - 移动模块
  - 战斗模块
  - 武器模块
  - 输入模块
### 敌人AI
   - 行为树驱动
   - 视野检测
   - 受击反馈
### 架构设计
   - 状态机管理角色行为（FSM + 行为树混合）
   - 组件化武器系统（BaseWeapon + IWeaponComponent）
   - 事件驱动的输入管理器

## 演示

![战斗演示](docs/combat_demo.gif)

*移动 → 拔刀 → 连击 → 敌人受击*

## 技术栈

| 技术 | 用途 |
|------|------|
| Unity 2022.3+ | 游戏引擎 |
| C# 9.0 | 编程语言 |
| Unity Input System | 新输入系统 |
| Cinemachine | 相机控制 |
| Behavior Tree | 敌人 AI |

## 技术亮点

### 状态机驱动战斗流程

```
         5秒无操作
待机状态 ←--------→ 战斗状态 ←→ 连击状态
(BeginningState)   (CombatState)  (ComboCState)
```

**优势**：
- 状态间职责清晰，易于维护
- 支持快速扩展新状态（格挡、闪避等）
- 避免大量if-else嵌套

**核心代码**：
```csharp
public override void ChangeState(StateBase newState)
{
    if(newState == CurrentState) return;
    CurrentState.Exit();
    CurrentState = newState;
    CurrentState?.Enter();
}
```

### 组件化武器架构

```csharp
BaseWeapon (抽象基类)
├─ SwordWeapon (具体武器)
│  ├─ SheathComponent  (拔刀/收刀)
│  └─ DamageComponent  (伤害判定)
└─ BowWeapon (可扩展)
```

**优势**：
- 功能模块化，易于复用
- 添加新武器只需继承 BaseWeapon
- 支持组合不同组件

**示例**：
```csharp
public class SwordWeapon : BaseWeapon
{
    void Start()
    {
        AddWeaponComponent(new SheathComponent(...));
        AddWeaponComponent(new DamageComponent(...));
    }
}
```

### 行为树 AI 系统

```
Selector (选择器)
├─ Sequence (序列)
│  ├─ CheckEnemyInFovRange (检测玩家)
│  └─ TaskGoToTarget (追击)
└─ TaskPatrol (巡逻)
```

**优势**：
- 逻辑清晰
- 支持复杂 AI 行为组合
- 支持行为打断，执行更高优先级行为

**节点示例**：
```csharp
public class CheckEnemyInFovRange : Node
{
    public override NodeState Evaluate()
    {
        int count = Physics.OverlapSphere(...);
        if (count > 0)
        {
            parent.SetData("target", enemy);
            return NodeState.Success;
        }
        return NodeState.Failure;
    }
}
```

### 动画事件控制攻击判定

在攻击动画的关键帧调用方法，精确控制伤害判定时机

```csharp
// 动画事件调用
public void OnEnableAttack()
{
    weaponDamage.ResetDamage(); // 允许造成伤害
}

public void OnDisableAttack()
{
    weaponDamage.canDamage = false; // 禁止伤害
}
```

**优势**：

- 伤害判定与动画同步
- 避免挥空也造成伤害
- 支持多段判定

### Unity Input System

统一输入管理，支持按键重绑定

```csharp
public class InputManager : MonoBehaviour
{
    public Action<string, InputControl> OnInputPressedWithControl;
    
    public Vector2 GetVector2(string actionName)
    {
        return inputActions[actionName].ReadValue<Vector2>();
    }
}
```

**优势**：
- 支持跨平台（PC、手柄、移动端）
- 支持运行时重新绑定按键
- 事件驱动，解耦性好
## 问题与解决方案

### 问题1：Root Motion 攻击偏移

** 问题描述** 
使用Root Motion动画实现攻击时，发现角色攻击方向会偏离目标。
** 问题分析**
1.Root Motion的位移会触发'CharacterController'的碰撞检测。
2.碰撞体与敌人的物理交互会导致攻击方向偏移。
** 解决方案 **

```csharp
//动画前40%时间锁定最近敌人
public override void Update()
{
    AnimatorStateInfo stateInfo = GetCurrentStateInfo(0);
    
    if (stateInfo.normalizedTime < 0.4f)
    {
        LockRotationToNearestEnemy();
    }
}
```
### 问题 2：Animator状态信息延迟
** 问题描述 **
状态机切换状态时，第一帧获取的normolizeTime大于1，导致攻击或者受击动画立即结束。
** 问题分析 **
Unity 的执行顺序问题：
```
1. MonoBehaviour.Update()
   - BeHitState.Enter() → animator.Play("BeHitState")
   - BeHitState.Update() → GetCurrentAnimatorStateInfo()
   ↑ 此时 animator.Play() 还未生效

2. Animator.Update() ← Unity 内部，在所有 Update 之后
   - 真正应用 animator.Play()
   
3. 下一帧的 Update()
   - 才能获取到新动画的状态
```

** 解决方案 1：**状态内部获取

```csharp
//旧设计
public void override Update(AnimatorStateInfo stateInfo)();
//新设计
public void override Update()
{
	//状态内部获取
	AnimatorStateInfo stateInfo = GetCurrentStateInfo(0);
}
```
** 解决方案2 ：**使用协程等待一帧

``` csharp
public void override Enter()
{
    enemyController.StartCoroutine(CheckAnimationEnd);
}
//等待一帧，再获取动画信息
private System.Collections.IEnumerator CheckAnimationEnd()
{
    yield return null;

    while(true)
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(beHitLayer);

        if(animator.IsInTransition(beHitLayer))
        {
            yield return null;
            continue;
        }

        if (info.normalizedTime >= 1f)
        {
            enemyFsm.ChangeState(enemyFsm.GetState(nameof(EnemyBeginningState)));
            checkCoroutine = null;
            yield break;
        }

        yield return null;
    }
}
```
### 问题3：状态机与行为树冲突
** 问题描述**
.敌人受击时仍然会追击玩家。
** 问题分析**
当玩家与敌人满足距离要求时，尽管敌人处于受击状态，行为树会检测到要追击敌人，导致状态切换。
**解决方案**

```csharp
//在创建节点时，加入打断条件
protected override Node SetUpTree()
{
    Node root = new Selector(new List<(Node, Func<bool>)>
    {
        (new HitReactionNode(enemyController, enemyFsm), null),
        (traceNode, () => enemyController.isHit),
        (patrolNode, () => enemyController.isHit)
    });

    return root;
}
//评估节点时，先判断打断条件
if(child.condition != null && child.condition()) continue;
```
### 资源说明

本项目专注于**系统设计与代码实现**，使用以下资源用于功能展示：

- **角色模型**：[Mixamo](https://www.mixamo.com/)（免费）
- **攻击动画**：虚幻商城 - Root Motion Attack Pack（购买） 
- **场景**：Unity ProBuilder + 原生几何体

## 快速开始

### 环境要求

- Unity 2022.3 或更高版本
- Cinemachine 包】
- Input System 包

### 安装步骤

1. **克隆项目**
```bash
git clone https://github.com/JunMoChang/Unity-Combat-System.git
```

2. **用 Unity 打开项目**

3. **安装必需包**
   - Window → Package Manager
   - 搜索并安装：Cinemachine、Animation Rigging、Input System

4. **运行场景**
   - 打开 `Assets/Scenes/CombatScene`
   - 点击 Play

### 操作说明

| 操作 | 按键 |
|------|------|
| 移动 | `W A S D` |
| 视角旋转 | 鼠标移动 |
| 行走 | `Ctrl` + 移动 |
| 冲刺 | `Shift` + 移动 |
| 普通攻击 | 鼠标左键 |
| 拾取武器 | `F` |

## 项目结构

```
Assets/
├── Scripts/
│   ├── Player/
│   │   ├── PlayerController.cs        # 玩家控制器（核心）
│   │   ├── StateManager/
│   │   │   ├── PlayerFsm.cs          # 状态机
│   │   │   ├── AbstractState/
│   │   │   │   ├── StateBase.cs      # 状态基类
│   │   │   │   └── StateMachine.cs   # 状态机基类
│   │   │   └── DetailState/
│   │   │       ├── BeginningState.cs # 待机状态
│   │   │       ├── CombatState.cs    # 战斗状态
│   │   │       └── ComboCState.cs    # 连击状态
│   │   └── Function/
│   │       ├── Movement.cs           # 移动控制
│   │       ├── SightControl.cs       # 视角控制
│   │       ├── AttackSetting.cs      # 攻击配置
│   │       └── WeaponHandel.cs       # 武器管理
│   ├── Enemy/
│   │   ├── EnemyController.cs        # 敌人控制器
│   │   └── EnemyBehaviorTree.cs      # 行为树
│   ├── BT/                            # 行为树框架
│   │   ├── Node.cs                    # 节点基类
│   │   ├── Selector.cs                # 选择器
│   │   └── Sequence.cs                # 序列
│   ├── Weapon/
│   │   ├── AbstractWeapon/
│   │   │   ├── BaseWeapon.cs         # 武器基类
│   │   │   └── IWeaponComponent.cs   # 组件接口
│   │   ├── WeaponComponent/
│   │   │   ├── SheathComponent.cs    # 拔刀组件
│   │   │   └── DamageComponent.cs    # 伤害组件
│   │   └── SpecificWeapon/
│   │       └── SwordWeapon.cs        # 剑武器
│   └── InputManager.cs                # 输入管理器
├── Animations/
│   ├── Player/
│   └── Enemy/
├── Prefabs/
│   ├── Player.prefab
│   ├── Enemy.prefab
│   └── Weapons/
└── Scenes/
    └── CombatScene.unity
```

## 核心类说明

### PlayerController
玩家控制器，整合所有功能

**职责**：

- 管理状态机
- 处理移动和视角
- 管理武器

**关键字段**：
```csharp
public PlayerFsm playerFsm;           // 状态机
private Movement movement;            // 移动控制
private AttackSetting attackSetting; // 攻击配置
```

### PlayerFsm
玩家状态机

**职责**：
- 管理所有战斗状态
- 处理状态切换
- 维护战斗标志

**状态列表**：
```csharp
public BeginningState BeginningState; // 待机
public CombatState combatState;       // 战斗
public ComboCState comboCState;       // 连击
```

### EnemyBehaviorTree
敌人行为树

**行为树结构**：
```csharp
Node root = new Selector(new List<Node>
{
    new Sequence(new List<Node>
    {
        new CheckEnemyInFovRange(transform, playerMask),
        new TaskGoToTarget(transform)
    }),
    new TaskPatrol(transform, wayPoints)
});
```

## 自定义配置

### 添加新状态

1. 继承 `StateBase`
2. 实现 `Enter()`, `Update()`, `Exit()`
3. 在 `PlayerFsm` 中注册

```csharp
public class DodgeState : StateBase
{
    public override void Enter()
    {
        // 闪避动画
    }
    
    public override void Update(AnimatorStateInfo stateInfo)
    {
        // 无敌判定
    }
}
```

### 添加新武器

1. 继承 `BaseWeapon`
2. 实现自定义组件
3. 在 `Start()` 中注册组件

```csharp
public class BowWeapon : BaseWeapon
{
    void Start()
    {
        AddWeaponComponent(new ShootComponent(...));
        AddWeaponComponent(new ReloadComponent(...));
    }
}
```

### 调整攻击参数

```csharp
// AttackSetting.cs
public int maxInputTimes = 5;  // 最大连击次数
private float inputResetTimes; // 输入重置时间
```

### 配置敌人巡逻

在场景中：
1. 创建空物体作为路径点
2. 拖入 `EnemyBehaviorTree.wayPoints` 数组
3. 调整巡逻速度和视野范围

## 🤝 贡献

欢迎提交 Issue 和 Pull Request！

## 📄 开源协议

本项目采用 [MIT License](LICENSE)

## 👤 作者

**你的名字**

- GitHub: [@JunMoChang](https://github.com/JunMoChang)
- Email: 3484773855@qq.com

---

⭐ 如果觉得有用，欢迎 Star！

💬 有问题？[开启讨论](https://github.com/JunMoChang/Unity-Combat-System/discussions)
