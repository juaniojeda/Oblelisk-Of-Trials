# Oblelisk of Trials

**A first-person multiplayer prototype built with Unity, C#, and Photon PUN.**

This early prototype explores networked player control and a room progression system. The source includes first-person movement, multiplayer spawning, and rooms organized into three difficulty tiers.

## Implemented systems

- **First-person control:** mouse look, walking, sprinting, jumping, and gravity through Unity's CharacterController.
- **Player ownership:** local input, camera, and audio behavior are separated from remote player instances.
- **Photon integration:** connection to the service, lobby joining, room creation/joining, and networked player spawning.
- **Room progression:** shuffled scene pools progress from easy to medium to hard without repeating a room within its tier.
- **Scene loading:** asynchronous transitions between room scenes.

## Technology

| Component | Version / approach |
| --- | --- |
| Unity Editor | **2023.2.20f1** |
| Language | C# |
| Multiplayer | Photon PUN |
| Player movement | Unity CharacterController |
| Room flow | Shuffled scene pools and asynchronous loading |

## Explore the code

The Unity project is located at **`La Torre Del Mago 1/La Torre Del Mago`**.

| Script | Responsibility |
| --- | --- |
| [`PlayerController.cs`](La%20Torre%20Del%20Mago%201/La%20Torre%20Del%20Mago/Assets/Scripts/PlayerController.cs) | First-person input, movement, and local player ownership |
| [`ServerManager.cs`](La%20Torre%20Del%20Mago%201/La%20Torre%20Del%20Mago/Assets/Scripts/ServerManager.cs) | Photon connection, room joining, and player spawning |
| [`RoomManager.cs`](La%20Torre%20Del%20Mago%201/La%20Torre%20Del%20Mago/Assets/Scripts/RoomManager.cs) | Randomized room order and difficulty progression |

## Open the project

1. Clone this repository.
2. In Unity Hub, add **`La Torre Del Mago 1/La Torre Del Mago`**.
3. Open it with **Unity 2023.2.20f1** and allow imports to complete.
4. Review the Photon connection settings and scene configuration before testing multiplayer.

## Project status and credits

This is an early prototype. See the [repository history](https://github.com/juaniojeda/Oblelisk-Of-Trials/commits/main/) for development progress and contributor credits.
