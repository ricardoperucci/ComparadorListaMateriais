using ComparadorListasDeMateriais.ComparadorCAM;
using ComparadorListasDeMateriais.ObjetosResultados;
using ComparadorListasDeMateriais.UI;
using ComparadorListasDeMateriais.Utils;
using ModelagemTorre;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        // ---- Cabeçalho (tema Hefesto) ----
        private Panel panelCabecalho;

        public Form1()
        {
            InitializeComponent();

            labelDescricaoPrograma.Text =
                "Comparar listas em formato xls/xlsx (Excel) ou listas em txt." + Environment.NewLine +
                "Não é necessário que as duas listas estejam no mesmo formato." + Environment.NewLine + Environment.NewLine +
                "Para comparar arquivos CAM, use a aba \"Comparar Arquivos CAM\".";

            PreencherCombosCAM();
            AplicarTema();
        }

        /// <summary>
        /// Aplica o tema escuro do Hefesto a toda a tela (cabeçalho com sublinhado accent, abas escuras,
        /// botões primários/secundários, campos e rótulos).
        /// </summary>
        private void AplicarTema()
        {
            TemaHefesto.AplicarNoFormulario(this);
            this.ClientSize = new Size(910, 552);

            MontarCabecalho();

            // Abas escuras (owner-draw), reposicionadas abaixo do cabeçalho.
            TemaHefesto.EstilizarTabControl(tabControlPrincipal);
            tabControlPrincipal.Location = new Point(12, panelCabecalho.Height + 8);
            tabControlPrincipal.Size = new Size(886, this.ClientSize.Height - tabControlPrincipal.Top - 10);

            // ---- Aba Comparar Listas ----
            Descricao(labelDescricaoPrograma);
            labelDescricaoPrograma.Location = new Point(20, 16);
            TemaHefesto.BotaoSecundario(buttonArquivo1);
            TemaHefesto.BotaoSecundario(buttonArquivo2);
            TemaHefesto.BotaoSecundario(buttonSelecionaPasta);
            TemaHefesto.BotaoPrimario(buttonComparar);
            TemaHefesto.Terciario(labelArquivo1);
            TemaHefesto.Terciario(labelArquivo2);
            TemaHefesto.Terciario(labelPastaResultado);
            TemaHefesto.Rotulo(labelTextResult);
            TemaHefesto.Campo(textBoxNomeResult);
            TemaHefesto.CheckTema(checkBoxTxt);
            TemaHefesto.CheckTema(checkBoxExcel);

            // ---- Aba Comparar Arquivos CAM ----
            Descricao(labelCAMInstrucoes);
            TemaHefesto.BotaoSecundario(buttonPastaCamA);
            TemaHefesto.BotaoSecundario(buttonPastaCamB);
            TemaHefesto.BotaoSecundario(buttonPastaResultadoCam);
            TemaHefesto.BotaoPrimario(buttonCompararCam);
            TemaHefesto.Terciario(labelPastaCamA);
            TemaHefesto.Terciario(labelPastaCamB);
            TemaHefesto.Terciario(labelPastaResultadoCam);
            TemaHefesto.Rotulo(labelFabricanteA);
            TemaHefesto.Rotulo(labelFabricanteB);
            TemaHefesto.Rotulo(labelParafusoTipo);
            TemaHefesto.Rotulo(labelNomeResultadoCam);
            TemaHefesto.Campo(comboFabricanteA);
            TemaHefesto.Campo(comboFabricanteB);
            TemaHefesto.Campo(comboParafusoTipo);
            TemaHefesto.Campo(textBoxNomeResultadoCam);
            TemaHefesto.CheckTema(checkBoxConsiderarDiametro);

            // ---- Aba ZIP Arquivos ----
            TemaHefesto.BotaoSecundario(buttonSelecionaArquivosZIP);
            TemaHefesto.BotaoPrimario(buttonCriarZips);
            TemaHefesto.Terciario(labelArquivosSelecionadosZIP);
            TemaHefesto.CheckTema(checkBoxZip);
        }

        private static void Descricao(Label l)
        {
            l.ForeColor = TemaHefesto.TextoSecundario;
            l.Font = TemaHefesto.FontePequena;
            l.BackColor = Color.Transparent;
        }

        /// <summary>Monta a barra de cabeçalho (logo + título), com o sublinhado accent do tema.</summary>
        private void MontarCabecalho()
        {
            panelCabecalho = new Panel
            {
                Dock = DockStyle.Top,
                Height = 64,
                BackColor = TemaHefesto.FundoCabecalho
            };
            panelCabecalho.Paint += (s, e) =>
            {
                using (var caneta = new Pen(TemaHefesto.Accent, 2))
                    e.Graphics.DrawLine(caneta, 0, panelCabecalho.Height - 1, panelCabecalho.Width, panelCabecalho.Height - 1);
            };

            // Reaproveita o logo existente, movendo-o do tab1 para o cabeçalho.
            pictureBox1.Parent = panelCabecalho;
            pictureBox1.BackColor = TemaHefesto.FundoCabecalho;
            pictureBox1.Location = new Point(16, 9);
            pictureBox1.Size = new Size(50, 46);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            var titulo = new Label { Text = "Comparador de Listas de Materiais", AutoSize = true, Location = new Point(80, 12) };
            TemaHefesto.Titulo(titulo);

            var subtitulo = new Label { Text = "Engetower · Hefesto", AutoSize = true, Location = new Point(82, 40) };
            TemaHefesto.Subtitulo(subtitulo);

            panelCabecalho.Controls.Add(titulo);
            panelCabecalho.Controls.Add(subtitulo);
            this.Controls.Add(panelCabecalho);
            panelCabecalho.BringToFront();
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
