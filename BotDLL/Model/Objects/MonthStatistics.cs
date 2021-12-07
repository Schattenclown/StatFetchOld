using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotDLL.Model.Objects
{
    class MonthStatistics
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Successful { get; set; }
        public int Unsuccessful { get; set; }

    }
}
