using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Services.Models
{
    public class Loc
    {
        private double lt;
        private double lg;

        public double Lg
        {
            get { return lg; }
            set { lg = value; }
        }

        public double Lt
        {
            get { return lt; }
            set { lt = value; }
        }

        public Loc(double lt, double lg)
        {
            this.lt = lt;
            this.lg = lg;
        }
    }
}
