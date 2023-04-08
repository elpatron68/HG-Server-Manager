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

## How notifications work

There are two use cases for notifications:

- You want to inform your player community that you have launched your game server
- You (or your players) want to be informed if your game server detects any penalties

Let your players never miss one of your races by letting them subscribe to your messages!

*HG Server-Manager* uses the free service [*Ntfy*](https://ntfy.sh/) to send notifications to subscribed clients. To setup *Ntfy* notifications as game host, you have to

- Select a *topic* for race notices and enter it in the corresponding field
- Select a *topic* for penalty notices and enter it in the corresponding field

![image-20230408175840258](./assets/image-20230408175840258.png)

A Ntfy *topic* is sort of a channel, users can subscribe to. You can enter any alphanumeric topic name you want. But be aware of the fact, that every topic is public and everyone can subscribe to it if the name of the topic is known. So, if you want to have some privacy, give your topic(s) a random alpha-numeric name like `qPd5AbhVfwv5FJFQtYRY4xCf`.

![image-20230408182649932](./assets/image-20230408182649932.png) 

Example notification (web browser)

<img src="./assets/image-20230408182840712.png" alt="image-20230408182840712" style="zoom:33%;" />

Example notifications (Android app)

As player, who wants to be informed about starting races of their favorite host, you can subscribe the Ntfy topic by

- Ask the game host for the topic(s) to subscribe to (if the default values were changed). Open the web site `https://ntfy.sh/<topic>` in your browser or
- install the [Ntfy app](https://ntfy.sh/#subscribe-phone) on your mobile and add to the *topic*.

If you have not configured own *topics*, *HG Server-Manager* sends notifications to the *topics* [`Hydrofoil_Generation_Servermonitor`](https://ntfy.sh/Hydrofoil_Generation_Servermonitor) (start game events) and [`Hydrofoil_Generation_Penaltymonitor`](https://ntfy.sh/Hydrofoil_Generation_Penaltymonitor). Click on the links to open the default topics in your web browser.

> Tip: If you use own topics, inform your player community about them.

### Disable Ntfy notifications

Leave one or both *topic* fields empty to disable Ntfy notifications.