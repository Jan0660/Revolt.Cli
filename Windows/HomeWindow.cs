using System.Collections.Generic;
using System.Linq;
using Revolt.Channels;
using Terminal.Gui;

namespace Revolt.Cli.Windows
{
    public class HomeWindow : Window
    {
        public HomeWindow()
        {
            Title = "Revolt.Cli - Home";
            Add(new Label(1, 1, $"Welcome to Revolt.Cli, {App.Client.Users.Get(App.Client.Self.UserId).Username}."));
            Add(new Label(1, 3, $"Online Friends"));
            List<View> onlineFriends = new();
            var widestName = 0;
            for (var i = 0; i < App.Client.UsersCache.Count; i++)
            {
                var user = App.Client.UsersCache[i];
                if (user.Relationship == RelationshipStatus.Friend && user?.Status?.Presence == "Online")
                {
                    // todo: open dm or create one and open it
                    var button = new Label(0, onlineFriends.Count, user.Username);
                    onlineFriends.Add(button);
                    if (user.Username.Length > widestName)
                        widestName = user.Username.Length;
                }
            }

            var friendsList = new ScrollView()
            {
                ContentSize = new(widestName, onlineFriends.Count),
                X = 3,
                Y = 4,
                Width = widestName + 2,
                Height = Dim.Fill() - 3,
            };
            foreach (var h in onlineFriends)
                friendsList.Add(h);
            Add(friendsList);
        }
    }
}