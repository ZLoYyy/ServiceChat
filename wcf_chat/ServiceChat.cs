using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace wcf_chat
{
  
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ServiceChat : IServiceChat
    {
        MySqlConnection conn = DB.GetParam();
        MySqlCommand script = new MySqlCommand();
        MySqlDataAdapter dataAdapter;
        DataTable dataTable = new DataTable();
        List<string> onlineUsers = new List<string>();
        List<string> messages = new List<string>();
        List<ServerUser> users = new List<ServerUser>();
        int id;
        int nextId;

        private int GetID(string userName)
        {
            try
            {
                //conn.Open();
                script.Connection = conn;
                script.CommandText = string.Format("SELECT id FROM `users` WHERE login='" + userName + "'");
                script.ExecuteNonQuery();
                object result = script.ExecuteScalar();
                int userID = Convert.ToInt32(result.ToString());
                //conn.Close();
                return userID;
                
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Пользователь с таким логином не найден", "Ошибка",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                conn.Close();
                return 0;
            }
        }

        private int GetMaxID()
        {
            conn.Open();
            script.Connection = conn;
            script.CommandText = string.Format("SELECT MAX(id) FROM `users`");
            script.ExecuteNonQuery();
            object result = script.ExecuteScalar();
            int userID = Convert.ToInt32(result.ToString());
            conn.Close();

            return userID;
        }

        private void StatusOnline(int id)
        {
            script.Connection = conn;
            script.CommandText = string.Format("UPDATE `users` SET `status` = '1' WHERE `users`.`id` = " + id + "");
            script.ExecuteNonQuery();
        }

        private string GetLogin(string name)
        {

            try
            {
                script.Connection = conn;
                script.CommandText = string.Format("SELECT `login` FROM `users` WHERE login='" + name + "'");

                script.ExecuteNonQuery();
                object result = script.ExecuteScalar();
                string login = Convert.ToString(result.ToString());
                return login;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Пользователь с таким логином не найден", "Ошибка",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                conn.Close();
                return "";
            }
        }

        private string GetPassword(string name, string password)
        {
            try
            {
                script.Connection = conn;
                script.CommandText = string.Format("SELECT `password` FROM `users` WHERE password='" + password + "' AND login='" + name + "'");
                script.ExecuteNonQuery();

                object result = script.ExecuteScalar();
                string psw = Convert.ToString(result.ToString());
                return psw;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Неверный пароль", "Ошибка",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                conn.Close();
                return "";
            }

            
        }
        private void StatusOffline(int id)
        {
            script.Connection = conn;
            script.CommandText = string.Format("UPDATE `users` SET `status` = '0' WHERE `users`.`id` = " + id + "");
            script.ExecuteNonQuery();
        }

        private string SaveMessage(int send_id, int get_id, string message)
        {
            byte[] UTF8encodes = UTF8Encoding.UTF8.GetBytes(message);
            string MainMessage = UTF8Encoding.UTF8.GetString(UTF8encodes);
            script.Connection = conn;
            script.CommandText = string.Format("INSERT INTO `messages` (`send_id`, `get_id`, `message`) VALUES ('" + send_id + "', '" + get_id + "', '" + MainMessage + "')");
            script.ExecuteNonQuery();

            return null;
        }

        private List<string> GetMessage(int send_id, int get_id)
        {
            messages.Clear();
            string message = "";
            string sendId = ""+send_id;
            string getId = "" + get_id;

            script.Connection = conn;
            script.CommandText = string.Format("SELECT * FROM  `messages` WHERE  `messages`.`send_id` = " + send_id + " AND  `messages`.`get_id` = " + get_id + "" +
                                               " OR  `messages`.`send_id` = " + get_id + " AND  `messages`.`get_id` = " + send_id + " LIMIT 0, 30");
            script.ExecuteNonQuery();

            dataAdapter = new MySqlDataAdapter(script);
            dataAdapter.Fill(dataTable);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                for (int j = 1; j < dataTable.Columns.Count; j++)
                {
                    if (dataTable.Rows[i][j].ToString() == sendId && dataTable.Rows[i][j + 1].ToString() == getId)
                    {
                        foreach(var item in users)
                        {
                            if (item.ID == send_id)
                            {
                                message = "[" + dataTable.Rows[i][0].ToString() + "]" + item.Name +": " + dataTable.Rows[i][3].ToString();
                                messages.Add(message);

                            }
                        }
                        break;
                    }
                    if (dataTable.Rows[i][j].ToString() == getId && dataTable.Rows[i][j + 1].ToString() == sendId)
                    {
                        foreach (var item in users)
                        {
                            if (item.ID == get_id)
                            {
                                message = "[" + dataTable.Rows[i][0].ToString() + "]" + item.Name + ": " + dataTable.Rows[i][3].ToString();
                                messages.Add(message);
                            }
                        }
                        break;
                    }
                }
            }

            return messages;
        }

        public void Registration(string name, string password)
        {
            try
            {        
                nextId = GetMaxID() + 1;
                conn.Open();

                script.Connection = conn;
                script.CommandText = string.Format("INSERT INTO `users` (`id`, `login`, `password`, `status`) VALUES ('" + nextId + "', '" + name + "', '" + password + "', '0')");
                script.ExecuteNonQuery();

                conn.Close();
                System.Windows.Forms.MessageBox.Show("Регистрация прошла успешно", "Уведомление",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Пользователь с таким логином уже зарегестрировани", "Ошибка",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                conn.Close();
            }
        }

        public int Connect(string name, string password)
        {
            id = 0;
            conn.Open();
            ServerUser user;
            string login = null;
            string pswd = null;

            try
            {
                login = GetLogin(name);
                pswd = GetPassword(name, password);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("ConnectionLost" + Environment.NewLine +
                   ex.Message, "ErrorDB",
                   System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                conn.Close();
            }
           

            if (login == name && pswd == password)
            {
                id = GetID(name);
                user = new ServerUser()
                {
                    ID = id,
                    Name = name,
                    operationContext = OperationContext.Current
                };
                SendMsg(": " + user.Name + " подключился к чату!", 0);
                users.Add(user);
                StatusOnline(user.ID);
            }

            conn.Close();
            return id;
        }


        public void Disconnect(int id)
        {
            var user = users.FirstOrDefault(i => i.ID == id);
            if (user != null)
            {
                users.Remove(user);
                SendMsg(": " + user.Name + " покинул чат!", 0);

                conn.Open();
                StatusOffline(id);
                conn.Close();
            }
        }

        public void SendMsg(string msg, int id)
        {            
            foreach (var item in users)
            {
                string answer = DateTime.Now.ToShortTimeString();

                var user = users.FirstOrDefault(i => i.ID == id);
                if (user != null)
                {
                    answer += ": " + user.Name + " ";
                }
                answer += msg;
                item.operationContext.GetCallbackChannel<IServerChatCallback>().MsgCallback(answer);
            }
        }

        public void PrivateSendMsg(string msg, int id, string name)
        {
            int getterID = 0;
            foreach (var item in users)
            {
                var user = users.FirstOrDefault(i => i.ID == id);
                if (item.Name == name)
                {
                    getterID = item.ID;
                    string answer = DateTime.Now.ToShortTimeString();
                    
                    if (user != null)
                    {
                        answer += ": от " + user.Name + " ";
                    }
                    answer += msg;
                    item.operationContext.GetCallbackChannel<IServerChatCallback>().PrivateMsgCallback(answer, user.Name);
                }                
            }
            conn.Open();
            SaveMessage(id, getterID, msg);
            conn.Close();
        }

        public List<string> UsersOnline()
        {
            onlineUsers.Clear();
            foreach (var item in users)
            {

                onlineUsers.Add(item.Name);
            }

            return onlineUsers;
        }

        public List<string> GetMessageStory(int send_id, string getterName)
        {
            List<string> msg = null;
            foreach (var item in users)
            {
                if (item.Name == getterName)
                {
                    conn.Open();
                    msg = GetMessage(send_id, item.ID);
                    conn.Close();
                }
            }
            return msg;
        }
    }
}
