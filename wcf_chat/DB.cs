using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace wcf_chat
{
    class DB
    {
        public static MySqlConnection GetParam()
        {

            /*string serverName = "127.0.0.1";
            string userName = "root";
            string dbName = "users";
            string password = "";*/

            string serverName = "31.31.196.204";
            string userName = "u0476674_testDB";
            string dbName = "u0476674_testdb";
            string password = "1V0p3C5i";

            return DBConnect(serverName, userName, password, dbName);
        }


        private static MySqlConnection DBConnect(string serverName, string userName, string password, string dbName)
        {

            string connStr = String.Format("server={0};user id={1}; password={2}; database={3}; pooling=false; connection timeout=50;Character Set=utf8",
                        serverName, userName, password, dbName);
            MySqlConnection connection = new MySqlConnection(connStr);

            return connection;
        }
    }
}
