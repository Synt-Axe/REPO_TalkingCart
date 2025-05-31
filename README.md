# Cart Talking Mod

This mod makes the cart talk with voice and subtitles. The cart will warn the players when an enemy is nearby, let you know when an enemy has despawned/respawned, and tell you how many items are in the vicinity.

It was created as a part of the following YouTube video: https://youtu.be/naOCUkJkmzk


# Supported Features

- The cart has a chance of reacting (aggressively) if you damage an item in front of it. You're also able to configure how high/low the chance is from the config file.
- The cart will tell the players which enemies are present in the level at the start of each level.
- If an enemy enters the area around the cart (within a 20 units of the cart's position), the cart will warn the players.
- If an enemy that has been communicated as near exits the area around the cart (more than 23 units away from the cart), the cart will notify the players
- If an enemy despawns, the cart will notify the players.
- If an enemy, that has been communicated as despawned, respawns. the cart will notify the players.
- If an enemy dies, the cart will notify the player.
- The player can disable the cart's communications by holding it in strong mode and pressing the Z key.
- The players can trigger the cart to tell them how many items are in the surrounding are by holding the cart in strong mode and pressing the X key.
- If modded enemies are present in the level, the cart will communicate their info using the game's TTS functionality.

All keybindings are changable in the log file. Here is a list of valid keycodes: https://docs.google.com/spreadsheets/d/17ehNwtSd3fG-BeMJhkKTawRa0C0B_jTt7XAwhTI-oWk/edit?usp=sharing

# Mod Type
Client-side.

