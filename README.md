SWORDS OF THE ANCESTORS - Applied Capstone Project
=========================================

Game Summary:
-------------
**Swords of the Ancestors** is a 2D action-adventure game developed in Unity. It explores Akan (Asante) cultural heritage through artifact collection, shrine restoration, and combat mechanics. The game is designed to reconnect Ghanaian youth with their traditions using cultural documents interwoven with the gameplay experience.


Gameplay Preview:
-----------------
A quick look at some gameplay features in *Swords of the Ancestors*.

### ⚔️ Combat Snippet

![Combat](Assets/Media/combat.gif)

### 💥 Taking Damage
Feedback animations and effects when the player is hit.

![Take Damage](Assets/Media/take-damage.gif)




GitHub Repository:
------------------
Access the full source code and project files here:  
🔗 https://github.com/kwadjoWusu/Applied-Capstone-Project

System Requirements:
--------------------
- Unity Editor Version: 2022.3 LTS or later  
- Operating System: Windows 10/11 or macOS Catalina+  
- Minimum RAM: 8GB  
- Graphics: Integrated or dedicated GPU that supports OpenGL 4.1 or DirectX 10+

How to Compile, Install, and Run:
---------------------------------

**Option 1: Run in Unity Editor (recommended for testing and development)**  
1. Download or clone the repository:  
   `git clone https://github.com/kwadjoWusu/Applied-Capstone-Project`

2. Open Unity Hub  
3. Click **Add Project**, then select the cloned folder  
4. Open the project and press the **Play** button in the Unity Editor to start the game

**Option 2: Build a Standalone Executable (for distribution/playtesting)**  
1. Open the project in Unity  
2. Go to `File > Build Settings`  
3. Choose your target platform (e.g., Windows or Mac)  
4. Click **Build** and choose an output folder  
5. Run the generated executable file

Packages Used:
--------------
Ensure these Unity packages are installed in **Window > Package Manager**:

- **Input System** (com.unity.inputsystem)  
  → Used for flexible input handling and rebinding support

- **TextMeshPro** (com.unity.textmeshpro)  
  → For crisp, scalable in-game text

- **Unity Netcode for GameObjects** (com.unity.netcode.gameobjects)  
  → Enables basic networking support for multiplayer features

- **Unity Navigation (NavMesh)**  
  → Used for AI pathfinding (enemy patrolling, chasing, etc.)  
  → Optional: Use [`NavMeshComponents`](https://github.com/Unity-Technologies/NavMeshComponents) if you need runtime baking or advanced features
  
External Assets Used:
---------------------
These external assets were integrated to improve the visual and gameplay experience. All assets were free and properly licensed for non-commercial or educational use.

**Straight Sword Animation Pack** by *Sebastian Graves*  
→ Used to make combat smooth and varied. Includes 73 high-quality sword animations:
- Light attacks, charged strikes
- Jump attacks, rolls, and blocks
- Eight-direction walking and running

**Fantasy Skybox FREE**  
→ Used to create dynamic and immersive skies for outdoor scenes. Includes:
- 18 high-resolution cube map skyboxes  
- 32 panoramic skyboxes

**Pixel Art Platformer – Village Props**  
→ Used to populate village levels with colorful and low-poly environmental assets, adding detail and cultural life to the scenes

Game Controls Guide:
--------------------
| Action               | Control                         |
|----------------------|---------------------------------|
| Move Character       | W / A / S / D                   |
| Jump                 | Spacebar                        |
| Light Attack         | Left Mouse Button               |
| Heavy Attack         | Right Mouse Button              |
| Interact (Shrines)   | I                               |
| Dodge / Roll         | E                               |
| Delete Save Slot     | X                               |
| Sprint               | Shift + Movement Keys (WASD)    |

Notes:
------
- The game currently supports single-player mode.  
- Progress can be saved in the **Inspector** using the `PlayerManager` script attached to the player GameObject.  
- To revive the player after death (for testing), use the `PlayerManager` script in the Inspector.  

Contact:
--------
For questions, feedback, or contributions, contact:  
📧 wkjwusu@gmail.com
