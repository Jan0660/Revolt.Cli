using Revolt.Cli.Views.Windows;
using Terminal.Gui;
using TextCopy;

namespace Revolt.Cli.Views
{
    public class MessageContextMenu : Window
    {
        public MessageContextMenu(Message message, Window aboveParent)
        {
            void Close()
            {
                this.SuperView.Remove(this);
                this.FocusPrev();
            }

            Width = 20;
            Height = 7;
            var currentY = 0;
            var showProfileButton = new Button(0, currentY++, "Show _Profile");
            showProfileButton.Clicked += () =>
            {
                new UserProfileWindow(message.Author).Show();
                aboveParent.Hide();
            };
            var copyIdButton = new Button(0, currentY++, "Copy _Id");
            copyIdButton.Clicked += () =>
            {
                ClipboardService.SetText(message._id);
                Close();
            };
            var copyTextButton = new Button(0, currentY++, "Copy _Text");
            copyTextButton.Clicked += () =>
            {
                ClipboardService.SetText(message.Content);
                Close();
            };
            var copyUserIdButton = new Button(0, currentY++, "Copy _User Id");
            copyUserIdButton.Clicked += () =>
            {
                ClipboardService.SetText(message.AuthorId);
                Close();
            };
            var closeButton = new Button(0, currentY++, "_Close");
            closeButton.Clicked += Close;
            Add(showProfileButton, copyIdButton, copyTextButton, copyUserIdButton, closeButton);
        }
    }
}