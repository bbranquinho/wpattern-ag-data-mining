using System;
using System.Collections.Generic;
using System.Text;

namespace AGs_em_Data_Mining
{
    public class Mundo
    {
        public static Random rand = new Random();

        private List<Individuo> populacao;
        public Int32 tamanhoPopulacao;
        public Int32 taxaCrossover;
        public Int32 taxaMutacao;
        public Int32 numeroGeracoes;
        public Int32 contaGeracoes;
        public Int32 numeroGenes;
        public Int32 limiar;
        public Int32 classe;
        public Int32 tipoEvolucao;
        public Int32 metodoSelecao;

        #region Construtores
        public Mundo(Int32 tamanhoPopulacao, Int32 taxaCrossover, Int32 taxaMutacao, Int32 numeroGeracoes, Int32 numeroGenes, Int32 limiar, Int32 classe, Int32 tipoEvolucao, Int32 metodoSelecao)
        {
            this.tamanhoPopulacao = tamanhoPopulacao;
            this.taxaCrossover    = taxaCrossover;
            this.taxaMutacao      = taxaMutacao;
            this.numeroGeracoes   = numeroGeracoes;
            this.numeroGenes      = numeroGenes;
            this.limiar           = limiar;
            this.classe           = classe;
            this.tipoEvolucao     = tipoEvolucao;
            this.metodoSelecao    = metodoSelecao;
        }

        public Mundo(Int32 numeroGenes)
        {
            this.numeroGenes = numeroGenes;
        }
        #endregion

        #region Métodos Públicos
        public Individuo AGDataMining(List<Int32[]> baseDado)
        {
            contaGeracoes = 0;
            populacao = new List<Individuo>(tamanhoPopulacao);

            CriarPopulacao();

            CalculaAptidao(baseDado);
            populacao.Sort(new IndividuoComparer());

            while (++contaGeracoes <= numeroGeracoes)
            {
                Reproducao((Int32)(tamanhoPopulacao * (taxaCrossover / 100.0f)));
                
                CalculaAptidao(baseDado, tamanhoPopulacao, populacao.Count - 1);

                Evolucao();
            }

            return populacao[populacao.Count - 1];
        }

        public Individuo TestaMelhorSolucao(List<Int32[]> baseDado, Individuo individuo)
        {
            Int32 numeroDados = (Int32)(baseDado.Count * 0.666666667);
            Int32 j, k, Tp, Tn, Fp, Fn;
            bool pertenceClasse;

            Tp = Tn = Fp = Fn = 0;

            for (k = numeroDados; k < baseDado.Count; k++)
            {
                for (j = 0, pertenceClasse = true; j < numeroGenes; j++)
                {
                    if (individuo.cromossomo[j].Weight >= limiar)
                    {
                        if (0 == individuo.cromossomo[j].Operator)
                        {
                            if (baseDado[k][j] != individuo.cromossomo[j].Value)
                            {
                                pertenceClasse = false;
                                break;
                            }
                        }
                        else if (1 == individuo.cromossomo[j].Operator)
                        {
                            if (baseDado[k][j] == individuo.cromossomo[j].Value)
                            {
                                pertenceClasse = false;
                                break;
                            }
                        }
                        else if (2 == individuo.cromossomo[j].Operator)
                        {
                            if (baseDado[k][j] < individuo.cromossomo[j].Value)
                            {
                                pertenceClasse = false;
                                break;
                            }
                        }
                        else if (3 == individuo.cromossomo[j].Operator)
                        {
                            if (baseDado[k][j] >= individuo.cromossomo[j].Value)
                            {
                                pertenceClasse = false;
                                break;
                            }
                        }
                    }
                }

                // Para "pertenceClasse = TRUE" temos Tp ou Tn.
                // Caso contrário temos Fp ou Fn.
                if (pertenceClasse)
                {
                    if (baseDado[k][numeroGenes] == classe)
                    {
                        Tp++;
                    }
                    else
                    {
                        Fp++;
                    }
                }
                else
                {
                    if (baseDado[k][numeroGenes] != classe)
                    {
                        Tn++;
                    }
                    else
                    {
                        Fn++;
                    }
                }
            }

            individuo.sensitividade = (float)Tp / (float)(Tp + Fn);
            individuo.especificidade = (float)Tn / (float)(Tn + Fp);
            individuo.aptidao = individuo.sensitividade * individuo.especificidade;

            return individuo;
        }
        #endregion

