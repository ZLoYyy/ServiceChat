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
using ChatClient.ServiceChat;

namespace ChatClient
{
    /// <summary>
    /// Логика взаимодействия для Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        MainWindow mainWindow = new MainWindow();

        public Register()
        {
            InitializeComponent();
        }

        void GetError(string error)
        {
            RegError.Text = error;
        }

        private void RegBTN(object sender, RoutedEventArgs e)
        {
            RegError.Text = "";
            if (RegName.Text != "" && RegPassword.Text != "" && RegPasswordRepeat.Text != "")
            {
                if (RegPassword.Text == RegPasswordRepeat.Text)
                {
                    mainWindow.Registration(RegName.Text, RegPassword.Text);
                    this.Close();
                }
                else
                {
                    //пароли не совпадают
                    GetError("пароли не совпадают");
                }
            }
            else
            {
                //заполнить все поля
                GetError("заполнить все поля");
            }
        }

        private void RegBTNClose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
