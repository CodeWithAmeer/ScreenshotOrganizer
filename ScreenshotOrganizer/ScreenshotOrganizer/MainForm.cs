// MainForm.cs
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ScreenshotOrganizer
{
    /// <summary>
    /// Main application window for Screenshot Organizer.
    /// Watches a folder for screenshot images and moves them into date-based subfolders.
    /// </summary>
    public class MainForm : Form
    {
        // UI
        private MenuStrip _menuStrip = null!;
        private StatusStrip _statusStrip = null!;
        private TextBox _txtFolder = null!;
        private Button _btnBrowse = null!;
        private Button _btnToggleWatching = null!;
        private Button _btnOrganizeNow = null!;
        private Button _btnOpenFolder = null!;
        private ListBox _lstLog = null!;
        private ToolStripStatusLabel _statusLabel = null!;
        private ToolStripStatusLabel _statsLabel = null!;
        private ToolStripMenuItem _startupMenuItem = null!;
        private ToolTip _toolTip = null!;

        // Logic
        private FileSystemWatcher? _watcher;
        private bool _isWatching;
        private string? _watchedFolder;
        private int _filesMoved;

        private AppTheme _currentTheme = AppTheme.Dark;
        private readonly AppSettings _settings;

        private static readonly Random _random = new Random();

        private const int MaxLogItems = 600;
        private const string RunRegistryKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string RunRegistryValueName = "ScreenshotOrganizer";

        private static readonly string[] _windowsTips = new[]
        {
            "Press Win + Shift + S to quickly take a screenshot with the built-in Snipping Tool.",
            "Use Win + D to show the desktop instantly. Press again to restore your windows.",
            "Press Win + E to open File Explorer.",
            "Press Win + L to lock your PC before you walk away.",
            "Right-click the Start button for quick access to tools like Device Manager and Disk Management.",
            "Use Win + Arrow keys to snap windows to the sides or corners for easier multitasking.",
            "Enable clipboard history in Settings > System > Clipboard, then press Win + V to access it.",
            "You can rename multiple files at once by selecting them and pressing F2.",
            "Press Win + Tab to open Task View and manage virtual desktops.",
            "Pin your most-used folders to Quick Access in File Explorer for faster navigation.",
            "Press Win + I to open Settings quickly.",
            "Press Win + R to open the Run dialog for fast access to apps and commands.",
            "Press Ctrl + Shift + Esc to open Task Manager directly.",
            "Press Alt + Tab to quickly switch between your open windows.",
            "Hold Shift while clicking a taskbar icon to open a new window of that app.",
            "Press Win + Plus or Win + Minus to zoom in and out with the Magnifier.",
            "Press Win + P to switch between display modes when using multiple monitors.",
            "Press Win + Ctrl + D to create a new virtual desktop, and Win + Ctrl + Left or Right to switch desktops.",
            "Press Win + Comma to peek at the desktop while holding the key.",
            "Drag a window to the top edge of the screen to maximize it automatically.",
            "Drag a window to the left or right edge to make it fill half the screen.",
            "Press Win + Number to open the app pinned in that taskbar position.",
            "Middle-click a taskbar icon to open another instance of that app.",
            "Use Storage Sense in Settings > System > Storage to automatically clean temporary files.",
            "Use Disk Cleanup to free up space from old system files and temporary data.",
            "Turn on Night light in Settings > System > Display to reduce blue light at night.",
            "Use Focus Assist in Settings > System > Focus assist to mute notifications while working or gaming.",
            "Use Snip & Sketch to capture screenshots with a delay and annotate them.",
            "Turn on File History in Settings > Update & Security > Backup to protect important files.",
            "Use OneDrive to sync your Desktop and Documents between devices.",
            "Press Win + S and start typing to quickly search apps, files, and settings.",
            "Right-click on the taskbar to access taskbar settings and customize its behavior.",
            "Press Win + X to open a quick menu for system tools like PowerShell and Device Manager.",
            "Press Win + Space to switch between keyboard layouts or input languages.",
            "Press Win + V after enabling clipboard history to paste from recent items.",
            "Press Ctrl + Shift + N in File Explorer to create a new folder instantly.",
            "Press Alt + Enter on a file or folder to open its properties.",
            "Press Alt + Up Arrow in File Explorer to go up one folder level.",
            "Press Alt + Left or Alt + Right in File Explorer to go back or forward.",
            "Press Ctrl + L or Alt + D in File Explorer to jump to the address bar.",
            "Change your default apps in Settings > Apps > Default apps for browser, mail, and more.",
            "Use the built-in Emoji panel with Win + Period or Win + Semicolon to insert emojis.",
            "Press Win + H to open the voice typing tool and dictate text.",
            "Resize the Start menu by dragging its edges to show more or fewer apps.",
            "Pin your favorite apps to the taskbar for one-click access.",
            "Use the notification center (Win + A) to quickly toggle Wi-Fi, Bluetooth, and other quick actions.",
            "Turn on battery saver mode on laptops to extend battery life when running low.",
            "Use the built-in steps recorder to capture a sequence of actions for troubleshooting.",
            "Use the Game Bar (Win + G) to record screen clips while gaming or working.",
            "Customize power and sleep settings in Settings > System > Power & sleep to control when the PC sleeps.",
            "Use Task Manager's Startup tab to disable programs that start automatically with Windows.",
            "Use the Performance tab in Task Manager to monitor CPU, memory, disk, and GPU usage.",
            "Use file grouping and sorting in File Explorer to organize large folders by type or date.",
            "Show file extensions by turning on File name extensions in the File Explorer View menu.",
            "Use the search box in File Explorer to quickly filter files within the current folder.",
            "Use Win + Shift + Left or Right Arrow to move a window between monitors.",
            "Use the volume mixer (right-click the speaker icon) to control sound levels per application.",
            "Use OneDrive settings to automatically save screenshots taken with the Print Screen key.",
            "Use built-in troubleshooters in Settings > Update & Security > Troubleshoot to fix common problems.",
            "Keep Windows updated via Settings > Update & Security > Windows Update for performance and security.",
            "Use Windows Security to run virus scans and manage protection settings.",
            "Use Sticky Notes to keep small reminders and sync them across devices.",
            "Use the Alarms & Clock app to set timers and focus sessions while you work.",
            "Use the Calculator app's Programmer and Converter modes for advanced calculations and unit conversions.",
            "Pin frequently used settings pages to Start for faster access.",
            "Use the Photos app to quickly crop, rotate, and lightly edit pictures.",
            "Right-click a file and choose Send to > Compressed (zipped) folder to create a quick ZIP file.",
            "Use Task View to see a timeline of documents and activity from previous days.",
            "Press Win + K to quickly connect to wireless displays and audio devices.",
            "Use Nearby sharing to send files to nearby Windows PCs over Bluetooth or Wi-Fi.",
            "Use the Devices page in Settings to manage Bluetooth mice, keyboards, controllers, and printers.",
            "Use Open file location from Start menu search results to find where an app is installed.",
            "Press Win + Ctrl + F4 to close the current virtual desktop without closing its apps.",
            "Sync your clipboard between devices by enabling cloud clipboard in Settings > System > Clipboard.",
            "Use Reset this PC in Settings > Update & Security > Recovery to repair a badly broken system.",
            "Create restore points so you can roll back system changes if an update causes issues.",
            "Use the zoom slider in File Explorer to change file and folder icon size quickly.",
            "Use custom folder icons to visually distinguish important project folders.",
            "Sort a folder by Date modified when working with many files to find your latest work quickly.",
            "Keep your Desktop clean by using it only for shortcuts you really need.",
            "Use separate virtual desktops to keep work, study, and gaming windows organized.",
            "Use Windows Hello (face, fingerprint, or PIN) for faster and more secure sign-in when available.",
            "Use Narrator to have on-screen text read aloud if you need audio assistance.",
            "Turn on color filters in Settings > Ease of Access if you have trouble seeing certain colors.",
            "Press Shift five times quickly to open the Sticky Keys options.",
            "Use Ctrl + Mouse wheel to zoom in and out in many apps, including browsers and File Explorer.",
            "Press Win + Ctrl + Enter to quickly toggle Narrator on or off.",
            "Clear clipboard history in Settings > System > Clipboard if you want to remove stored clipboard data.",
            "Use Storage recommendations under Settings > System > Storage to find large files and unused apps.",
            "Use Open with on a file to pick a specific app for that file type when needed.",
            "Use Restore previous versions on files and folders if File History or backups are enabled.",
            "Customize the Quick Access toolbar in File Explorer to pin commands like New folder or Properties.",
            "Use Check for updates regularly to get driver and security improvements.",
            "Use the Feedback Hub app to report bugs or suggest features directly to Microsoft.",
            "Use the taskbar search to quickly launch control panel items by typing their names.",
            "Press Win + Ctrl + Shift + B if your display driver crashes to try resetting it without rebooting.",
            "Use auto-scrolling with the middle mouse button in many apps to read long pages more easily.",
            "Learn browser shortcuts like Ctrl + T for new tab and Ctrl + W for close tab to combine with Windows shortcuts.",
            "Use a standard (non-admin) account for day-to-day work and an admin account only when necessary.",
            "Back up important files to both the cloud and an external drive for extra protection.",
            "Use Dynamic lock in sign-in options so Windows locks automatically when your paired phone moves away."
        };
        
        public MainForm()
        {
            _settings = SettingsManager.Load();
            InitializeComponents();
            LoadInitialSettings();
        }

        #region UI Initialization

        private void InitializeComponents()
        {
            Text = "Screenshot Organizer";
            Width = 900;
            Height = 550;
            MinimumSize = new Size(800, 450);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9F);

            try
            {
                Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch
            {
            }

            _menuStrip = new MenuStrip();

            var fileMenu = new ToolStripMenuItem("File");
            var exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += (s, e) => Close();
            fileMenu.DropDownItems.Add(exitItem);

            var viewMenu = new ToolStripMenuItem("View");
            var lightThemeItem = new ToolStripMenuItem("Light theme") { Tag = AppTheme.Light };
            lightThemeItem.Click += ThemeMenuItem_Click;
            var darkThemeItem = new ToolStripMenuItem("Dark theme") { Tag = AppTheme.Dark };
            darkThemeItem.Click += ThemeMenuItem_Click;
            var separatorView = new ToolStripSeparator();
            var clearLogItem = new ToolStripMenuItem("Clear log");
            clearLogItem.Click += ClearLogMenu_Click;
            var resetStatsItem = new ToolStripMenuItem("Reset counters");
            resetStatsItem.Click += ResetStatsMenu_Click;

            viewMenu.DropDownItems.Add(lightThemeItem);
            viewMenu.DropDownItems.Add(darkThemeItem);
            viewMenu.DropDownItems.Add(separatorView);
            viewMenu.DropDownItems.Add(clearLogItem);
            viewMenu.DropDownItems.Add(resetStatsItem);

            var settingsMenu = new ToolStripMenuItem("Settings");
            _startupMenuItem = new ToolStripMenuItem("Run at Windows startup")
            {
                CheckOnClick = true,
                Checked = _settings.RunOnStartup
            };
            _startupMenuItem.Click += StartupMenuItem_Click;
            settingsMenu.DropDownItems.Add(_startupMenuItem);

            var helpMenu = new ToolStripMenuItem("Help");
            var aboutItem = new ToolStripMenuItem("About");
            aboutItem.Click += ShowAboutDialog;
            helpMenu.DropDownItems.Add(aboutItem);

            _menuStrip.Items.Add(fileMenu);
            _menuStrip.Items.Add(viewMenu);
            _menuStrip.Items.Add(settingsMenu);
            _menuStrip.Items.Add(helpMenu);

            _statusStrip = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel("Not watching")
            {
                Spring = true,
                TextAlign = ContentAlignment.MiddleLeft
            };
            _statsLabel = new ToolStripStatusLabel("Files moved: 0")
            {
                TextAlign = ContentAlignment.MiddleRight
            };
            _statusStrip.Items.Add(_statusLabel);
            _statusStrip.Items.Add(_statsLabel);

            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                Padding = new Padding(12, 10, 12, 5)
            };

            var titleLabel = new Label
            {
                Text = "Screenshot Organizer",
                AutoSize = true,
                Font = new Font("Segoe UI", 15f, FontStyle.Bold),
                Location = new Point(5, 5)
            };

            var subtitleLabel = new Label
            {
                Text = "Automatically sort your screenshots into date-based folders. Choose a folder, start watching, and relax.",
                AutoSize = true,
                MaximumSize = new Size(850, 0),
                Location = new Point(7, 35)
            };

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(subtitleLabel);

            var topPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 3,
                AutoSize = true,
                Padding = new Padding(12, 5, 12, 5),
            };
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var lblFolder = new Label
            {
                Text = "Screenshots folder:",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 6, 10, 0)
            };

            _txtFolder = new TextBox
            {
                ReadOnly = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Width = 450
            };

            _btnBrowse = new Button
            {
                Text = "Browse...",
                AutoSize = true,
                Anchor = AnchorStyles.Right
            };
            _btnBrowse.Click += BrowseFolderClicked;

            topPanel.Controls.Add(lblFolder, 0, 0);
            topPanel.Controls.Add(_txtFolder, 1, 0);
            topPanel.Controls.Add(_btnBrowse, 2, 0);

            var secondRowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(12, 0, 12, 10),
                AutoSize = true
            };

            _btnToggleWatching = new Button
            {
                Text = "Start watching",
                AutoSize = true,
                Margin = new Padding(0, 5, 8, 0)
            };
            _btnToggleWatching.Click += ToggleWatchingClicked;

            _btnOrganizeNow = new Button
            {
                Text = "Organize existing now",
                AutoSize = true,
                Margin = new Padding(0, 5, 8, 0)
            };
            _btnOrganizeNow.Click += OrganizeExistingClicked;

            _btnOpenFolder = new Button
            {
                Text = "Open folder",
                AutoSize = true,
                Margin = new Padding(0, 5, 8, 0)
            };
            _btnOpenFolder.Click += OpenFolderClicked;

            secondRowPanel.Controls.Add(_btnToggleWatching);
            secondRowPanel.Controls.Add(_btnOrganizeNow);
            secondRowPanel.Controls.Add(_btnOpenFolder);

            var logLabel = new Label
            {
                Text = "Activity log",
                AutoSize = true,
                Padding = new Padding(12, 0, 0, 0),
                Dock = DockStyle.Top
            };

            _lstLog = new ListBox
            {
                Dock = DockStyle.Fill,
                HorizontalScrollbar = true
            };

            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12, 4, 12, 0)
            };
            mainPanel.Controls.Add(_lstLog);

            Controls.Add(mainPanel);
            Controls.Add(logLabel);
            Controls.Add(secondRowPanel);
            Controls.Add(topPanel);
            Controls.Add(headerPanel);
            Controls.Add(_statusStrip);
            Controls.Add(_menuStrip);

            MainMenuStrip = _menuStrip;

            FormClosing += MainForm_FormClosing;

            SetupToolTips();
            ApplyTheme(_currentTheme);
        }

        #endregion

        #region Settings

        private void LoadInitialSettings()
        {
            if (!string.IsNullOrWhiteSpace(_settings.WatchedFolder) &&
                Directory.Exists(_settings.WatchedFolder))
            {
                _txtFolder.Text = _settings.WatchedFolder;
                _watchedFolder = _settings.WatchedFolder;
            }

            if (!string.IsNullOrWhiteSpace(_settings.Theme) &&
                Enum.TryParse<AppTheme>(_settings.Theme, out var savedTheme))
            {
                _currentTheme = savedTheme;
            }

            ApplyTheme(_currentTheme);
            UpdateStatusLabel();
            UpdateStatsLabel();

            if (_startupMenuItem != null)
            {
                _startupMenuItem.Checked = _settings.RunOnStartup;
            }

            ApplyRunOnStartupSetting();
            ShowStartupTipIfEnabled();
        }

        private void SaveSettings()
        {
            _settings.WatchedFolder = _watchedFolder;
            _settings.Theme = _currentTheme.ToString();
            SettingsManager.Save(_settings);
        }

        private void ApplyRunOnStartupSetting()
        {
            try
            {
                if (_settings.RunOnStartup)
                {
                    EnableRunOnStartup();
                }
                else
                {
                    DisableRunOnStartup();
                }
            }
            catch (Exception ex)
            {
                AddLog($"Failed to update startup setting: {ex.Message}");
            }
        }

        #endregion

        #region Event Handlers

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            StopWatching();
            SaveSettings();
        }

        private void ShowAboutDialog(object? sender, EventArgs e)
        {
            var message =
                "Screenshot Organizer\n" +
                "A 100% free and open-source tool created by Ameer.\n\n" +
                "It automatically organizes your screenshots into clean, date-based folders,\n" +
                "so you spend less time searching and more time doing what matters.\n\n" +
                "No ads. No tracking. Everything runs locally on your PC.\n" +
                "Enjoy a simple, lightweight utility built to make your Windows life easier. <3";

            MessageBox.Show(this, message, "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BrowseFolderClicked(object? sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Select the folder where your screenshot files are saved."
            };

            if (!string.IsNullOrWhiteSpace(_watchedFolder) &&
                Directory.Exists(_watchedFolder))
            {
                dialog.SelectedPath = _watchedFolder;
            }

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                _watchedFolder = dialog.SelectedPath;
                _txtFolder.Text = _watchedFolder;
                SaveSettings();

                AddLog($"Selected folder: {_watchedFolder}");
            }
        }

        private void ToggleWatchingClicked(object? sender, EventArgs e)
        {
            if (_isWatching)
            {
                StopWatching();
            }
            else
            {
                StartWatching();
            }
        }

        private async void OrganizeExistingClicked(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_watchedFolder) || !Directory.Exists(_watchedFolder))
            {
                MessageBox.Show(this,
                    "Please select a valid screenshots folder first.",
                    "No folder selected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var files = Directory.GetFiles(_watchedFolder);
            int processed = 0;

            foreach (var file in files)
            {
                if (!IsImageFile(file))
                    continue;

                await ProcessNewFileAsync(file);
                processed++;
            }

            AddLog($"Organized {processed} existing file(s).");
        }

        private void OpenFolderClicked(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_watchedFolder) || !Directory.Exists(_watchedFolder))
            {
                MessageBox.Show(this,
                    "Please select a valid screenshots folder first.",
                    "No folder selected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = _watchedFolder,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                AddLog($"Error opening folder: {ex.Message}");
            }
        }

        private void ThemeMenuItem_Click(object? sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item && item.Tag is AppTheme theme)
            {
                ApplyTheme(theme);
                SaveSettings();
            }
        }

        private void ClearLogMenu_Click(object? sender, EventArgs e)
        {
            _lstLog.Items.Clear();
            AddLog("Log cleared.");
        }

        private void ResetStatsMenu_Click(object? sender, EventArgs e)
        {
            _filesMoved = 0;
            UpdateStatsLabel();
            AddLog("File counters reset.");
        }

        private void StartupMenuItem_Click(object? sender, EventArgs e)
        {
            bool enable = _startupMenuItem.Checked;
            _settings.RunOnStartup = enable;
            ApplyRunOnStartupSetting();
            SaveSettings();
        }

        private void WatcherOnCreated(object sender, FileSystemEventArgs e)
        {
            if (!IsImageFile(e.FullPath))
                return;

            Task.Run(() => ProcessNewFileAsync(e.FullPath));
        }

        private void WatcherOnRenamed(object sender, RenamedEventArgs e)
        {
            if (!IsImageFile(e.FullPath))
                return;

            Task.Run(() => ProcessNewFileAsync(e.FullPath));
        }

        #endregion

        #region Watching Logic

        private void StartWatching()
        {
            if (string.IsNullOrWhiteSpace(_watchedFolder))
            {
                MessageBox.Show(this,
                    "Please select a screenshots folder first.",
                    "No folder selected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!Directory.Exists(_watchedFolder))
            {
                MessageBox.Show(this,
                    "The selected folder does not exist anymore. Please choose a valid folder.",
                    "Folder not found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            try
            {
                StopWatching();

                _watcher = new FileSystemWatcher(_watchedFolder)
                {
                    IncludeSubdirectories = false,
                    Filter = "*.*",
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime
                };

                _watcher.Created += WatcherOnCreated;
                _watcher.Renamed += WatcherOnRenamed;
                _watcher.EnableRaisingEvents = true;

                _isWatching = true;
                _btnToggleWatching.Text = "Stop watching";
                AddLog($"Started watching folder: {_watchedFolder}");
            }
            catch (Exception ex)
            {
                _isWatching = false;
                _btnToggleWatching.Text = "Start watching";
                AddLog($"Error starting watcher: {ex.Message}");
                MessageBox.Show(this,
                    "Failed to start watching the folder. See log for details.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                UpdateStatusLabel();
            }
        }

        private void StopWatching()
        {
            if (_watcher != null)
            {
                try
                {
                    _watcher.EnableRaisingEvents = false;
                    _watcher.Created -= WatcherOnCreated;
                    _watcher.Renamed -= WatcherOnRenamed;
                    _watcher.Dispose();
                }
                catch
                {
                }
                finally
                {
                    _watcher = null;
                }
            }

            if (_isWatching)
            {
                AddLog("Stopped watching.");
            }

            _isWatching = false;
            _btnToggleWatching.Text = "Start watching";
            UpdateStatusLabel();
        }

        private bool IsImageFile(string fullPath)
        {
            string extension = Path.GetExtension(fullPath).ToLowerInvariant();

            switch (extension)
            {
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".bmp":
                case ".gif":
                case ".tiff":
                case ".webp":
                    return true;
                default:
                    return false;
            }
        }

        private async Task ProcessNewFileAsync(string fullPath)
        {
            string fileName = Path.GetFileName(fullPath);

            try
            {
                await WaitUntilFileIsReadyAsync(fullPath);

                if (_watchedFolder == null)
                    return;

                DateTime creationTime;
                try
                {
                    creationTime = File.GetCreationTime(fullPath);
                }
                catch
                {
                    creationTime = DateTime.Now;
                }

                string dateFolderName = creationTime.ToString("yyyy-MM-dd");
                string targetFolder = Path.Combine(_watchedFolder, dateFolderName);
                Directory.CreateDirectory(targetFolder);

                string targetPath = Path.Combine(targetFolder, fileName);
                targetPath = GetUniquePath(targetPath);

                File.Move(fullPath, targetPath);

                IncrementFilesMoved();
                AddLog($"Moved \"{fileName}\" → \"{dateFolderName}\"");
            }
            catch (OperationCanceledException)
            {
                AddLog($"Skipped \"{fileName}\" because it was never ready to be read.");
            }
            catch (Exception ex)
            {
                AddLog($"Error moving \"{fileName}\": {ex.Message}");
            }
        }

        private async Task WaitUntilFileIsReadyAsync(string fullPath)
        {
            const int maxAttempts = 10;
            const int delayMs = 500;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                try
                {
                    using (var stream = new FileStream(
                               fullPath,
                               FileMode.Open,
                               FileAccess.Read,
                               FileShare.None))
                    {
                        return;
                    }
                }
                catch (IOException)
                {
                    await Task.Delay(delayMs);
                }
                catch (UnauthorizedAccessException)
                {
                    await Task.Delay(delayMs);
                }
            }

            throw new OperationCanceledException(
                $"File \"{fullPath}\" was not ready after multiple attempts.");
        }

        private string GetUniquePath(string targetPath)
        {
            if (!File.Exists(targetPath))
                return targetPath;

            string directory = Path.GetDirectoryName(targetPath) ?? "";
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(targetPath);
            string extension = Path.GetExtension(targetPath);

            int index = 1;
            string newPath;
            do
            {
                string newFileName = $"{fileNameWithoutExtension}_{index}{extension}";
                newPath = Path.Combine(directory, newFileName);
                index++;
            } while (File.Exists(newPath));

            return newPath;
        }

        #endregion

        #region Startup Helpers

        private void EnableRunOnStartup()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunRegistryKeyPath, writable: true)
                          ?? Registry.CurrentUser.CreateSubKey(RunRegistryKeyPath)!;

            key.SetValue(RunRegistryValueName, Application.ExecutablePath);
        }

        private void DisableRunOnStartup()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunRegistryKeyPath, writable: true);
            key?.DeleteValue(RunRegistryValueName, throwOnMissingValue: false);
        }

        #endregion

        #region Logging, Status, Tips, Theme, Tooltips

        private void AddLog(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(AddLog), message);
                return;
            }

            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            string line = $"{timestamp}  {message}";

            _lstLog.Items.Insert(0, line);

            if (_lstLog.Items.Count > MaxLogItems)
            {
                _lstLog.Items.RemoveAt(_lstLog.Items.Count - 1);
            }
        }

        private void UpdateStatusLabel()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(UpdateStatusLabel));
                return;
            }

            string folderPart = string.IsNullOrWhiteSpace(_watchedFolder)
                ? "(no folder selected)"
                : _watchedFolder;

            _statusLabel.Text = _isWatching
                ? $"Watching: {folderPart}"
                : $"Not watching. Folder: {folderPart}";
        }

        private void IncrementFilesMoved()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(IncrementFilesMoved));
                return;
            }

            _filesMoved++;
            UpdateStatsLabel();
        }

        private void UpdateStatsLabel()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(UpdateStatsLabel));
                return;
            }

            _statsLabel.Text = $"Files moved: {_filesMoved}";
        }

        private void ShowStartupTipIfEnabled()
        {
            if (!_settings.ShowTipsOnStartup)
                return;

            if (_windowsTips.Length == 0)
                return;

            int index = _random.Next(_windowsTips.Length);
            string tip = _windowsTips[index];

            ShowTipDialog(tip, out bool dontShowAgain);

            if (dontShowAgain)
            {
                _settings.ShowTipsOnStartup = false;
                SaveSettings();
            }
        }

        private void ShowTipDialog(string tip, out bool dontShowAgain)
        {
            using (Form dlg = new Form())
            {
                dlg.Text = "Tip of the day";
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MaximizeBox = false;
                dlg.MinimizeBox = false;
                dlg.ClientSize = new Size(420, 180);
                dlg.ShowInTaskbar = false;

                var iconBox = new PictureBox
                {
                    Image = SystemIcons.Question.ToBitmap(),
                    SizeMode = PictureBoxSizeMode.CenterImage,
                    Location = new Point(18, 25),
                    Size = new Size(32, 32)
                };

                var label = new Label
                {
                    Text = tip,
                    AutoSize = false,
                    Location = new Point(60, 20),
                    Size = new Size(330, 70)
                };

                var chk = new CheckBox
                {
                    Text = "Don't show tips on startup",
                    AutoSize = true,
                    Location = new Point(60, 95)
                };

                var btnOk = new Button
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                    Location = new Point(dlg.ClientSize.Width - 170, dlg.ClientSize.Height - 40),
                    Size = new Size(75, 23)
                };

                var btnCancel = new Button
                {
                    Text = "Cancel",
                    DialogResult = DialogResult.Cancel,
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                    Location = new Point(dlg.ClientSize.Width - 90, dlg.ClientSize.Height - 40),
                    Size = new Size(75, 23)
                };

                dlg.Controls.Add(iconBox);
                dlg.Controls.Add(label);
                dlg.Controls.Add(chk);
                dlg.Controls.Add(btnOk);
                dlg.Controls.Add(btnCancel);

                dlg.AcceptButton = btnOk;
                dlg.CancelButton = btnCancel;

                dlg.ShowDialog(this);

                dontShowAgain = chk.Checked;
            }
        }

        private void ApplyTheme(AppTheme theme)
        {
            _currentTheme = theme;

            Color formBack;
            Color formFore;
            Color panelBack;
            Color controlBack;
            Color controlFore;
            Color listBack;
            Color listFore;
            Color statusBack;
            Color statusFore;
            Color menuBack;
            Color menuFore;

            if (theme == AppTheme.Dark)
            {
                formBack = Color.FromArgb(32, 32, 36);
                formFore = Color.White;
                panelBack = Color.FromArgb(40, 40, 46);
                controlBack = Color.FromArgb(55, 55, 65);
                controlFore = Color.White;
                listBack = Color.FromArgb(24, 24, 28);
                listFore = Color.WhiteSmoke;
                statusBack = Color.FromArgb(30, 30, 34);
                statusFore = Color.Gainsboro;
                menuBack = Color.FromArgb(30, 30, 34);
                menuFore = Color.Gainsboro;
            }
            else
            {
                formBack = Color.White;
                formFore = Color.Black;
                panelBack = Color.FromArgb(245, 245, 248);
                controlBack = Color.White;
                controlFore = Color.Black;
                listBack = Color.White;
                listFore = Color.Black;
                statusBack = Color.FromArgb(240, 240, 244);
                statusFore = Color.Black;
                menuBack = Color.FromArgb(240, 240, 244);
                menuFore = Color.Black;
            }

            BackColor = formBack;
            ForeColor = formFore;
            Font = new Font("Segoe UI", 9F);

            foreach (Control control in Controls)
            {
                ApplyThemeToControl(control, formBack, formFore, panelBack, controlBack, controlFore, listBack, listFore);
            }

            if (_menuStrip != null)
            {
                _menuStrip.BackColor = menuBack;
                _menuStrip.ForeColor = menuFore;
                foreach (ToolStripItem item in _menuStrip.Items)
                {
                    item.ForeColor = menuFore;
                }
            }

            if (_statusStrip != null)
            {
                _statusStrip.BackColor = statusBack;
                _statusStrip.ForeColor = statusFore;
                foreach (ToolStripItem item in _statusStrip.Items)
                {
                    item.ForeColor = statusFore;
                }
            }

            UpdateThemeMenuChecks();
        }

        private void ApplyThemeToControl(
            Control control,
            Color formBack,
            Color formFore,
            Color panelBack,
            Color controlBack,
            Color controlFore,
            Color listBack,
            Color listFore)
        {
            if (control is Panel || control is TableLayoutPanel || control is FlowLayoutPanel)
            {
                control.BackColor = panelBack;
                control.ForeColor = formFore;
            }
            else if (control is Button)
            {
                control.BackColor = controlBack;
                control.ForeColor = controlFore;
                control.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            }
            else if (control is TextBox)
            {
                control.BackColor = controlBack;
                control.ForeColor = controlFore;
            }
            else if (control is ListBox)
            {
                control.BackColor = listBack;
                control.ForeColor = listFore;
            }
            else if (control is Label)
            {
                control.BackColor = Color.Transparent;
                control.ForeColor = formFore;
            }
            else
            {
                control.BackColor = formBack;
                control.ForeColor = formFore;
            }

            foreach (Control child in control.Controls)
            {
                ApplyThemeToControl(child, formBack, formFore, panelBack, controlBack, controlFore, listBack, listFore);
            }
        }

        private void UpdateThemeMenuChecks()
        {
            foreach (ToolStripItem topItem in _menuStrip.Items)
            {
                if (topItem is ToolStripMenuItem menu && menu.Text == "View")
                {
                    foreach (ToolStripItem sub in menu.DropDownItems)
                    {
                        if (sub is ToolStripMenuItem mi && mi.Tag is AppTheme theme)
                        {
                            mi.Checked = theme == _currentTheme;
                        }
                    }
                }
            }
        }

        private void SetupToolTips()
        {
            _toolTip = new ToolTip
            {
                AutoPopDelay = 8000,
                InitialDelay = 500,
                ReshowDelay = 200,
                ShowAlways = true
            };

            _toolTip.SetToolTip(_txtFolder, "This is the folder that will be watched and organized.");
            _toolTip.SetToolTip(_btnBrowse, "Choose the folder where your screenshots are saved.");
            _toolTip.SetToolTip(_btnToggleWatching, "Start or stop watching the selected folder for new screenshots.");
            _toolTip.SetToolTip(_btnOrganizeNow, "Immediately organize all existing image files in the selected folder.");
            _toolTip.SetToolTip(_btnOpenFolder, "Open the selected screenshots folder in File Explorer.");
            _toolTip.SetToolTip(_lstLog, "Shows a history of actions: moved files, errors, and other events.");
        }

        #endregion
    }

    public enum AppTheme
    {
        Light,
        Dark
    }

    public class AppSettings
    {
        public string? WatchedFolder { get; set; }
        public string? Theme { get; set; } = "Dark";
        public bool ShowTipsOnStartup { get; set; } = true;
        public bool RunOnStartup { get; set; } = true;
    }

    public static class SettingsManager
    {
        private const string AppFolderName = "ScreenshotOrganizer";
        private const string SettingsFileName = "settings.json";

        private static string AppFolderPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                AppFolderName);

        private static string SettingsFilePath =>
            Path.Combine(AppFolderPath, SettingsFileName);

        public static AppSettings Load()
        {
            try
            {
                if (!File.Exists(SettingsFilePath))
                {
                    return new AppSettings();
                }

                string json = File.ReadAllText(SettingsFilePath);
                var result = JsonSerializer.Deserialize<AppSettings>(json);
                return result ?? new AppSettings();
            }
            catch
            {
                return new AppSettings();
            }
        }

        public static void Save(AppSettings settings)
        {
            try
            {
                Directory.CreateDirectory(AppFolderPath);

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(settings, options);
                File.WriteAllText(SettingsFilePath, json);
            }
            catch
            {
            }
        }
    }
}