        #region Métodos Privados
        private void CriarPopulacao()
        {
            Int32 i, j;

            for (i = 0; i < tamanhoPopulacao; i++)
            {
                populacao.Add(new Individuo(numeroGenes));

                for (j = 0; j < numeroGenes; j++)
                {
                    if (j == 10 || j == 33)
                    {
                        // Family History (0 = não possui; 1 = possui).
                        if (j == 10)
                        {
                            populacao[i].cromossomo.Add(new Gene(rand.Next(11), rand.Next(2), rand.Next(2)));
                        }
                        // Age (Valor Linear): 7 <= Age <= 80.
                        else
                        {
                            populacao[i].cromossomo.Add(new Gene(rand.Next(11), rand.Next(4), rand.Next(74) + 7));
                        }
                    }
                    else
                    {
                        populacao[i].cromossomo.Add(new Gene(rand.Next(11), rand.Next(4), rand.Next(4)));
                    }
                }
            }
        }

        private void CalculaAptidao(List<Int32[]> baseDado)
        {
            Int32 numeroDados = (Int32)(baseDado.Count * 0.666666667);
            Int32 i, j, k, Tp, Tn, Fp, Fn;
            bool pertenceClasse;

            // Busca da aptidão por indivíduo.
            for(i = 0; i < populacao.Count; i++)
            {
                // Determina se existe pelo menos 1 regra.

                // Essa variável deveria se chamar possuiRegra,
                // mas para evitar gastar variável vou reutilizar essa mesmo.
                pertenceClasse = false;

                foreach (Gene gene in populacao[i].cromossomo)
                {
                    if (gene.Weight >= limiar)
                    {
                        pertenceClasse = true;
                        break;
                    }
                }

                if (!pertenceClasse)
                {
                    populacao[i].sensitividade = 0.0f;
                    populacao[i].especificidade = 0.0f;
                    populacao[i].aptidao = 0.0f;
                    continue;
                }

                // Começa a busca na base de dados aplicando as regras.
                Tp = Tn = Fp = Fn = 0;

                for (k = 0; k < numeroDados; k++)
                {
                    // Aplica a regra na base de dados.
                    // Com isso determinamos Tp; Tn; Fp; Fn.
                    // Finalmente é calculada a aptidão.
                    for (j = 0, pertenceClasse = true; j < numeroGenes; j++)
                    {
                        if (populacao[i].cromossomo[j].Weight >= limiar)
                        {
                            if (0 == populacao[i].cromossomo[j].Operator)
                            {
                                if (baseDado[k][j] != populacao[i].cromossomo[j].Value)
                                {
                                    pertenceClasse = false;
                                    break;
                                }
                            }
                            else if (1 == populacao[i].cromossomo[j].Operator)
                            {
                                if (baseDado[k][j] == populacao[i].cromossomo[j].Value)
                                {
                                    pertenceClasse = false;
                                    break;
                                }
                            }
                            else if (2 == populacao[i].cromossomo[j].Operator)
                            {
                                if (baseDado[k][j] < populacao[i].cromossomo[j].Value)
                                {
                                    pertenceClasse = false;
                                    break;
                                }
                            }
                            else if (3 == populacao[i].cromossomo[j].Operator)
                            {
                                if (baseDado[k][j] >= populacao[i].cromossomo[j].Value)
                                {
                                    pertenceClasse = false;
                                    break;
                                }
                            }
                        }
                    }

                    // Para "pertenceClasse = TRUE" temos Tp ou Tn.
                    // Caso contrário temos Fp ou Fn.
                    if (pertenceClasse)
                    {
                        if (baseDado[k][numeroGenes] == classe)
                        {
                            Tp++;
                        }
                        else
                        {
                            Fp++;
                        }
                    }
                    else
                    {
                        if (baseDado[k][numeroGenes] != classe)
                        {
                            Tn++;
                        }
                        else
                        {
                            Fn++;
                        }
                    }
                }

                populacao[i].sensitividade  = (float)(Tp + 1) / (float)(Tp + Fn + 1);
                populacao[i].especificidade = (float)(Tn + 1) / (float)(Tn + Fp + 1);
                populacao[i].aptidao        = populacao[i].sensitividade * populacao[i].especificidade;
            }
        }

