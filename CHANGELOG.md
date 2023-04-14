# Changelog

## v1.0.5

- Add regatta results viewer (*File* => *View results*)

## v1.0.4

- Fix reading server name with blanks
- Add chat textbox
- Replace Ntfy by Discord
- Add more messages from `hg_server.exe` to the *Protocol` textbox

## v1.0.3
- Add new penalties to the top of the list
- Auto scroll Protocol text to the end
- ~~Copy `/admin <password>`, `/set_course` and `/set_wind` admin commands to clipboard (double-click on the label left to the value)~~ HG seems not to support pasting text from the clipboard to the chat.
- Rearrange controls, widen window size add tiles
- Parse `hg_server.exe` output, extract and display values (numer of players, server status, selected course) in new tiles
- AI generated icon added (created with [Stable-Diffusion](https://github.com/AUTOMATIC1111/stable-diffusion-webui))


## v1.0.2
- Rearrange controls
- Add link to Ntfy Penality topic
- Fix problems concerning unsaved values
- Fix password problems
- Fix penalty detection
- Add cleanup snaps directory


## v1.0.1

- Make Ntfy notifications configurable (disable, enable, set own topic) in the UI (via race configuration file)
- Add hot slots for quick start of a pre-defined configuration
- Add hotkey `Crtl+s` (start / stop the server)

## v1.0

First release