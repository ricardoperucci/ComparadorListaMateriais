using ComparadorListasDeMateriais.ComparadorCAM;
using ComparadorListasDeMateriais.ObjetosResultados;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComparadorListasDeMateriais
{
    public partial class Form1 : Form
    {
        private string caminhoArquivo1;
        private string caminhoArquivo2;

        private string caminhoSalvarResultado;

        private string nomeArquivoResultado;

        private bool salvarTxt = false;
        private bool salvarExcel = false;

        public Form1()
        {
            InitializeComponent();
            labelDescricaoPrograma.Text = "Comparar listas em formato xls/xlsx (Excel) ou listas em txt." + "\r\n" +
                "Não é necessário que as duas listas estejam no mesmo formato." + "\r\n" + "\r\n" +
                "TEMPORÁRIO: Para comparação de arquivos CAM, selecionar um arquivo de cada pasta e " + "\r\n" +
                "escolher a opção de salvar em txt.";
        }

        private void buttonArquivo1_Click(object sender, EventArgs e)
        {
            caminhoArquivo1 = SelecionaArquivo();
            labelArquivo1.Text = Path.GetFileNameWithoutExtension(caminhoArquivo1);
        }

        private void buttonArquivo2_Click(object sender, EventArgs e)
        {

            caminhoArquivo2 = SelecionaArquivo();
            labelArquivo2.Text = Path.GetFileNameWithoutExtension(caminhoArquivo2);
        }

        private void buttonComparar_Click(object sender, EventArgs e)
        {
            string arquivoResultado = caminhoSalvarResultado + "\\" + nomeArquivoResultado;

            if (Path.GetExtension(caminhoArquivo1).Contains("CAM"))
            {
                caminhoArquivo1 = caminhoArquivo1.Replace(Path.GetFileName(caminhoArquivo1), "");
                caminhoArquivo2 = caminhoArquivo2.Replace(Path.GetFileName(caminhoArquivo2), "");

                //FALTANDO COLOCAR INPUT PRA PEGAR O FABRICANTE PRA CADA PASTA
                FuncoesComparadorCAM.ComparaArquivosCAM(
                    ModelagemTorre.FabricanteEnum.CAW,
                    ModelagemTorre.FabricanteEnum.CAW,
                    caminhoArquivo1,
                    caminhoArquivo2,
                    out StringBuilder strBuilder);

                string nome = arquivoResultado + ".txt";

                using (StreamWriter outputFile = new StreamWriter(nome, false))
                {
                    outputFile.Write(strBuilder.ToString());
                }
            }
            else
            {
                Comparador comp = new Comparador();

                comp.CompararListas(caminhoArquivo1, caminhoArquivo2, out ObjetoResultadoComparacao resultado);

                List<string> caminhos = new List<string>();

                if (salvarTxt)
                    FuncoesUteis.EscreveTxt(arquivoResultado, resultado);

                if (salvarExcel)
                    FuncoesUteis.EscreveExcel(arquivoResultado + ".xlsx", resultado);
            }


            if (salvarTxt || salvarExcel)
                MessageBox.Show(string.Format("Resultado salvo em {0}", arquivoResultado));
        }

        private string SelecionaArquivo()
        {
            var openFileDialog1 = new System.Windows.Forms.OpenFileDialog();

            //openFileDialog1.Filter = "*.xls|*.txt";

            openFileDialog1.ShowDialog();

            return openFileDialog1.FileName;
        }

        private string SelecionaPasta()
        {
            var openFileDialog1 = new System.Windows.Forms.FolderBrowserDialog();

            //openFileDialog1.Filter = "*.xls|*.txt";

            openFileDialog1.ShowDialog();

            return openFileDialog1.SelectedPath;
        }

        private void buttonSelecionaPasta_Click(object sender, EventArgs e)
        {
            caminhoSalvarResultado = SelecionaPasta();

            labelPastaResultado.Text = caminhoSalvarResultado.Split('/').Last();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            nomeArquivoResultado = textBoxNomeResult.Text;
        }

        private void checkBoxTxt_CheckedChanged(object sender, EventArgs e)
        {
            salvarTxt = checkBoxTxt.Checked;
        }

        private void checkBoxExcel_CheckedChanged(object sender, EventArgs e)
        {
            salvarExcel = checkBoxExcel.Checked;
        }

        private void labelTextResult_Click(object sender, EventArgs e)
        {

        }

        List<string> ListaArquivosZipar;

        private void buttonSelecionaArquivosZIP_Click(object sender, EventArgs e)
        {
            ListaArquivosZipar = new List<string>();
            this.openFileDialog1.Multiselect = true;

            DialogResult dr = this.openFileDialog1.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                // Read the files
                foreach (String file in openFileDialog1.FileNames)
                {
                    ListaArquivosZipar.Add(file);
                }
            }

            labelArquivosSelecionadosZIP.Text = $"{ListaArquivosZipar.Count} arquivos";
        }

        private void buttonCriarZips_Click(object sender, EventArgs e)
        {
            if(checkBoxZip.Checked)
            {
                int certos = 0;
                int errados = 0;

                var split = ListaArquivosZipar.First().Split('.');

                string caminhoNovo = "";
                for (int i = 0; i < split.Length - 1; i++)
                {
                    caminhoNovo += split[i];
                }

                caminhoNovo += ".zip";

                using (ZipArchive zip = ZipFile.Open(caminhoNovo, ZipArchiveMode.Create))
                {
                    try
                    {
                        foreach (string arquivo in ListaArquivosZipar)
                        {
                            string nome = Path.GetFileName(arquivo);

                            zip.CreateEntryFromFile(arquivo, nome);

                            certos++;
                        }
                    }
                    catch
                    {
                        errados++;
                    }
                    
                }

                string msgFinal = $"{certos} arquivo(s) compactados em {caminhoNovo}.";
                if (errados > 0)
                    msgFinal += $" {errados} com erro.";

                MessageBox.Show(msgFinal);

            }
            else
            {
                int certos = 0;
                int errados = 0;

                foreach (string arquivo in ListaArquivosZipar)
                {
                    try
                    {
                        var split = arquivo.Split('.');

                        string nome = Path.GetFileName(arquivo);

                        string caminhoNovo = "";
                        for (int i = 0; i < split.Length - 1; i++)
                        {
                            caminhoNovo += split[i] + ".";
                        }

                        caminhoNovo += "zip";
                        using (ZipArchive zip = ZipFile.Open(caminhoNovo, ZipArchiveMode.Create))
                        {
                            zip.CreateEntryFromFile(arquivo, nome);
                            certos++;
                        }
                    }
                    catch
                    {
                        errados++;
                    }
                }

                string msgFinal = $"{certos} arquivo(s) gerado(s).";
                if (errados > 0)
                    msgFinal += $" {errados} com erro.";

                MessageBox.Show(msgFinal);
            }

        }
    }
}
