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
            var groupsMenuItems = new List<MenuItem>();
            var serversMenuItems = new List<MenuItem>();
            var dmMenuItems = new List<MenuItem>();
            foreach (var channel in App.Client.ChannelsCache)
            {
                if (channel is GroupChannel groupChannel)
                    groupsMenuItems.Add(new MenuItem(groupChannel.Name, "", () => new ChannelWindow(channel).Show()));
                if (channel is DirectMessageChannel dmChannel)
                    dmMenuItems.Add(new MenuItem(dmChannel.OtherUser.Username, "",
                        () => new ChannelWindow(channel).Show()));
            }

            foreach (var server in App.Client.ServersCache)
            {
                var children = new List<MenuItem>();
                foreach (var channelId in server.ChannelIds)
                {
                    var channel = (TextChannel)App.Client.ChannelsCache.First(c => c._id == channelId);
                    children.Add(new(channel.Name, "", () => new ChannelWindow(channel).Show()));
                }

                var menu = new MenuBarItem(server.Name, server.Description?.Shorten(30) ?? "", null)
                {
                    Children = children.ToArray()
                };
                serversMenuItems.Add(menu);
            }

            App.GroupsMenuBarItem.Children = groupsMenuItems.ToArray();
            App.ServersMenuBarItem.Children = serversMenuItems.ToArray();
            App.DirectMessagesMenuBarItem.Children = dmMenuItems.ToArray();
        }
    }
}