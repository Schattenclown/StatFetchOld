using BotDLL.Model.Objects;

namespace BotDLL.Persistence
{
    public class CSV_Connections
    {
        private static readonly Uri path = new($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/StatFetch");
        private static readonly Uri filepath = new($"{path}/Connections.csv");
        public static Connections ReadAll()
        {
            try
            {
                Connections connections = new();
                StreamReader streamReader = new(filepath.LocalPath);
                while (!streamReader.EndOfStream)
                {
                    string row = streamReader.ReadLine();
#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
                    string[] infos = row.Split(';');
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.

                    switch (infos[0])
                    {
                        case "DiscordBotKey":
                            connections.DiscordBotKey = infos[1];
                            break;
                        case "DiscordBotKeyDebug":
                            connections.DiscordBotDebug = infos[1];
                            break;
                        case "TelegramBotKey":
                            connections.TelegramBotKey = infos[1];
                            break;
                        case "TelegramBotKeyDebug":
                            connections.TelegramBotKeyDebug = infos[1];
                            break;
                        case "MySqlConStr":
                            connections.MySqlConStr = infos[1].Replace(',', ';');
                            break;
                        case "MySqlConStrDebug":
                            connections.MySqlConStrDebug = infos[1].Replace(',', ';');
                            break;
                        case "QuickChartApi":
                            connections.QuickChartApi = infos[1];
                            break;
                        case "QuickChartApiDebug":
                            connections.QuickChartApiDebug = infos[1];
                            break;
                        default:
                            break;
                    }
                }
                streamReader.Close();
                return connections;
            }
            catch (Exception)
            {
                DirectoryInfo directory = new(path.LocalPath);
                if (!directory.Exists)
                    directory.Create();

                StreamWriter streamWriter = new(filepath.LocalPath);
                streamWriter.WriteLine("DiscordBotKey;<API Key here>\n" +
                                        "DiscordBotKeyDebug;<API Key here>\n" +
                                        "TelegramBotKey;<API Key here>\n" +
                                        "TelegramBotKeyDebug;<API Key here>\n" +
                                        "MySqlConStr;<DBConnectionString here>\n" +
                                        "MySqlConStrDebug;<DBConnectionString here>\n" +
                                        "QuickChartApi;<API Key here>\n" +
                                        "QuickChartApiDebug;<API Key here>");

                streamWriter.Close();
                throw new Exception($"{path.LocalPath}\n" +
                                    $"API Keys and Database string not configurated!");
            }
        }
    }
}
