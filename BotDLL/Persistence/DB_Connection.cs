using BotDLL.Model.HelpClasses;
using BotDLL.Model.Objects;
using MySql.Data.MySqlClient;

namespace BotDLL.Persistence
{
    class DB_Connection
    {
        private static string token = "";
        private static int virgin = 0;
        public static void SetDB()
        {
            Connections connections = Connections.GetConnections();
#pragma warning disable CS8601 // Mögliche Nullverweiszuweisung.
            token = connections.MySqlConStr;
#if DEBUG
            token = connections.MySqlConStrDebug;
#endif
#pragma warning restore CS8601 // Mögliche Nullverweiszuweisung.
        }
        public static MySqlConnection OpenDB()
        {
            if (virgin == 0)
                SetDB(); virgin = 69;
            MySqlConnection mySqlConnection = new(token);
            do
            {
                try
                {
#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
                    mySqlConnection.Open();
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: Open database failed! " + ex.Message);
                    Thread.Sleep(10);
                }
            } while (mySqlConnection == null);

            return mySqlConnection;
        }
        public static void CloseDB(MySqlConnection mySqlConnection)
        {
            mySqlConnection.Close();
        }
        public static void ExecuteNonQuery(string sqlCommand)
        {
            MySqlConnection mySqlConnection = OpenDB();
            MySqlCommand mySqlCommand = new(sqlCommand, mySqlConnection);
            mySqlCommand.ExecuteNonQuery();
            CloseDB(mySqlConnection);
        }
        public static MySqlDataReader ExecuteReader(string sqlCommand, MySqlConnection mySqlConnection)
        {
            MySqlCommand mySqlCommand = new(sqlCommand, mySqlConnection);
            try
            {
                MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
                return mySqlDataReader;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{"".PadRight(Console.WindowWidth - 2, '█')}");
                ConsoleForamter.Center("DB IS DEAD");
                Console.WriteLine($"{"".PadRight(Console.WindowWidth - 2, '█')}");
                Reset.RestartProgram();
                throw new Exception("DB DeaD " + ex.Message);
            }
        }
    }
}
