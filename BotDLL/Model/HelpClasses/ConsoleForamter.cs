using BotDLL.Model.Objects;

namespace BotDLL.Model.HelpClasses;

public class ConsoleForamter
{
   /// <summary>
   ///    Centers the console.
   /// </summary>
   /// <param name="centerString">The text.</param>
   public static void Center(string centerString)
   {
      //╔ ═ v ╣╠ ╝╚ ╣ ╠

      try
      {
         Console.Write("");
         Console.SetCursorPosition((Console.WindowWidth - centerString.Length) / 2, Console.CursorTop);
         Console.Write(centerString);
         Console.SetCursorPosition(Console.WindowWidth - 4, Console.CursorTop);
         Console.WriteLine("");
      }
      catch
      {
         centerString = "Console to small!";
         Console.SetCursorPosition((Console.WindowWidth - centerString.Length) / 2, Console.CursorTop);
         Console.Write(centerString);
         Console.SetCursorPosition(Console.WindowWidth - 4, Console.CursorTop);
         Console.WriteLine("");
      }
   }

   public static void FillRow(int topMiddleBottom)
   {
      if (topMiddleBottom == 1)
      {
         Center($"╔═{"═".PadLeft(5, '═')}═╗╔═{"═".PadLeft(10, '═')}═╗╔═{"═".PadLeft(15, '═')}═╗╔═{"═".PadLeft(15, '═')}═{"═".PadLeft(5, '═')}═╗╔═{"═".PadLeft(3, '═')}═{"═".PadLeft(5, '═')}═╗╔═{"═".PadLeft(5, '═')}═╗╔═{"═".PadLeft(12, '═')}═╗╔═{"═".PadLeft(20, '═')}═╗╔═{"═".PadLeft(35, '═')}═╗╔═{"═".PadLeft(15, '═')}═╗╔═{"═".PadLeft(6, '═')}══╗╔═{"═".PadLeft(21, '═')}═╗");
      }
      else if (topMiddleBottom == 2)
      {
         Center($"╠═{"═".PadLeft(5, '═')}═╣╠═{"═".PadLeft(10, '═')}═╣╠═{"═".PadLeft(15, '═')}═╣╠═{"═".PadLeft(15, '═')}═{"═".PadLeft(5, '═')}═╣╠═{"═".PadLeft(3, '═')}═{"═".PadLeft(5, '═')}═╣╠═{"═".PadLeft(5, '═')}═╣╠═{"═".PadLeft(12, '═')}═╣╠═{"═".PadLeft(20, '═')}═╣╠═{"═".PadLeft(35, '═')}═╣╠═{"═".PadLeft(15, '═')}═╣╠═{"═".PadLeft(6, '═')}══╣╠═{"═".PadLeft(21, '═')}═╣");
      }
      else if (topMiddleBottom == 3)
      {
         Center($"╚═{"═".PadLeft(5, '═')}═╝╚═{"═".PadLeft(10, '═')}═╝╚═{"═".PadLeft(15, '═')}═╝╚═{"═".PadLeft(15, '═')}═{"═".PadLeft(5, '═')}═╝╚═{"═".PadLeft(3, '═')}═{"═".PadLeft(5, '═')}═╝╚═{"═".PadLeft(5, '═')}═╝╚═{"═".PadLeft(12, '═')}═╝╚═{"═".PadLeft(20, '═')}═╝╚═{"═".PadLeft(35, '═')}═╝╚═{"═".PadLeft(15, '═')}═╝╚═{"═".PadLeft(6, '═')}══╝╚═{"═".PadLeft(21, '═')}═╝");
      }
   }

   public static void WriteStatList(List<ServerStat> serverStatList)
   {
      try
      {
         int counter = 0;
         FillRow(1);
         Center($"╠ {"Id",5} ╣╠ {"Name",-10} ╣╠ {"DynDnsAddress",-15} ╣╠ {"Address",15}:{"Port",-5} ╣╠ {"pL",3}/{"MpL",-5} ╣╠ {"IsUp",-5} ╣╠ {"Version",12} ╣╠ {"Map",-20} ╣╠ {"Game",-35} ╣╠ {"GameID",15} ╣╠ {"uT",6}% ╣╠ {"FetchTime",21} ╣");
         FillRow(2);
         foreach (var serverStatObjItem in serverStatList)
         {
            if (serverStatObjItem.ServerUp)
            {
               if(counter % 2 == 0)
                  Console.ForegroundColor = ConsoleColor.Green;
               else
                  Console.ForegroundColor = ConsoleColor.DarkGreen;
               counter++;
            }
            else
               Console.ForegroundColor = ConsoleColor.Red;

            Center(serverStatObjItem.ToString());
            Console.ForegroundColor = ConsoleColor.Gray;
         }

         FillRow(3);
      }
      catch
      {
         Center("Console to small!");
      }
   }
}