# Networked DOTS Tool
## About
[How To Use](#how-to-use) | [Networking](#networking)

This project attempts to refine the current <a href="https://github.com/Unity-Technologies/FPSSample/">FPSSample</a> and <a href="https://github.com/Unity-Technologies/DOTSSample/">DOTSSample</a> projects, which provide tools and guidance for developing networked games on the new DOTS architecture, into a lightweight version. Having used these tools previously and by researching Unity forums, I have identified a strong need for a lightwieght DOTS tool in the game development community. The current tools consist of Unity projects with fully functional examples that make them bulky and difficult to work with. These included examples increase build times and make testing difficult. Consequently, these tools are not efficient and are of little use to developers new to the DOTS architecture. This refined tool utilizes scriptable objects and a custom editor to generate the scripts necessary for ECS implementation. This allows users to more readily transition from object oriented programming to data oriented programming. Users may wish to continue using these scriptable objects to build their ECS scripts or inspect the generated scripts to get a better understanding of their structure for transitioning away from the tool. Ultimately, I hope users can come away from using this tool with a stronger understanding of DOTS ECS implementation in Unity and applying it, where applicable, to their projects.

>I highly recommend you download at least one (DOTSSample is more recent and smaller) and play around to get a feel for what is possible with DOTS architecture. Additionally, Unity has put together a <a href="unity.com/megacity">Megacity</a> where 4.5 million mesh renderers, 5000 dynamic vehicles, 100,000 unique audio sources, and 200,000 unique building objects are all in one scene. All while maintaining 60FPS! It's an amazing milestone, and can be <a href="http://megacity.unity3d.com/MegaCity_GDC2019_Release_OC.zip">downloaded here as well</a>.

> These two resources helped me tremendously in understanding the configuration of DOTS ECS and NetCode. <a href="https://www.youtube.com/watch?v=P_-FoJuaYOI&t">UNITY COPENHAGEN - DOTSSample & the NetCode Behind It</a> | <a href="https://www.youtube.com/playlist?list=PLzDRvYVwl53s40yP5RQXitbT--IRcHqba">DOTS ECS Tutorials</a>

## DOTS & ECS
The Data Oriented Technology Stack (DOTS) and the Entity Component System (ECS) drastically increase the performance of games developed in Unity. This performance boost permits for stable and reliable network communications necessary for immersive and consistent networked and non-networked gameplay. DOTS is comprised of three major components: the C# Job System, ECS, and a burst compiler. These in conjunction allow for processes to be split across multiple threads as opposed to them all running on the main thread. DOTS is not applicable to everything, and MonoBehaviors are not being deprecated. However, DOTS is the preferred architecture for systems that are extremely intensive such as instantiating thousands of objects and managing networked items. This project focuses on the ECS elements, as this is where the code implementation truly changes. The C# Job System and burst compiler can be obtained via installing Microsoft's Visual Studio.  Utilizing their performance boosting capabilities is simply a matter of enabling them.
### Entities
The entities are essentially lightweight GameObjects without any data that serve as placeholders or ID’s that components are assigned to. At first they may seem foreign as they have no components, not even a Transform, like that of an empty GameObject. Yet, this reduction allows for more compact storage in memory so that Systems can easily identify the Components to act upon.
### Components
The components are the actual variables that hold data. While we are currently accustomed to putting variables and logic scripts all on a singular GameObject, this is an inefficient means of storing the data in memory and leads to unoptimized applications. Therefore, only Components are associated with Entities, not the System scripts.
### Systems
Systems identify specified Components and act upon them. If you have multiple entities with a health Component, and you create a system to slowly regenerate it over time, every Entity with this component will have their health regenerated. One system can be created to update every single Component, and they do not get assigned to individual game objects. These scripts simply live in your project and utilize the job system to schedule their processes across all threads (if specified to do so) to increase efficiency. Updating the health of a thousand enemies in one scene could kill your framerate if you utilized the standard GameObject approach, each with their own individual script. These Systems can be scheduled with the C# Job System allowing for full utilization of a devices resources.
### Pure ECS vs. Hybrid ECS
Game development is uniquely visual and we have become extremely familiar with the GameObject approach to development. However, when implementing ECS, there is no need for GameObjects that represent your players or objects in the game world. Instead, Entities get instantiated at the start of playing the scene. This strict implementation of ECS without GameObjects is commonly referred to as 'Pure ECS.' Pure ECS has its applications, particularly when you're instantiating thousands of entities that you would never want to manually place.  However, this would make development extremely difficult if you couldn't visualize the scene you're designing. Consequently, Unity has been working on their GameObject to Entity conversion system. This system allows for either the use of Subscenes or Convert components to transition exisiting GameObjects into Entities with their associated Components. This conversion of GameObjects to Entities and Components is considered 'Hybrid ECS.' The conversion is not perfect, but most basic properties of a GameObject can be converted into a Component (i.e., Mesh, MeshRenderer, Translation, RigidBody, Physics, etc.). Additionally, custom Component scripts can be applied to GameObjects that your System scripts will act upon, allowing you to take full advantage of both GameObjects for visualization and ECS for optimization

This tool permits for both Pure and Hybrid ECS implementations. However, it is important to note that NetCode (the DOTS implementation of Networking) requires the hybrid approach.
## How To Use
### Requirements
[Back to Top](#about)

This project requires:
- Unity 2019.3.0f6
- Microsoft Visual Studio (2017 or 2019)

The project includes the following packages (dependencies):
- Burst 1.3.0-preview.7
- Collections 0.7.1-preview.3
- DOTS Editor 0.6.0-preview
- Entities 0.90-preview.6
- Hybrid Renderer 0.4.1-preview.6
- Jobs 0.2.8-preview.3
- Mathematics v. 1.1.0
- Unity NetCode 0.1.0-preview.6
- Unity Transport 0.3.0 - preview.6

### General Use Notes
1. Do not change the folder structure of the current project. These are configured in a way that the generated assets and scripts are neatly organized for you.
2. Networked gameplay is configured by default. If you want to disable the networking capabilities of this project, you **MUST** uninstall the NetCode and NetCode.Transport packages.
3. The Game.cs script is required for networked connections. This script connects you to your sever. Only remove if going for a non-networked game.
4. Networked games require that the entites (GameObjects) need to be a child of either the SharedData or ServerData parent objects.
5. Server entities (GameObjects) must be prefabs for NetCode to correctly function. 

### Installation / Navigation
To begin using the tool, download this repository and open the project in Unity.
The Netwoked DOTS Tool can be found under the Toolbar DOTS > Networked DOTS Tool.

This will open the custom editor. Seen below.
<img src="Images/NetworkedDOTSTool.png"/>

#### Creating a Component
1. Under the Component section of the tool, enter a name of the component
2. Select the data type of this store. TAG is used for unique identifiers. COMPOSITE is for the creation of more complex components with multiple variables. Create each individual one first, then add them to the array (set the size).
3. Click either the 'Create Component' or 'Create Composite Component' buttons.
4. Navigate to the Editor > Components folder. Your newly created Component Asset is available.
5. If you want to create a script for this componet, click the asset. In the inspector there should be a custom button for creating the final script.
6. Navigate to the Scripts > Components folder and the final component script is available. This can be now attached to GameObjects if utilizing the Hybrid ECS approach.

#### Creating an Entity
1. Enter a name for the Entity.
2. Attach a previously created Component Asset. Multiple Component assets can be added if desired, the size must reflect your desired amount.
3. Click the Create Entity button.
4. Navigate to the Editor > Entities folder. Your newly created Entity (Archetype) Asset is there.
5. To create a spawner script for this entity, click the asset. In the inspector there should be a custom button for creating the final spawner script.
6. Navigate to the Scripts > Entities folder and the final entity spawner script is available. This can be attached to a GameObject to create your desired amount of entities.

>Note: The Entity asset is not required for developing networked games. The Convert to Client Server Entity script present on the SeverData and SharedData GameObjects in the project automatically convert these objects to entities. If you need to create entities separate from the standard or are developing a non-networked game, use this as a template.

#### Creating a System
1. Enter the name of the System you are creating. (ie. Regenerate, or LevelUp)
2. Attach the Component Assets this will be acting upon. Do not attach duplicates. Alternatively, you can attach an Entity asset and the tool will find the associated components.
3. Click the Create System button.
4. Navigate to the Editor > Systems (Jobs) folder. Your newly created System Asset is there.
5. To create the final system script, click the asset. In the inspector there should be a custom button for creating the final system script.
6. Navigate to the Scripts > Systems (Jobs) folder and the final system script is available. This does not need to be attached to an object to be active/ effective.
7. Open the script and update it accordingly to provide logic.

Please follow this <a href="https://youtu.be/E8NhTyO2Joo">DEMO</a> for a more detailed approach and application.

### Entity Debugger
When you start the game, you may notice that none of your objects appear in the project heirarchy. Oh no! No worries, to view them you must navigate to the Entity Debugger for inspection. This menu can be found in the Toolbar under Window > Analysis > Entity Debugger.
 
## Networking
### NetCode
[Back to Top](#about)

This project utilizes NetCode for the implementation of networking. At the time of developing this tool, this package is still in preview and fairly unstable and ever-changing.
The project currently is configured for Client and Server data communication utilizing RPC requests, GhostCollectors, and GhostAuthoring Components.

For items that will be on both the Client and Server, put these GameObjects under the SharedData object.
> These items **must** be a child of the SharedData GameObject.

For objects that will be solely handled by the Server, put these GameObjects under the ServerData object.
Server objects **must** have a GhostAuthoring Component. The inspector has been updated to have a button readily available to add these components.
Once these are added, you must Select Update Components and then Generate Code.
> Server objects **must** be prefabs.
> These items **must** be a child of the ServerData GameObject.

Once this code has been generated, you must update the GhostCollection GameObject. To do so, simply click the object and in the inspector use the 'Update Ghost List' button and then follow up with clicking the 'Generate Collection Code' button.

### Multiplayer PlayMode Tools
To run your networked game, the Multiplayer PlayMode Tools editor permits you to run multiple clients and switch between them. Optionally, you can run only the client or only the server to see how your code is performing.
To access this window you navigate to Multiplayer > PlayMode Tools in the editor toolbar.
> If the NetCode package is installed, your Editor will run in a Client and Server mode by default. When developing non-networked games you want your code to be created in the 'Default' world. The only way to ensure your objects are instantiated in the default world is to remove this package entirely.