using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if(EmailTextBox.Text.Length == 0)
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

                    //TODO: send credentials to Authentication Service
                }
            }
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow home = new MainWindow();
            home.Show();
            Close();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            Close();
        }
    }
}
