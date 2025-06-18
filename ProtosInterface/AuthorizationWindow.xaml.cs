using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProtosInterface
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        public AuthorizationWindow()
        {
            InitializeComponent();
        }

        private bool _isPasswordVisible = false;

        private void TogglePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;

            if (_isPasswordVisible)
            {
                VisiblePassword.Text = PasswordBox.Password;
                PasswordBox.Visibility = Visibility.Collapsed;
                VisiblePassword.Visibility = Visibility.Visible;
            }
            else
            {
                PasswordBox.Password = VisiblePassword.Text;
                PasswordBox.Visibility = Visibility.Visible;
                VisiblePassword.Visibility = Visibility.Collapsed;
            }
        }

        private void AuthorizationButton_Click(object sender, RoutedEventArgs e)
        {
            AppDbContext _context = new AppDbContext();

            string login = LoginBox.Text;

            if (_context.Authorizations.Any(a => a.Login == login))
            {
                if (_context.Authorizations.Any(a => a.Login == login && a.Password == PasswordBox.Password))
                {
                    this.Hide();
                    MainWindow main = new MainWindow();
                    main.Show();
                }
                else
                {
                    MessageBox.Show("Неверные данные входа", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Введён неверный логин", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
