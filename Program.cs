using System;
using Terminal.Gui;
using Revolt.Cli;

if (!OperatingSystem.IsWindows())
    Application.UseSystemConsole = true;
Application.Init();

App.Run();

if (OperatingSystem.IsLinux())
    System.Diagnostics.Process.Start("reset");
