using System.Diagnostics;
using System.Reflection;

namespace BotDLL.Model.HelpClasses
{
   public class Reset
   {
      /// <summary>
      ///    Restarts the program.
      /// </summary>
      public static void RestartProgram(Exception exception)
      {
         Console.ForegroundColor = ConsoleColor.Red;
         Console.WriteLine($"{"".PadRight(Console.WindowWidth - 2, '█')}");
         ConsoleFormatter.Center(" ");
         ConsoleFormatter.Center(@"██████╗ ███████╗███████╗████████╗ █████╗ ██████╗ ████████╗██╗███╗   ██╗ ██████╗ ");
         ConsoleFormatter.Center(@"██╔══██╗██╔════╝██╔════╝╚══██╔══╝██╔══██╗██╔══██╗╚══██╔══╝██║████╗  ██║██╔════╝ ");
         ConsoleFormatter.Center(@"██████╔╝█████╗  ███████╗   ██║   ███████║██████╔╝   ██║   ██║██╔██╗ ██║██║  ███╗");
         ConsoleFormatter.Center(@"██╔══██╗██╔══╝  ╚════██║   ██║   ██╔══██║██╔══██╗   ██║   ██║██║╚██╗██║██║   ██║");
         ConsoleFormatter.Center(@"██║  ██║███████╗███████║   ██║   ██║  ██║██║  ██║   ██║   ██║██║ ╚████║╚██████╔╝");
         ConsoleFormatter.Center(@"╚═╝  ╚═╝╚══════╝╚══════╝   ╚═╝   ╚═╝  ╚═╝╚═╝  ╚═╝   ╚═╝   ╚═╝╚═╝  ╚═══╝ ╚═════╝ ");
         ConsoleFormatter.Center(" ");
         Console.WriteLine($"{"".PadRight(Console.WindowWidth - 2, '█')}");
         ConsoleFormatter.Center(exception.Message);
         Console.WriteLine($"{"".PadRight(Console.WindowWidth - 2, '█')}");

         // Get file path of current process 
         string? filePath = Assembly.GetExecutingAssembly().Location;
         string newFilepath = "";
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

         Console.WriteLine("Sleeping for 60 seconds!");
         Thread.Sleep(1000 * 60);
         // Start program
         Process.Start(newFilepath);

         // For all Windows application but typically for Console app.
         Environment.Exit(0);
      }
   }
}