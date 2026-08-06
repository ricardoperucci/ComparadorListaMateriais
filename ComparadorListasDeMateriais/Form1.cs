using ComparadorListasDeMateriais.ComparadorCAM;
using ComparadorListasDeMateriais.ComparadorV2;
using ComparadorListasDeMateriais.DiffCamTextual;
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

            // Abas extras (montadas em código), antes de estilizar para entrarem no owner-draw.
            MontarAbaComparadorV2();
            MontarAbaDiffCam();
            MontarAbaRenomear();

            // Abas (owner-draw), reposicionadas abaixo do cabeçalho.
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

        // =====================================================================
        //  Aba: Comparar Listas v2
        // =====================================================================

        private string caminhoV2Original;
        private string caminhoV2Nova;
        private string caminhoV2Saida;
        private Label labelV2Original;
        private Label labelV2Nova;
        private Label labelV2Pasta;
        private TextBox textBoxV2Nome;
        private CheckBox checkBoxV2Txt;
        private CheckBox checkBoxV2Html;
        private Label labelV2Status;

        /// <summary>
        /// Monta, em código, a aba do comparador de listas v2 — o que muda em relação à aba antiga
        /// é o pareamento (reconhece troca de material na mesma posição, 65H → 65G) e os relatórios.
        /// </summary>
        private void MontarAbaComparadorV2()
        {
            var pagina = new TabPage("Comparar Listas v2") { BackColor = TemaHefesto.FundoJanela };

            var instrucoes = new Label
            {
                Text = "Compara duas listas (xls/xlsx ou txt) reconhecendo troca de material na mesma posição: " +
                       "65H que virou 65G aparece como material alterado, e não como uma posição eliminada mais uma nova. " +
                       "Gera um relatório em txt e um em HTML, este último com as duas listas lado a lado.",
                Location = new Point(20, 16),
                Size = new Size(820, 46),
                AutoSize = false
            };
            Descricao(instrucoes);

            var btnOriginal = new Button { Text = "Selecionar lista ORIGINAL", Location = new Point(20, 72), Size = new Size(270, 30) };
            TemaHefesto.BotaoSecundario(btnOriginal);
            btnOriginal.Click += (s, e) =>
            {
                string caminho = SelecionaArquivo();
                if (caminho == null) return;
                caminhoV2Original = caminho;
                labelV2Original.Text = Path.GetFileName(caminho);
            };

            labelV2Original = new Label { Text = "Nenhuma lista selecionada", Location = new Point(306, 78), AutoSize = true };
            TemaHefesto.Terciario(labelV2Original);

            var btnNova = new Button { Text = "Selecionar lista NOVA", Location = new Point(20, 112), Size = new Size(270, 30) };
            TemaHefesto.BotaoSecundario(btnNova);
            btnNova.Click += (s, e) =>
            {
                string caminho = SelecionaArquivo();
                if (caminho == null) return;
                caminhoV2Nova = caminho;
                labelV2Nova.Text = Path.GetFileName(caminho);
            };

            labelV2Nova = new Label { Text = "Nenhuma lista selecionada", Location = new Point(306, 118), AutoSize = true };
            TemaHefesto.Terciario(labelV2Nova);

            var btnPasta = new Button { Text = "Selecionar pasta do resultado", Location = new Point(20, 152), Size = new Size(270, 30) };
            TemaHefesto.BotaoSecundario(btnPasta);
            btnPasta.Click += (s, e) =>
            {
                string caminho = SelecionaPasta();
                if (caminho == null) return;
                caminhoV2Saida = caminho;
                labelV2Pasta.Text = NomePasta(caminho);
            };

            labelV2Pasta = new Label { Text = "Nenhuma pasta selecionada", Location = new Point(306, 158), AutoSize = true };
            TemaHefesto.Terciario(labelV2Pasta);

            var lblNome = new Label { Text = "Nome do arquivo:", Location = new Point(20, 204), AutoSize = true };
            TemaHefesto.Rotulo(lblNome);

            textBoxV2Nome = new TextBox { Text = "Comparacao", Location = new Point(160, 201), Size = new Size(240, 24) };
            TemaHefesto.Campo(textBoxV2Nome);

            checkBoxV2Txt = new CheckBox { Text = "txt", Checked = true, Location = new Point(420, 202), AutoSize = true };
            TemaHefesto.CheckTema(checkBoxV2Txt);

            checkBoxV2Html = new CheckBox { Text = "HTML", Checked = true, Location = new Point(490, 202), AutoSize = true };
            TemaHefesto.CheckTema(checkBoxV2Html);

            var btnComparar = new Button { Text = "Comparar", Location = new Point(20, 244), Size = new Size(270, 44) };
            TemaHefesto.BotaoPrimario(btnComparar);
            btnComparar.Click += (s, e) => CompararListasV2();

            labelV2Status = new Label { Text = "", Location = new Point(20, 300), Size = new Size(820, 60), AutoSize = false };
            TemaHefesto.Terciario(labelV2Status);

            pagina.Controls.Add(instrucoes);
            pagina.Controls.Add(btnOriginal);
            pagina.Controls.Add(labelV2Original);
            pagina.Controls.Add(btnNova);
            pagina.Controls.Add(labelV2Nova);
            pagina.Controls.Add(btnPasta);
            pagina.Controls.Add(labelV2Pasta);
            pagina.Controls.Add(lblNome);
            pagina.Controls.Add(textBoxV2Nome);
            pagina.Controls.Add(checkBoxV2Txt);
            pagina.Controls.Add(checkBoxV2Html);
            pagina.Controls.Add(btnComparar);
            pagina.Controls.Add(labelV2Status);

            // Logo após a aba antiga, para ficar fácil comparar as duas.
            tabControlPrincipal.TabPages.Insert(1, pagina);
        }

        private void CompararListasV2()
        {
            if (string.IsNullOrWhiteSpace(caminhoV2Original) || string.IsNullOrWhiteSpace(caminhoV2Nova))
            {
                MessageBox.Show("Selecione as duas listas (Original e Nova).");
                return;
            }

            if (!checkBoxV2Txt.Checked && !checkBoxV2Html.Checked)
            {
                MessageBox.Show("Marque pelo menos uma saída: txt ou HTML.");
                return;
            }

            if (string.IsNullOrWhiteSpace(caminhoV2Saida))
            {
                MessageBox.Show("Selecione a pasta para salvar o resultado.");
                return;
            }

            string nome = string.IsNullOrWhiteSpace(textBoxV2Nome.Text) ? "Comparacao" : textBoxV2Nome.Text.Trim();
            string caminhoBase = Path.Combine(caminhoV2Saida, nome);

            try
            {
                UseWaitCursor = true;

                ResultadoComparacaoV2 resultado = new ComparadorListasV2().Comparar(caminhoV2Original, caminhoV2Nova);

                var gerados = new List<string>();

                if (checkBoxV2Txt.Checked)
                    gerados.Add(ArquivosRelatorioV2.EscreverTxt(caminhoBase, resultado));

                if (checkBoxV2Html.Checked)
                {
                    TextoLista textoOriginal = RenderizadorListaParaTexto.Obter(caminhoV2Original);
                    TextoLista textoNova = RenderizadorListaParaTexto.Obter(caminhoV2Nova);

                    string html = ArquivosRelatorioV2.EscreverHtml(caminhoBase, resultado, textoOriginal, textoNova);
                    gerados.Add(html);

                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(html) { UseShellExecute = true });
                }

                ResumoComparacao resumo = resultado.Resumo();

                labelV2Status.ForeColor = TemaHefesto.Accent;
                labelV2Status.Text = $"Concluído: {resumo.PosicoesAlteradas} alteradas, {resumo.MaterialTrocado} com material trocado, " +
                                     $"{resumo.PosicoesNovas} novas, {resumo.PosicoesEliminadas} eliminadas." +
                                     Environment.NewLine + string.Join("  |  ", gerados.Select(Path.GetFileName));
            }
            catch (Exception ex)
            {
                labelV2Status.ForeColor = Color.Firebrick;
                labelV2Status.Text = "Erro ao comparar as listas.";
                MessageBox.Show($"Erro ao comparar listas: {ex.Message}");
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        // =====================================================================
        //  Aba: Diff CAM (HTML)  — comparação textual de pastas .CAM (porta do estudo CAMUSA)
        // =====================================================================

        private string caminhoDiffAntes;
        private string caminhoDiffDepois;
        private Label labelDiffPastaAntes;
        private Label labelDiffPastaDepois;
        private Label labelDiffStatus;

        /// <summary>Monta, em código, a aba de diff textual de CAM (gera relatório HTML).</summary>
        private void MontarAbaDiffCam()
        {
            var pagina = new TabPage("Diff CAM (HTML)") { BackColor = TemaHefesto.FundoJanela };

            var instrucoes = new Label
            {
                Text = "Compara duas pastas de arquivos .CAM linha a linha (textual) e gera um relatório HTML " +
                       "com as diferenças por arquivo, resumos por tag e inventários, destacando deltas > 0,8 mm. " +
                       "O relatório é salvo na pasta \"depois\" e aberto no navegador.",
                Location = new Point(20, 16),
                Size = new Size(820, 46),
                AutoSize = false
            };
            Descricao(instrucoes);

            var btnAntes = new Button { Text = "Selecionar pasta ORIGINAL (antes)", Location = new Point(20, 72), Size = new Size(270, 30) };
            TemaHefesto.BotaoSecundario(btnAntes);
            btnAntes.Click += (s, e) =>
            {
                string p = SelecionaPasta();
                if (p == null) return;
                caminhoDiffAntes = p;
                labelDiffPastaAntes.Text = NomePasta(p);
            };

            labelDiffPastaAntes = new Label { Text = "Nenhuma pasta selecionada", Location = new Point(306, 78), AutoSize = true };
            TemaHefesto.Terciario(labelDiffPastaAntes);

            var btnDepois = new Button { Text = "Selecionar pasta ALTERADA (depois)", Location = new Point(20, 112), Size = new Size(270, 30) };
            TemaHefesto.BotaoSecundario(btnDepois);
            btnDepois.Click += (s, e) =>
            {
                string p = SelecionaPasta();
                if (p == null) return;
                caminhoDiffDepois = p;
                labelDiffPastaDepois.Text = NomePasta(p);
            };

            labelDiffPastaDepois = new Label { Text = "Nenhuma pasta selecionada", Location = new Point(306, 118), AutoSize = true };
            TemaHefesto.Terciario(labelDiffPastaDepois);

            var btnGerar = new Button { Text = "Gerar relatório HTML", Location = new Point(20, 170), Size = new Size(270, 44) };
            TemaHefesto.BotaoPrimario(btnGerar);
            btnGerar.Click += (s, e) => GerarDiffCam();

            labelDiffStatus = new Label { Text = "", Location = new Point(20, 232), Size = new Size(820, 44), AutoSize = false };
            TemaHefesto.Terciario(labelDiffStatus);

            pagina.Controls.Add(instrucoes);
            pagina.Controls.Add(btnAntes);
            pagina.Controls.Add(labelDiffPastaAntes);
            pagina.Controls.Add(btnDepois);
            pagina.Controls.Add(labelDiffPastaDepois);
            pagina.Controls.Add(btnGerar);
            pagina.Controls.Add(labelDiffStatus);

            tabControlPrincipal.TabPages.Add(pagina);
        }

        private void GerarDiffCam()
        {
            if (string.IsNullOrWhiteSpace(caminhoDiffAntes) || string.IsNullOrWhiteSpace(caminhoDiffDepois))
            {
                MessageBox.Show("Selecione as pastas ORIGINAL (antes) e ALTERADA (depois).");
                return;
            }

            if (!Directory.Exists(caminhoDiffAntes) || !Directory.Exists(caminhoDiffDepois))
            {
                MessageBox.Show("Uma das pastas selecionadas não existe.");
                return;
            }

            if (string.Equals(Path.GetFullPath(caminhoDiffAntes), Path.GetFullPath(caminhoDiffDepois), StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("As duas pastas selecionadas são iguais. Selecione pastas diferentes.");
                return;
            }

            string htmlPath = Path.Combine(caminhoDiffDepois, $"relatorio-diff_{DateTime.Now:yyyyMMdd_HHmmss}.html");

            try
            {
                UseWaitCursor = true;

                ResultadoDiffCam resultado = GeradorHtmlDiffCam.Gerar(caminhoDiffAntes, caminhoDiffDepois, htmlPath);

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(htmlPath) { UseShellExecute = true });

                labelDiffStatus.ForeColor = TemaHefesto.Accent;
                labelDiffStatus.Text = $"Concluído: {resultado.Comuns} comuns, {resultado.Alterados} alterados, " +
                                       $"{resultado.SoNoAntes} só no antes, {resultado.SoNoDepois} só no depois. Relatório aberto.";
            }
            catch (Exception ex)
            {
                labelDiffStatus.ForeColor = Color.Firebrick;
                labelDiffStatus.Text = "Erro ao gerar o relatório.";
                MessageBox.Show($"Ocorreu um erro ao gerar o diff: {ex.Message}");
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        // =====================================================================
        //  Aba: Renomear Arquivos  — troca em lote de um trecho do nome dos arquivos
        // =====================================================================

        private List<string> arquivosRenomear;
        private Label labelArquivosRenomear;
        private TextBox textBoxRenProcurar;
        private TextBox textBoxRenSubstituir;
        private Label labelRenStatus;

        /// <summary>
        /// Monta, em código, a aba que renomeia vários arquivos trocando um trecho do nome por outro
        /// (ex.: procurar "-0B" e substituir por ".0B" transforma "AAAAA-0B.pdf" em "AAAAA.0B.pdf").
        /// </summary>
        private void MontarAbaRenomear()
        {
            var pagina = new TabPage("Renomear Arquivos") { BackColor = TemaHefesto.FundoJanela };

            var instrucoes = new Label
            {
                Text = "Renomeia em lote os arquivos selecionados, trocando um trecho do nome por outro. " +
                       "Selecione os arquivos, informe o texto a procurar e o texto para substituir e clique em Executar. " +
                       "Ex.: procurar \"-0B\" e substituir por \".0B\" transforma \"AAAAA-0B.pdf\" em \"AAAAA.0B.pdf\".",
                Location = new Point(20, 16),
                Size = new Size(820, 46),
                AutoSize = false
            };
            Descricao(instrucoes);

            var btnSelecionar = new Button { Text = "Selecionar arquivos", Location = new Point(20, 72), Size = new Size(270, 30) };
            TemaHefesto.BotaoSecundario(btnSelecionar);
            btnSelecionar.Click += (s, e) =>
            {
                List<string> selecionados = SelecionaArquivos();
                if (selecionados == null) return;
                arquivosRenomear = selecionados;
                labelArquivosRenomear.Text = $"{arquivosRenomear.Count} arquivo(s) selecionado(s)";
            };

            labelArquivosRenomear = new Label { Text = "Nenhum arquivo selecionado", Location = new Point(306, 78), AutoSize = true };
            TemaHefesto.Terciario(labelArquivosRenomear);

            var lblProcurar = new Label { Text = "Procurar no nome:", Location = new Point(20, 124), AutoSize = true };
            TemaHefesto.Rotulo(lblProcurar);
            textBoxRenProcurar = new TextBox { Location = new Point(160, 121), Size = new Size(240, 24) };
            TemaHefesto.Campo(textBoxRenProcurar);

            var lblSubstituir = new Label { Text = "Substituir por:", Location = new Point(20, 159), AutoSize = true };
            TemaHefesto.Rotulo(lblSubstituir);
            textBoxRenSubstituir = new TextBox { Location = new Point(160, 156), Size = new Size(240, 24) };
            TemaHefesto.Campo(textBoxRenSubstituir);

            var btnExecutar = new Button { Text = "Executar", Location = new Point(20, 200), Size = new Size(270, 44) };
            TemaHefesto.BotaoPrimario(btnExecutar);
            btnExecutar.Click += (s, e) => ExecutarRenomear();

            labelRenStatus = new Label { Text = "", Location = new Point(20, 260), Size = new Size(820, 60), AutoSize = false };
            TemaHefesto.Terciario(labelRenStatus);

            pagina.Controls.Add(instrucoes);
            pagina.Controls.Add(btnSelecionar);
            pagina.Controls.Add(labelArquivosRenomear);
            pagina.Controls.Add(lblProcurar);
            pagina.Controls.Add(textBoxRenProcurar);
            pagina.Controls.Add(lblSubstituir);
            pagina.Controls.Add(textBoxRenSubstituir);
            pagina.Controls.Add(btnExecutar);
            pagina.Controls.Add(labelRenStatus);

            tabControlPrincipal.TabPages.Add(pagina);
        }

        private void ExecutarRenomear()
        {
            if (arquivosRenomear == null || arquivosRenomear.Count == 0)
            {
                MessageBox.Show("Selecione os arquivos que deseja renomear.");
                return;
            }

            string procurar = textBoxRenProcurar.Text;
            string substituir = textBoxRenSubstituir.Text ?? string.Empty;

            if (string.IsNullOrEmpty(procurar))
            {
                MessageBox.Show("Informe o texto a procurar no nome dos arquivos.");
                return;
            }

            int renomeados = 0, semAlteracao = 0, erros = 0;
            var detalhesErro = new List<string>();
            var novosCaminhos = new List<string>();

            foreach (string caminho in arquivosRenomear)
            {
                string nomeAtual = Path.GetFileName(caminho);
                try
                {
                    if (!File.Exists(caminho))
                    {
                        erros++;
                        detalhesErro.Add($"{nomeAtual}: arquivo não encontrado");
                        continue;
                    }

                    if (!nomeAtual.Contains(procurar))
                    {
                        semAlteracao++;
                        novosCaminhos.Add(caminho);
                        continue;
                    }

                    string nomeNovo = nomeAtual.Replace(procurar, substituir);

                    if (string.IsNullOrWhiteSpace(nomeNovo))
                    {
                        erros++;
                        detalhesErro.Add($"{nomeAtual}: nome resultante vazio");
                        continue;
                    }

                    string pasta = Path.GetDirectoryName(caminho);
                    string caminhoNovo = Path.Combine(pasta, nomeNovo);

                    if (string.Equals(caminhoNovo, caminho, StringComparison.OrdinalIgnoreCase))
                    {
                        semAlteracao++;
                        novosCaminhos.Add(caminho);
                        continue;
                    }

                    if (File.Exists(caminhoNovo))
                    {
                        erros++;
                        detalhesErro.Add($"{nomeAtual} -> {nomeNovo}: já existe um arquivo com esse nome");
                        continue;
                    }

                    File.Move(caminho, caminhoNovo);
                    renomeados++;
                    novosCaminhos.Add(caminhoNovo);
                }
                catch (Exception ex)
                {
                    erros++;
                    detalhesErro.Add($"{nomeAtual}: {ex.Message}");
                }
            }

            // Atualiza a seleção para os novos caminhos (permite executar de novo sem reselecionar).
            arquivosRenomear = novosCaminhos;

            labelRenStatus.ForeColor = erros > 0 ? Color.Firebrick : TemaHefesto.Accent;
            labelRenStatus.Text = $"Concluído: {renomeados} renomeado(s), {semAlteracao} sem alteração, {erros} com erro.";

            string msg = $"{renomeados} arquivo(s) renomeado(s).";
            if (semAlteracao > 0) msg += $"\n{semAlteracao} sem o texto procurado (inalterado).";
            if (erros > 0) msg += $"\n{erros} com erro:\n" + string.Join("\n", detalhesErro.Take(10));

            MessageBox.Show(msg);
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

        /// <summary>Abre o diálogo permitindo selecionar vários arquivos; devolve null se cancelado.</summary>
        private List<string> SelecionaArquivos()
        {
            using (var openFileDialog = new OpenFileDialog { Multiselect = true })
            {
                return openFileDialog.ShowDialog() == DialogResult.OK
                    ? new List<string>(openFileDialog.FileNames)
                    : null;
            }
        }

        /// <summary>
        /// Abre o seletor de pasta; devolve null se cancelado.
        /// <para>
        /// Delega para <see cref="SeletorPasta"/>, que usa o diálogo moderno do Explorer — o mesmo
        /// visual do <see cref="OpenFileDialog"/> usado para escolher as listas. Antes aqui havia um
        /// <c>FolderBrowserDialog</c> direto, que destoava.
        /// </para>
        /// </summary>
        private string SelecionaPasta()
        {
            return SeletorPasta.Selecionar(this, "Selecionar pasta");
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
