using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Terminal.Gui;
using NStack;
using Revolt;
using Revolt.Channels;
using Revolt.Cli;

string GetVersion()
    => typeof(ExtensionMethods).Assembly.GetName().Version.ToString();

RevoltClient client = null;
if (!OperatingSystem.IsWindows())
    Application.UseSystemConsole = true;
Application.Init();
var top = Application.Top;

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
var groupsMenuBarItem = new MenuBarItem("_Groups", new List<MenuItem[]>());
var serversMenuBarItem = new MenuBarItem("_Servers", new List<MenuItem[]>());
var directMessagesMenuBarItem = new MenuBarItem("_Direct Messages", new List<MenuItem[]>());
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
            if (Quit()) top.Running = false;
        }),
    }),
    groupsMenuBarItem, serversMenuBarItem, directMessagesMenuBarItem,
});
top.Add(menu);

static bool Quit()
{
    var n = MessageBox.Query(50, 7, "Quit Revolt.Cli", "Are you sure you want to quit Revolt.Cli?", "NOO!!!", "No");
    return n == 0;
}

void ChannelLogic(Channel channel)
{
    var groupChannel = channel as GroupChannel;
    var textChannel = channel as TextChannel;
    var dmChannel = channel as DirectMessageChannel;
    top.RemoveAll();
    var newWin = new Window(groupChannel?.Name ?? textChannel?.Name ?? dmChannel?.OtherUser.Username)
    {
        X = 0,
        Y = 1,
    };
    newWin.SizeFix(top);
    var messagesView = new ScrollView()
    {
        X = 1,
        Y = 1,
        Width = Dim.Fill(),
        Height = Dim.Fill() - 1,
        ContentSize = new Size(2100, 1000)
    };
    var fuckImTired = 0;
    client.MessageReceived += message =>
    {
        Application.MainLoop.Invoke(() =>
        {
            if (message.ChannelId == channel._id)
            {
                var height = 1 + message.Content.Count(c => c == '\n');
                var userButton = new Button()
                {
                    X = 0,
                    Y = fuckImTired,
                    Width = message.Author.Username.Length,
                    Height = 1,
                    Text = message.Author.Username
                };
                userButton.Clicked += () => { MessageBox.Query(message.Author.Username, "hi", "Bye"); };
                messagesView.Add(userButton);
                messagesView.Add(new TextView
                {
                    X = Pos.Right(userButton) + 1,
                    Y = fuckImTired,
                    Width = Dim.Fill(),
                    Height = height,
                    Text = message.Content,
                    ReadOnly = true, CanFocus = false, Selecting = false
                });
                fuckImTired += height;
            }
        });
        return Task.CompletedTask;
    };
    newWin.Add(messagesView);
    // add controls to group chat
    var messageField = new TextField()
    {
        X = 1,
        Y = Pos.Bottom(newWin) - 4,
        Width = Dim.Fill(),
        Height = 1,
    };
    messageField.KeyPress += eventArgs =>
    {
        if (eventArgs.KeyEvent.Key == Key.Enter)
        {
            channel.SendMessageAsync(messageField.Text.ToString()!);
            messageField.Text = ustring.Empty;
        }
    };
    newWin.Add(messageField);
    top.Add(newWin);
    top.Add(menu);
    messageField.SetFocus();
}

void MainView()
{
    client = new(JsonConvert.DeserializeObject<Session>(
        File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/session.json"))!);
    client.OnReady += async () =>
    {
        try
        {
            var groupsMenuItems = new List<MenuItem>();
            var serversMenuItems = new List<MenuItem>();
            var dmMenuItems = new List<MenuItem>();
            foreach (var channel in client.ChannelsCache)
            {
                if (channel is GroupChannel groupChannel)
                    groupsMenuItems.Add(new MenuItem(groupChannel.Name, "", () => ChannelLogic(groupChannel)));
                if (channel is DirectMessageChannel dmChannel)
                    dmMenuItems.Add(new MenuItem(dmChannel.OtherUser.Username, "",
                        () => ChannelLogic(dmChannel)));
            }

            foreach (var server in client.ServersCache)
            {
                var children = new List<MenuItem>();
                foreach (var channelId in server.ChannelIds)
                {
                    var channel = (TextChannel) client.ChannelsCache.First(c => c._id == channelId);
                    children.Add(new(channel.Name, "", () => ChannelLogic(channel)));
                }

                var menu = new MenuBarItem(server.Name, server.Description?.Shorten(30) ?? "", null)
                {
                    Children = children.ToArray()
                };
                serversMenuItems.Add(menu);
            }

            groupsMenuBarItem.Children = groupsMenuItems.ToArray();
            serversMenuBarItem.Children = serversMenuItems.ToArray();
            directMessagesMenuBarItem.Children = dmMenuItems.ToArray();
        }
        catch (Exception exc)
        {
        }
    };
    client.ConnectWebSocketAsync();
}

void LogInView()
{
    var userIdLabel = new Label("User Id: ") {X = 3, Y = 2};
    var sessionIdLabel = new Label("Session Id: ")
    {
        X = Pos.Left(userIdLabel),
        Y = Pos.Top(userIdLabel) + 1
    };
    var sessionTokenLabel = new Label("Session Token: ")
    {
        X = Pos.Left(sessionIdLabel),
        Y = Pos.Top(sessionIdLabel) + 1
    };
    var userIdField = new TextField("")
    {
        X = Pos.Right(sessionTokenLabel),
        Y = Pos.Top(userIdLabel),
        Width = 40
    };
    var sessionIdField = new TextField("")
    {
        Secret = true,
        X = Pos.Left(userIdField),
        Y = Pos.Top(sessionIdLabel),
        Width = Dim.Width(userIdField)
    };
    var sessionTokenField = new TextField("")
    {
        Secret = true,
        X = Pos.Left(sessionIdField),
        Y = Pos.Top(sessionTokenLabel),
        Width = 70
    };
    var logInbutton = new Button("Log In")
    {
        X = 3,
        Y = Pos.Top(sessionTokenField) + 2,
    };
    logInbutton.Clicked += () =>
    {
        win.RemoveAll();
        File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/session.json",
            JsonConvert.SerializeObject(new Session()
            {
                UserId = userIdField.Text.ToString()!,
                Id = sessionIdField.Text.ToString(),
                SessionToken = sessionTokenField.Text.ToString()!,
            }));
        MainView();
    };
// Add some controls, 
    win.Add(
        // The ones with my favorite layout system, Computed
        userIdLabel, sessionIdLabel, sessionTokenLabel, userIdField, sessionIdField, sessionTokenField,
        logInbutton
        // new Label(3, 18, "Press F9 or ESC plus 9 to activate the menubar")
    );
}

if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/session.json"))
    LogInView();
else
    MainView();
Task.Delay(100).ContinueWith((_) => { Application.Refresh(); });
Application.Run();