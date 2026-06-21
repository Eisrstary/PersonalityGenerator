# PersonalityGenerator

A zero-dependency, deterministic personality parameter generation library for .NET Standard 2.0.  
Generates AI-readable character profiles from integer seeds for role-playing, simulation, and game development.

## Overview

PersonalityGenerator transforms a single integer seed into a complete 84-parameter personality profile, then renders it as natural language text suitable for AI-driven character扮演 (role-play). Every personality is fully deterministic — the same seed always produces the exact same character.

## Key Features

- **Zero Dependencies** — Pure C# implementation. No external libraries. Compatible with Unity, WASM, iOS, Android, and any .NET runtime.
- **Deterministic** — Same seed → same personality. Perfect for procedural generation, save systems, and reproducible simulations.
- **84 Atomic Parameters** — Covers 8 domains: perception, memory, cognition, emotion, motivation, behavior, social signals, meta-cognition, embodiment, and temporal development.
- **Bias System** — Steer generation toward specific traits with a simple string-based DSL: `"B015=0.9,D040=-0.8,S=0.85"`.
- **Partial Missing** — ~15% of parameters are randomly omitted to simulate realistic human incompleteness.
- **3 Output Formats** — Roleplay (AI-ready character sheet), Compact (key traits only), Detailed (full parameter report).
- **Battle-Tested** — Validated through 111 billion+ personality generations with zero errors.
- **Lightweight** — Release DLL is only 22 KB.

## Quick Start

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

## Bias DSL Syntax

```
"B015=0.9"          → Single parameter bias (-1.0 to +1.0)
"A=0.5"             → Entire domain bias (A-H)
"S=0.8"             → Bias strength (0-1, default 0.7)
"STRENGTH=1.0"      → Full keyword also supported
"B015=0.9,D040=-0.8,S=0.85"  → Combined
```

## Parameter Domains (84 total)

| Domain | Code | Parameters | Description |
|--------|------|-----------|-------------|
| Perception & Attention | A | A001-A010 | Visual scanning, threat detection, pain sensitivity |
| Memory & Association | — | — | (embedded in other domains) |
| Emotion Generation | B | B011-B024 | Guilt, shame, envy, schadenfreude, emotional contagion |
| Motivation & Values | C | C025-C038 | Power, affiliation, achievement, deception acceptance |
| Behavior Execution | D | D039-D042 | Aggression baseline, rule compliance, impulsivity |
| Meta-Cognition | E | E043-E055 | Self-deception, moral disengagement, sense of mission |
| Social Signals | F | F056-F062 | Trust, betrayal detection, impression management |
| Temporal Development | G | G063-G066 | Personality drift, context switching, identity narrative |
| Body-Environment | H | H067-H084 | Posture-mind links, hunger-risk, altitude-abstraction |

## Installation

### Unity
Copy `PersonalityGenerator.dll` and `PersonalityGenerator.deps.json` into `Assets/Plugins/`.

### .NET Project
```xml
<Reference Include="PersonalityGenerator">
  <HintPath>path/to/PersonalityGenerator.dll</HintPath>
</Reference>
```

## Platform Support

| Platform | Status |
|----------|--------|
| .NET Framework 4.6.1+ | ✓ |
| .NET Core 2.0+ / .NET 5+ | ✓ |
| Unity 2019+ / Unity 6 | ✓ |
| WebAssembly (WASM) | ✓ |
| iOS (AOT) | ✓ |
| Android (Mono/IL2CPP) | ✓ |

## Performance

| Operation | Throughput |
|-----------|-----------|
| Single generate | ~0.002 ms |
| Batch 10,000 | ~2.6 ms |
| Batch 50,000 | ~12 ms |
| Batch 200,000 | ~50 ms |
| Text generation | ~0.01 ms each |

*Measured on .NET 8, Release build, x64.*

## Project Structure

```
PersonalityGenerator/
├── PersonalityGenerator.csproj   # .NET Standard 2.0 class library
├── Core.cs                       # Parameters, Generator, Textify, Bias
├── Seed.cs                       # Seed expansion & PRNG (xorshift128+)
└── StressTest/                   # Independent stress-testing harness
```

## License

MIT
