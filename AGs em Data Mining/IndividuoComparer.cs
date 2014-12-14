using System;
using System.Collections.Generic;
using System.Text;

namespace AGs_em_Data_Mining
{
    public class IndividuoComparer : IComparer<Individuo>
    {
        public int Compare(Individuo x, Individuo y)
        {
            if (x.aptidao > y.aptidao)
            {
                return 1;
            }
            else
                if (x.aptidao < y.aptidao)
                {
                    return - 1;
                }
                else
                {
                    return 0;
                }
        }
    }
}
