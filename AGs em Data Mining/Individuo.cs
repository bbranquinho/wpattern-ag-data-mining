using System;
using System.Collections.Generic;
using System.Text;

namespace AGs_em_Data_Mining
{
    public class Individuo
    {
        public List<Gene> cromossomo;
        public float sensitividade, especificidade, aptidao;

        #region Construtores
        public Individuo(Int32 numeroGene)
        {
            cromossomo = new List<Gene>(numeroGene);
        }

        public Individuo(Individuo individuo)
        {
            cromossomo = new List<Gene>(individuo.cromossomo.Count);

            for (Int32 i = 0; i < individuo.cromossomo.Count; i++)
            {
                this.cromossomo.Add(new Gene(individuo.cromossomo[i].Weight,
                                             individuo.cromossomo[i].Operator,
                                             individuo.cromossomo[i].Value));
            }

            // Essa copia é integral, porem, na parte de reprodução os valores
            // de Se, Sp e Fitness são calculados novamente para o "novo" indivíduo.
            this.sensitividade  = individuo.sensitividade;
            this.especificidade = individuo.especificidade;
            this.aptidao        = individuo.aptidao;
        }

        public override bool Equals(object obj)
        {
            if (this.aptidao        != ((Individuo)obj).aptidao ||
                this.especificidade != ((Individuo)obj).especificidade ||
                this.sensitividade  != ((Individuo)obj).sensitividade)
            {
                return false;
            }

            for (Int32 i = 0; i < this.cromossomo.Count; i++)
            {
                if (this.cromossomo[i].Equals(((Individuo)obj).cromossomo[i]))
                {
                    return false;
                }
            }

            return true;
        }
        #endregion
    }
}
