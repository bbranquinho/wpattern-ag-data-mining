using System;
using System.Collections.Generic;
using System.Text;

namespace AGs_em_Data_Mining
{
    public class Gene
    {
        #region Propriedades e Variáveis
        private Int32 _weight, _operator, _value;

        /// <summary>
        /// Varia entre 0 e 10.
        /// </summary>
        public Int32 Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }

        /// <summary>
        /// Pode possuir 2 ou 4 valores possíveis.
        /// Sendo assim:
        ///     0 => =
        ///     1 => !=
        ///     2 => >=
        ///     3 => <
        /// </summary>
        public Int32 Operator
        {
            get { return _operator; }
            set { _operator = value; }
        }

        /// <summary>
        /// Se tipo igual a 0, possui: 0, 1, 2 ou 3.
        /// Se tipo igual a 1, possui: 0 ou 1.
        /// Se tipo igual a 2, possui: um valor entre 7 e 80.
        /// </summary>
        public Int32 Value
        {
            get { return _value; }
            set { _value = value; }
        }

        #endregion

        #region Construtores
        public Gene(Int32 W, Int32 O, Int32 V)
        {
            Weight    = W;
            Operator  = O;
            Value     = V;
        }
        #endregion

        public override bool Equals(object obj)
        {
            if (this.Operator != ((Gene)obj).Operator)
            {
                return false;
            }

            if (this.Value != ((Gene)obj).Value)
            {
                return false;
            }

            if (this.Weight != ((Gene)obj).Weight)
            {
                return false;
            }

            return true;
        }
    }
}
