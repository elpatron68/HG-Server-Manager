using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HG_ServerUI
{
    public class Boatpoints
    {
        public string? boatname { get; set; }
        public int races { get; set; }
        public int points { get; set; }
        public Boatpoints()
        {
            boatname = string.Empty;
            races = 0;
            points = 0;
        }
    }
}
