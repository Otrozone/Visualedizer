# Visualedizer Client Scene Effect Skill

Use this guide when adding a new C# client scene effect under `Client/Visualedizer`.

## Purpose

The client scene system has five main responsibilities:

1. Define the scene type and scene-specific config.
2. Persist the config to `config.json`.
3. Expose an editor form in the WinForms UI.
4. Render the scene into LED frames in `SceneRunners.cs`.
5. Keep summaries, previews, and runtime behavior in sync.

This skill is for adding a new scene cleanly without missing one of those integration points.

## Where This Skill Lives

This `SKILL.md` is intentionally in `Client/Visualedizer/` because the scene effect architecture is specific to the C# client project, not the whole workspace.

## Core Files

- `SceneConfig.cs`
  Add the `SceneType` enum value, the scene config class, clone behavior, display name, and summary text.
- `AppConfig.cs`
  Add save/load support for persisted scene fields.
- `FrmMain.cs`
  Register the editor form, route previews if needed, and make sure the scene is shown correctly.
- `SceneRunners.cs`
  Add the runtime rendering logic inside `CompositeSceneRunner`.
- `ISceneEditorForm.cs`
  Editor contract used by hosted scene forms.
- `*SceneEditorForm.cs` and `*.Designer.cs`
  Scene-specific WinForms editor UI.

## Standard Implementation Flow

### 1. Add the Scene Type and Config

In `SceneConfig.cs`:

- Add a new `SceneType` enum value.
- Add a config object property to `SceneConfig`.
- Add clone support in `SceneConfig.Clone()`.
- Create a dedicated scene config class with sensible defaults.
- Add a human-readable display name in `SceneTypeNames.GetDisplayName`.
- Add a concise summary in `SceneSummaryBuilder.Build`.

Guidelines:

- Persisted config belongs here.
- Session-only or live runtime state may also live here if it must be shared between the editor and runner, but do not serialize it in `AppConfig`, and do not include it in scene summaries.
- If runtime-only fields are added, make sure `Clone()` does not copy transient state unless that is explicitly intended.

### 2. Persist Only Real Config

In `AppConfig.cs`:

- Add serializable scene fields to the scene config model.
- Clamp loaded values when needed.

Rules:

- Do not persist temporary UI state.
- Do not persist runtime control flags like pause, seek requests, preview state, or cached selections unless the feature explicitly requires that behavior.

### 3. Create the Editor Form

Create a new form modeled after the existing scene editors:

- Namespace should be `Ledqualizer`.
- Form should implement `ISceneEditorForm`.
- Constructor should set:
  - `FormBorderStyle = None`
  - `TopLevel = false`
  - `Dock = DockStyle.Fill`
- Keep a `CurrentScene` reference.
- Use an `isLoading` guard to avoid firing updates while populating controls.
- Raise `SceneChanged` when persisted config changes.

Recommended editor pattern:

- `LoadScene(SceneConfig scene)` copies current config values into controls.
- Control change handlers update `CurrentScene.<YourConfig>` directly.
- Use dedicated helper methods like:
  - `UpdateControlStates()`
  - `UpdateSceneFromControls()`
  - `SelectXxx(...)`

If the scene has preview behavior:

- Prefer a dedicated `UpdatePreview(...)` method on the form.
- For capture-like scenes, use `CaptureScenePreview` routing through `FrmMain.UpdatePreview`.

### 4. Register the Editor in FrmMain

In `FrmMain.InitializeSceneEditors()`:

- Instantiate the new editor.
- Wire `SceneChanged += Editor_SceneChanged`.
- Add any extra scene-specific events if needed.
- Register it in `sceneEditors[SceneType.YourScene]`.
- The shared scene grid already picks up new `SceneType` enum values automatically.

Also check:

- `ShowSelectedSceneEditor()`
  Usually no special work is needed beyond editor registration, unless the scene needs special overlay or side effects.
- `UpdatePreview(...)`
  If the scene produces previews, route them to the correct editor here.

### 5. Add Runtime Rendering

In `SceneRunners.cs`, the real device output is built inside `CompositeSceneRunner`.

