using System;
using System.Windows;
using System.Windows.Input;
using Zen.Ux.Mvvm;

namespace Zen.Ux.WpfApp
{
    public partial class SignonScreen : IView
    {
        private readonly IProvider _provider;

        public SignonScreen(IProvider provider)
        {
            InitializeComponent();
            _provider = provider;
        }

        private void HyperlinkClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("For demonstration purposes please use\nUserName: leroy\nPassword: secret123", "Sign on credentials");
        }

        private void ButtonOkClick(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            var username = usernameBox.Text.Trim();
            var password = passwordBox.Password.Trim();

            try
            {
                _provider.Login(username, password);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Arrow;
                MessageBox.Show(ex.Message + " Please try again.", "Login failed");
            }
        }
    }
}
