# King's Raid QOL Mod

A mod for King's Raid (PC) that automates common repetitive interactions.
This mod does not provide any cheat functionality and only presses UI elements for you when they appear.

## Features

- **Auto Skip Cutscenes** — Automatically skips cutscenes
- **Auto Login** — Automatically presses the 'tap to play' screen when opening the game
- **Auto Close Reward Popups** — Automatically closes reward popups after battles
- **Auto Skip Victory Screen** — Automatically skips the end battle victory animation screen
- **Auto Next Battle** — Automatically clicks the next battle button after battles
- **Auto Claim Rewards** — Automatically claims quest/achievement/etc rewards
- **Auto Receive Mail** — Automatically clicks receive all on the mail screen

## Requirements

- [King's Raid](https://store.steampowered.com/app/3689540/KINGs_RAID/) (PC)
- [BepInEx 5](https://github.com/BepInEx/BepInEx/releases) (v5, Mono, x64)

## Installation

1. [Download](https://github.com/BepInEx/BepInEx/releases) and [install](https://docs.bepinex.dev/articles/user_guide/installation/index.html) BepInEx 5 into your KR game folder
   - You likely want BepInEx_win_x64_5.X.XX.X.zip - This mod was developed for BepInEx v5 and not tested with v6
   - Extract BepInEx into the folder containing the game executable 
   - Example: `C:\Program Files (x86)\Steam\steamapps\common\KING's RAID Playtest`
   - Run the game once to let BepInEx initialize
2. Download the latest `KRQOL.dll` from [Releases](../../releases)
3. Place `KRQOL.dll` into `BepInEx/plugins/`
4. Launch the game

## Configuration

Settings are available in `BepInEx/config/KRQOL.cfg` after first launch, or via [BepInExConfigManager](https://github.com/BepInEx/BepInEx.ConfigurationManager)

## Notes

- Developed during CBT — expect updates as the game changes
- This program provides no cheats, it only presses in-game buttons for you. I use it myself, however I provide no guarantee your account will not be banned or etc.

## Development

- Create `LocalPaths.props` and point to game install directory. Reference `LocalPaths.props.example`.
