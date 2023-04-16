# Changelog

## v1.0.6.2

- Decrease app loading time by removing Network.Testport in PreFlightCheck
- Fix image height
- Minor refactorings

## v1.0.6.1

- Change archive directory to `<HG Server Directory>\archive`
- Update README.md

## v1.0.6

- Add results window (menu *Regattas - View results*)
  - Add cumulative results from all races (regatta series)
- Send results to Discord
- Move menu "Snaps" to menu *Regattas*
- Option to archive old regatta results (menu *Regattas - archive old results*)


## v1.0.5.2

- Remove dummy text

## v1.0.5.1

- Make Penalties textbox scrollable
- Make Chat textbox scrollable

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