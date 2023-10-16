using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mery
{
    public class well
    {
        public string wellname;
        public string wellfield;
        public List<string> wellstrings;
        public string wellstart;
        public double wellhours;
        public List<string[]> wellstroki;
        public List<Plast> plasty;

        //public well()
        //{
            
        //}

        public well()
        {
            wellname = "";
            wellfield = "";
            wellhours = 0;
            wellstart = "";
            wellstrings = new List<string>();
            wellstroki = new List<string[]>();
            plasty = new List<Plast>();
        }

        public void clear()
        {
            this.wellname = "";
            this.wellfield = "";
            this.wellhours = 0;
            this.wellstart = "";
            this.wellstrings.Clear();
            this.wellstroki.Clear();
            plasty.Clear();
        }

 

    }
}
