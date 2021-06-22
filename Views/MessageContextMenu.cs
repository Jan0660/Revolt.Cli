using Terminal.Gui;
using TextCopy;

namespace Revolt.Cli.Views
{
    public class MessageContextMenu : Window
    {
        public MessageContextMenu(Message message)
        {
            void Close()
            {
                this.SuperView.Remove(this);
                this.FocusPrev();
            }

            Width = 20;
            Height = 6;
            var copyIdButton = new Button(0, 0, "Copy _Id");
            copyIdButton.Clicked += () =>
            {
                ClipboardService.SetText(message._id);
                Close();
            };
            var copyTextButton = new Button(0, 1, "Copy _Text");
            copyTextButton.Clicked += () =>
            {
                ClipboardService.SetText(message.Content);
                Close();
            };
            var copyUserIdButton = new Button(0, 2, "Copy _User Id");
            copyUserIdButton.Clicked += () =>
            {
                ClipboardService.SetText(message.AuthorId);
                Close();
            };
            var closeButton = new Button(0, 3, "_Close");
            closeButton.Clicked += Close;
            Add(copyIdButton, copyTextButton, copyUserIdButton, closeButton);
        }
    }
}