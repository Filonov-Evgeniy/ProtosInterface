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
    public partial class Authorization : Window
    {
        public Authorization()
        {
            InitializeComponent();
        }

        private bool _isPasswordVisible = false;
        private string _passwordBackup = ""; // Храним пароль при переключении

        private void TogglePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;

            if (_isPasswordVisible)
            {
                // Сохраняем пароль перед показом
                _passwordBackup = PasswordBox.Password;

                // Показываем пароль
                PasswordBox.PasswordChar = '\0';
                TogglePasswordButton.Content = "🙈";
            }
            else
            {
                // Восстанавливаем пароль
                PasswordBox.PasswordChar = '•';
                TogglePasswordButton.Content = "👁";

                // Важно: сначала меняем символ, потом восстанавливаем пароль
                PasswordBox.Password = _passwordBackup;
            }

            // Возвращаем фокус
            PasswordBox.Focus();
        }

        private void AuthorizationButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
