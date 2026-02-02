# Tangram-Unity-Game
Tangram game with cross-platform 

# 🧩 Unity Puzzle Interaction System

This project demonstrates an interactive puzzle system built in Unity, designed with a kid-friendly and polished experience in mind.

## ✨ Features

### 🧩 Puzzle Pieces
- Drag & drop puzzle pieces using mouse (desktop) or touch (mobile)
- Pieces lift slightly and scale up while being dragged
- Smooth snap-to-target when placed correctly
- Automatic return animation when placed incorrectly
- Rotation support:
  - Right-click on desktop
  - Double-tap on mobile

### 🎯 Visual Feedback
- Target highlight when a piece is close to the correct position
- Subtle scaling and movement for better interaction feedback
- Celebration animation when the puzzle is completed

### 🔊 Audio Feedback
- Sound effects for:
  - Piece rotation
  - Successful snapping
  - Puzzle completion
- Slight random pitch variation for more natural sound feel

### 🏁 Puzzle Completion
- PuzzleManager checks when all pieces are correctly placed
- Triggers win effects and audio on completion

---

## Project Structure

Assets/
├── Scripts/
│   ├── Piece.cs
│   ├── PuzzleManager.cs
│   └── AudioManager.cs
├── Prefabs/
├── Materials/
└── Audio/


## 🚀 How to Run

1. Open the project in Unity (Any Unity 6 LTS recommended)
2. Open the Sample scene
3. Press **Play**
4. Drag pieces into place and complete the puzzle 🎉

---

## 🧠 Design Notes

- Input handling supports both desktop and mobile platforms
- Audio and puzzle logic are centralized for clean architecture
- Extra polish added for accessibility and child-friendly interaction

---

## 📌 Possible Improvements
- Add hint system for younger players
- Timer or scoring system
- Multiple puzzle levels
- Particle effects per piece snap

---

##
Created as part of an interactive puzzle assignment using Unity.
