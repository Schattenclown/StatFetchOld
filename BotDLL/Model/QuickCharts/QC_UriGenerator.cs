using BotDLL.Model.Objects;

namespace BotDLL.Model.QuickCharts
{
   public class QC_UriGenerator
   {
      private static string? _token = "";
      private static bool _virgin = true;
      public Uri? QcUri { get; private set; }

      public static QC_UriGenerator CreateObj(ServerInfo? serverInfoObj)
      {
         if (_virgin)
         {
            Connections connections = Connections.GetConnections();

#if RELEASE
            _token = connections.QuickChartApi;
#elif DEBUG
            _token = connections.QuickChartApiDebug;
#endif
            _virgin = false;
         }

         QC_UriGenerator qcUriObj = new();

         if (serverInfoObj != null)
         {
            string quickChartString = $"https://quickchart.io/chart/render/{_token}?title={serverInfoObj.Name?.Replace(" ", "%20")}";

            serverInfoObj = MonthStatistics.Read(serverInfoObj);

            string labels = "&labels=";
            string data1 = "&data1=";
            string data2 = "&data2=";

            int ii = 0;

            List<string> labelsList = new();
            List<int> data1List = new();
            List<int> data2List = new();

            foreach (MonthStatistics monthStatisticsItem in serverInfoObj?.MonthStatisticsList!)
            {
               switch (ii)
               {
                  case < 30:
                     labelsList.Add(monthStatisticsItem.Date.ToShortDateString());
                     data1List.Add(monthStatisticsItem.MaxPlayers);
                     break;
                  case < 60:
                     data2List.Add(monthStatisticsItem.MaxPlayers);
                     break;
               }

               ii++;
            }

            for (int i = 0; i < 30; i++)
            {
               if (data1List.Count < 30)
               {
                  data1List.Add(0);
               }

               if (data2List.Count < 30)
               {
                  data2List.Add(0);
               }
            }

            labelsList.Reverse();
            data1List.Reverse();
            data2List.Reverse();

            for (int i = 0; i < 30; i++)
            {
               labels += $"{labelsList[i]},";
               data1 += $"{data1List[i]},";
            }

            for (int i = 0; i < 30; i++)
            {
               data2 += $"{data2List[i]},";
            }

            /*foreach (MonthStatistics monthStatisticsItem in serverInfoObj.MonthStatisticsList)
            {
                if(ii < 30)
                {
                    labels += $"{monthStatisticsItem.Date.ToShortDateString()},";
                    data1 += $"{monthStatisticsItem.MaxPlayers},";
                }
                else if (ii < 60)
                {
                    data2 += $"{monthStatisticsItem.MaxPlayers},";
                }
   
                */ /*if (monthStatisticsItem.Date.Month == DateTime.Now.Month)
                {
                    labels += $"{monthStatisticsItem.Date.ToShortDateString()},";
                    data1 += $"{monthStatisticsItem.MaxPlayers},";
                }
                else if (monthStatisticsItem.Date.Month == DateTime.Now.AddMonths(-1).Month)
                {
                    data2 += $"{monthStatisticsItem.MaxPlayers},";
                }*/ /*

                ii++;
            }*/

            labels = labels.TrimEnd(',');
            data1 = data1.TrimEnd(',');
            quickChartString += labels + data1 + data2;
            //Q1,Q2,Q3,Q4
            //50,40,30,20

            qcUriObj.QcUri = new Uri(quickChartString);
         }
         else
         {
            qcUriObj.QcUri = new Uri("https://quickchart.io/chart/render/{token}");
         }

         return qcUriObj;
      }
   }
}