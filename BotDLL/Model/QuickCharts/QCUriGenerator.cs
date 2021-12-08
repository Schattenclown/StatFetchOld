using BotDLL.Model.Objects;

namespace BotDLL.Model.QuickCharts
{
    public class QCUriGenerator
    {
        public Uri? QCUri { get; set; }
        private static string token = "";
        private static bool virgin = true;
        public static QCUriGenerator CreateObj(ServerInfo serverInfoObj)
        {
            if (virgin)
            {
                Connections connections = Connections.GetConnections();
#pragma warning disable CS8601 // Mögliche Nullverweiszuweisung.
                token = connections.QuickChartApi;
#if DEBUG
                token = connections.QuickChartApiDebug;
#endif
#pragma warning restore CS8601 // Mögliche Nullverweiszuweisung.
                virgin = false;
            }

            QCUriGenerator QCUriObj = new QCUriGenerator();

            string quickChartString = $"https://quickchart.io/chart/render/{token}?title={serverInfoObj.Name.Replace(" ", "%20")}";

            serverInfoObj = MonthStatistics.Read(serverInfoObj);

            string lables = "&labels=";
            string data1 = "&data1=";

            foreach (MonthStatistics monthStatisticsItem in serverInfoObj.MonthStatisticsList)
            {
                lables += $"{monthStatisticsItem.Date.ToShortDateString()},";
                data1 += $"{monthStatisticsItem.MaxPlayers},";
            }

            lables = lables.TrimEnd(',');
            data1 = data1.TrimEnd(',');
            quickChartString += lables += data1;
            //Q1,Q2,Q3,Q4
            //50,40,30,20

            QCUriObj.QCUri = new Uri(quickChartString);

            return QCUriObj;
        }
    }
}