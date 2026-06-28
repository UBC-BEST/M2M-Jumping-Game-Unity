# M2M Jumping Game (Unity)

A 2D vertical endless platformer built with Unity. The player auto-bounces upward by landing on platforms, steers left and right, and climbs as high as possible before falling or hitting a hazard. The project was developed by [UBC-BEST](https://github.com/UBC-BEST) as a foundation for future feature work.

## Game Overview

### Core Loop

1. The player character (**Jumper**) spawns at the bottom of the level.
2. Landing on a platform applies an upward velocity (automatic jump).
3. The player moves horizontally with keyboard input while gravity pulls them down between platforms.
4. The camera follows the player upward only — it never moves back down.
5. The run ends when the player falls below the death zone or touches a hazard.
6. A game-over screen appears with options to restart or quit.

### Controls

| Input | Action |
|-------|--------|
| `A` / `Left Arrow` | Move left |
| `D` / `Right Arrow` | Move right |

There is no manual jump button. Jumping is handled automatically on platform contact.

### Platform Types

| Prefab | Script | Behavior |
|--------|--------|----------|
| `Platform.prefab` | `Platformer` | Standard one-way platform. Bounces the player upward on landing. |
| `movingPlatform.prefab` | `MovingPlatform` | Bounces like a normal platform and oscillates horizontally. |
| `breakingPlatform.prefab` | `BreakingPlatform` | Bounces once, then destroys itself after a short delay. |
| `spikeyPlatform.prefab` | `SpikePlatform` | Kills the player on contact. Does not bounce. |

All bounce platforms use a `PlatformEffector2D` (one-way surface) plus an `EdgeCollider2D` so the player can pass through from below but land on top.

### Death Conditions

- **Fall death:** Player Y position drops below `-10` (see `GameManager.GetDeathZone()`).
- **Hazard death:** Collision with any object tagged `Hazard`, or contact with a `SpikePlatform`.
- **Game over:** Time scale is set to `0`, and the `GameOverPanel` UI is shown.

---

## Tech Stack

| Item | Version / Details |
|------|-------------------|
| Engine | Unity **2022.3.59f1** (LTS) |
| Language | C# |
| Rendering | 2D (SpriteRenderer, Tilemap assets included) |
| UI | Unity UI (uGUI) + TextMeshPro |
| Physics | Unity 2D Physics (`Rigidbody2D`, `Collider2D`) |
| Input | Legacy Input Manager (`Input.GetAxis`) |

### Key Packages (`Packages/manifest.json`)

- `com.unity.feature.2d` — 2D tooling and sprites
- `com.unity.textmeshpro` — UI text
- `com.unity.inputsystem` — installed but **not currently wired up** in scripts
- `com.unity.cinemachine` — installed but **not currently used** (camera uses custom `CameraFollow`)

---

## Project Structure

```
/
├── Assets/
│   ├── Scenes/
│   │   └── SampleScene.unity      # Main (and only) playable scene
│   ├── Platform.prefab            # Default platform
│   ├── movingPlatform.prefab
│   ├── breakingPlatform.prefab
│   ├── spikeyPlatform.prefab
│   ├── Bouncy.physicsMaterial2D   # Physics material on the player
│   ├── Tilemap/                   # Monochrome tile assets + palette (not used in core loop yet)
│   ├── GameManager.cs
│   ├── PlayerController.cs
│   ├── Platformer.cs
│   ├── MovingPlatform.cs
│   ├── BreakingPlatform.cs
│   ├── SpikePlatform.cs
│   ├── CameraFollow.cs
│   └── GameOverUI.cs
├── Packages/
├── ProjectSettings/
└── README.md
```

---

## Architecture

### Script Responsibilities

```
GameManager (singleton)
├── Procedural platform generation at Start()
├── Clamps vertical gaps to stay within jump reach
├── Game-over state + time freeze
└── Scene restart / quit

PlayerController
├── Horizontal movement (Update + FixedUpdate)
├── Death zone check
└── Hazard collision handling

Platformer (base class)
└── Applies jumpForce on downward collision

MovingPlatform : Platformer
└── Horizontal PingPong movement

BreakingPlatform : Platformer
└── Destroys self after bounce

SpikePlatform
└── Calls PlayerController.Die() on contact

CameraFollow
└── SmoothDamp upward-only follow of player

GameOverUI
└── Restart / Quit button wiring
```

### Platform Generation (`GameManager`)

On `Start()`, the game spawns **300 platforms** procedurally:

- Each platform is placed above the previous one by a random vertical gap (`minGapY` to `maxGapY`).
- Horizontal position is randomized between `-0.2` and `0.9`.
- Gaps are clamped using the physics formula `H = v² / (2g)` so every platform remains reachable.
- **70%** of spawns use the default `platformPrefab`; **30%** pick randomly from `platformPrefabs`.

Current scene defaults (Inspector on `GameManager`):

| Field | Value |
|-------|-------|
| `count` | 300 |
| `minGapY` | 0.3 |
| `maxGapY` | 0.7 |
| `jumpForce` (on platform prefab) | 3 |
| Player `moveSpeed` | 3 |
| Player `gravityScale` | 0.5 |

### Scene Hierarchy (`SampleScene`)

| GameObject | Components / Role |
|------------|-------------------|
| **Jumper** | Player sprite, `Rigidbody2D`, `BoxCollider2D`, `PlayerController` |
| **Main Camera** | `Camera`, `CameraFollow` (target = Jumper) |
| **GameManager** | `GameManager` — spawns platforms at runtime |
| **Platform** | Starting platform instance (prefab override in scene) |
| **Canvas / GameOverPanel** | UI with restart/quit buttons and `GameOverUI` |
| **EventSystem** | Required for UI interaction |

---

## Getting Started

### Prerequisites

- [Unity Hub](https://unity.com/download)
- Unity Editor **2022.3.59f1** (install via Hub → Installs → Add → select 2022.3 LTS)

### Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/UBC-BEST/M2M-Jumping-Game-Unity.git
   cd M2M-Jumping-Game-Unity
   ```

2. Open the project in Unity Hub (**Add → select project folder**).

3. Open `Assets/Scenes/SampleScene.unity`.

4. Press **Play**.

Unity will regenerate the `Library/` folder on first open. This folder is gitignored and is expected.

### First-Time Inspector Checklist

Before extending the game, verify these scene references in the Inspector:

1. **GameManager → Platform Prefabs**
   - Assign `movingPlatform`, `breakingPlatform`, and `spikeyPlatform` to the `platformPrefabs` list.
   - Without this list, only default platforms spawn and a console error is logged.

2. **GameManager → Game Over UI**
   - Should reference the `GameOverPanel` GameObject (already wired in the current scene).

3. **Main Camera → CameraFollow → Target**
   - Should reference the Jumper's Transform (already wired).

4. **Canvas → GameOverPanel → GameOverUI**
   - Wire `scoreText`, `restartButton`, and `quitButton` if adding score tracking.

---

## Extending the Game

### Adding a New Platform Type

1. Duplicate `Platform.prefab` in `Assets/`.
2. Create a new script that inherits from `Platformer` (or `MonoBehaviour` for hazards):
   ```csharp
   public class MyPlatform : Platformer
   {
       protected override void OnCollisionEnter2D(Collision2D collision)
       {
           if (collision.relativeVelocity.y <= 0f)
           {
               base.OnCollisionEnter2D(collision);
               // Custom behavior here
           }
       }
   }
   ```
3. Attach the script to the prefab. Keep `PlatformEffector2D` + `EdgeCollider2D` for bounce platforms.
4. Add the prefab to `GameManager.platformPrefabs`.

### Adding a Score System

`GameOverUI` has a placeholder for score display:

```csharp
// GameOverUI.cs — currently shows "Game Over!"
scoreText.text = "Game Over!";
// Intended integration:
// scoreText.text = "Score: " + ScoreManager.Instance.GetScore();
```

Suggested approach:

1. Create a `ScoreManager` singleton that tracks `Mathf.FloorToInt(player.transform.position.y)`.
2. Update the score in `Update()` while the game is not over.
3. Display the final score in `GameOverUI.OnEnable()`.

### Tuning Difficulty

| Parameter | Location | Effect |
|-----------|----------|--------|
| `jumpForce` | Platform prefabs (`Platformer`) | Max jump height |
| `minGapY` / `maxGapY` | `GameManager` | Vertical spacing between platforms |
| `count` | `GameManager` | Total level height |
| `moveSpeed` | `PlayerController` | Horizontal control responsiveness |
| `gravityScale` | Jumper `Rigidbody2D` | Fall speed |
| Special platform spawn rate | `GameManager.Start()` | Change the `0.3f` threshold (currently 30% special) |

When changing `jumpForce` or gravity, the gap clamping in `GameManager` will auto-adjust at runtime — but test in Play mode to confirm platforms feel fair.

### Building for Release

1. **File → Build Settings**
2. Confirm `Assets/Scenes/SampleScene.unity` is in **Scenes In Build**.
3. Select target platform and click **Build**.

---

## Known Gaps & Improvement Opportunities

These are intentional stubs or incomplete wiring that new developers should be aware of:

| Area | Status | Notes |
|------|--------|-------|
| Score system | Not implemented | UI hook exists in `GameOverUI` |
| `platformPrefabs` list | May be empty in scene | Must be assigned in Inspector for special platforms |
| `Hazard` tag | Defined but unused on prefabs | `PlayerController` checks tag; spikes use `SpikePlatform` script instead |
| New Input System | Package installed, not used | Scripts use legacy `Input.GetAxis("Horizontal")` |
| Cinemachine | Package installed, not used | Custom `CameraFollow` handles camera |
| Tilemap assets | Present in `Assets/Tilemap/` | Not integrated into procedural generation |
| Object pooling | Not implemented | 300 platforms are `Instantiate`d at start |
| Moving platform parenting | Not implemented | Player may not inherit platform velocity when standing on movers |
| Tests | None | No automated test coverage yet |

---

## Git Workflow

- **Main branch:** `main` — stable, playable baseline
- **Feature branches:** Use descriptive names (e.g. `feature/score-system`)
- **Ignored by git:** `Library/`, `Temp/`, `Logs/`, `UserSettings/`, build outputs (see `.gitignore`)

Typical workflow for new work:

```bash
git checkout main
git pull origin main
git checkout -b feature/your-feature-name
# make changes, test in Unity Play mode
git add .
git commit -m "Describe your change"
git push -u origin feature/your-feature-name
# Open a pull request on GitHub
```

---

## Troubleshooting

| Problem | Likely Cause | Fix |
|---------|--------------|-----|
| Only plain platforms appear | `platformPrefabs` list is empty | Assign special prefabs on `GameManager` |
| Player falls through platforms | Missing `PlatformEffector2D` or wrong collision setup | Compare with `Platform.prefab` |
| Player cannot reach next platform | Gaps too large for current `jumpForce` | Lower `maxGapY` or increase `jumpForce` |
| Game over UI does not appear | `gameOverUI` reference missing on `GameManager` | Assign `GameOverPanel` in Inspector |
| UI buttons do not work after death | `Time.timeScale = 0` | Buttons use `GameManager.RestartGame()` which resets time scale — ensure listeners are wired |
| Console error about empty prefab list | Expected until `platformPrefabs` is configured | See First-Time Inspector Checklist |

---

## License

No license file is included in this repository. Confirm licensing with the UBC-BEST maintainers before distributing or reusing the project.

---

## Contact / Ownership

- **Repository:** [UBC-BEST/M2M-Jumping-Game-Unity](https://github.com/UBC-BEST/M2M-Jumping-Game-Unity)
- **Organization:** UBC-BEST

For questions about game design intent or deployment targets, reach out to the repository maintainers via GitHub Issues.
