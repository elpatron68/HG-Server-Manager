# HG-Server-Manager

*HG Server Manager* is a .NET Windows application that eases the management of the [Hydrofoil Generation](https://store.steampowered.com/app/1448820/Hydrofoil_Generation/) game server.

## Features

- Clean and nice looking user interface
- All server parameters editable
- Control your server:
  - Start/stop game server
  - Get notified of the occurrence of penalties
  - See if your server is publicly reachable
- Notify players of your server launches
- Load and save an unlimited number of different configurations
- Open game server log file and *snaps* directory directly from the app

![image-20230407193824206](./assets/image-20230407193824206.png)



## Installation

- Download latest release from https://github.com/elpatron68/HG-Server-Manager/releases (zip file)
- Extract the archive to a directory of your choice
- Start `HG-ServerUI.exe`

## Ntfy notifications

Let your players be notified when you start your game server! *HG-Server-Manager* sends messages to the [Ntfy](https://ntfy.sh/) *topic* `Hydrofoil_Generation_Servermonitor` when you hit the launch button.

Let your players never miss one of your races by letting them subscribe to the *topic*! Open [this](https://ntfy.sh/Hydrofoil_Generation_Servermonitor) web browser tab and let it opened. Or - way better! - install the [Ntfy app](https://ntfy.sh/#subscribe-phone) on your mobile and subscribe to the *topic*. The Ntfy app is available for iOS and Android.

### Modify Ntfy topic

The default topic `Hydrofoil_Generation_Servermonitor` can be changed to an individual one by renaming the file `Hydrofoil_Generation_Servermonitor.ntfy` in the application directory. Just change the file name to a topic name of your choice. Note, that Ntfy topics are public for everyone who knows the topic name! If you want to have some privacy, give it a random alpha-numeric name like `qPd5AbhVfwv5FJFQtYRY4xCf`

### Disable Ntfy notifications

To disable all Ntfy notifications, delete all files with the extension `*.ntfy` in the app´s directory.