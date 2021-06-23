using System;
using System.Collections.Generic;
using System.Linq;
using Revolt.Channels;
using Terminal.Gui;
using TextCopy;

namespace Revolt.Cli.Views.Windows
{
    public class UserProfileWindow : Window
    {
        public UserProfileWindow(User user)
        {
            X = 0;
            Y = 1;
            Title = user.Username;
            var addFriendButton =
                new Button(
                    (user.Relationship switch
                    {
                        RelationshipStatus.None => "Add Friend",
                        RelationshipStatus.Incoming => "Accept Friend Request",
                        RelationshipStatus.Outgoing => "Cancel Outgoing",
                        RelationshipStatus.Friend => "Unfriend",
                        _ => "Other Relationship"
                    }), false)
                {
                    X = 2, Y = 0
                };
            addFriendButton.Clicked += () =>
            {
                if (user.Relationship == RelationshipStatus.None || user.Relationship == RelationshipStatus.Incoming)
                    user.AddFriendAsync();
                if (user.Relationship == RelationshipStatus.Outgoing || user.Relationship == RelationshipStatus.Friend)
                    user.RemoveFriendAsync();
            };
            var copyIdButton = new Button("Copy User Id")
            {
                Y = addFriendButton.Y,
                X = Pos.Right(addFriendButton)
            };
            copyIdButton.Clicked += () => { ClipboardService.SetText(user._id); };
            var copyPfpButton = new Button()
            {
                X = Pos.Right(copyIdButton),
                Y = copyIdButton.Y,
                Text = "Copy Avatar Url"
            };
            copyPfpButton.Clicked += () => { ClipboardService.SetText(user.AvatarUrl); };
            Add(addFriendButton,copyIdButton, copyPfpButton);
            Add(new Label(2, 1, user.Status.Presence), new Label(2, 2, user.Status.Text ?? ""),
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
                Height = Dim.Fill(),
                ReadOnly = true,
                Multiline = true,
                WordWrap = true
            }), false);

            #region mutual friends

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
            foreach (var mutualFriend in mutualFriends)
                mutualFriendsView.Add(mutualFriend);
            tabView.AddTab(new TabView.Tab("Mutual Friends", mutualFriendsView), false);

            #endregion

            #region mutual groups

            var mutualGroups = new List<Button>();
            foreach (var group in App.Client.ChannelsCache.OfType<GroupChannel>()
                .Where(c => c.RecipientIds.Contains(user._id)))
            {
                var button = new Button(0, mutualGroups.Count, group.Name);
                button.Clicked += () =>
                {
                    new ChannelWindow(group).Show();
                    this.Hide();
                };
                mutualGroups.Add(button);
            }

            var mutualGroupsView = new ScrollView()
            {
                X = 0, Y = 0,
                Height = Dim.Fill(),
                Width = Dim.Fill(),
                ContentSize = new(mutualGroups.GetLongest(), mutualGroups.Count)
            };
            foreach (var mutualGroup in mutualGroups)
                mutualGroupsView.Add(mutualGroup);
            tabView.AddTab(new TabView.Tab("Mutual Groups", mutualGroupsView), false);

            #endregion

            Add(badgesList, tabView);
        }
    }
}