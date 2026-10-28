# 双人坦克 · Unity 学习工程

以项目为导向学 Unity：美术素材与代码骨架来自 Unity 官方 **Tanks!** 教程包，玩法逻辑由我自己实现。

- 引擎版本：**2022.3.62f3c1**（URP）
- 学习跨度：2026-10-14 → 2026-10-28，11 次提交
- 当前状态：坦克可开火、可扣血、可爆炸击退，相机跟随并自动缩放视野，音频走 AudioMixer，Windows 包已打出来能玩

---

## 换电脑打开前必读：环境要求

| 项目 | 要求 | 为什么 |
|---|---|---|
| Unity 版本 | **2022.3.62f3c1**，必须完全一致 | 版本不同会重新导入整个 `Library`，还可能悄悄改写场景文件 |
| 本机需装 Git | 是（`git --version` 能出结果） | 工程依赖一个从 GitHub 直接安装的包，没装 Git 打不开 |
| 首次打开耗时 | 十几分钟到半小时 | 仓库故意不含 `Library/`，Unity 要自己重建，属正常现象 |

打开步骤：`git clone` 本仓库 → Unity Hub → `Add project from disk` → 选中 clone 出来的文件夹 → 版本选 `2022.3.62f3c1` → 等右下角进度条走完。

---

## 我自己写的脚本

| 文件 | 负责什么 | 状态 |
|---|---|---|
| `Assets/Scripts/Tank/MyTankMovement.cs` | 移动、转向、引擎音效随输入切换 | 已完成 |
| `Assets/Scripts/Tank/MyTankShooting.cs` | 蓄力发射炮弹、开火音效、瞄准滑条 | 已完成 |
| `Assets/Scripts/Shell/MyShellExplosion.cs` | 命中判定、范围伤害、爆炸推力、特效与声音 | 已完成 |
| `Assets/Scripts/Tank/MyTankHealth.cs` | （预备替换官方血量逻辑） | **空文件，还没写** |

官方骨架（照教程填写，非我原创）：`Camera/CameraControl.cs`（跟随+缩放，已完成）、`Managers/GameManager.cs`、`Managers/TankManager.cs`、`UI/UIDirectionControl.cs`、`Tank/TankHealth.cs`（目前场景里的坦克仍挂在它上面）。

命名规律：`My*` = 我写的；`Complete*` / `_Completed-Assets` = 官方完整答案，只读对照，不要改。

---

## 实现要点（这个工程真正学到的东西）

**输入与移动**
- 轴名按玩家编号拼接：`"Vertical" + PlayerNumber`、`"Fire" + PlayerNumber`，两套控制共用一份脚本。
- 位移 = `transform.forward * 输入 * Speed * Time.deltaTime`，交给 `rb.MovePosition()`。`forward` 是**世界**向量（绑定物体朝向，数值读自世界系），所以车头转向后自然朝新方向开。
- 转向 = `Quaternion.Euler(0, turn, 0)` 再 `rb.MoveRotation(rb.rotation * 该动作)`。四元数乘法**不满足交换律**，两个因子顺序写反手感就错。

**蓄力发射**
- `GetButtonDown` 起算并把力压到 `MinLaunchForce`；按住期间按 `ChargeSpeed = (Max - Min) / ChargeTime` 累加；到上限或 `GetButtonUp` 松手即发射。炮弹初速度 = 炮口 `forward * 当前力`。

**爆炸（范围伤害 + 击退）**
- 炮弹碰撞体 `isTrigger = true`，所以用 `OnTriggerEnter` 接收命中。
- 以 `Physics.OverlapSphere(爆点, ExplosionRadius, LayerMask)` 找目标，再对目标刚体 `AddExplosionForce(力, 爆点, 半径)`。
- **两处半径必须是同一个值**：`OverlapSphere` 管"找谁"，`AddExplosionForce` 管"给多大力"，不一致就会找到吃不到力的目标，或漏掉该受力的目标。
- 层级掩码用 `Players`（层 9），所以地面、岩石不会被子弹掀飞。

**相机（`CameraControl.cs`）**
- 俯视正交相机：位置靠 `Vector3.SmoothDamp` 带惯性跟随，速度状态存在字段 `m_MoveVelocity` 里（`ref` 传进去，必须跨帧保留，写成局部变量就退化成硬跟）。
- `averagePos.y = transform.position.y` 把高度钉死，相机只在水平面平移，不随坦克上下颠簸。
- 死亡即 `SetActive(false)`，靠 `gameObject.activeSelf` 过滤，相机自动丢掉残骸并收紧视野。
- 视野大小用**相机本地坐标**算：`InverseTransformPoint` 把世界点换成"以相机为原点"的偏移，此时 `x` 才是画面左右、`y` 才是画面上下。
- `orthographicSize` = 画面**垂直半高**（世界单位）。所以装下垂直偏移要 `size ≥ |dy|`，装下水平偏移要 `size ≥ |dx| / aspect`——除以 `aspect` 是因为它按高度定义。最后加边距、再夹一个最小值。

**音频**
- 每辆坦克两个 `AudioSource`（引擎声 / 开火声），经 `AudioMixer` 统一分组；怠速与行驶音之间切换，并给引擎音随机 pitch 避免循环感。

**预制体**
- `Assets/Prefabs/MyShell.prefab` 是 `Assets/Models/Shell.fbx` 的 **Prefab Variant**，内部嵌套 `ShellExplosion.prefab` 与 `ExplosionSound.prefab`。改模型会连带影响炮弹外观。

---

## 学习历程

