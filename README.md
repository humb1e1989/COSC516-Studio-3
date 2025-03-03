# 🎮 **Third Person Controller Game - _COSC516 Studio 3_**

## 🎬 Video Demo

<video src="https://github.com/user-attachments/assets/b9d0ecf6-2680-4dce-82ce-85ce7edc4430"></video>
_Screenshot of the game in action_

## 📝 Overview
This project implements a 3D third-person platformer game using Unity. The game features a **character controller** that allows the player to navigate through a **procedurally generated environment**, collecting coins while using platforming mechanics such as **double jumping** to reach higher areas.

## ✨ Features

### 🕹️ Character Controller
- **Smooth third-person movement** using WASD keys
- **Camera-relative movement** (character moves in the direction the camera is facing)
- Run functionality (hold Shift to move faster)
- **Double jump capability** (press Space while in the air after an initial jump)
- Ground detection system that works with any platform

### 🏞️ Environment
- **Procedurally generated platforms** with varying heights and sizes
- Different types of platforms:
  - Static platforms (magenta)
  - **Moving platforms** that travel back and forth (blue)
  - **Rotating platforms** that spin around their center (green)
  - High-altitude platforms (yellow)
- **Invisible walls** to prevent the player from falling off the edges of the game world
- **Boxes to jump on** strategically placed throughout the environment

### 💰 Collectibles and Scoring
- **Rotating collectible coins** that can be gathered by the player
- **Score UI** that displays the number of coins collected
- Coins are strategically placed on platforms of varying difficulty

### 📷 Camera System
- **Third-person camera** using Cinemachine's FreeLook Camera
- Smooth camera rotation around the player using mouse input
- Camera collision prevention to ensure visibility of the player

## 🎛️ Controls
- **WASD**: Move the player
- **Mouse**: Control the camera angle
- **Space**: Jump (press once while grounded for a normal jump, again in the air for a double jump)
- **Shift**: Run (hold to move faster)

## 🛠️ Implementation Details

### 🏃‍♂️ Player Controller
The player controller is implemented using Unity's physics system with a Rigidbody component. Key features include:
- Physics-based movement with **adjustable speed parameters for tight platforming**
- **Custom ground detection** using raycasts
- Double jump state management
- Camera-relative movement calculations

### 🧩 Platform Generation
Platforms are procedurally generated at runtime with the following characteristics:
- **Random positioning** within the game area
- Varied sizes and heights
- Different platform types with unique behaviors
- Coverage percentage to ensure a playable level layout
- **Collision detection** to prevent overlapping platforms

### ⌨️ Input System
The game uses an **event-based input system** that:
- Separates input detection from character control logic
- Uses UnityEvents to communicate between components
- Enables easy extension with additional input types

## 💻 Technical Requirements
- Unity 6000.0.31f1 or compatible version
- Universal Render Pipeline (URP) for visual effects
- **Cinemachine package** for camera controls

## 🔧 Tuning for Platforming
- **Movement parameters carefully adjusted** for precise platform navigation
- Jump height and distance calibrated for challenging but fair gameplay
- Ground detection fine-tuned to work reliably on all platform types
- Movement momentum balanced to allow for tight control on small platforms

## 👏 Credits
- **_LostboiSurviveA1one_** made this 
- Built using Unity Engine
