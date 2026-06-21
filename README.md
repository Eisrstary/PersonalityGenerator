[English](#english) | [中文](#chinese)

---

## English

# PersonalityGenerator

A zero-dependency, deterministic personality parameter generation library for .NET Standard 2.0.  
Generates AI-readable character profiles from integer seeds for role-playing, simulation, and game development.

### Overview

PersonalityGenerator transforms a single integer seed into a complete 84-parameter personality profile, then renders it as natural language text suitable for AI-driven character扮演 (role-play). Every personality is fully deterministic — the same seed always produces the exact same character.

### Key Features

- **Zero Dependencies** — Pure C# implementation. No external libraries. Compatible with Unity, WASM, iOS, Android, and any .NET runtime.
- **Deterministic** — Same seed → same personality. Perfect for procedural generation, save systems, and reproducible simulations.
- **84 Atomic Parameters** — Covers 8 domains: perception, memory, cognition, emotion, motivation, behavior, social signals, meta-cognition, embodiment, and temporal development.
- **Bias System** — Steer generation toward specific traits with a simple string-based DSL: `"B015=0.9,D040=-0.8,S=0.85"`.
- **Partial Missing** — ~15% of parameters are randomly omitted to simulate realistic human incompleteness.
- **3 Output Formats** — Roleplay (AI-ready character sheet), Compact (key traits only), Detailed (full parameter report).
- **Battle-Tested** — Validated through 111 billion+ personality generations with zero errors.
- **Lightweight** — Release DLL is only 22 KB.

### Quick Start

```csharp
using PersonalityGenerator;

// Generate 100 random personalities
Personality[] batch = Generator.Generate(100);

// Generate with bias (high guilt, low aggression, strong pull)
Personality[] saints = Generator.Generate(50, "B015=0.9,D040=-0.8,S=0.85");

// Single personality from a specific seed
Personality p = Generator.GenerateFromSeed(42);

// Read a specific parameter (normalized 0-1)
double guilt = p.Get("B015");

// Generate AI-ready character description
string characterSheet = Textify.ToRoleplay(p);

// Compact summary
string summary = Textify.ToCompact(p);

// Full parameter report
string fullReport = Textify.ToDetailed(p);
```

### Bias DSL Syntax

```
"B015=0.9"          → Single parameter bias (-1.0 to +1.0)
"A=0.5"             → Entire domain bias (A-H)
"S=0.8"             → Bias strength (0-1, default 0.7)
"STRENGTH=1.0"      → Full keyword also supported
"B015=0.9,D040=-0.8,S=0.85"  → Combined
```

### Parameter Domains (84 total)

| Domain | Code | Parameters | Description |
|--------|------|-----------|-------------|
| Perception & Attention | A | A001-A010 | Visual scanning, threat detection, pain sensitivity |
| Emotion Generation | B | B011-B024 | Guilt, shame, envy, schadenfreude, emotional contagion |
| Motivation & Values | C | C025-C038 | Power, affiliation, achievement, deception acceptance |
| Behavior Execution | D | D039-D042 | Aggression baseline, rule compliance, impulsivity |
| Meta-Cognition & Self | E | E043-E055 | Self-deception, moral disengagement, sense of mission |
| Social Signals | F | F056-F062 | Trust, betrayal detection, impression management |
| Temporal Development | G | G063-G066 | Personality drift, context switching, identity narrative |
| Body-Environment | H | H067-H084 | Posture-mind links, hunger-risk, altitude-abstraction |

### Installation

**Unity:** Copy `PersonalityGenerator.dll` and `PersonalityGenerator.deps.json` into `Assets/Plugins/`.

**.NET Project:**
```xml
<Reference Include="PersonalityGenerator">
  <HintPath>path/to/PersonalityGenerator.dll</HintPath>
</Reference>
```

### Platform Support

| Platform | Status |
|----------|--------|
| .NET Framework 4.6.1+ | ✓ |
| .NET Core 2.0+ / .NET 5+ | ✓ |
| Unity 2019+ / Unity 6 | ✓ |
| WebAssembly (WASM) | ✓ |
| iOS (AOT) | ✓ |
| Android (Mono/IL2CPP) | ✓ |

### Performance

| Operation | Throughput |
|-----------|-----------|
| Single generate | ~0.002 ms |
| Batch 10,000 | ~2.6 ms |
| Batch 50,000 | ~12 ms |
| Batch 200,000 | ~50 ms |
| Text generation | ~0.01 ms each |

*Measured on .NET 8, Release build, x64.*

### Project Structure

```
PersonalityGenerator/
├── PersonalityGenerator.csproj   # .NET Standard 2.0 class library
├── Core.cs                       # Parameters, Generator, Textify, Bias
├── Seed.cs                       # Seed expansion & PRNG (xorshift128+)
└── StressTest/                   # Independent stress-testing harness
```

### License

MIT

---

## 中文

# PersonalityGenerator（人格生成器）

零依赖、确定性的 .NET Standard 2.0 人格参数生成类库。  
从整数种子生成 AI 可读的角色人设文本，用于角色扮演、模拟和游戏开发。

### 概述

PersonalityGenerator 将单个整数种子转换为完整的 84 参数人格画像，然后渲染为适合 AI 角色扮演的自然语言文本。每个人格都是完全确定性的——相同的种子总是产生完全相同的角色。

### 核心特性

- **零依赖**——纯 C# 实现，无任何外部库。兼容 Unity、WASM、iOS、Android 及所有 .NET 运行时。
- **确定性**——相同种子 → 相同人格。适用于程序化生成、存档系统和可复现模拟。
- **84 个原子参数**——覆盖 8 大领域：感知注意、情绪生成、动机价值、行为执行、元认知自我、社交信号、时间性发展、身体环境耦合。
- **偏向系统**——通过简洁的字符串 DSL 控制生成倾向：`"B015=0.9,D040=-0.8,S=0.85"`。
- **参数缺失**——约 15% 的参数随机缺失，模拟真实人类的不完整性。
- **3 种输出格式**——角色扮演（AI 可读人设）、紧凑模式（仅关键特征）、详细模式（完整参数报告）。
- **久经考验**——通过 111 亿+ 人格生成的极限压力测试，零错误。
- **轻量**——发行版 DLL 仅 22 KB。

### 快速开始

```csharp
using PersonalityGenerator;

// 生成 100 个随机人格
Personality[] batch = Generator.Generate(100);

// 带偏向生成（高内疚、低攻击、强拉力）
Personality[] saints = Generator.Generate(50, "B015=0.9,D040=-0.8,S=0.85");

// 从指定种子生成单个人格
Personality p = Generator.GenerateFromSeed(42);

// 读取特定参数（归一化值 0-1）
double guilt = p.Get("B015");

// 生成 AI 可读的角色描述
string characterSheet = Textify.ToRoleplay(p);

// 紧凑摘要
string summary = Textify.ToCompact(p);

// 完整参数报告
string fullReport = Textify.ToDetailed(p);
```

### 偏向语法

```
"B015=0.9"          → 单个参数偏向（-1.0 到 +1.0）
"A=0.5"             → 整个领域偏向（A-H）
"S=0.8"             → 偏向强度（0-1，默认 0.7）
"STRENGTH=1.0"      → 完整关键词也支持
"B015=0.9,D040=-0.8,S=0.85"  → 组合使用
```

### 参数领域（共 84 个）

| 领域 | 代码 | 参数范围 | 描述 |
|------|------|---------|------|
| 感知与注意 | A | A001-A010 | 视觉扫描、威胁检测、痛苦敏感度 |
| 情绪生成与调节 | B | B011-B024 | 内疚、羞耻、嫉妒、幸灾乐祸、情绪传染 |
| 动机与价值 | C | C025-C038 | 权力、亲和、成就、欺骗接受度 |
| 行为执行 | D | D039-D042 | 攻击基线、规则遵循、冲动控制 |
| 元认知与自我 | E | E043-E055 | 自我欺骗、道德推脱、使命感 |
| 社交信号 | F | F056-F062 | 信任、背叛检测、印象管理 |
| 时间与发展 | G | G063-G066 | 人格漂移、情境切换、身份叙事 |
| 身体-环境 | H | H067-H084 | 姿势-思维关联、饥饿-风险、海拔-抽象 |

### 安装

**Unity：** 将 `PersonalityGenerator.dll` 和 `PersonalityGenerator.deps.json` 放入 `Assets/Plugins/` 目录。

**.NET 项目：**
```xml
<Reference Include="PersonalityGenerator">
  <HintPath>path/to/PersonalityGenerator.dll</HintPath>
</Reference>
```

### 平台支持

| 平台 | 状态 |
|------|------|
| .NET Framework 4.6.1+ | ✓ |
| .NET Core 2.0+ / .NET 5+ | ✓ |
| Unity 2019+ / Unity 6 | ✓ |
| WebAssembly (WASM) | ✓ |
| iOS (AOT) | ✓ |
| Android (Mono/IL2CPP) | ✓ |

### 性能

| 操作 | 吞吐量 |
|------|--------|
| 单次生成 | ~0.002 ms |
| 批量 10,000 | ~2.6 ms |
| 批量 50,000 | ~12 ms |
| 批量 200,000 | ~50 ms |
| 文本生成 | ~0.01 ms/个 |

*基于 .NET 8、Release 构建、x64 平台测试。*

### 项目结构

```
PersonalityGenerator/
├── PersonalityGenerator.csproj   # .NET Standard 2.0 类库
├── Core.cs                       # 参数定义、生成器、文本输出、偏向系统
├── Seed.cs                       # 种子扩展与伪随机数生成（xorshift128+）
└── StressTest/                   # 独立压力测试工具
```

### 设计哲学

本库描述的是"人格可能空间"而非"理想人格"。每个参数都是一条从负极到正极的连续光谱，两端都是人格空间中同等合法的存在。圣人与恶棍、利他者与剥削者——都是参数空间中的合法点。人格多样性 = 参数空间的全覆盖。

### 许可证

MIT
