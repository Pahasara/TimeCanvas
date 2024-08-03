# TimeCanvas

TimeCanvas is a powerful time management and task organization software designed to simplify your daily life. It is written in C# and is powered by avalonia & sqlite. Specially made for someone who wants to stay organized, TimeCanvas provides the tools you need to manage your time efficiently.! (ganbare, ganbare!)

## Downloads

### For Windows (x64)
[![ZIP](https://img.shields.io/badge/win%20x64.7z-[30.9MB]-darkgreen)](https://github.com/Pahasara/TimeCanvas/releases/download/1.2.0/win-x64.7z)
[![SETUP](https://img.shields.io/badge/setup.exe-[33.7MB]-blue)](https://github.com/Pahasara/TimeCanvas/releases/download/1.2.0/setup.exe)

### For Linux (x64)
[![TAR](https://img.shields.io/badge/linux%20x64.tar.xz-[39.2MB]-darkgreen)](https://github.com/Pahasara/TimeCanvas/releases/download/1.2.0/linux-x64.tar.xz)

<img width="848" alt="App screenshot" src="https://github.com/Pahasara/TimeCanvas/assets/46932317/b0617432-e778-419c-93c5-cdd48cb0bf15">

## Troubleshooting

### Wayland Scaling Issue
Monitor DPI is calculated from values provided by XRANDR extension. These might be not accurate for your particular monitor, so you can override scaling factors for particular monitors via environment variable.

1. _xrandr --listactivemonitors_ will give you output like this:

_Monitors: 1
 0: +*eDP-1 1920/344x1080/194+0+0  eDP-1_

eDP-1, HDMI-1, DP-1 are output names that your can configure DPI for.

2. Add _AVALONIA_SCREEN_SCALE_FACTORS_ environment variable to your /etc/profile, $HOME/.profile or other suitable location and relogin.

Example:
*AVALONIA_SCREEN_SCALE_FACTORS='eDP-1=2;HDMI-1=1;DP-1=1.5'*

this will set eDP-1 to 192 DPI, HDMI-1 to 96 DPI and DP-1 to 144 DPI.
