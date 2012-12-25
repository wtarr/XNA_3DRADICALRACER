using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3D_Radical_Racer
{
    class CompareDriversPoints : IComparer< Car >
    {
        public int Compare(Car a, Car b)
        {
            if (a.driverPoints < b.driverPoints)
                return 1;
            else if (a.driverPoints > b.driverPoints)
                return -1;
            else
                return 0;
        }
    }
}
