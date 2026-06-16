using ComparadorListasDeMateriais.ComparadorCAM;
using ComparadorListasDeMateriais.ObjetosResultados;
using ComparadorListasDeMateriais.Utils;
using ModelagemTorre;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ComparadorListasDeMateriais
{
    public partial class Form1 : Form
    {
        // ---- Estado da aba "Comparar Listas" ----
        private string caminhoArquivo1;
        private string caminhoArquivo2;
        private string caminhoSalvarResultado;
        private string nomeArquivoResultado;
        private bool salvarTxt = false;
        private bool salvarExcel = false;

        // ---- Estado da aba "Comparar Arquivos CAM" ----
        private string caminhoPastaCamA;
        private string caminhoPastaCamB;
        private string caminhoSalvarResultadoCam;

        // ---- Estado da aba "ZIP Arquivos" ----
        private List<string> ListaArquivosZipar;

        public Form1()
        {
            InitializeComponent();

            labelDescricaoPrograma.Text =
                "Comparar listas em formato xls/xlsx (Excel) ou listas em txt." + Environment.NewLine +
                "Não é necessário que as duas listas estejam no mesmo formato." + Environment.NewLine + Environment.NewLine +
                "Para comparar arquivos CAM, use a aba \"Comparar Arquivos CAM\".";

            PreencherCombosCAM();
        }

        /// <summary>
        /// Preenche os combos da aba CAM a partir dos enums da HefestoCAM/ModelagemTorre.
        /// </summary>
        private void PreencherCombosCAM()
        {
            foreach (FabricanteEnum fabricante in Enum.GetValues(typeof(FabricanteEnum)))
            {
                comboFabricanteA.Items.Add(fabricante);
                comboFabricanteB.Items.Add(fabricante);
            }

            foreach (ParafusoTipoEnum parafuso in Enum.GetValues(typeof(ParafusoTipoEnum)))
            {
                comboParafusoTipo.Items.Add(parafuso);
            }

            // Padrões anteriores do código (que eram fixos): CAW / CAW / A394CM.
            comboFabricanteA.SelectedItem = FabricanteEnum.CAW;
            comboFabricanteB.SelectedItem = FabricanteEnum.CAW;
            comboParafusoTipo.SelectedItem = ParafusoTipoEnum.A394CM;
        }

        // =====================================================================
        //  Aba: Comparar Listas
        // =====================================================================

        private void buttonArquivo1_Click(object sender, EventArgs e)
        {
            string selecionado = SelecionaArquivo();
            if (selecionado == null) return;

            caminhoArquivo1 = selecionado;
            labelArquivo1.Text = Path.GetFileNameWithoutExtension(caminhoArquivo1);
        }

        private void buttonArquivo2_Click(object sender, EventArgs e)
        {
            string selecionado = SelecionaArquivo();
            if (selecionado == null) return;

            caminhoArquivo2 = selecionado;
            labelArquivo2.Text = Path.GetFileNameWithoutExtension(caminhoArquivo2);
        }

        private void buttonSelecionaPasta_Click(object sender, EventArgs e)
        {
            caminhoSalvarResultado = SelecionaPasta();
            labelPastaResultado.Text = NomePasta(caminhoSalvarResultado);
        }

        private void textBoxNomeResult_TextChanged(object sender, EventArgs e)
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

        private void buttonComparar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(caminhoArquivo1) || string.IsNullOrWhiteSpace(caminhoArquivo2))
            {
                MessageBox.Show("Selecione as duas listas (Original e Nova).");
                return;
            }

            if ((salvarTxt || salvarExcel) && string.IsNullOrWhiteSpace(caminhoSalvarResultado))
            {
                MessageBox.Show("Selecione a pasta para salvar o resultado.");
                return;
            }

            string nome = string.IsNullOrWhiteSpace(nomeArquivoResultado) ? "Comparacao" : nomeArquivoResultado;
            string arquivoResultado = Path.Combine(caminhoSalvarResultado ?? string.Empty, nome);

            try
            {
                var comparador = new Comparador();
                comparador.CompararListas(caminhoArquivo1, caminhoArquivo2, out ObjetoResultadoComparacao resultado);

                if (salvarTxt)
                    FuncoesUteis.EscreveTxt(arquivoResultado, resultado);

                if (salvarExcel)
                    FuncoesUteis.EscreveExcel(arquivoResultado + ".xlsx", resultado);

                if (salvarTxt || salvarExcel)
                    MessageBox.Show(string.Format("Resultado salvo em {0}", arquivoResultado));
                else
                    MessageBox.Show("Comparação concluída. Marque \"txt\" e/ou \"PMV\" para salvar o resultado.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao comparar listas: {ex.Message}");
            }
        }

        // =====================================================================
        //  Aba: Comparar Arquivos CAM
        // =====================================================================

        private void buttonPastaCamA_Click(object sender, EventArgs e)
        {
            caminhoPastaCamA = SelecionaPasta();
            labelPastaCamA.Text = NomePasta(caminhoPastaCamA);
        }

        private void buttonPastaCamB_Click(object sender, EventArgs e)
        {
            caminhoPastaCamB = SelecionaPasta();
            labelPastaCamB.Text = NomePasta(caminhoPastaCamB);
        }

        private void buttonPastaResultadoCam_Click(object sender, EventArgs e)
        {
            caminhoSalvarResultadoCam = SelecionaPasta();
            labelPastaResultadoCam.Text = NomePasta(caminhoSalvarResultadoCam);
        }

        private void buttonCompararCam_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(caminhoPastaCamA) || string.IsNullOrWhiteSpace(caminhoPastaCamB))
            {
                MessageBox.Show("Selecione as pastas A e B com os arquivos CAM.");
                return;
            }

            if (string.IsNullOrWhiteSpace(caminhoSalvarResultadoCam))
            {
                MessageBox.Show("Selecione a pasta para salvar o resultado.");
                return;
            }

            if (comboFabricanteA.SelectedItem == null || comboFabricanteB.SelectedItem == null || comboParafusoTipo.SelectedItem == null)
            {
                MessageBox.Show("Selecione os fabricantes (A e B) e o tipo de parafuso.");
                return;
            }

            var opcoes = new OpcoesComparacaoCAM(
                (FabricanteEnum)comboFabricanteA.SelectedItem,
                (FabricanteEnum)comboFabricanteB.SelectedItem,
                (ParafusoTipoEnum)comboParafusoTipo.SelectedItem,
                checkBoxConsiderarDiametro.Checked);

            string nome = string.IsNullOrWhiteSpace(textBoxNomeResultadoCam.Text) ? "ComparacaoCAM" : textBoxNomeResultadoCam.Text;
            string arquivoResultado = Path.Combine(caminhoSalvarResultadoCam, nome + ".txt");

            try
            {
                string relatorio = FuncoesComparadorCAM.ComparaArquivosCAM(caminhoPastaCamA, caminhoPastaCamB, opcoes);

                File.WriteAllText(arquivoResultado, relatorio);

                MessageBox.Show(string.Format("Resultado salvo em {0}", arquivoResultado));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao comparar arquivos CAM: {ex.Message}");
            }
        }

        // =====================================================================
        //  Aba: ZIP Arquivos
        // =====================================================================

        private void buttonSelecionaArquivosZIP_Click(object sender, EventArgs e)
        {
            ListaArquivosZipar = new List<string>();
            this.openFileDialog1.Multiselect = true;

            DialogResult dr = this.openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                ListaArquivosZipar.AddRange(openFileDialog1.FileNames);
            }

            labelArquivosSelecionadosZIP.Text = $"{ListaArquivosZipar.Count} arquivos";
        }

        private void buttonCriarZips_Click(object sender, EventArgs e)
        {
            if (ListaArquivosZipar == null || ListaArquivosZipar.Count == 0)
            {
                MessageBox.Show("Selecione os arquivos para compactar.");
                return;
            }

            ResultadoZip resultado = checkBoxZip.Checked
                ? ZipadorArquivos.CriarZipUnico(ListaArquivosZipar)
                : ZipadorArquivos.CriarZipPorArquivo(ListaArquivosZipar);

            string msgFinal = checkBoxZip.Checked
                ? $"{resultado.Sucesso} arquivo(s) compactados em {resultado.ArquivosGerados.FirstOrDefault()}."
                : $"{resultado.Sucesso} arquivo(s) gerado(s).";

            if (resultado.Erros > 0)
                msgFinal += $" {resultado.Erros} com erro.";

            MessageBox.Show(msgFinal);
        }

        // =====================================================================
        //  Helpers de UI
        // =====================================================================

        private string SelecionaArquivo()
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                return openFileDialog.ShowDialog() == DialogResult.OK ? openFileDialog.FileName : null;
            }
        }

        private string SelecionaPasta()
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                return folderBrowserDialog.ShowDialog() == DialogResult.OK ? folderBrowserDialog.SelectedPath : null;
            }
        }

        /// <summary>
        /// Devolve apenas o nome da pasta (último segmento), para exibir no label.
        /// </summary>
        private static string NomePasta(string pCaminho)
        {
            if (string.IsNullOrWhiteSpace(pCaminho))
                return "Nenhuma pasta selecionada";

            try
            {
                return new DirectoryInfo(pCaminho).Name;
            }
            catch
            {
                return pCaminho;
            }
        }
    }
}