        private void CalculaAptidao(List<Int32[]> baseDado, Int32 inicio, Int32 fim)
        {
            Int32 numeroDados = (Int32)(baseDado.Count * 0.666666667);
            Int32 i, j, k, Tp, Tn, Fp, Fn;
            bool pertenceClasse;

            // Busca da aptidão por indivíduo.
            for (i = inicio; i <= fim; i++)
            {
                // Determina se existe pelo menos 1 regra.

                // Essa variável deveria se chamar possuiRegra,
                // mas para evitar gastar variável vou reutilizar essa mesmo.
                pertenceClasse = false;

                foreach (Gene gene in populacao[i].cromossomo)
                {
                    if (gene.Weight >= limiar)
                    {
                        pertenceClasse = true;
                        break;
                    }
                }

                if (!pertenceClasse)
                {
                    populacao[i].sensitividade = 0.0f;
                    populacao[i].especificidade = 0.0f;
                    populacao[i].aptidao = 0.0f;
                    continue;
                }

                // Começa a busca na base de dados aplicando as regras.
                Tp = Tn = Fp = Fn = 0;

                for (k = 0; k < numeroDados; k++)
                {
                    // Aplica a regra na base de dados.
                    // Com isso determinamos Tp; Tn; Fp; Fn.
                    // Finalmente é calculada a aptidão.
                    for (j = 0, pertenceClasse = true; j < numeroGenes; j++)
                    {
                        if (populacao[i].cromossomo[j].Weight >= limiar)
                        {
                            if (0 == populacao[i].cromossomo[j].Operator)
                            {
                                if (baseDado[k][j] != populacao[i].cromossomo[j].Value)
                                {
                                    pertenceClasse = false;
                                    break;
                                }
                            }
                            else if (1 == populacao[i].cromossomo[j].Operator)
                            {
                                if (baseDado[k][j] == populacao[i].cromossomo[j].Value)
                                {
                                    pertenceClasse = false;
                                    break;
                                }
                            }
                            else if (2 == populacao[i].cromossomo[j].Operator)
                            {
                                if (baseDado[k][j] < populacao[i].cromossomo[j].Value)
                                {
                                    pertenceClasse = false;
                                    break;
                                }
                            }
                            else if (3 == populacao[i].cromossomo[j].Operator)
                            {
                                if (baseDado[k][j] >= populacao[i].cromossomo[j].Value)
                                {
                                    pertenceClasse = false;
                                    break;
                                }
                            }
                        }
                    }

                    // Para "pertenceClasse = TRUE" temos Tp ou Tn.
                    // Caso contrário temos Fp ou Fn.
                    if (pertenceClasse)
                    {
                        if (baseDado[k][numeroGenes] == classe)
                        {
                            Tp++;
                        }
                        else
                        {
                            Fp++;
                        }
                    }
                    else
                    {
                        if (baseDado[k][numeroGenes] != classe)
                        {
                            Tn++;
                        }
                        else
                        {
                            Fn++;
                        }
                    }
                }

                populacao[i].sensitividade = (float)(Tp + 1) / (float)(Tp + Fn + 1);
                populacao[i].especificidade = (float)(Tn + 1) / (float)(Tn + Fp + 1);
                populacao[i].aptidao = populacao[i].sensitividade * populacao[i].especificidade;
            }
        }

