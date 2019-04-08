using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace ChatClient.Views
{
    public partial class MainWindow : Window
    {

        /// <summary>
        /// This name is simply added to sent messages to identify the user; this 
        /// sample does not include authentication.
        /// </summary>
        public String UserName { get; set; }
        //public IHubProxy HubProxy { get; set; }
        const string ServerURI = "https://localhost:44334//chathub";
        public HubConnection Connection { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Connection.InvokeAsync("Send", UserName, TextBoxMessage.Text);
                TextBoxMessage.Text = String.Empty;
                TextBoxMessage.Focus();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
           
        }

        /// <summary>
        /// Creates and connects the hub connection and hub proxy. This method
        /// is called asynchronously from SignInButton_Click.
        /// </summary>
        private async void ConnectAsync()
        {
            Connection = new HubConnectionBuilder()
        .WithUrl(ServerURI)
        .Build();
            Connection.On<string, string>("AddMessage", (name, message) =>
                this.Dispatcher.Invoke(() =>
                    RichTextBoxConsole.AppendText(String.Format("{0}: {1}\r", name, message))
                )
            );
            try
            {
                await Connection.StartAsync();
            }
            catch (HttpRequestException)
            {
                StatusText.Content = "Unable to connect to server: Start server before connecting clients.";
                //No connection: Don't enable Send button or show chat UI
                return;
            }

            //Show chat UI; hide login UI
            SignInPanel.Visibility = Visibility.Collapsed;
            ChatPanel.Visibility = Visibility.Visible;
            ButtonSend.IsEnabled = true;
            TextBoxMessage.Focus();
            RichTextBoxConsole.AppendText("Connected to server at " + ServerURI + "\r");
        }

        /// <summary>
        /// If the server is stopped, the connection will time out after 30 seconds (default), and the 
        /// Closed event will fire.
        /// </summary>
        void Connection_Closed()
        {
            //Hide chat UI; show login UI
            var dispatcher = Application.Current.Dispatcher;
            dispatcher.Invoke(() => ChatPanel.Visibility = Visibility.Collapsed);
            dispatcher.Invoke(() => ButtonSend.IsEnabled = false);
            dispatcher.Invoke(() => StatusText.Content = "You have been disconnected.");
            dispatcher.Invoke(() => SignInPanel.Visibility = Visibility.Visible);
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            UserName = UserNameTextBox.Text;
            //Connect to server (use async method to avoid blocking UI thread)
            if (!String.IsNullOrEmpty(UserName))
            {
                StatusText.Visibility = Visibility.Visible;
                StatusText.Content = "Connecting to server...";
                ConnectAsync();
            }
        }

        private void WPFClient_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Connection != null)
            {
                Connection.StopAsync();
                Connection.DisposeAsync();
            }
        }
    }
}
