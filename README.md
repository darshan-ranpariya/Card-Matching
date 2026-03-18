# Card-Matching

A simple and extensible Card-Matching game built with Unity.

## Features

- **Classic Memory Gameplay**: Flip cards and find matches to clear the board.
- **Dynamic Grid Sizes**: Choose from multiple grid configurations (e.g., 2x2, 2x3, 4x4, etc.) to adjust difficulty.
- **Combo System**: Earn bonus points for consecutive matches.
- **Save/Load System**: Progress is saved and can be resumed later.
- **Responsive UI**: Built with Unity UI, supporting various resolutions.

## Technical Details

- **Unity Version**: 6000.3.10f1
- **Language**: C# 9.0
- **Architecture**:
    - **Service Pattern**: Decoupled logic using services like `ComboService`, `ItemFactory`, and `SaveLoadService`.
    - **Interface-Driven Design**: Uses interfaces (`IUIService`, `IGameManager`, `ISaveLoadService`) for better testability and maintainability.
    - **ScriptableObjects**: Utilizes `SpriteCollection` ScriptableObject for easy management of card assets.

## Project Structure

- `Assets/CardMatch/Scripts/`: Contains all game logic.
    - `Common/`: Interfaces and utility classes.
    - `Services/`: Domain-specific logic (Combo, Save/Load, Factory).
    - `ItemsHandler.cs`: Central manager for game state and card lifecycle.
- `Assets/CardMatch/Sprites/`: Game assets and card graphics.

## How to Play

1. Open the project in Unity 6000.3.10f1 or later.
2. Load the main scene.
3. Select a grid size from the main menu.
4. Click on cards to flip them and find pairs.
5. Try to get a high score with combos!

