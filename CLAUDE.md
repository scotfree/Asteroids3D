# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity 6 (6000.4.1f1) 3D game project — an Asteroids-style game built on the Universal Render Pipeline (URP). The project is currently at an early/blank-template stage: no gameplay scripts yet, just the configured Unity project scaffold.

## Build & Run

All development happens inside the Unity Editor — there is no CLI build step for day-to-day work.

- **Open project**: Unity Hub → Add → `/Users/scot/Projects/ai/claude/Asteroids3D`
- **Play in editor**: `Cmd+P` while in Unity, or the Play button
- **Build standalone**: File > Build Settings → select platform → Build and Run
- **CLI batch build** (if a build script exists):
  ```bash
  /Applications/Unity/Hub/Editor/6000.4.1f1/Unity.app/Contents/MacOS/Unity \
    -projectPath /Users/scot/Projects/ai/claude/Asteroids3D \
    -executeMethod BuildScript.PerformBuild \
    -quit -batchmode
  ```

## Testing

Unity Test Framework (v1.6.0) is included. Tests run via **Window > General > Test Runner** inside the Unity Editor. There are no tests yet.

## Architecture

### Technology Stack

| Layer | Choice |
|---|---|
| Engine | Unity 6.0.4.1f1 |
| Render pipeline | URP 17.4.0 |
| Input | New Input System 1.19.0 |
| Language | C# (netstandard2.1) |
| Primary target | macOS (StandaloneOSX) |

### Key Directories

- `Assets/` — all game code (C# scripts) and assets go here
- `Assets/Scenes/SampleScene.unity` — the one and only scene; all game objects live here
- `Assets/Settings/` — URP render pipeline assets (separate PC and Mobile configs)
- `Assets/InputSystem_Actions.inputactions` — input action bindings for the New Input System
- `Packages/manifest.json` — Unity package dependencies
- `ProjectSettings/` — Unity player/platform/graphics settings (YAML, Unity-managed)

### Rendering

Dual URP configs are set up:
- **PC**: `PC_RPAsset.asset` / `PC_Renderer.asset` — higher quality settings
- **Mobile**: `Mobile_RPAsset.asset` / `Mobile_Renderer.asset` — performance-optimised

Post-processing is handled via Volume Profiles attached to the scene.

### Input

The New Input System is active. All input bindings are defined in `InputSystem_Actions.inputactions`. Player scripts should use `PlayerInput` component or `InputAction` API, not the legacy `Input.*` static methods.

### Unity Editor Workflow Notes

- **Always save the scene after wiring up objects**: `Cmd+S`. Unity does not auto-save. Changes to the Hierarchy and Inspector are lost if you close without saving.
- **Play mode does not save**: any changes made while the game is running are discarded when you stop. Wire up objects outside of Play mode.
- **The user is new to Unity**: call out steps that are automatic for experienced Unity developers — saving, where to find panels, how to add components, what "resetting a Transform" means, etc.

### Known Unity Editor Input Quirks

- **TAB is consumed by the Unity editor UI** even during Play mode — do not bind gameplay actions to TAB. Use other keys instead.
- **Left mouse click releases the cursor lock** in the editor when the cursor is locked — Shift keys are more reliable for fire bindings during editor testing.

### Adding Game Code

All C# game scripts go in `Assets/` (or subdirectories within it). Unity auto-compiles anything in `Assets/` into `Assembly-CSharp`. Editor-only scripts go in `Assets/Editor/` or any folder named `Editor`.

## Decisions & Constraints

- **URP only** — do not use the Built-in Render Pipeline or HDRP; all shaders and materials must be URP-compatible.
- **New Input System only** — legacy `Input.*` API is disabled; use `InputAction` or `PlayerInput`.
- **Single scene** — gameplay is contained in `SampleScene.unity`; add new scenes only if there's a clear reason (e.g., main menu, separate level).
