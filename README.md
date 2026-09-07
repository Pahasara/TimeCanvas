# TimeCanvas

TimeCanvas is a powerful, lightweight time management and task organization software designed to simplify your daily life. Built with modern C#, Avalonia UI, and SQLite, it provides a clean, highly structured environment to manage your time efficiently and track your actual productivity. (Ganbare, ganbare!)

<div align="center">
  <img width="848" alt="TimeCanvas Screenshot" src="https://github.com/user-attachments/assets/9866baf3-6823-4900-9cc7-d307f3a2fa60">
</div>

---

## 📥 Downloads

Available as standalone, highly-optimized executables. No installation required.

### Windows (x64)
[![ZIP](https://img.shields.io/badge/Windows_x64.zip-[41MB]-blue)](https://github.com/Pahasara/TimeCanvas/releases/download/2.0.1/TimeCanvas_Windows_x64.zip)

### Linux (x64)
[![ZST](https://img.shields.io/badge/Linux_x64.zst-[42MB]-darkgreen)](https://github.com/Pahasara/TimeCanvas/releases/download/2.0.1/TimeCanvas_Linux_x64.zst)

### macOS (Apple Silicon / ARM64)
[![GZ](https://img.shields.io/badge/macOS_ARM64.gz-[40MB]-white)](https://github.com/Pahasara/TimeCanvas/releases/download/2.0.1/TimeCanvas_OSX_ARM64.gz)

---

## ✨ What's New in v2.0
* **Dynamic Task Management:** Add an unlimited number of tasks per day.
* **True Historical Tracking:** Tasks are logged by calendar date, preserving your full history instead of overwriting weekdays.
* **Advanced Efficiency Metrics:** Compare planned task duration against actual time used via a seamless dropdown upon completion.
* **Cross-Platform Compatibility:** Native, optimized support for Windows, Linux, and macOS.

---

## 🛠️ For Developers

TimeCanvas is built using **.NET 10**, **C# 14**, and **Avalonia UI**. It follows the MVVM pattern and uses Entity Framework Core for local database management.

### Build Instructions

Ensure you have the `.NET 10 SDK` installed. Clone the repository and run the following commands from the directory containing `TimeCanvas.App.csproj` to publish the executables.

```bash
dotnet publish -c Release -r linux-x64
dotnet publish -c Release -r win-x64
dotnet publish -c Release -r osx-arm64

```

---

## 🐛 Troubleshooting

### Wayland Scaling Issue (Linux)

Monitor DPI is calculated from values provided by the XRANDR extension. These might not be accurate for your particular monitor, causing scaling issues. You can override scaling factors via an environment variable.

1. Find your output names by listing active monitors:
```bash
xrandr --listactivemonitors

```

*Example output:*
`0: +*eDP-1 1920/344x1080/194+0+0  eDP-1`
*(Outputs like `eDP-1`, `HDMI-1`, `DP-1` are the names you can configure).*

2. Add the `AVALONIA_SCREEN_SCALE_FACTORS` environment variable to your `/etc/profile`, `$HOME/.profile`, or other suitable location and re-login.
**Example:**
```bash
export AVALONIA_SCREEN_SCALE_FACTORS='eDP-1=2;HDMI-1=1;DP-1=1.5'

```

*(This sets eDP-1 to 192 DPI, HDMI-1 to 96 DPI, and DP-1 to 144 DPI).*

