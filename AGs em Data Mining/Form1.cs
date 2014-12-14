using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace AGs_em_Data_Mining
{
    public partial class frmPrincipal : Form
    {

        public frmPrincipal()
        {
            InitializeComponent();
        }

        private void frmPrincipal_Load(object sender, EventArgs e)
        {
            cargaClasse(@"defaultClasse.ag");
            cargaAtributo(@"defaultAtributo.ag");
            //cargaClasse(@"..\..\defaultClasse.ag");
            //cargaAtributo(@"..\..\defaultAtributo.ag");

            if (lbxAtributo.Items.Count > 0)
            {
                //cargaBaseDados(@"..\..\defaultDataBase.ag");
                cargaBaseDados(@"defaultDataBase.ag");
            }
        }

        #region Carga
        private void cargaClasse(String caminho)
        {
            String linha;
            Int32 count = 0;

            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(caminho);

                lbxClasse.Items.Clear();

                while ((linha = file.ReadLine()) != null && ++count <= 50)
                {
                    lbxClasse.Items.Add(linha);
                }

                if (lbxClasse.Items.Count > 0)
                {
                    lbxClasse.SelectedIndex = 0;
                }

                if (count > 50)
                {
                    txtErroCargaClasseAtributo.Text += "Número máximo de classes é 50. Não foram inseridos todas classes do arquivo.\r\n";
                }
            }
            catch (Exception e)
            {
                txtErroCargaClasseAtributo.Text += e.Message + "\r\n";
            }
        }

        private void cargaAtributo(String caminho)
        {
            String linha;
            Int32 count = 0;

            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(caminho);

                lbxAtributo.Items.Clear();

                while ((linha = file.ReadLine()) != null && ++count <= 100)
                {
                    lbxAtributo.Items.Add(linha);
                }

                if (lbxAtributo.Items.Count > 0)
                {
                    lbxAtributo.SelectedIndex = 0;
                }

                if (count > 100)
                {
                    txtErroCargaClasseAtributo.Text += "Número máximo de atributos é 100. Não foram inseridos todos atributos do arquivo.\r\n";
                    dgvCargaBaseDado.ColumnCount = count;
                }
                else
                {
                    dgvCargaBaseDado.ColumnCount = count + 1;
                }

                DataGridViewCellStyle style = dgvCargaBaseDado.ColumnHeadersDefaultCellStyle;
                style.Font = new Font(dgvCargaBaseDado.Font, FontStyle.Bold);

                dgvCargaBaseDado.EditMode = DataGridViewEditMode.EditOnF2;
                dgvCargaBaseDado.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
                dgvCargaBaseDado.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                dgvCargaBaseDado.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                dgvCargaBaseDado.GridColor = SystemColors.ActiveBorder;
                dgvCargaBaseDado.RowHeadersVisible = false;

                for (Int32 i = 0; i < lbxAtributo.Items.Count; i++)
                {
                    if (lbxAtributo.Items[i].ToString().Contains("|"))
                    {
                        dgvCargaBaseDado.Columns[i].Name = lbxAtributo.Items[i].ToString().Substring(0, lbxAtributo.Items[i].ToString().IndexOf('|'));
                    }
                    else
                    {
                        dgvCargaBaseDado.Columns[i].Name = lbxAtributo.Items[i].ToString();
                    }
                }

                dgvCargaBaseDado.Columns[lbxAtributo.Items.Count].Name = ((Int32)(lbxAtributo.Items.Count + 1)).ToString() + ": Classe";
            }
            catch (Exception e)
            {
                txtErroCargaClasseAtributo.Text += e.Message + "\r\n";
            }
        }

        private void cargaBaseDados(String caminho)
        {
            String linha, tmp;
            Int32 count;
            bool inserir;

            try
            {
                String[] str = new String[dgvCargaBaseDado.ColumnCount];
                System.IO.StreamReader file = new System.IO.StreamReader(caminho);
                DataGridViewRowCollection rows = dgvCargaBaseDado.Rows;

                while ((linha = file.ReadLine()) != null)
                {
                    tmp = "";
                    count = 0;
                    inserir = true;

                    foreach (Char caracter in linha)
                    {
                        if (caracter.Equals(',') && count < str.Length)
                        {
                            try
                            {
                                Convert.ToInt32(tmp);
                                str[count++] = tmp;
                                tmp = "";
                            }
                            catch (Exception e)
                            {
                                txtErroCargaClasseAtributo.Text += e.Message + "\r\n" + linha + "\r\n";
                                inserir = false;
                                break;
                            }
                        }
                        else
                        {
                            if (tmp.Length < 8)
                            {
                                tmp += caracter;
                            }
                        }
                    }

                    if (inserir)
                    {
                        if (!str.Equals("") && count < str.Length)
                        {
                            str[count++] = tmp;
                        }

                        rows.Add(str);
                    }
                }
            }
            catch (Exception e)
            {
                txtErroCargaClasseAtributo.Text += e.Message + "\r\n";
            }
        }
        #endregion

        #region Ações dos Botões de Carga
        private void lbxClasse_DoubleClick(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Carga das Classes";
            openFileDialog1.Filter = "Arquivo de Classes (*.ag)|*.ag|All files (*.*)|*.*";
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Stream myStream = openFileDialog1.OpenFile();

                if (myStream != null)
                {
                    cargaClasse(((FileStream)myStream).Name);
                    myStream.Close();
                }
            }
        }

        private void lbxAtributo_DoubleClick(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Carga dos Atributos";
            openFileDialog1.Filter = "Arquivo de Atributos (*.ag)|*.ag|All files (*.*)|*.*";
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Stream myStream = openFileDialog1.OpenFile();

                if (myStream != null)
                {
                    cargaAtributo(((FileStream)myStream).Name);
                    myStream.Close();
                }
            }
        }

        private void btnCargaDado_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Carga da Base de Dados";
            openFileDialog1.Filter = "Base de Dados (*.ag)|*.ag|All files (*.*)|*.*";
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Stream myStream = openFileDialog1.OpenFile();

                if (myStream != null)
                {
                    cargaBaseDados(((FileStream)myStream).Name);
                    myStream.Close();
                }
            }
        }

        private void btnLimparBaseDado_Click(object sender, EventArgs e)
        {
            dgvCargaBaseDado.Rows.Clear();
        }
        #endregion
        
        #region Processamento do AG
        private void btnAGDataMining_Click(object sender, EventArgs e)
        {
            Mundo mundo = new Mundo(Convert.ToInt32(txtTamanhoPopulacao.Text),
                                    Convert.ToInt32(txtTaxaCrossover.Text),
                                    Convert.ToInt32(txtTaxaMutacao.Text),
                                    Convert.ToInt32(txtNumeroGeracao.Text),
                                    dgvCargaBaseDado.ColumnCount - 1,
                                    Convert.ToInt32(txtLimiar.Text),
                                    lbxClasse.SelectedIndex + 1,
                                    rdbElitista.Checked ? 0 : 1,
                                    rdbEstocastico.Checked ? 0 : (rdbTorneioSimples.Checked ? 1 : 2));
            
            Individuo melhorIndividuo;

            List<Int32[]> baseDados = BaseDados();

            melhorIndividuo = mundo.AGDataMining(baseDados);

            if (melhorIndividuo != null)
            {
                txtErroCargaClasseAtributo.Text += "BASE DE TREINAMENTO\r\n";
                txtErroCargaClasseAtributo.Text += "Aptidão: " + melhorIndividuo.aptidao.ToString() + "\r\n";
                txtErroCargaClasseAtributo.Text += "Se: " + melhorIndividuo.sensitividade.ToString() + "\r\n";
                txtErroCargaClasseAtributo.Text += "Sp: " + melhorIndividuo.especificidade.ToString() + "\r\n";

                mundo.TestaMelhorSolucao(baseDados, melhorIndividuo);

                txtErroCargaClasseAtributo.Text += "BASE DE TESTE\r\n";
                txtErroCargaClasseAtributo.Text += "Aptidão: " + melhorIndividuo.aptidao.ToString() + "\r\n";
                txtErroCargaClasseAtributo.Text += "Se: " + melhorIndividuo.sensitividade.ToString() + "\r\n";
                txtErroCargaClasseAtributo.Text += "Sp: " + melhorIndividuo.especificidade.ToString() + "\r\n";
                txtErroCargaClasseAtributo.Text += "REGRA:\r\n";

                for (Int32 i = 0; i < melhorIndividuo.cromossomo.Count; i++)
                {
                    if (melhorIndividuo.cromossomo[i].Weight >= Convert.ToInt32(txtLimiar.Text))
                    {
                        string concat;

                        if (0 == melhorIndividuo.cromossomo[i].Operator)
                        {
                            concat = "=";
                        }
                        else if (1 == melhorIndividuo.cromossomo[i].Operator)
                        {
                            concat = "!=";
                        }
                        else if (2 == melhorIndividuo.cromossomo[i].Operator)
                        {
                            concat = ">=";
                        }
                        else
                        {
                            concat = "<";
                        }

                        txtErroCargaClasseAtributo.Text += "IF " + lbxAtributo.Items[i].ToString() + " " + concat + " " + melhorIndividuo.cromossomo[i].Value + "\r\n";
                    }
                }
            }
        }

        private List<Int32[]> BaseDados()
        {
            List<Int32[]> baseDados = new List<Int32[]>(dgvCargaBaseDado.RowCount - 1);
            Int32[] linha;
            Int32 i, j;

            for (i = 0; i < baseDados.Capacity; i++)
            {
                linha = new Int32[dgvCargaBaseDado.ColumnCount];

                for (j = 0; j < linha.Length; j++)
                {
                    linha[j] = Convert.ToInt32(dgvCargaBaseDado.Rows[i].Cells[j].EditedFormattedValue);
                }

                baseDados.Add(linha);
            }

            return baseDados;
        }
        #endregion
        private void verificaChecked()
        {
            if ((chk1Psoriasis.Checked == false &&
                 chk2SeboreicDermatitis.Checked == false &&
                 chk3LichenPlanus.Checked == false &&
                 chk4PityriasisRosea.Checked == false &&
                 chk5CronicDermatitis.Checked == false &&
                 chk6PityriasisRubraPilaris.Checked == false) ||
                (chkEstocastico.Checked == false &&
                 chkTorneioSimple3.Checked == false &&
                 chkRoleta.Checked == false) ||
                (chkElitista.Checked == false &&
                 chkMelhores.Checked == false))
            {
                btnProcessaBusca.Enabled = false;
            }
            else
            {
                btnProcessaBusca.Enabled = true;
            }
        }

        private void chkEstocastico_CheckedChanged(object sender, EventArgs e)
        {
            verificaChecked();
        }

        private void chkTorneioSimple3_CheckedChanged(object sender, EventArgs e)
        {
            verificaChecked();
        }

        private void chkRoleta_CheckedChanged(object sender, EventArgs e)
        {
            verificaChecked();
        }

        private void chkElitista_CheckedChanged(object sender, EventArgs e)
        {
            verificaChecked();
        }

        private void chkMelhores_CheckedChanged(object sender, EventArgs e)
        {
            verificaChecked();
        }

        private void chk1Psoriasis_CheckedChanged(object sender, EventArgs e)
        {
            verificaChecked();
        }

        private void chk2SeboreicDermatitis_CheckedChanged(object sender, EventArgs e)
        {
            verificaChecked();
        }

        private void chk3LichenPlanus_CheckedChanged(object sender, EventArgs e)
        {
            verificaChecked();
        }

        private void chk4PityriasisRosea_CheckedChanged(object sender, EventArgs e)
        {
            verificaChecked();
        }

        private void chk5CronicDermatitis_CheckedChanged(object sender, EventArgs e)
        {
            verificaChecked();
        }

        private void chk6PityriasisRubraPilaris_CheckedChanged(object sender, EventArgs e)
        {
            verificaChecked();
        }

        private void btnProcessaBusca_Click(object sender, EventArgs e)
        {
            Mundo mundo = new Mundo(dgvCargaBaseDado.ColumnCount - 1);
            Int32 tamanhoPopulacaoInicio, tamanhoPopulacaoFim, tamanhoPopulacaoRazao;
            Int32 numeroGeracaoInicio, numeroGeracaoFim, numeroGeracaoRazao;
            Int32 taxaCrossoverInicio, taxaCrossoverFim, taxaCrossoverRazao;
            Int32 taxaMutacaoInicio, taxaMutacaoFim, taxaMutacaoRazao;
            Int32 limiarInicio, limiarFim, limiarRazao;
            Int32 numeroTesteTotal, numeroRegra = 0;
            Int32 classe, evolucao, selecao, tamanhoPopulacao, numeroGeracao, taxaCrossover, taxaMutacao, limiar, numeroTeste;
            Int32 i, linhaPlanilha = 1;

            Individuo melhorIndividuo;

            object seaGreen = "SeaGreen";
            object black = "Black";
            object blue = "Yellow";

            float somaApTr, somaSeTr, somaSpTr, somaApTe, somaSeTe, somaSpTe;

            //axPlanilhaResultado.Cells.Clear();
            txtBuscaResultado.Text = "";

            List<Int32[]> baseDados = BaseDados();

            try
            {
                // Tamanho da População.
                tamanhoPopulacaoInicio = Convert.ToInt32(txtBuscaTamPopIni.Text);
                tamanhoPopulacaoFim    = Convert.ToInt32(txtBuscaTamPopFim.Text);
                tamanhoPopulacaoRazao  = Convert.ToInt32(txtBuscaTamPopRazao.Text);

                // Número de Gerações.
                numeroGeracaoInicio = Convert.ToInt32(txtBuscaNumGerInicio.Text);
                numeroGeracaoFim    = Convert.ToInt32(txtBuscaNumGerFim.Text);
                numeroGeracaoRazao  = Convert.ToInt32(txtBuscaNumGerRazao.Text);

                // Taxa de Crossover.
                taxaCrossoverInicio = Convert.ToInt32(txtBuscaTaxaCrossInicio.Text);
                taxaCrossoverFim    = Convert.ToInt32(txtBuscaTaxaCrossFim.Text);
                taxaCrossoverRazao  = Convert.ToInt32(txtBuscaTaxaCrossFim.Text);

                // Taxa de Mutação.
                taxaMutacaoInicio = Convert.ToInt32(txtBuscaTaxaMutaInicio.Text);
                taxaMutacaoFim    = Convert.ToInt32(txtBuscaTaxaMutaFim.Text);
                taxaMutacaoRazao  = Convert.ToInt32(txtBuscaTaxaMutaRazao.Text);

                // Limiar.
                limiarInicio = Convert.ToInt32(txtBuscaLimiarInicio.Text);
                limiarFim    = Convert.ToInt32(txtBuscaLimiarFim.Text);
                limiarRazao  = Convert.ToInt32(txtBuscaLimiarRazao.Text);

                // Número de Testes.
                numeroTesteTotal = Convert.ToInt32(txtNumeroTeste.Text);
            }
            catch (Exception exception)
            {
                txtBuscaResultado.Text += exception.Message.ToString() + "\r\n";
                return;
            }

            // 1 - Classes.
            for (classe = 1; classe <= 6; classe++)
            {
            if((1 == classe && true == chk1Psoriasis.Checked) ||
               (2 == classe && true == chk2SeboreicDermatitis.Checked) ||
               (3 == classe && true == chk3LichenPlanus.Checked) ||
               (4 == classe && true == chk4PityriasisRosea.Checked) ||
               (5 == classe && true == chk5CronicDermatitis.Checked) ||
               (6 == classe && true == chk6PityriasisRubraPilaris.Checked))
            {
                mundo.classe = classe;
                //axPlanilhaResultado.Cells[linhaPlanilha++, 1] = "Classe: " + classe.ToString();

                // 2 - Evolução.
                for (evolucao = 0; evolucao <= 1; evolucao++)
                {
                if ((0 == evolucao && true == chkElitista.Checked) ||
                    (1 == evolucao && true == chkMelhores.Checked))
                {
                    mundo.tipoEvolucao = evolucao;

                    // 3 - Seleção.
                    for (selecao = 0; selecao <= 2; selecao++)
                    {
                    if ((0 == selecao && true == chkEstocastico.Checked) ||
                       (1 == selecao && true == chkTorneioSimple3.Checked) ||
                       (2 == selecao && true == chkRoleta.Checked))
                    {
                        mundo.metodoSelecao = selecao;

                        // 4 - Parâmetros.
                        //   a: Tamanho da População.
                        for (tamanhoPopulacao = tamanhoPopulacaoInicio; tamanhoPopulacao <= tamanhoPopulacaoFim; tamanhoPopulacao += tamanhoPopulacaoRazao)
                        {
                            mundo.tamanhoPopulacao = tamanhoPopulacao;

                            // b: Número de Gerações.
                            for (numeroGeracao = numeroGeracaoInicio; numeroGeracao <= numeroGeracaoFim; numeroGeracao += numeroGeracaoRazao)
                            {
                                mundo.numeroGeracoes = numeroGeracao;

                                // c: Taxa de Crossover.
                                for (taxaCrossover = taxaCrossoverInicio; taxaCrossover <= taxaCrossoverFim; taxaCrossover += taxaCrossoverRazao)
                                {
                                    mundo.taxaCrossover = taxaCrossover;

                                    // d: Taxa de Mutação.
                                    for (taxaMutacao = taxaMutacaoInicio; taxaMutacao <= taxaMutacaoFim; taxaMutacao += taxaMutacaoRazao)
                                    {
                                        mundo.taxaMutacao = taxaMutacao;

                                        // e: Limiar.
                                        for (limiar = limiarInicio; limiar <= limiarFim; limiar += limiarRazao)
                                        {
                                            mundo.limiar = limiar;

                                            //Colocar cor nas bordas.
                                            //axPlanilhaResultado.Cells.get_Range(axPlanilhaResultado.Cells[linhaPlanilha, 1], axPlanilhaResultado.Cells[linhaPlanilha + numeroTesteTotal + 3, 8]).Borders.set_Color(ref black);

                                            // Formatar os dados na planilha.
                                            //axPlanilhaResultado.Cells[linhaPlanilha, 2] = "Tam. Pop.";
                                            //axPlanilhaResultado.Cells[linhaPlanilha, 3] = "Núm. Ger.";
                                            //axPlanilhaResultado.Cells[linhaPlanilha, 4] = "Tax. Cro.";
                                            //axPlanilhaResultado.Cells[linhaPlanilha, 5] = "Tax. Mut.";
                                            //axPlanilhaResultado.Cells[linhaPlanilha, 6] = "Limiar";
                                            //axPlanilhaResultado.Cells[linhaPlanilha, 7] = "Seleção";
                                            //axPlanilhaResultado.Cells[linhaPlanilha, 8] = "Evolução";
                                            //axPlanilhaResultado.Cells.get_Range(axPlanilhaResultado.Cells[linhaPlanilha, 2], axPlanilhaResultado.Cells[linhaPlanilha, 8]).Interior.set_Color(ref seaGreen);
                                            linhaPlanilha++;

                                            //axPlanilhaResultado.Cells[linhaPlanilha, 2] = tamanhoPopulacao.ToString();
                                            //axPlanilhaResultado.Cells[linhaPlanilha, 3] = numeroGeracao.ToString();
                                            //axPlanilhaResultado.Cells[linhaPlanilha, 4] = taxaCrossover.ToString();
                                            //axPlanilhaResultado.Cells[linhaPlanilha, 5] = taxaMutacao.ToString();
                                            //axPlanilhaResultado.Cells[linhaPlanilha, 6] = limiar.ToString();
                                            //axPlanilhaResultado.Cells[linhaPlanilha, 7] = ((selecao == 0) ? "Estocástico" : ((selecao == 1) ? "Torneio Simples" : "Roleta"));
                                            //axPlanilhaResultado.Cells[linhaPlanilha, 8] = ((evolucao == 0) ? "Elitista" : "Melhores");
                                            linhaPlanilha++;

                                            //axPlanilhaResultado.Cells[linhaPlanilha, 2] = "Apt. Tre.";
                                            //axPlanilhaResultado.Cells[linhaPlanilha, 3] = "Se Tre.";
                                            //axPlanilhaResultado.Cells[linhaPlanilha, 4] = "Sp Tre.";
                                            //axPlanilhaResultado.Cells[linhaPlanilha, 5] = "Apt. Tes.";
                                            //axPlanilhaResultado.Cells[linhaPlanilha, 6] = "Se Tes.";
                                            //axPlanilhaResultado.Cells[linhaPlanilha, 7] = "Sp Tes.";
                                            //axPlanilhaResultado.Cells[linhaPlanilha, 8] = "Regra";
                                            //axPlanilhaResultado.Cells.get_Range(axPlanilhaResultado.Cells[linhaPlanilha, 2], axPlanilhaResultado.Cells[linhaPlanilha, 8]).Interior.set_Color(ref seaGreen);
                                            linhaPlanilha++;

                                            Console.WriteLine("Classe: " + classe.ToString() + " Evolução: " + evolucao.ToString() + " Seleção: " + selecao.ToString() + " Tamanho População: " + tamanhoPopulacao.ToString() + " Número de Gerações: " + numeroGeracao.ToString() + " Taxa de Crossover: " + taxaCrossover.ToString() + " Taxa de Mutação: " + taxaMutacao.ToString() + " Limiar: " + limiar.ToString());

                                            somaApTr = somaSeTr = somaSpTr = somaApTe = somaSeTe = somaSpTe = 0.0f;

                                            // 5 - Número de Testes.
                                            for (numeroTeste = 1; numeroTeste <= numeroTesteTotal; numeroTeste++)
                                            {
                                                melhorIndividuo = mundo.AGDataMining(baseDados);

                                                if (null != melhorIndividuo)
                                                {
                                                    //axPlanilhaResultado.Cells.get_Range(axPlanilhaResultado.Cells[linhaPlanilha, 1], axPlanilhaResultado.Cells[linhaPlanilha, 1]).Interior.set_Color(ref seaGreen);
                                                    //axPlanilhaResultado.Cells[linhaPlanilha, 1] = "Teste " + numeroTeste.ToString();
                                                    
                                                    //axPlanilhaResultado.Cells[linhaPlanilha, 2] = melhorIndividuo.aptidao.ToString();
                                                    somaApTr += melhorIndividuo.aptidao;

                                                    //axPlanilhaResultado.Cells[linhaPlanilha, 3] = melhorIndividuo.sensitividade.ToString();
                                                    somaSeTr += melhorIndividuo.sensitividade;

                                                    //axPlanilhaResultado.Cells[linhaPlanilha, 4] = melhorIndividuo.especificidade.ToString();
                                                    somaSpTr += melhorIndividuo.especificidade;

                                                    mundo.TestaMelhorSolucao(baseDados, melhorIndividuo);

                                                    //axPlanilhaResultado.Cells[linhaPlanilha, 5] = melhorIndividuo.aptidao.ToString();
                                                    somaApTe += melhorIndividuo.aptidao;

                                                    //axPlanilhaResultado.Cells[linhaPlanilha, 6] = melhorIndividuo.sensitividade.ToString();
                                                    somaSeTe += melhorIndividuo.sensitividade;

                                                    //axPlanilhaResultado.Cells[linhaPlanilha, 7] = melhorIndividuo.especificidade.ToString();
                                                    somaSpTe += melhorIndividuo.especificidade;
                                                    
                                                    numeroRegra++;
                                                    //axPlanilhaResultado.Cells[linhaPlanilha, 8] = numeroRegra.ToString();
                                                    linhaPlanilha++;

                                                    txtBuscaResultado.Text += "Regra: " + numeroRegra.ToString() + "\r\n";

                                                    for (i = 0; i < melhorIndividuo.cromossomo.Count; i++)
                                                    {
                                                        if (melhorIndividuo.cromossomo[i].Weight >= Convert.ToInt32(txtLimiar.Text))
                                                        {
                                                            string concat;

                                                            if (0 == melhorIndividuo.cromossomo[i].Operator)
                                                            {
                                                                concat = "=";
                                                            }
                                                            else if (1 == melhorIndividuo.cromossomo[i].Operator)
                                                            {
                                                                concat = "!=";
                                                            }
                                                            else if (2 == melhorIndividuo.cromossomo[i].Operator)
                                                            {
                                                                concat = ">=";
                                                            }
                                                            else
                                                            {
                                                                concat = "<";
                                                            }

                                                            txtBuscaResultado.Text += "IF " + lbxAtributo.Items[i].ToString() + " " + concat + " " + melhorIndividuo.cromossomo[i].Value + "\r\n";
                                                        }
                                                    }

                                                    txtBuscaResultado.Text += "\r\n";
                                                }
                                                else
                                                {
                                                    return;
                                                }
                                            }

                                            //axPlanilhaResultado.Cells[linhaPlanilha, 1] = "Média";
                                            //axPlanilhaResultado.Cells[linhaPlanilha, 2] = Convert.ToString(somaApTr / (float)(numeroTeste - 1));
                                            //axPlanilhaResultado.Cells[linhaPlanilha, 3] = Convert.ToString(somaSeTr / (float)(numeroTeste - 1));
                                            //axPlanilhaResultado.Cells[linhaPlanilha, 4] = Convert.ToString(somaSpTr / (float)(numeroTeste - 1));
                                            //axPlanilhaResultado.Cells[linhaPlanilha, 5] = Convert.ToString(somaApTe / (float)(numeroTeste - 1));
                                            //axPlanilhaResultado.Cells[linhaPlanilha, 6] = Convert.ToString(somaSeTe / (float)(numeroTeste - 1));
                                            //axPlanilhaResultado.Cells[linhaPlanilha, 7] = Convert.ToString(somaSpTe / (float)(numeroTeste - 1));

                                            //axPlanilhaResultado.Cells.get_Range(axPlanilhaResultado.Cells[linhaPlanilha, 1], axPlanilhaResultado.Cells[linhaPlanilha, 8]).Interior.set_Color(ref blue);
                                            linhaPlanilha += 2;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    }
                }
                }
            }
            }
        }
    }
}