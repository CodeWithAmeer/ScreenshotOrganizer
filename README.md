# Screenshot Organizer

A lightweight, 100% free and open-source Windows utility that automatically organizes your screenshots into clean, date-based folders, so your desktop and Pictures folder stay clutter-free.

---

## Overview

Screenshot Organizer watches a folder of your choice (for example, your default **Screenshots** folder) and automatically moves new image files into subfolders named by date:

C:\Users\You\Pictures\Screenshots
 ├─ 2025-01-10
 ├─ 2025-01-11
 └─ 2025-01-12
It is designed to be:

Simple – pick a folder, click “Start watching”, and let it run in the background

Comfortable – built-in light and dark themes suitable for long usage sessions

Automatic – optional “Run at Windows startup” toggle

Private – no network access, no telemetry, everything runs locally

Open source – written in C# / .NET, easy to read, audit, and extend

Features
Automatic organization
Watches a selected folder for new image files (.png, .jpg, .jpeg, .bmp, .gif, .tiff, .webp)

Moves each screenshot into a date-based subfolder using the yyyy-MM-dd format

Avoids overwriting files by generating unique file names when necessary

One-click cleanup for existing files
“Organize existing now” button scans the selected folder

Automatically sorts all existing images into date-based subfolders

Light and dark UI themes
Switch between Light and Dark themes from the View menu

Buttons, panels, and the log area adapt to the selected theme for a consistent, comfortable visual experience

Optional startup with Windows
Settings → Run at Windows startup menu item

When enabled, the application adds an entry under the current user’s Run registry key so Screenshot Organizer starts automatically after login

Can be disabled at any time from the same settings menu

Tip of the day (Windows productivity tips)
On startup, displays a small “Tip of the day” dialog with a random Windows shortcut or productivity tip

Includes a “Don’t show tips on startup” checkbox to permanently disable startup tips for users who prefer a silent launch

Activity log
Scrollable log pane with timestamps showing:

When watching starts and stops

Which files were moved and to which date folder

Any errors encountered during file operations

“Clear log” and “Reset counters” options are available in the View menu

Persistent settings
Remembers:

Last watched folder

Selected theme (Light or Dark)

Whether tips are shown on startup

Whether the application should run at Windows startup

Settings are stored in %APPDATA%\ScreenshotOrganizer\settings.json

Requirements
Operating system: Windows 10 or later

Runtime: .NET 6 Desktop Runtime (or newer) if running from source

Architecture: x64 or x86, depending on how you build the release

Getting Started (for Users)
Download the latest release (once available) from the Releases section.

Extract the .zip file to any folder.

Run ScreenshotOrganizer.exe.

Inside the application:

Click Browse… and select the folder where your screenshots are stored.

Click Start watching to begin automatic organization.

Optionally enable Settings → Run at Windows startup if you want Screenshot Organizer to launch automatically when you log in.

Building from Source (for Developers)
Clone the repository:

bash
Copy code
git clone https://github.com/CodeWithAmeer/ScreenshotOrganizer.git
cd ScreenshotOrganizer
Open the project in Visual Studio 2022 (or later) with the .NET 6 desktop development workload installed.

Build and run:

Press F5 to run in Debug mode, or

Use Build → Publish to create a standalone release build.

The project is a single Windows Forms application written in C# and targeting .NET 6.

Project Structure
Program.cs
Entry point for the WinForms application.

MainForm.cs
Main user interface and core logic:

File system watcher integration

Folder selection and commands

Activity log handling

Theme switching (Light/Dark)

Tip-of-the-day dialog

Registry integration for the “Run at startup” option

Loading and saving of application settings

AppSettings and SettingsManager
Simple JSON-based settings system that persists user preferences under %APPDATA%\ScreenshotOrganizer.

Settings and Menus
View menu
Light theme / Dark theme – switches the overall UI theme

Clear log – clears all entries from the on-screen log

Reset counters – resets the “Files moved” counter back to zero

Settings menu
Run at Windows startup

When checked, the application writes its executable path to:
HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run

When unchecked, the registry entry is removed and the application will no longer start automatically with Windows

Help menu
About – shows a short description of the project and acknowledges that it is a free, open-source tool

Privacy
No external network calls are made

No cloud storage or telemetry is used

All file operations occur locally on the user’s machine

Settings are stored in a JSON file in the user’s application data directory

Screenshot Organizer is suitable for offline environments and for users who prefer tools that do not communicate with external services.

Tech Stack
Language: C#

Framework: .NET 6 (Windows Desktop)

UI framework: Windows Forms

Settings: JSON via System.Text.Json

Automation: FileSystemWatcher for real-time folder monitoring

Startup integration: Windows Registry (HKCU\Software\Microsoft\Windows\CurrentVersion\Run)

Possible Future Enhancements
Potential future improvements include:

Customizable naming patterns for date folders (for example yyyy/MM/dd or Screenshots_yyyyMMdd)

Rules based on filename or resolution (for example separate routing for game screenshots vs. browser screenshots)

Optional system tray mode with a context menu

Multi-folder support and profile-based configuration

Localized user interface (multi-language support)

Contributing
Contributions, suggestions, and bug reports are very welcome.

For issues, please open a GitHub issue describing the problem or feature request.

For pull requests:

Keep the code style consistent with the existing project

Aim to preserve the goals of the application: simple, lightweight, and privacy-friendly

This project can also serve as a practical example for developers who are learning C#, .NET, or Windows Forms.

License
Screenshot Organizer is released as 100% free and open source under the MIT License.
See the LICENSE file for full license details.
