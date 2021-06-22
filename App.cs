using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Revolt.Cli.Windows;
using Terminal.Gui;

namespace Revolt.Cli
{
    public static class App
    {
        public static Toplevel Toplevel;
        public static MenuBar MenuBar;
        public static RevoltClient Client;
        public static MenuBarItem GroupsMenuBarItem;
        public static MenuBarItem ServersMenuBarItem;
        public static MenuBarItem DirectMessagesMenuBarItem;

        public static void Run()
        {
            RevoltClient client = null;
            var top = Application.Top;
            Toplevel = top;

// Creates the top-level window to show
            var win = new Window("Revolt.Cli")
            {
                X = 0,
                Y = 1, // Leave one row for the toplevel menu

                // By using Dim.Fill(), it will automatically resize without manual intervention
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
// win.ColorScheme.Normal = new(Color.White);
            top.Add(win);

// Creates a menubar, the item "New" has a help menu.
            GroupsMenuBarItem = new MenuBarItem("_Groups", new List<MenuItem[]>());
            ServersMenuBarItem = new MenuBarItem("_Servers", new List<MenuItem[]>());
            DirectMessagesMenuBarItem = new MenuBarItem("_Direct Messages", new List<MenuItem[]>());
            var menu = new MenuBar(new[]
            {
                new("_App", new MenuItem[]
                {
                    new("API Info", "", () =>
                    {
                        var info = client.ApiInfo;
                        var vortex = client.GetVortexInfo();
                        var autumn = client.GetAutumnInfo();

                        // todo: ansi
                        string StrBool(bool b)
                            => b ? "true" : "false";

                        MessageBox.Query("Revolt API Info", $@"Versions
Api: {info.Version}
Vortex: {vortex.Version}
Autumn: {autumn.Version}

Features
Registration: {StrBool(info.Features.Registration)}
Email: {StrBool(info.Features.Email)}
Captcha: {StrBool(info.Features.Captcha.Enabled)}
Max Attachment Size: {autumn.Tags.Attachments.MaxSize / 1000 / 1000}MB", "Ok");
                    }),
                    new("_Reconnect", "", () =>
                    {
                        client.DisconnectWebsocket();
                        client.ConnectWebSocketAsync();
                    }),
                    new("_About", "", () =>
                    {
                        MessageBox.Query("Revolt.Cli", $@"Developed by Jan0660
Version {GetVersion()}
Source: https://github.com/Jan0660/Revolt.Cli", "Ok");
                    }),
                    new("_Quit", "", () =>
                    {
                        if (Quit())
                        {
                            top.Running = false;
                        }
                    }),
                }),
                GroupsMenuBarItem, ServersMenuBarItem, DirectMessagesMenuBarItem,
            });
            MenuBar = menu;
            top.Add(menu);

            static bool Quit()
            {
                var n = MessageBox.Query(50, 7, "Quit Revolt.Cli", "Are you sure you want to quit Revolt.Cli?",
                    "NOO!!!", "No");
                return n == 0;
            }

            void MainView()
            {
                client = new(JsonConvert.DeserializeObject<Session>(
                    File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                     "/session.json"))!);
                Client = client;
                client.OnReady += async () =>
                {
                    try
                    {
                        new HomeWindow().Show();
                    }
                    catch (Exception exc)
                    {
                    }
                };
                client.ConnectWebSocketAsync();
            }

            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/session.json"))
                new LogInWindow().Show();
            else
                MainView();
            Task.Delay(100).ContinueWith(_ => { Application.Refresh(); });
            Application.Run();
        }

        public static string GetVersion()
            => typeof(ExtensionMethods).Assembly.GetName().Version!.ToString();
    }
}