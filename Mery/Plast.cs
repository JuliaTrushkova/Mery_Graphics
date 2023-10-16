using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mery
{
    public class Plast
    {
        public List<God> gods;
        public List<string[]> wellstrokipl;
        public string name;
        public double sumdays;
        public double sumhours;
        public double sumoilproduction;
        public double sumwaterproduction;
        public string sumpercentstart;
        public string sumpercentend;
        public string sposob;

        public Plast()
        {
            name = "";
            gods = new List<God>();
            wellstrokipl = new List<string[]>();
        }

        public Plast(string name)
        {
            this.name = name;
        }

    }
}
