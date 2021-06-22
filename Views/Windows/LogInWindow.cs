using System;
using System.IO;
using Newtonsoft.Json;
using Terminal.Gui;

namespace Revolt.Cli.Views.Windows
{
    public class LogInWindow : Window
    {
        public LogInWindow()
        {
            var userIdLabel = new Label("User Id: ") { X = 3, Y = 2 };
            var sessionIdLabel = new Label("Session Id: ")
            {
                X = Pos.Left(userIdLabel),
                Y = Pos.Top(userIdLabel) + 1
            };
            var sessionTokenLabel = new Label("Session Token: ")
            {
                X = Pos.Left(sessionIdLabel),
                Y = Pos.Top(sessionIdLabel) + 1
            };
            var userIdField = new TextField("")
            {
                X = Pos.Right(sessionTokenLabel),
                Y = Pos.Top(userIdLabel),
                Width = 40
            };
            var sessionIdField = new TextField("")
            {
                Secret = true,
                X = Pos.Left(userIdField),
                Y = Pos.Top(sessionIdLabel),
                Width = Dim.Width(userIdField)
            };
            var sessionTokenField = new TextField("")
            {
                Secret = true,
                X = Pos.Left(sessionIdField),
                Y = Pos.Top(sessionTokenLabel),
                Width = 70
            };
            var logInbutton = new Button("Log In")
            {
                X = 3,
                Y = Pos.Top(sessionTokenField) + 2,
            };
            logInbutton.Clicked += () =>
            {
                File.WriteAllText(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/session.json",
                    JsonConvert.SerializeObject(new Session()
                    {
                        UserId = userIdField.Text.ToString()!,
                        Id = sessionIdField.Text.ToString(),
                        SessionToken = sessionTokenField.Text.ToString()!,
                    }));
                new HomeWindow().Show();
            };
            Add(
                userIdLabel, sessionIdLabel, sessionTokenLabel, userIdField, sessionIdField, sessionTokenField,
                logInbutton
            );
        }
    }
}