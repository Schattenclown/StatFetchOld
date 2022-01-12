using System.Diagnostics;
using System.Reflection;

namespace BotDLL.Model.HelpClasses
{
    public class Reset
    {
        /// <summary>
        /// Restarts the program.
        /// </summary>
        public static void RestartProgram()
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
                newFilepath = filePath + "Debug\\net6.0\\StatFetch.exe";
            }
            else if (filePath.Contains("Release"))
            {
                filePath = WordCutter.RemoveAfterWord(filePath, "Release", 0);
                newFilepath = filePath + "Release\\net6.0\\StatFetch.exe";
            }
            Console.WriteLine("Sleeping for 2 minutes!");
            Thread.Sleep(1000 * 60);
            // Start program
            Process.Start(newFilepath);

            // For all Windows application but typically for Console app.
            Environment.Exit(0);
        }
    }
}