        private void Reproducao(Int32 individuosGerados)
        {
            Int32 i;

            individuosGerados += individuosGerados % 2;

            for (i = 1; i < individuosGerados; i += 2)
            {
                Reproduz();
            }
        }

        private void Reproduz()
        {
            Individuo individuo1, individuo2;
            Int32 inicio, fim, i, tmpOperador, tmpValor, tmpPeso;

            // Sorteio dos Indivíduos.
            if (0 == metodoSelecao) // Estocástico.
            {
                individuo1 = new Individuo(Estocastica());
                individuo2 = new Individuo(Estocastica());
            }
            else if (1 == metodoSelecao) // Torneio Simples 3.
            {
                individuo1 = new Individuo(TorneioSimples3());
                individuo2 = new Individuo(TorneioSimples3());
            }
            else // Roleta.
            {
                individuo1 = new Individuo(Roleta());
                individuo2 = new Individuo(Roleta());
            }

            // CROSSOVER DE 2 PONTOS.
            do
            {
                inicio = rand.Next(numeroGenes);
                fim    = rand.Next(numeroGenes);

                if (inicio > fim)
                {
                    Int32 tmp = inicio;
                    inicio = fim;
                    fim = tmp;
                }
            }
            while (0 == inicio && (numeroGenes - 1) == fim);

            for (i = inicio; i <= fim; i++)
            {
                tmpOperador = individuo1.cromossomo[i].Operator;
                tmpValor    = individuo1.cromossomo[i].Value;
                tmpPeso     = individuo1.cromossomo[i].Weight;
                
                individuo1.cromossomo[i].Operator  = individuo2.cromossomo[i].Operator;
                individuo1.cromossomo[i].Value     = individuo2.cromossomo[i].Value;
                individuo1.cromossomo[i].Weight    = individuo2.cromossomo[i].Weight;

                individuo2.cromossomo[i].Operator  = tmpOperador;
                individuo2.cromossomo[i].Value     = tmpValor;
                individuo2.cromossomo[i].Weight    = tmpPeso;
            }

            // Mutação.
            for (i = 0; i < numeroGenes; i++)
            {
                // Mutação no Peso/Weight.
                // Indivíduo 1.
                if (rand.Next(101) <= taxaMutacao)
                {
                    individuo1.cromossomo[i].Weight = rand.Next(11);
                }
                // Indivíduo 2.
                if (rand.Next(101) <= taxaMutacao)
                {
                    individuo2.cromossomo[i].Weight = rand.Next(11);
                }

                // Mutação no Operador/Operator.
                // Indivíduo 1.
                if (rand.Next(101) <= taxaMutacao)
                {
                    if (i != 10)
                    {
                        individuo1.cromossomo[i].Operator = rand.Next(4);
                    }
                    else
                    {
                        individuo1.cromossomo[i].Operator = rand.Next(2);
                    }
                }
                // Indivíduo 2.
                if (rand.Next(101) <= taxaMutacao)
                {
                    if (i == 10)
                    {
                        individuo2.cromossomo[i].Operator = rand.Next(2);
                    }
                    else
                    {
                        individuo2.cromossomo[i].Operator = rand.Next(4);
                    }
                }

                // Mutação no Valor/Value.
                // Indivíduo 1.
                if (rand.Next(101) <= taxaMutacao)
                {
                    if (i == 10)
                    {
                        individuo1.cromossomo[i].Value = rand.Next(2);
                    }
                    else if (i == 33)
                    {
                        individuo1.cromossomo[i].Value = rand.Next(74) + 7;
                    }
                    else
                    {
                        individuo1.cromossomo[i].Value = rand.Next(4);
                    }
                }
                // Indivíduo 2.
                if (rand.Next(101) <= taxaMutacao)
                {
                    if (i == 10)
                    {
                        individuo2.cromossomo[i].Value = rand.Next(2);
                    }
                    else if (i == 33)
                    {
                        individuo2.cromossomo[i].Value = rand.Next(74) + 7;
                    }
                    else
                    {
                        individuo2.cromossomo[i].Value = rand.Next(4);
                    }
                }
            }

            populacao.Add(individuo1);
            populacao.Add(individuo2);
        }

