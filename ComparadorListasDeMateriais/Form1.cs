﻿using ComparadorListasDeMateriais.ComparadorCAM;
using ComparadorListasDeMateriais.ObjetosResultados;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
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
            labelDescricaoPrograma.Text = "Ferramenta para comparar listas de materiais. É possível comparar listas em formato xls/xlsx (Excel) " + "\r\n"+
                "ou listas em txt. Não é necessário que as duas listas estejam no mesmo formato.";
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
            List<string> arquivosA = Directory.GetFiles(caminhoArquivo1).Where(x => x.Split(".".ToCharArray()).Last().Equals("CAM")).ToList();

            string arquivoResultado = caminhoSalvarResultado + "\\" + nomeArquivoResultado;

            if (caminhoArquivo1.Contains("CAM"))
            {
                caminhoArquivo1 = Path.GetFullPath(caminhoArquivo1);
                caminhoArquivo2 = Path.GetFullPath(caminhoArquivo2);

                FuncoesComparadorCAM.ComparaArquivosCAM(ModelagemTorre.FabricanteEnum.Brametal, caminhoArquivo1, caminhoArquivo2, out StringBuilder strBuilder);

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
    }
}
