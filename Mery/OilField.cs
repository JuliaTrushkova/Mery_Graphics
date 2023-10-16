using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mery
{
    public class OilField
    {
        public string name;
        public List<Plast> Plasty;
        public List<string> nameplasty;

        public OilField()
        {
            name = "";
            Plasty = new List<Plast>();
            nameplasty = new List<string>();
        }

        public OilField(string name)
        {
            this.name = name;
            Plasty = new List<Plast>();
            nameplasty = new List<string>();
        }

        public OilField(string name1, List<Plast> Plasty1, List<string> nameplasty1)
        {
            this.name = name1;
            this.Plasty = Plasty1;
            this.nameplasty = nameplasty1;
        }

        public void Clear()
        {
            name = "";
            Plasty.Clear();
            nameplasty.Clear();
        }

    }
}
