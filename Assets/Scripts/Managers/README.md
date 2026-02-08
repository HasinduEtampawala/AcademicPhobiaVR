# Manager Scripts

This folder contains core manager scripts for the VR Phobia Management System.

## Scripts Overview

### LevelManager.cs

Controls the visibility of student groups based on the current exposure level.

**Levels:**
| Level | Description | Students |
|-------|-------------|----------|
| 1 | Empty Classroom | 0 |
| 2 | Small Group | 5 |
| 3 | Medium Class | 12 |
| 4 | Full Classroom | 20 |

**Public Methods:**

- `SetLevel(int level)` - Set exposure level (1-4)
- `GetCurrentLevel()` - Get current level number

### SessionManager.cs

Manages the VR practice session state and timing.

**Public Methods:**

- `StartSession(int level)` - Start a new session
- `EndSession()` - End current session
- `PauseSession()` - Pause the session
- `ResumeSession()` - Resume paused session

### VRInputManager.cs

Handles Meta Quest 2 controller input.

**Controller Mapping:**
| Button | Action |
|--------|--------|
| A | Confirm/Select |
| B | Back/Exit |
| Trigger | UI Click |

## Setup Instructions

1. Create empty GameObject named "GameManager"
2. Attach all three scripts
3. Connect references in Inspector
4. Connect UI buttons to call SetLevel methods

## Dependencies

- UnityEngine
- TMPro (TextMeshPro)
- Meta XR SDK

## Author

DulakshiniDharmarathne

## Last Updated

February 2026