        private Individuo Estocastica()
        {
            float[] roleta = new float[tamanhoPopulacao];
            float valorSorteio;
            Int32 i, j, melhorIndividuo;

            roleta[0] = populacao[0].aptidao;

            // Faz o acumulado.
            for (i = 1; i < tamanhoPopulacao; i++)
            {
                roleta[i] = roleta[i - 1] + populacao[i].aptidao;
            }

            // Realiza a normalização
            for (i = 0; i < tamanhoPopulacao; i++)
            {
                roleta[i] = roleta[i] / roleta[tamanhoPopulacao - 1];
            }

            // Sorteio do melhor indivíduo.
            melhorIndividuo = 0;

            for (i = 0; i < 3; i++)
            {
                valorSorteio = (float)rand.NextDouble();

                for (j = 0; j < tamanhoPopulacao; j++)
                {
                    if (valorSorteio <= roleta[j])
                    {
                        if (populacao[melhorIndividuo].aptidao <= populacao[j].aptidao)
                        {
                            melhorIndividuo = j;
                        }

                        break;
                    }
                }
            }

            return populacao[melhorIndividuo];
        }

        private Individuo TorneioSimples3()
        {
            Int32 melhorIndividuo = 0;
            Int32 valorSorteio;

            for (Int32 i = 0; i < 3; i++)
            {
                valorSorteio = rand.Next(tamanhoPopulacao);

                if (populacao[melhorIndividuo].aptidao < populacao[valorSorteio].aptidao)
                {
                    melhorIndividuo = valorSorteio;
                }
            }

            return populacao[melhorIndividuo];
        }

        private Individuo Roleta()
        {
            float[] roleta = new float[tamanhoPopulacao];
            float valorSorteio;
            Int32 i, j, melhorIndividuo;

            roleta[0] = populacao[0].aptidao;

            // Faz o acumulado.
            for (i = 1; i < tamanhoPopulacao; i++)
            {
                roleta[i] = roleta[i - 1] + populacao[i].aptidao;
            }

            // Realiza a normalização
            for (i = 0; i < tamanhoPopulacao; i++)
            {
                roleta[i] = roleta[i] / roleta[tamanhoPopulacao - 1];
            }
            
            // Sorteio do Melhor Indivíduo.
            melhorIndividuo = 0;

            valorSorteio = (float)rand.NextDouble();

            for (j = 0; j < tamanhoPopulacao; j++)
            {
                if (valorSorteio <= roleta[j])
                {
                    if (populacao[melhorIndividuo].aptidao <= populacao[j].aptidao)
                    {
                        melhorIndividuo = j;
                    }

                    break;
                }
            }

            return populacao[melhorIndividuo];
        }

        private void Evolucao()
        {
            if (0 == tipoEvolucao)
            {
                if (populacao.Count > tamanhoPopulacao)
                {
                    Int32 numIndDel = (Int32)(tamanhoPopulacao * (taxaCrossover / 100.0f));

                    numIndDel += numIndDel % 2;

                    populacao.RemoveRange(0, numIndDel - 1);

                    populacao.Sort(new IndividuoComparer());

                    populacao.RemoveRange(0, 1);
                }
            }
            else
            {
                populacao.Sort(new IndividuoComparer());

                if (populacao.Count > tamanhoPopulacao)
                {
                    populacao.RemoveRange(0, populacao.Count - tamanhoPopulacao);
                }
            }
        }
        #endregion
    }
}
