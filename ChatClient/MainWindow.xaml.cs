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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChatClient.ServiceChat;

namespace ChatClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IServiceChatCallback
    {
        private string getterUser;
        bool isConnected = false;
        ServiceChatClient client;
        int ID;
        public MainWindow()
        {
            InitializeComponent();
        }
        public void Registration(string login, string password)
        {
            try
            {
                client = new ServiceChatClient(new System.ServiceModel.InstanceContext(this));
                client.Registration(login, password);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Не удалось подключиться к БД." + Environment.NewLine +
                    ex.Message, "Ошибка",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        void ConnectUser()
        {
            
            if (tbUserName.Text != "" && tbUserPassword.Text != "")
            {
                if (!isConnected)
                {

                    client = new ServiceChatClient(new System.ServiceModel.InstanceContext(this));

                    
                    ID = client.Connect(tbUserName.Text, tbUserPassword.Text);
                    if (ID > 0)
                    {
                        tbUserName.IsEnabled = false;
                        tbUserPassword.IsEnabled = false;
                        tbMessage.IsEnabled = true;
                        bConnDicon.Content = "Отключиться";
                        isConnected = true;
                        buttonRegister.Visibility = Visibility.Hidden;
                        OnlineUsers();
                    }                   
                }
            }
            else
            {
                //Не все поля заполнены
                TbGetError("Не все поля заполнены");
            }
        }

        void DisconnectUser()
        {
            if (isConnected)
            {
                client.Disconnect(ID);
                client = null;
                tbUserName.IsEnabled = true;
                tbUserPassword.IsEnabled = true;
                tbMessage.IsEnabled = false;
                bConnDicon.Content = "Подключиться";
                isConnected = false;
                buttonRegister.Visibility = Visibility.Visible;
                btnCommonChat.Visibility = Visibility.Hidden;
                getHistory.Visibility = Visibility.Hidden;
                lbChat.Items.Clear();
                usersOnline.Items.Clear();
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                DisconnectUser();
            }
            else
            {
                ConnectUser();
            }

        }

        public void MsgCallback(string msg)
        {
            lbChat.Items.Add(msg);
            lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count-1]);
            OnlineUsers();
        }

        public void PrivateMsgCallback(string msg, string name)
        {
            tittleChat.Text = "Только для " + name;
            getterUser = name;
            lbChat.Visibility = Visibility.Hidden;
            tbMessage.Visibility = Visibility.Hidden;

            privateChat.Visibility = Visibility.Visible;
            privateMessage.Visibility = Visibility.Visible;
            btnCommonChat.Visibility = Visibility.Visible;
            getHistory.Visibility = Visibility.Visible;

            privateChat.Items.Add(msg);
            privateChat.ScrollIntoView(privateChat.Items[privateChat.Items.Count - 1]);
        }

        public void OnlineUsers()
        {
            usersOnline.Items.Clear();
            string[] onlineUsers = null;

            client = new ServiceChatClient(new System.ServiceModel.InstanceContext(this));

            onlineUsers = client.UsersOnline();
            
            foreach (var item in onlineUsers)
            {
                usersOnline.Items.Add(item);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DisconnectUser();
        }

        private void tbMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (client != null)
                {
                    client.SendMsg(tbMessage.Text, ID);
                    tbMessage.Text = string.Empty;                    
                }               
            }
        }

        private void PrivateMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (client != null)
                {
                    string answer = DateTime.Now.ToShortTimeString();
                    answer += ": для " + getterUser + " " + privateMessage.Text;

                    client.PrivateSendMsg(privateMessage.Text, ID, getterUser);
                    privateChat.Items.Add(answer);
                    privateChat.ScrollIntoView(privateChat.Items[privateChat.Items.Count - 1]);
                    privateMessage.Text = string.Empty;
                    

                }
            }
        }

        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            Register registerForm = new Register();
            registerForm.Show();
        }

        void TbGetError(string error)
        {
            tbError.Text = error;
        }

        private void UsersOnline_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (client != null)
            {
                privateChat.Items.Clear();
                lbChat.Visibility = Visibility.Hidden;
                tbMessage.Visibility = Visibility.Hidden;

                privateChat.Visibility = Visibility.Visible;
                privateMessage.Visibility = Visibility.Visible;
                btnCommonChat.Visibility = Visibility.Visible;
                getHistory.Visibility = Visibility.Visible;

                var userName = usersOnline.SelectedItem;

                getterUser = userName.ToString();
                tittleChat.Text = "Только для " + getterUser;
            }            
        }

        private void UsersOnline_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnCommonChat_Click(object sender, RoutedEventArgs e)
        {
            tittleChat.Text = "Общий чат";
            lbChat.Visibility = Visibility.Visible;
            tbMessage.Visibility = Visibility.Visible;

            privateChat.Visibility = Visibility.Hidden;
            privateMessage.Visibility = Visibility.Hidden;
            btnCommonChat.Visibility = Visibility.Hidden;
            getHistory.Visibility = Visibility.Hidden;
        }

        private void GetHistory_Click(object sender, RoutedEventArgs e)
        {
            MessageStoryCallback();
        }

        public void MessageStoryCallback()
        {
            privateChat.Items.Clear();
            string[] messageStory = null;

            client = new ServiceChatClient(new System.ServiceModel.InstanceContext(this));

            messageStory = client.GetMessageStory(ID, getterUser);

            foreach (var item in messageStory)
            {
                privateChat.Items.Add(item);
            }
        }
    }
}
