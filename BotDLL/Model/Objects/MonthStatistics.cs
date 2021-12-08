using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotDLL.Model.Objects
{
    public class MonthStatistics
    {
        public ushort MonthStatisticId { get; set; }
        public ushort ServerInfoId { get; set; }
        public ushort MaxPlayers { get; set; }
        public DateTime Date { get; set; }
        public MonthStatistics()
        {

        }
    }
}
