# Space Game
Small-scale space exploration game, with 5 planet/moons. The planets each have a distinct look, with different color palettes and objects spawning on the surface. 

## Gameplay
You are an Astronaut crashlanded in an unfamiliar system. You must explore the various planets, using resources found on each one to craft new parts and repair your ship to escape the system.

All objects are part of a realistic gravity simulation. This means that the player can orbit large bodies, and needs to consider this while making transfers between planets

### Minables
Each planet is fully destructible, allowing the player to mine in to them and find burried resources.
<img src="https://github.com/user-attachments/assets/4d0d0d08-8e2f-41e1-af63-480c1bc35670" width="500">

https://github.com/user-attachments/assets/b69d9753-ecf1-4ff3-b2e8-522b2a355b47

The planets are stored as an array of densities at grid points. Using that data, the meshes are generated using marching cubes implemented as a compute shader, allowing for great performance.

### Crafting
There is a basic crafting system implemented, however not many recipes have been added

https://github.com/user-attachments/assets/2ed09ec8-10f3-4d69-afd3-5f1a6d648a11

### Ship
You can craft (And eventually find, hoping to add other crash sites) various upgrades for your ship, allowing you to customize it, and eventually equip a warp drive to escape the system

<img src="https://github.com/user-attachments/assets/ef27408b-66ba-4cb5-83c4-c9068fe6aa3e" width="500">

# Planets
The main reason for this project was to design interesting, procedurally generated planets

There are 5 distinct bodies. The objects that spawn on them (rocks, vegetation, etc.) are procedurally placed, and various noise functions are used to give the surface interesting features to explore. 

Each planet also has procedurally generated caves.

### Earth-Like Planet
<img src="https://github.com/user-attachments/assets/bcc28697-badf-4001-a052-dc046201af3b" width="500">
<img src="https://github.com/user-attachments/assets/e97cb724-955f-43f4-a239-39a97c030569" width="500">

### Baren Planet with Small Moon
<img src="https://github.com/user-attachments/assets/e092f211-0128-4020-ba90-0c908da467d0" width="500">
<img src="https://github.com/user-attachments/assets/582c6c3a-5872-4583-9ea4-03c28851d094" width="500">

### Binary Planet System
<img src="https://github.com/user-attachments/assets/1ee3831b-33c4-43c0-b186-059ec46ebd70" width="500">
<img src="https://github.com/user-attachments/assets/29991d89-ae46-4092-a5f0-8b9c1724d540" width="500">
<img src="https://github.com/user-attachments/assets/1fca0b68-c802-42f7-9142-562d39355aef" width="500">

https://github.com/user-attachments/assets/9bba1fa5-0bdd-43cc-88a1-419c8910e88a


