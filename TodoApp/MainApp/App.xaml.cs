using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using TodoApp.Models;
using TodoApp.Windows;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Collections;

namespace TodoApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private System.Windows.Forms.NotifyIcon _notifyIcon;

        public void StartUp(object sender, StartupEventArgs e)
        {
            string rootDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TodoApp");
            if (!File.Exists(rootDirectory))
            {
                Directory.CreateDirectory(rootDirectory);
            }
            bool settingsExists = GetSettings(rootDirectory);
            GetTodos(rootDirectory);
            ShowNotifications();
            SetupNotifyIcon();
            AddToStartup();
            ShowMainWindow(!settingsExists, false);
        }

        private bool GetSettings(string rootDirectory)
        {
            string settingsFilePath = Path.Combine(rootDirectory, "Settings.json");
            bool settingsExists = false;

            if (File.Exists(settingsFilePath))
            {
                try
                {
                    string json = File.ReadAllText(settingsFilePath);
                    Models.Application.Instance.SettingsFile = JsonConvert.DeserializeObject<Settings>(json);
                    settingsExists = true;
                }
                catch
                {
                    Models.Application.Instance.SettingsFile = new Settings();
                    settingsExists = false;
                }
            }
            else
            {
                Settings newSettings = new Settings();
                Models.Application.Instance.SettingsFile = newSettings;
                Directory.CreateDirectory(rootDirectory);
                using (var settingsFile = File.CreateText(settingsFilePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(settingsFile, newSettings);
                }
                settingsExists = false;
            }
            return settingsExists;
        }

        private void GetTodos(string rootDirectory)
        {
            string todoFilePath = Path.Combine(rootDirectory, "Todo.json");
            if (File.Exists(todoFilePath))
            {
                try
                {
                    string json = File.ReadAllText(todoFilePath);
                    Models.Application.Instance.allTodos = JsonConvert.DeserializeObject<List<Todo>>(json);
                    if (Models.Application.Instance.allTodos == null || !(Models.Application.Instance.allTodos.Count > 0))
                        Models.Application.Instance.allTodos = new List<Todo>();
                }
                catch
                {
                    Models.Application.Instance.allTodos = new List<Todo>();
                }
            }
            else
            {
                var todoFile = File.Create(todoFilePath);
                Models.Application.Instance.allTodos = new List<Todo>();
                todoFile.Close();
            }
        }

        private void ShowNotifications()
        {
            if (Models.Application.Instance.SettingsFile.ShowNotifications)
            {
                Todo todoToShow = null;
                int todosHappeningToday = 0;
                foreach (Todo todo in Models.Application.Instance.allTodos)
                {
                    TimeSpan timeInterval = todo.EndDate - DateTime.Now;
                    if (timeInterval.TotalDays < 1)
                    {
                        todoToShow = todo;
                        todosHappeningToday += 1;
                    }
                }

                if (todosHappeningToday > 0)
                {
                    new ToastContentBuilder()
                    .AddArgument("action", "viewConversation")
                    .AddArgument("conversationId", 9813)
                    .AddText(todosHappeningToday.ToString() + " Todo's are finishing today!").Show();
                    ToastNotificationManagerCompat.OnActivated += toastArgs =>
                    {
                        ShowMainWindow(false, true);
                    };
                }
            }
        }

        private void SetupNotifyIcon()
        {
            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            _notifyIcon.DoubleClick += (s, args) => ShowMainWindow(false, true);
            _notifyIcon.Icon = TodoApp.Properties.Resources.TodoAppIcon;
            _notifyIcon.Visible = true;
            CreateContextMenu();
        }

        private void CreateContextMenu()
        {
            _notifyIcon.ContextMenuStrip =
              new System.Windows.Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Open").Click += (s, e) => ShowMainWindow(false, true);
            _notifyIcon.ContextMenuStrip.Items.Add("Exit").Click += (s, e) => CloseApp();
        }

        private void ShowMainWindow(bool showSettings, bool notificationClicked)
        {
            if (!Models.Application.Instance.SettingsFile.StartInBackground && !notificationClicked)
            {
                MainWindow window = new MainWindow();
                window.Show();
                if (showSettings)
                {
                    SettingsWindow sWindow = new SettingsWindow();
                    sWindow.Show();
                }
            }
            else if (notificationClicked)
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (MainWindow != null)
                    {
                        if (MainWindow.IsVisible)
                        {
                            if (MainWindow.WindowState == WindowState.Minimized)
                            {
                                MainWindow.WindowState = WindowState.Normal;
                            }
                            MainWindow.Activate();
                        }
                        else
                        {
                            MainWindow.Show();
                        }
                    }
                    else
                    {
                        MainWindow window = new MainWindow();
                        window.Show();
                    }
                });
            }
        }

        private void AddToStartup()
        {
            if (Models.Application.Instance.SettingsFile.StartUp)
            {
                try
                {
                    Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                    Assembly curAssembly = Assembly.GetExecutingAssembly();
                    key.SetValue(curAssembly.GetName().Name, curAssembly.Location);
                }
                catch { }
            }
        }

        private void CloseApp()
        {
            if (MainWindow != null)
            {
                MainWindow.Close();
                _notifyIcon.Dispose();
            }
            Current.Shutdown();
        }

        //Update the settings and todo area on exit
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            AddToStartup();
            string rootDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TodoApp");
            string settingsFilePath = Path.Combine(rootDirectory, "Settings.json");
            string todoFilePath = Path.Combine(rootDirectory, "Todo.json");
            string settingJson = JsonConvert.SerializeObject(Models.Application.Instance.SettingsFile);
            string todoJson = JsonConvert.SerializeObject(Models.Application.Instance.allTodos.ToArray());
            File.WriteAllText(settingsFilePath, settingJson);
            File.WriteAllText(todoFilePath, todoJson);
        }
    }
}