| 日期 | 提交 | 内容 |
|---|---|---|
| 2026-10-14 | 第一次提交，坦克移动和声音 | 导入 Tanks! 素材，实现移动/转向/引擎音 |
| 2026-10-15 | 2.加入灰尘特效 | Particle System 车尾扬尘 |
| 2026-10-15 | 3.发射炮弹及相关内容 | 蓄力发射、炮口、滑条 |
| 2026-10-16 | 4.炮弹爆炸的逻辑实现 | 范围伤害、爆炸推力、特效声音拆解 |
| 2026-10-16 | 5.坦克血量UI显示 | 血条、扣血 |
| 2026-10-18 | 6.摄像机移动 | 跟随 + 按目标距离自动缩放视野 |
| 2026-10-19 | 6.1代码检查 | 逐项核对代码 |
| 2026-10-24 | 6.2游戏运行逻辑 | 胜负流程 |
| 2026-10-27 | 7.游戏逻辑完善 | 流程与状态收尾 |
| 2026-10-28 | 8.音频混合器与游戏打包 | AudioMixer、出 Windows 包 |
| 2026-10-28 | 9.规范仓库内容 | 把误入库的构建产物移出跟踪 |

---

## 目录结构

| 路径 | 里面是什么 | 能不能改 |
|---|---|---|
| `Assets/Scenes/SampleScene.unity` | 我自己搭的场景，主战场 | ✅ |
| `Assets/Scripts/` | 我写的 `My*` + 官方骨架，按 `Camera/Managers/Shell/Tank/UI` 分类 | ✅ |
| `Assets/Prefabs/` | 我这边在用的预制体：坦克、炮弹、爆炸、扬尘、音效容器 | ✅ |
| `Assets/_Completed-Assets/` | 官方完整答案，脚本包在 `namespace Complete` 里所以不重名 | ❌ 只读对照 |
| `Assets/_Complete-Game.unity` + `Assets/_Complete-Game/` | 官方成品场景及其烘焙光照数据 | ❌ 只读；Lightmap 删了光影就废 |
| `Assets/Models` `Materials` `AudioClips` `AudioMixers` `Sprites` `Fonts` `Animators` | 素材原料 | 只读 |
| `Assets/Profiles/` | URP 管线资产 + 后处理 Volume | ❌ 别删 |
| `Assets/Editor/` | 编辑器专用脚本（`Editor` 是特殊文件夹，内容不进游戏） | 不用管 |
| `Assets/TutorialInfo/` + `Assets/Readme.asset` | 开包欢迎页 | 嫌烦可删 |
| `ProjectSettings/` | Tag、Layer、物理、输入轴、URP 指定 | ⚠️ 必须提交 |
| `Packages/` | `manifest.json` 包清单 | ⚠️ 必须提交 |

---

## 依赖的包

见 `Packages/manifest.json`。两个值得注意：

- `com.coplaydev.unity-mcp` —— 从 GitHub 直接装的 git URL 依赖，用于让 AI 工具通过 MCP 操控编辑器。`Window → MCP for Unity → Toggle MCP Window`，Connect 页点 `Start Server`，端点 `http://127.0.0.1:8080/mcp`。**不需要它也能玩，换电脑打开工程时若拉不下来可以先移除这一行。**
- `com.unity.render-pipelines.universal` —— 本教程工程本身就是 URP 做的。

---

## 仓库规则

判断标准只有一条：**换台电脑能不能自己长出来？能 → 不入库；不能（那是劳动成果）→ 必须提交。**

已忽略：`Library/`、`Temp/`、`obj/`、`Logs/`、`UserSettings/`、`*.csproj`/`*.sln`/`.vs/`、构建产物目录、`*.exe`。

⚠️ **一个自己埋的坑**：磁盘上构建产物文件夹叫 **`Bulid`**（拼写反了），而 `.gitignore` 写的是 `/[Bb]uild/`，**规则匹配不上**。2026-10-28 已把误提交的 182 个构建文件移出跟踪，但规则仍是错的——目前只靠 `*.exe` 挡住了主程序，其余运行时文件仍可能被 `git add .` 顺手收进来。**改法：把文件夹重命名为 `Build`（Unity 里 `File → Build Settings → Output folder` 一并改掉）。**

## 打包与分享

Windows 包在 `Bulid/`（改名后是 `Build/`），约 94 MB。**构建产物不进代码仓库是故意的**：想让别人的话，把整个文件夹压成 zip 传到仓库的 **Releases**（单文件 ≤100 MB），那才是放二进制的地方。

---

## 已知问题与待办

- **资源序列化模式是 Forced Binary**：场景、预制体、`ProjectSettings/*.asset` 都是二进制，Git 里只能看到 `Bin 34492 -> 37340 bytes`，看不到逐行差异，也无法在别的电脑上审阅。建议改 `Edit → Project Settings → Editor → Asset Serialization → Force Text`（改完会重新序列化，产生一次大 diff，属正常）。
- **物理调用写在 `Update()`**：`MyTankMovement` 的 `Move()`/`Turn()` 在 `Update` 里调 `rb.MovePosition`。官方答案的做法是读输入留在 `Update`、把物理调用挪到 `FixedUpdate`，用字段交接，否则帧率波动时手感会变。
- **`MyTankHealth.cs` 还是空文件**：血量和死亡目前用的是官方 `TankHealth`，替换未完成。
- **`CameraControl.cs` 第 1 行有一个多余的 `using System.Diagnostics;`**：它和 `UnityEngine.Debug` 撞名，会让 `Debug.Log` 报 `CS0104 ambiguous reference`。删掉即可。

---

## 素材授权

`Assets/` 下的美术、音频、模型素材来自 Unity 官方 Learn 平台的 **Tanks!** 教程资源，含第三方授权声明 `Assets/Tanks!_Third-PartyNotice.txt`。本仓库仅用于个人学习与跨设备同步，不用于再分发。仓库可见性请自行确认：若为 Public，请注意上述素材的再分发条款。
