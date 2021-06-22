using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Terminal.Gui;
using SysCon = System.Console;

namespace Revolt.Cli
{
    public static class ExtensionMethods
    {
        public static void SizeFix(this View view, View parent)
        {
            view.Width = parent.Width;
            view.Height = parent.Height - 1;
            Task.Delay(100).ContinueWith(_ =>
            {
                view.Width = Dim.Fill();
                view.Height = Dim.Fill();
            });
        }

        [Pure]
        public static string Shorten(this string str, int length)
            => str.Length > length ? str[..(length - 5)] + "(...)" : str;

        private static View _lastShownView;

        public static void Show(this Window window, bool hideLast = true, bool sizeFix = true, View parent = null)
        {
            if(sizeFix)
                window.SizeFix(App.Toplevel);
            Show((View)window, hideLast, parent);
        }

        public static void Show(this View view, bool hideLast = true, View parent = null)
        {
            if (_lastShownView != null && hideLast)
                _lastShownView.Hide();
            (parent ?? App.Toplevel).Add(view);
            _lastShownView = view;
        }

        public static void Hide(this View view)
            => App.Toplevel.Remove(view);
    }
}