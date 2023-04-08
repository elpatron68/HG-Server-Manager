# HG Server-Manager

*HG Server Manager* is a .NET Windows application that eases the management of the [Hydrofoil Generation](https://store.steampowered.com/app/1448820/Hydrofoil_Generation/) game server.

## Features

- 🦋 Clean and beautiful user interface
- ⚙️All server parameters editable 
- 🛠️ Control your server:
  - 🚀 Start/stop game server
  - ⚠️ Get notified of the occurrence of penalties
  - 👨‍👩‍👧‍👦 See if your server is publicly reachable
- 📬 Notify players of your server launches
- ♾️ Load and save an unlimited number of different configurations
- ⛵ Instantly switch between up to 10 user-defined presets by hot key
- 📃 Open game server log file and *snaps* directory directly from the application

![image-20230407193824206](./assets/image-20230407193824206.png)



## Installation

- Download latest release from https://github.com/elpatron68/HG-Server-Manager/releases (zip file)
- Extract the archive to a directory of your choice
- Start `HG-ServerUI.exe`

## Usage

*HG-Server-Manager* loads the active server configuration from the file `server_cfg.kl` at startup. You can modify the configuration according to your requirements. All settings are saved at the moment you click the *Start Server* button.

### Load an existing configuration from a file

Load an existing configuration from a file by opening the Menu *File* and selecting *Load configuration*.

### Save configuration to a file

You can save a configuration for later use by opening the Menu *File* and selecting *Save configuration as*.

### Save configuration without starting the server

Open *File* - *Save current configuration* saves the configuration to the default file `server_cfg.kl` without starting the server.

### Manually edit configuration

Open *File* - *Manually edit configuration* to edit `server_cfg.kl` in the *Notepad* text editor. Changes to the file will be loaded to the user interface after saving the file.

### Open server log

You can open the server log `log.log` in the *Notepad* text editor by opening *File* - *Open server log*.

### Open Ntfy website

The menu *Server launches* opens the Ntfy topic with the latest server launch events. For details, see section about Ntfy below.

### Open Snaps

The menu *Snaps* - *Open snaps* opens an Windows *Explorer* window in your *snaps* directory.

### Status bar

The status bar at the bottom of the window shows informations about the state of your server.

### Protocol

![image-20230407202414163](./assets/image-20230407202414163.png)

The protocol text box informs you about events and problems. Have a look at it if something does not work as expected.

### Penalties

Occurring penalties are displayed in the *Penalties* text box. New penalties are signalized with a sound effect.

## Ntfy notifications

Let your players be notified when you start your game server! *HG-Server-Manager* sends messages to the [Ntfy](https://ntfy.sh/) *topic* `Hydrofoil_Generation_Servermonitor` when you hit the launch button.

Let your players never miss one of your races by letting them subscribe to the *topic*! Open [this](https://ntfy.sh/Hydrofoil_Generation_Servermonitor) web browser tab and let it opened. Or - way better! - install the [Ntfy app](https://ntfy.sh/#subscribe-phone) on your mobile and subscribe to the *topic*. The Ntfy app is available for iOS and Android.

### Modify Ntfy topic

The default topic `Hydrofoil_Generation_Servermonitor` can be changed to an individual one by renaming the file `Hydrofoil_Generation_Servermonitor.ntfy` in the application directory. Just change the file name to a topic name of your choice. Note, that Ntfy topics are public for everyone who knows the topic name! If you want to have some privacy, give it a random alpha-numeric name like `qPd5AbhVfwv5FJFQtYRY4xCf`

### Disable Ntfy notifications

To disable all Ntfy notifications, delete all files with the extension `*.ntfy` in the app´s directory.