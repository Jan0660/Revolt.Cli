using System;
using System.Collections.Generic;
using Terminal.Gui;

namespace Revolt.Cli.Views.Windows
{
    public class UserProfileWindow : Window
    {
        public UserProfileWindow(User user)
        {
            X = 0;
            Y = 1;
            Title = user.Username;
            Add(new Label(2, 1, user.Status.Presence), new Label(2, 2, user.Status.Text),
                new Label(2, 3, "Badges:"));
            var longestBadge = 0;
            var badges = new List<Label>();
            foreach (var badgeType in Enum.GetValues<Badges>())
                if (user.Badges.HasFlag(badgeType))
                {
                    var str = badgeType.ToString();
                    badges.Add(new Label(0, badges.Count, str));
                    if (str.Length > longestBadge)
                        longestBadge = str.Length;
                }

            var badgesList = new ScrollView()
            {
                X = 4,
                Y = 4,
                Width = longestBadge + 5,
                Height = 5,
                ContentSize = new(longestBadge, badges.Count)
            };
            foreach (var badge in badges)
                badgesList.Add(badge);
            var tabView = new TabView()
            {
                X = 2,
                Y = 8,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            tabView.AddTab(new TabView.Tab("Profile", new TextView()
            {
                Text = user.GetProfile().Content,
                X = 0, Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 8,
                ReadOnly = true,
                Multiline = true,
                WordWrap = true
            }), false);
            var mutualFriends = new List<Label>();
            foreach (var userId in user.GetMutualRelationships().UserIds)
            {
                mutualFriends.Add(new(0, mutualFriends.Count, user.Client.Users.Get(userId).Username));
            }

            var mutualFriendsView = new ScrollView()
            {
                X = 0, Y = 0,
                Height = Dim.Fill(),
                Width = Dim.Fill(),
                ContentSize = new(mutualFriends.GetLongest(), mutualFriends.Count)
            };
            foreach(var mutualFriend in mutualFriends)
                mutualFriendsView.Add(mutualFriend);
            tabView.AddTab(new TabView.Tab("Mutual Friends", mutualFriendsView), false);
            Add(badgesList, tabView);
        }
    }
}