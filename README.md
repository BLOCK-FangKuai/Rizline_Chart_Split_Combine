# Rizline谱面分段合并工具

一个用于 **Rizline** 谱面分段合并的交互式控制台工具。将多个按时间切分的谱面片段根据指定的分割时间点拼合为完整谱面，支持通过配置文件实现全参数化批量处理。

## 功能

- 支持手动导入、自动导入和全自动三种处理模式
- 根据分割时间点自动截取并拼接 `lines`、`notes`、`canvasMoves`、`cameraMove` 等谱面数据
- `linePoints` 和 `canvasMoves` 在分割点处支持双向扩展（`overlapTime`）
- 自动修正合并后的 `canvasIndex` 偏移及 Note 数据
- 摄像机移动边界关键帧支持拼接修正（`cameraMoveOffset` / `finalCameraMoveEaseSetZero`）
- 支持通过 `settings.json` 配置文件实现全参数化批量处理
- 输出文件覆盖保护，写入失败时自动提示重新输入

## 下载与运行

### 1. 下载

从 [Releases](https://github.com/BLOCK-FangKuai/Rizline_Chart_Split_Combine/releases) 页面下载最新版本的压缩包。

### 2. 运行

解压后双击 `Rizline_Chart_Split_Combine.exe` 即可运行。

## 导入模式

程序提供了三种导入模式，可根据需求选择。

### 自动导入模式（推荐）

适合需要精细设置的场景。通过 `settings.json` 配置文件指定所有参数，程序按配置读取目录中的文件并合并谱面。

程序启动后输入文件目录并选择「自动导入」，工具会按以下顺序处理：

1. 读取目录下的 `settings.json` 配置文件
2. 读取配置中 `baseChartName` 指定的基础谱面文件，未指定时使用 `base.json`
3. 获取谱面文件列表：
   - `files` 为空时 → **自动扫描**目录中所有文件名中第一个 `.` 前的部分能被解析为整数的 `.json` 文件，并按编号从小到大排序
   - `files` 不为空时 → **直接使用** `files` 中指定的文件名列表（保持原始顺序）
4. 校验谱面文件数量与 `splitTimes` 数量是否匹配（`files数量 = splitTimes数量 + 1`）
5. 根据分割时间自动合并并输出结果

### 全自动模式

自动导入模式的扩展。在程序所在目录创建 `charts` 文件夹，放置 `settings.json`（`automatic` 设为 `true`）和谱面文件，程序启动后无需任何操作即可自动处理并输出结果。

### 手动导入模式

适合快速测试或简单合并场景。可设置的项目较少，如需精细控制请使用自动导入模式。

程序启动后按以下步骤操作：

1. 输入谱面文件所在目录
2. 选择「手动导入」
3. 按照提示输入**基础谱面文件名**
4. 按照提示交替输入**谱面文件名**和**分割时间（节拍）**：
   - 每次输入一个谱面文件名 → 输入该谱面结束的节拍位置 → 继续下一个谱面
   - 最后一段谱面不需要输入分割时间
   - 空输入结束输入流程
5. 程序自动合并并输出结果谱面文件

## 配置文件 (settings.json)

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `splitTimes` | `float[]` | `[]` | 分割时间点（节拍），数量 = 谱面文件数 - 1 |
| `overlapTime` | `float` | `0` | `linePoints` 和 `canvasMoves` 截取范围的扩展量（节拍）。使 `linePoints` 的截取区间为 `[start - overlapTime, end + overlapTime]`， `canvasMoves` 的截取区间为 `[0, end + overlapTime]`|
| `finalCameraMoveEaseSetZero` | `bool` | `false` | 是否移除截取结果中最后一个时间超出段边界的关键帧，并将相邻末帧的缓动类型设为 `zero`。此项为 `true` 时 `cameraMoveOffset` 不生效 |
| `cameraMoveOffset` | `float` | `0.015625` | 将每段截取结果中最后一个摄像机移动关键帧的时间向前偏移的量（节拍）。当 `finalCameraMoveEaseSetZero` 的值为 `true` 时此项不生效。范围 `(0, 0.015625]` |
| `baseChartName` | `string` | `"base.json"` | 基础谱面文件名 |
| `files` | `string[]` | `[]` | 指定谱面文件名列表，为空时自动扫描文件夹内符合条件的文件 |
| `resultName` | `string` | `"result"` | 输出谱面文件名（不含后缀） |
| `autoOverWritten` | `bool` | `false` | 是否自动覆盖已存在的输出文件。若写入失败，会让用户重新输入文件名 |
| `automatic` | `bool` | `false` | 是否启用全自动模式 |


### 示例配置

```json
{
  "splitTimes": [16, 32, 48],
  "overlapTime": 4,
  "finalCameraMoveEaseSetZero": false,
  "cameraMoveOffset": 0.01,
  "baseChartName": "base_chart.json",
  "files": ["Chart_EZ.json", "Chart_IN.json"],
  "resultName": "result_chart",
  "autoOverWritten": true,
  "automatic": false
}
```

### 谱面文件说明

- **基础谱面文件**：用于确定谱面的基本信息，包含 `chartDelayMs`、`themes`、`challengeTimes`、`bPM`、`bpmShifts` 等元数据。合并时基础谱面的这些字段会被保留，分段谱面文件中的同名字段将被忽略。默认文件名为 `base.json`，可通过配置项 `baseChartName` 自定义。
- **分段谱面文件**：存储实际的谱面数据，包括 `lines`、`notes`、`canvasMoves`、`cameraMove` 等。自动模式下，如果没有在配置文件中设置 `files`，程序会扫描文件名中第一个 `.` 前的部分能被解析为整数的 `.json` 文件（如 `1.json`、`2.Chart_HD.json`），并按编号从小到大排序后依次合并。

## 项目结构

```
Rizline_Chart/
├── Program.cs       # 主程序入口及合并逻辑
├── Chart.cs         # 谱面数据模型定义
├── Settings.cs      # 配置模型定义
└── Rizline_Chart_Split_Combine.csproj  # 项目文件
```

## 工作原理

1. 读取基础谱面文件获取元数据
2. 读取各分段谱面文件
3. 根据分割时间点，对每个谱面文件进行截取：
   - 截取指定时间范围内的 `linePoints`、`notes`
   - 截取 `canvasMoves` 和 `cameraMove` 关键帧
   - 自动修正 `canvasIndex` 引用
   - Hold 音符尾部超界时自动修正结束时间和 `floorPosition`
4. 将所有截取后的数据合并到结果谱面中
5. 输出最终的 JSON 文件

## 许可证

MIT
