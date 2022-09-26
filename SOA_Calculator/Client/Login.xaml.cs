using Authenticator_DLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        Authenticator_Interface authenticator;
        int token;

        public Login()
        {
            InitializeComponent();
            ChannelFactory<Authenticator_Interface> authFactory;
            NetTcpBinding tcp = new NetTcpBinding();

            string URL = "net.tcp://localhost/AuthenticationService";
            authFactory = new ChannelFactory<Authenticator_Interface>(tcp, URL);
            authenticator = authFactory.CreateChannel();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (EmailTextBox.Text.Length == 0)
            {
                EmailErrorText.Text = "Email field must be filled";
                EmailTextBox.Focus();
            }
            else if (!Regex.IsMatch(EmailTextBox.Text, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
            {
                EmailErrorText.Text = "Enter a valid email, e.g. example@email.com";
                EmailTextBox.Select(0, EmailTextBox.Text.Length);
                EmailTextBox.Focus();
            }
            else
            {
                EmailErrorText.Text = "";

                if (PasswordTextBox.Text.Length == 0)
                {
                    PasswordErrorText.Text = "Password field must be filled";
                    PasswordTextBox.Focus();
                }
                else
                {
                    PasswordErrorText.Text = "";
                    token = authenticator.Login(EmailErrorText.Text, PasswordTextBox.Text);
                }
            }
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow home = new MainWindow();
            home.Show();
            Close();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            Registration registration = new Registration();
            registration.Show();
            Close();
        }
    }
}
