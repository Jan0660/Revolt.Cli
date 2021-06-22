using Terminal.Gui;
using TextCopy;

namespace Revolt.Cli.Views
{
    public class MessageContextMenu : Window
    {
        public MessageContextMenu(Message message)
        {
            Width = 20;
            Height = 5;
            var copyIdButton = new Button(0, 0, "Copy Id");
            copyIdButton.Clicked += () => { ClipboardService.SetText(message._id); };
            var copyTextButton = new Button(0, 1, "Copy Text");
            copyTextButton.Clicked += () => { ClipboardService.SetText(message.Content); };
            var closeButton = new Button(0, 2,"Close");
            closeButton.Clicked += () => { this.SuperView.Remove(this);
                this.FocusPrev();
            };
            Add(copyIdButton, copyTextButton, closeButton);
        }
    }
}