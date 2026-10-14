# 双人坦克 · Unity 学习工程

以项目为导向学 Unity 的练习工程，素材来自 Unity 官方 **Tanks!** 教程包，自己实现坦克的移动与转向。

---

## 换电脑打开前必读：环境要求

| 项目 | 要求 | 为什么 |
|---|---|---|
| Unity 版本 | **2022.3.62f3c1**（必须完全一致） | 版本不同会重新导入整个 `Library`，还可能悄悄改写场景文件 |
| 本机需装 Git | 是（`git --version` 能出结果） | 工程依赖一个从 GitHub 直接安装的包，没装 Git 打不开工程 |
| 首次打开耗时 | 十几分钟到半小时 | 仓库里故意不含 `Library/`，Unity 会自己重建它，属正常现象 |

打开步骤：`git clone` 本仓库 →  Unity Hub → `Add project from disk` → 选中 clone 出来的文件夹 → 版本选 `2022.3.62f3c1` → 等右下角进度条走完。

---

## 目录结构

| 路径 | 里面是什么 | 能不能改 |
|---|---|---|
| `Assets/Scenes/SampleScene.unity` | **我自己搭的场景**，主战场 | ✅ |
| `Assets/Scripts/Tank/MyTankMovement.cs` | **我自己写的脚本**：坦克移动 + 转向 | ✅ |
| `Assets/Scripts/`（其余 8 个） | 官方给的**半成品骨架**，按 `Camera` / `Managers` / `Shell` / `Tank` / `UI` 分类，关键几行留空让你填 | ✅ 练习用 |
| `Assets/_Completed-Assets/` | 官方的**完整答案**，脚本包在 `namespace Complete` 里所以和上面不重名 | ❌ 只读对照 |
| `Assets/_Complete-Game.unity` | 官方做好的**完整游戏场景**，可直接进 Play 玩 | ❌ 只读 |
| `Assets/_Complete-Game/` | 上面那个场景烘焙好的光照数据（Lightmap） | ❌ 删了光影就废 |
| `Assets/Models` `Materials` `Prefabs` `AudioClips` `Sprites` `Fonts` `Animators` | 素材原料：模型、材质、半成品预制体、音效、UI 图、字体、动画状态机 | 只读 |
| `Assets/Profiles/` | URP 渲染管线资产 + 后处理 Volume | ❌ 别删 |
| `Assets/Editor/` | 编辑器专用小工具脚本（`Editor` 是 Unity 的特殊文件夹名，内容不进游戏） | 不用管 |
| `Assets/TutorialInfo/` | 打开包时弹的 Readme 欢迎页 | 嫌烦可删 |
| `ProjectSettings/` | **工程设置**：Tag、Layer、物理、输入轴、URP 指定 | ⚠️ 必须提交 |
| `Packages/` | `manifest.json` 包清单 | ⚠️ 必须提交 |

命名规律：下划线 `_` 开头只是为了在 Project 窗口里排到最后，把"官方成品"和你的工作区隔开；`Complete` 前缀一律是官方答案，不要改。

---

## 我实现的玩法逻辑

`Assets/Scripts/Tank/MyTankMovement.cs`：

- 读输入：`Input.GetAxis(MovementAxisName)` / `Input.GetAxis(TurnAxisName)`，轴名在 `Start()` 里赋值为 `VerticalUI` 和 `HorizontalUI`。
- 移动：`transform.forward * 输入 * Speed * Time.deltaTime` 得到位移向量，交给 `rb.MovePosition()`。
- 转向：`Quaternion.Euler(0, turn, 0)` 造出"绕 Y 轴转 turn 度"这个动作，用 `rb.MoveRotation(rb.rotation * 该动作)` 叠加到当前朝向上。注意四元数乘法**不满足交换律**，两个因子的顺序不能写反。

> 待办：物理调用目前写在 `Update()` 里，按官方答案应挪到 `FixedUpdate()`（读输入留在 `Update`，两者用字段交接）。

---

## 依赖的包

见 `Packages/manifest.json`。两个值得注意的：

- `com.coplaydev.unity-mcp` —— 从 GitHub 直接安装的 git URL 依赖，作用是让 AI 工具通过 MCP 操控 Unity 编辑器。装好后在 Unity 里 `Window → MCP for Unity → Toggle MCP Window`，Connect 页点 `Start Server`，端点是 `http://127.0.0.1:8080/mcp`。
- `com.unity.render-pipelines.universal` (URP) —— 这个教程工程本身就是用 URP 做的，所以 `Assets/Profiles/` 里放着管线资产。

---

## 已知事项

- 本工程的资源序列化模式当前是 **Forced Binary**（场景和 `ProjectSettings/*.asset` 都是二进制，Git 里看不出逐行差异）。建议改成 `Edit → Project Settings → Editor → Asset Serialization → Force Text` 以便版本管理。
- 仓库**故意不含** `Library/`、`Temp/`、`obj/`、`Logs/`、`UserSettings/`、IDE 工程文件，规则见 `.gitignore`。判断标准：换台电脑能自己重新生成的，一律不入库。

---

## 素材授权

`Assets/` 下的美术、音频、模型素材来自 Unity 官方 Learn 平台的 **Tanks!** 教程资源，其中包含 `Tanks!_Third-PartyNotice.txt` 第三方授权声明。本仓库为**私有仓库**，仅用于个人学习与跨设备同步，不用于再分发。
