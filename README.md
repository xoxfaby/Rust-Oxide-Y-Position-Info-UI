**Y-Position Info UI** adds a simple UI to show your Y position, meaning your vertical position in the world. This mod is intended to help with building, more specifically building pixelbunkers. 

You can set an Entity as your Y Reference Entity, meaning instead of showing your overall Y position, it shows you how far above or below you are in relation to a specific entity. This is intended to be used with foundations, showing your position as exactly 0 when standing on the foundation. You can change your Y Reference Entity by looking at an entity and pressing "Reload" while not holding anything in your hand, or with the `setyentity` command. 

# Commands (both chat and console)
* `toggleyui` - Toggles the UI
* `setyentity` - Sets the Entity you are looking at as your Y Reference Entity.

# Configuration
* `ShowUIByDefault` - `bool: default = true` - Whether or not players should see the UI by default when they join the server.
*  `UIRefreshRateMilliSeconds` - `float: default = 0` - How often to update the UI, in milliseconds. If you run into performance issues you can change this. 0 means it updates as often as possible.  
*  `CommandOnlyMode` - `bool: default = false` - This disables the functionailty of settings your Y Reference Entity with the hammer and you must use the command to change it.