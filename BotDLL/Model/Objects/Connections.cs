using BotDLL.Persistence;

namespace BotDLL.Model.Objects
{
    /// <summary>
    /// The connections.
    /// </summary>
    public class Connections
    {
        /// <summary>
        /// Gets or sets the discord bot key.
        /// </summary>
        public string? DiscordBotKey { get; set; }
        /// <summary>
        /// Gets or sets the discord bot debug key.
        /// </summary>
        public string? DiscordBotDebug { get; set; }
        /// <summary>
        /// Gets or sets the telegram bot key.
        /// </summary>
        public string? TelegramBotKey { get; set; }
        /// <summary>
        /// Gets or sets the telegram bot debug key.
        /// </summary>
        public string? TelegramBotKeyDebug { get; set; }
        /// <summary>
        /// Gets or sets the mysql connection string.
        /// </summary>
        public string? MySqlConStr { get; set; }
        /// <summary>
        /// Gets or sets the debug mysql connection string.
        /// </summary>
        public string? MySqlConStrDebug { get; set; }
        /// <summary>
        /// Gets or sets the quick chart api.
        /// </summary>
        public string? QuickChartApi { get; set; }
        /// <summary>
        /// Gets or sets the quick chart api debug.
        /// </summary>
        public string? QuickChartApiDebug { get; set; }

        /// <summary>
        /// Gets the connections.
        /// </summary>
        /// <returns>A Connections.</returns>
        public static Connections GetConnections()
        {
            try
            {
                return CSV_Connections.ReadAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
