using BotDLL.Model.Objects;

namespace BotDLL.Model.HelpClasses
{
   public class ConsoleForamter
   {
      /// <summary>
      /// Centers the console.
      /// </summary>
      /// <param name="centerString">The text.</param>
      public static void Center(string centerString)
      {
         try
         {
            Console.Write("██");
            Console.SetCursorPosition((Console.WindowWidth - centerString.Length) / 2, Console.CursorTop);
            Console.Write(centerString);
            Console.SetCursorPosition((Console.WindowWidth - 4), Console.CursorTop);
            Console.WriteLine("██");
         }
         catch
         {
            centerString = "Console to small!";
            Console.SetCursorPosition((Console.WindowWidth - centerString.Length) / 2, Console.CursorTop);
            Console.Write(centerString);
            Console.SetCursorPosition((Console.WindowWidth - 4), Console.CursorTop);
            Console.WriteLine("██");
         }
      }
      public static void FillRow()
      {
         Console.WriteLine($"{"".PadRight(Console.WindowWidth - 2, '█')}");
      }

      public static void WriteStatList(List<ServerStat> serverStatList)
      {
         try
         {
            FillRow();
            Center($"██ {"Id",5} ██ {"Name",-10} ██ {"DynDnsAddress",-15} ██ {"Address",15}:{"Port",-5} ██ {"pL",3}/{"MpL",-5} ██ {"IsUp",-5} ██ {"Version",12} ██ {"Map",-20} ██ {"Game",-35} ██ {"GameID",15} ██ {"uTime",5}% ██ {"FetchTime",21} ██");
            FillRow();
            foreach (ServerStat serverStatObjItem in serverStatList)
            {
               if (serverStatObjItem.ServerUp)
                  Console.ForegroundColor = ConsoleColor.Green;
               else
                  Console.ForegroundColor = ConsoleColor.Red;

               Center(serverStatObjItem.ToString());
               Console.ForegroundColor = ConsoleColor.Gray;
            }
            FillRow();
         }
         catch
         {
            Center("Console to small!");
         }
      }
   }
}
