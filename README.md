# VR Unity Job Simulator – Mechanics Explained

Think of a small cabinet with an elevator-like door and a vertical selector on the left. Your hand moves a round knob up and down; when it touches one of five marked segments, the cabinet switches to the matching level and shows its content by opening the doors.

## Overview
- Left side input: a `Knob` moves vertically and “touches” one of five `LeverSeg` segments.
- Right side: two doors (`DoorTop`, `DoorBottom`) close together, content switches, then the doors open together.
- Content: five `Level` cubes. Switching drops the previous front cube slightly and brings the new cube forward.

## How It Works (Plain English)
- The knob is like a finger moving up and down a ruler. The ruler has five labeled zones (segments).
- Every frame, we ask: “Is the finger overlapping any labeled zone?” If yes, we pick that zone’s number.
- When the number changes, we shut the doors so the audience can’t see the change.
- Behind closed doors, we move the old item down a bit, reset all items, and slide the new item forward.
- Then we open the doors again to reveal the new item.

## Scene Hierarchy
```
CabinetRoot  (attach CabinetMenuController)
  LeftSide
    LeverBar
      LeverSeg_0 … LeverSeg_4 (BoxCollider, Tag=CabinetMenu)
    Knob (SphereCollider + Rigidbody)
  RightSide
    DoorTop   (Transform)
    DoorBottom(Transform)
  Content
    Level_0 … Level_4 (Transform)
```

## Inspector Mapping
- `CabinetMenuController.knob` → `LeftSide/Knob`
- `CabinetMenuController.leverSegments` → 5 `LeverSeg_*` BoxColliders in order (top→bottom)
- `CabinetMenuController.levelTransforms` → 5 `Level_*` Transforms in the same order
- `CabinetMenuController.segmentTag` → `CabinetMenu`
- `CabinetMenuController.doorTopClosedLocal` / `doorTopOpenLocal`
- `CabinetMenuController.doorBottomClosedLocal` / `doorBottomOpenLocal`
- Optional tunables: `probeRadius`, `forwardOffset`, `downOffset`, `doorTime`, `contentTime`

## Key Mechanics (with Code Pointers)
- Detect knob overlap with lever segments (tag filtered):
  - `Assets/Scripts/CabinetMenuController.cs:47–57` – `Physics.OverlapSphere(knob.position, probeRadius)` and `CompareTag(segmentTag)`
  - Print on hit: `trigger level and it's number: i` (`Assets/Scripts/CabinetMenuController.cs:56`)
- Only switch when index changes and no animation is running:
  - `Assets/Scripts/CabinetMenuController.cs:61`
- Double-door synchronized animation (close and open together):
  - Close: `Assets/Scripts/CabinetMenuController.cs:76`
  - Open:  `Assets/Scripts/CabinetMenuController.cs:93`
  - Pair animation helper: `Assets/Scripts/CabinetMenuController.cs:111–124`
- Content transition:
  - Previous level down: `Assets/Scripts/CabinetMenuController.cs:79–82`
  - Reset all, new level forward: `Assets/Scripts/CabinetMenuController.cs:83–90`

## Feynman-style Explanation
- Why collide instead of measuring distance? Humans naturally understand “touching.” If your finger overlaps a marked area, it’s selected. No angles or thresholds to memorize—only contact.
- Why close doors before switching? Like a magician’s curtain: hide the messy rearrangement, then reveal the result cleanly.
- Why move the old cube down and the new forward? The audience needs a cue that content changed. A small down-and-forward shift reads as “old exits, new enters” to our eyes.
- Why synchronize top and bottom doors? If one door moves sooner, the brain feels jitter. Synchronized motion reduces perceived latency and looks intentional.

## Setup Tips
- `Knob` movement: constrain Rigidbody to allow only Y movement (freeze X/Z position and all rotations).
- Segment spacing: ensure segments are vertically separated enough so `probeRadius` only hits one at a time.
- Doors: use local positions for open/closed targets; avoid non-kinematic Rigidbodies on doors if you animate transforms.

## Minimal Audio (Buttons)
- `CashMachineButton` uses a minimal audio approach: assign an `AudioSource` to the button and it plays on press.
  - Play on press: `Assets/Scripts/CashMachineButton.cs:45`
  - Auto-get source: `Assets/Scripts/CashMachineButton.cs:30–31`

## Troubleshooting
- “Detected segment but doors don’t move”: verify door Transforms are bound and `Open/Closed` local positions differ.
- “One door faster than the other”: both doors now animate together via `AnimatePair`. If speeds still feel different, match open/closed distances.
- “Switch retriggers endlessly”: switching is gated by `routine==null` and index change.

## Files of Interest
- `Assets/Scripts/CabinetMenuController.cs`
- `Assets/Scripts/CashMachineButton.cs`

## Unity Git Practices
- Keep `Assets/`, `ProjectSettings/`, `Packages/` tracked.
- Ignore Unity cache folders in `.gitignore` (e.g., `Library/`, `Temp/`).
- Track large binary assets (audio, textures, models) with Git LFS as specified in `.gitattributes`.