Typical steps:

- Extend the `BuildAssignmentFrame(...)` switch.
- Add a scene-specific frame builder.
- Return a `byte[]` sized to `ledCount * 3`.
- Use `OverlayFrame(...)` behavior already in place for strip assignments.

Choose the right runtime model:

- Stateless scene:
  Use only current config and `ledCount`.
- Stateful scene:
  Add per-scene state inside `CompositeSceneRunner`, keyed by `scene.Id`.

Use stateful scenes when the effect has:

- progression over time
- playback position
- cached assets
- random values that must stay stable for a pass
- live controls like pause/seek

For stateful scenes:

- Reset state when real config changes.
- Build a config signature from persisted behavior fields only.
- Exclude runtime-only fields like pause or pending seek from the reset signature unless you intentionally want device runs to restart.

### 6. Handle Previews Deliberately

If the scene needs UI preview:

- Use `CaptureScenePreview` for capture-like or sampled-image effects.
- Include enough metadata for the editor to render the overlay meaningfully:
  - `SceneId`
  - `Colors`
  - `SourcePath` when relevant
  - `SourceSize`
  - `SampleIndex`
  - `Direction`
- Route preview updates through `FrmMain.UpdatePreview(...)`.

Important:

- Preview routing must be scene-aware, not just scene-type-aware, if multiple scenes of the same kind can be active.

### 7. Build and Smoke Test

Minimum verification:

- `dotnet build Client/Visualedizer/Visualedizer.csproj`

If the main exe is locked by a running app, build to an alternate output folder:

```powershell
dotnet build Client/Visualedizer/Visualedizer.csproj -p:OutDir=c:\Users\cek\source\repos\Visualedizer\tmpbuild\
```

Manual checks:

- New scene appears in the scene type dropdown.
- Editor loads when the scene is selected.
- Summary updates when values change.
- Scene survives save/reload if it has persisted config.
- Assigned devices render the expected output.
- Preview behavior matches runtime behavior.

## Patterns Already Present in This Repo

### Simple Scene

Examples:

- `SolidColor`
- `Gradient`

Characteristics:

- Small config object
- Small editor
- Pure frame generation from current config

### Audio Reactive Scene

Examples:

- `VolumeReactive`
- `SpectralAnalysis`

Characteristics:

- Shared config patterns
- Progress reporting
- More specialized runner helpers

### Capture / Sampled Scene

Examples:

- `ScreenRowCapture`
- `ImageRowCapture`

Characteristics:

- Preview routing through `CaptureScenePreview`
- Potential runtime state
- More editor-to-runner coordination

If the new scene is visually sampled from an external source, model it after these scenes.

## Common Mistakes to Avoid

- Adding the config class but forgetting `SceneConfig.Clone()`.
- Persisting runtime-only control state in `AppConfig`.
- Changing runtime-only fields in a way that forces run restarts.
- Forgetting `SceneSummaryBuilder`, which leaves blank or stale summaries.
- Forgetting editor registration in `FrmMain.InitializeSceneEditors()`.
- Updating controls during `LoadScene()` without an `isLoading` guard.
- Building preview behavior that does not match the actual runner logic.
- Using scene type instead of `scene.Id` for runtime state or preview routing when multiple instances can exist.
- Returning the wrong frame length from the runner.

## Decision Rules

When adding a new scene effect:

- Put persisted behavior in `SceneConfig` + `AppConfig`.
- Put live-only behavior in runtime fields that are not saved.
- Use stateless rendering unless the effect truly needs continuity between ticks.
- Reuse preview routing patterns if the effect has a sampled or inspectable source.
- Keep editor forms narrow and event-driven; do not duplicate scene state outside `CurrentScene` unless it is preview-only UI state.

## Suggested Checklist

- Add `SceneType`
- Add scene config class
- Add config property to `SceneConfig`
- Add clone support
- Add display name
- Add summary
- Add JSON serialization/import support
- Create editor form
- Register editor in `FrmMain`
- Add runner switch case
- Add rendering/state logic
- Add preview routing if needed
- Build
- Manual smoke test
