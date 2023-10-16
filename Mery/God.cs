using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mery
{
    public class God
    {
        public int year;
        public List<string> month;
        public List<string> sposobexpl;
        public List<string> days;
        public List<string> hours;
        public List<double> sumhours;
        public List<string> oilproduction;
        public List<string> sumoilproduction;
        public List<string> monoilproduction;
        public List<string> waterproduction;
        public List<string> sumwaterproduction;
        public List<string> monwaterproduction;
        public List<string> percent;
        public List<string> oilrate;
        public List<string> liquidrate;        

        public God()
        {
            this.year = 0;
            this.month = new List<string>();
            this.sposobexpl = new List<string>();
            this.days = new List<string>();
            this.hours = new List<string>();
            this.oilproduction = new List<string>();
            this.waterproduction = new List<string>();
            this.percent = new List<string>();
            this.oilrate = new List<string>();
            this.liquidrate = new List<string>();
            this.sumhours = new List<double>();
            this.sumoilproduction = new List<string>();
            this.sumwaterproduction = new List<string>();
            this.monoilproduction = new List<string>();
            this.monwaterproduction = new List<string>();
        }
    }
}
