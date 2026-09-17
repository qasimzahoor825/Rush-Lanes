# Rush Lanes

A 3D endless runner game built in Unity.

## Project overview
This project is a personal Unity game prototype inspired by endless runner mechanics. The gameplay includes lane switching, jumping, sliding, coins, obstacles, power-ups, mission progression, and score tracking.

## Features
- 3-lane endless runner gameplay
- Swipe and keyboard controls
- Coin collection and score system
- Obstacles with jump/slide avoidance
- Power-ups: shield, magnet, and speed boost
- Mission system and persistent save data
- Main menu, tutorial, pause, and game-over screens

## Open in Unity
1. Open Unity Hub
2. Open this project folder:
   `C:\Users\Lenovo\Desktop\3D-Endless-Runner-in-Unity`
3. Wait for Unity to import project files
4. Create or open a scene in `Assets/Scenes`
5. Press Play to run the game

## Controls
- Move left: `A` / Left Arrow / swipe left
- Move right: `D` / Right Arrow / swipe right
- Jump: `W` / Up Arrow / Space / swipe up
- Slide: `S` / Down Arrow / swipe down

## Project structure
- `Assets/Scripts` — gameplay logic and UI
- `Assets/Scenes` — Unity scenes
- `ProjectSettings` — Unity project settings
- `Packages` — Unity package configuration

## Notes
- This project is configured for Unity 6000.6.0f1.
- The editor can auto-create runtime objects, so a scene file is not required for the game loop to boot.
- If the scene folder looks empty, create a new scene and save it inside `Assets/Scenes` before pressing Play.

## Local run instructions
- Start Unity and load the project
- Create or open a scene saved under `Assets/Scenes`
- Press the Play button

## Status
- C# project compile check passed
- Build verified with `dotnet build "Assembly-CSharp.csproj" -nologo`
