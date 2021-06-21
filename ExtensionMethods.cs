using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Terminal.Gui;
using SysCon = System.Console;

namespace Revolt.Cli
{
    public static class ExtensionMethods
    {
        public static void SizeFix(this Window view, View parent)
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
    }
}