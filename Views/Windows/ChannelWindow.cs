using System.Linq;
using System.Threading.Tasks;
using NStack;
using Revolt.Channels;
using Terminal.Gui;

namespace Revolt.Cli.Views.Windows
{
    public class ChannelWindow : Window
    {
        public ChannelWindow(Channel channel)
        {
            var groupChannel = channel as GroupChannel;
            var textChannel = channel as TextChannel;
            var dmChannel = channel as DirectMessageChannel;
            X = 0;
            Y = 1;
            Title = groupChannel?.Name ?? textChannel?.Name ?? dmChannel?.OtherUser.Username;
            var messagesView = new ScrollView()
            {
                X = 1,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1,
                ContentSize = new Size(2100, 1000)
            };
            var fuckImTired = 0;

            void AddMessage(Message message)
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
                userButton.Clicked += () =>
                {
                    new MessageContextMenu(message)
                    {
                        Y = Pos.Bottom(userButton),
                        X = userButton.X
                    }.Show(false, false, messagesView);
                };
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

            channel.Client.MessageReceived += message =>
            {
                Application.MainLoop.Invoke(() =>
                {
                    if (message.ChannelId == channel._id)
                    {
                        AddMessage(message);
                    }
                });
                return Task.CompletedTask;
            };
            this.Add(messagesView);
            // add controls to group chat
            var messageField = new TextField()
            {
                X = 1,
                Y = Pos.Bottom(this) - 4,
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
            this.Add(messageField);
            messageField.SetFocus();
            App.Client.Channels.GetMessagesAsync(channel._id, 5).ContinueWith(val =>
            {
                Application.MainLoop.Invoke(() =>
                {
                    try
                    {
                        foreach (var message in val.Result.Reverse())
                            AddMessage(message);
                    }
                    catch
                    {
                        // CBA to handle system messages rn
                    }
                });
            });
        }
    }
}