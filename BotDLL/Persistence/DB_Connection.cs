using BotDLL.Model.HelpClasses;
using BotDLL.Model.Objects;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Reflection;

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
#pragma warning restore CS8601 // Mögliche Nullverweiszuweisung.
#if DEBUG
            token = connections.MySqlConStrDebug;
#endif
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
                RestartProgram();
                throw new Exception("DB DeaD " + ex.Message);
            }
        }

        /// <summary>
        /// Restarts the program.
        /// </summary>
        private static void RestartProgram()
        {

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{"".PadRight(Console.WindowWidth - 2, '█')}");
            ConsoleForamter.Center(" ");
            ConsoleForamter.Center(@"██████╗ ███████╗███████╗████████╗ █████╗ ██████╗ ████████╗██╗███╗   ██╗ ██████╗ ");
            ConsoleForamter.Center(@"██╔══██╗██╔════╝██╔════╝╚══██╔══╝██╔══██╗██╔══██╗╚══██╔══╝██║████╗  ██║██╔════╝ ");
            ConsoleForamter.Center(@"██████╔╝█████╗  ███████╗   ██║   ███████║██████╔╝   ██║   ██║██╔██╗ ██║██║  ███╗");
            ConsoleForamter.Center(@"██╔══██╗██╔══╝  ╚════██║   ██║   ██╔══██║██╔══██╗   ██║   ██║██║╚██╗██║██║   ██║");
            ConsoleForamter.Center(@"██║  ██║███████╗███████║   ██║   ██║  ██║██║  ██║   ██║   ██║██║ ╚████║╚██████╔╝");
            ConsoleForamter.Center(@"╚═╝  ╚═╝╚══════╝╚══════╝   ╚═╝   ╚═╝  ╚═╝╚═╝  ╚═╝   ╚═╝   ╚═╝╚═╝  ╚═══╝ ╚═════╝ ");
            ConsoleForamter.Center(" ");
            Console.WriteLine($"{"".PadRight(Console.WindowWidth - 2, '█')}");
            ConsoleForamter.Center("DB IS DEAD");
            Console.WriteLine($"{"".PadRight(Console.WindowWidth - 2, '█')}");

            // Get file path of current process 
            var filePath = Assembly.GetExecutingAssembly().Location;
            var newFilepath = "";
            //BotDLL.dll

            if (filePath.Contains("Debug"))
            {
                filePath = WordCutter.RemoveAfterWord(filePath, "Debug", 0);
                newFilepath = filePath + "Debug\\ListforgeBot.exe";
            }
            else if (filePath.Contains("Release"))
            {
                filePath = WordCutter.RemoveAfterWord(filePath, "Release", 0);
                newFilepath = filePath + "Release\\ListforgeBot.exe";
            }
            Console.WriteLine("Before 120 secound sleep");
            Thread.Sleep(1000 * 60);
            Console.WriteLine("After 120 secound sleep");
            // Start program
            Process.Start(newFilepath);

            // For all Windows application but typically for Console app.
            Environment.Exit(0);
        }
    }
}
