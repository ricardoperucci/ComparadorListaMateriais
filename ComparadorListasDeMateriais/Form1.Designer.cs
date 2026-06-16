namespace ComparadorListasDeMateriais
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControlPrincipal = new System.Windows.Forms.TabControl();
            this.tabPageListas = new System.Windows.Forms.TabPage();
            this.tabPageCAM = new System.Windows.Forms.TabPage();
            this.tabPageZip = new System.Windows.Forms.TabPage();
            // Aba Listas
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelDescricaoPrograma = new System.Windows.Forms.Label();
            this.buttonArquivo1 = new System.Windows.Forms.Button();
            this.labelArquivo1 = new System.Windows.Forms.Label();
            this.buttonArquivo2 = new System.Windows.Forms.Button();
            this.labelArquivo2 = new System.Windows.Forms.Label();
            this.buttonSelecionaPasta = new System.Windows.Forms.Button();
            this.labelPastaResultado = new System.Windows.Forms.Label();
            this.textBoxNomeResult = new System.Windows.Forms.TextBox();
            this.labelTextResult = new System.Windows.Forms.Label();
            this.checkBoxTxt = new System.Windows.Forms.CheckBox();
            this.checkBoxExcel = new System.Windows.Forms.CheckBox();
            this.buttonComparar = new System.Windows.Forms.Button();
            // Aba CAM
            this.labelCAMInstrucoes = new System.Windows.Forms.Label();
            this.buttonPastaCamA = new System.Windows.Forms.Button();
            this.labelPastaCamA = new System.Windows.Forms.Label();
            this.buttonPastaCamB = new System.Windows.Forms.Button();
            this.labelPastaCamB = new System.Windows.Forms.Label();
            this.labelFabricanteA = new System.Windows.Forms.Label();
            this.comboFabricanteA = new System.Windows.Forms.ComboBox();
            this.labelFabricanteB = new System.Windows.Forms.Label();
            this.comboFabricanteB = new System.Windows.Forms.ComboBox();
            this.labelParafusoTipo = new System.Windows.Forms.Label();
            this.comboParafusoTipo = new System.Windows.Forms.ComboBox();
            this.checkBoxConsiderarDiametro = new System.Windows.Forms.CheckBox();
            this.buttonPastaResultadoCam = new System.Windows.Forms.Button();
            this.labelPastaResultadoCam = new System.Windows.Forms.Label();
            this.labelNomeResultadoCam = new System.Windows.Forms.Label();
            this.textBoxNomeResultadoCam = new System.Windows.Forms.TextBox();
            this.buttonCompararCam = new System.Windows.Forms.Button();
            // Aba ZIP
            this.buttonSelecionaArquivosZIP = new System.Windows.Forms.Button();
            this.labelArquivosSelecionadosZIP = new System.Windows.Forms.Label();
            this.buttonCriarZips = new System.Windows.Forms.Button();
            this.checkBoxZip = new System.Windows.Forms.CheckBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabControlPrincipal.SuspendLayout();
            this.tabPageListas.SuspendLayout();
            this.tabPageCAM.SuspendLayout();
            this.tabPageZip.SuspendLayout();
            this.SuspendLayout();
            //
            // tabControlPrincipal
            //
            this.tabControlPrincipal.Controls.Add(this.tabPageListas);
            this.tabControlPrincipal.Controls.Add(this.tabPageCAM);
            this.tabControlPrincipal.Controls.Add(this.tabPageZip);
            this.tabControlPrincipal.Location = new System.Drawing.Point(12, 2);
            this.tabControlPrincipal.Multiline = true;
            this.tabControlPrincipal.Name = "tabControlPrincipal";
            this.tabControlPrincipal.SelectedIndex = 0;
            this.tabControlPrincipal.Size = new System.Drawing.Size(886, 470);
            this.tabControlPrincipal.TabIndex = 0;
            //
            // tabPageListas
            //
            this.tabPageListas.Controls.Add(this.pictureBox1);
            this.tabPageListas.Controls.Add(this.labelDescricaoPrograma);
            this.tabPageListas.Controls.Add(this.buttonArquivo1);
            this.tabPageListas.Controls.Add(this.labelArquivo1);
            this.tabPageListas.Controls.Add(this.buttonArquivo2);
            this.tabPageListas.Controls.Add(this.labelArquivo2);
            this.tabPageListas.Controls.Add(this.buttonSelecionaPasta);
            this.tabPageListas.Controls.Add(this.labelPastaResultado);
            this.tabPageListas.Controls.Add(this.textBoxNomeResult);
            this.tabPageListas.Controls.Add(this.labelTextResult);
            this.tabPageListas.Controls.Add(this.checkBoxTxt);
            this.tabPageListas.Controls.Add(this.checkBoxExcel);
            this.tabPageListas.Controls.Add(this.buttonComparar);
            this.tabPageListas.Location = new System.Drawing.Point(4, 22);
            this.tabPageListas.Name = "tabPageListas";
            this.tabPageListas.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageListas.Size = new System.Drawing.Size(878, 444);
            this.tabPageListas.TabIndex = 0;
            this.tabPageListas.Text = "Comparar Listas";
            this.tabPageListas.UseVisualStyleBackColor = true;
            //
            // pictureBox1
            //
            this.pictureBox1.Image = global::ComparadorListasDeMateriais.Properties.Resources.LogoEEC;
            this.pictureBox1.Location = new System.Drawing.Point(6, 19);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(98, 84);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            //
            // labelDescricaoPrograma
            //
            this.labelDescricaoPrograma.AutoSize = true;
            this.labelDescricaoPrograma.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDescricaoPrograma.Location = new System.Drawing.Point(138, 51);
            this.labelDescricaoPrograma.Name = "labelDescricaoPrograma";
            this.labelDescricaoPrograma.Size = new System.Drawing.Size(71, 17);
            this.labelDescricaoPrograma.TabIndex = 1;
            this.labelDescricaoPrograma.Text = "Descricao";
            //
            // buttonArquivo1
            //
            this.buttonArquivo1.Location = new System.Drawing.Point(31, 188);
            this.buttonArquivo1.Name = "buttonArquivo1";
            this.buttonArquivo1.Size = new System.Drawing.Size(163, 23);
            this.buttonArquivo1.TabIndex = 2;
            this.buttonArquivo1.Text = "Selecione a Lista Original";
            this.buttonArquivo1.UseVisualStyleBackColor = true;
            this.buttonArquivo1.Click += new System.EventHandler(this.buttonArquivo1_Click);
            //
            // labelArquivo1
            //
            this.labelArquivo1.AutoSize = true;
            this.labelArquivo1.Location = new System.Drawing.Point(38, 214);
            this.labelArquivo1.Name = "labelArquivo1";
            this.labelArquivo1.Size = new System.Drawing.Size(145, 13);
            this.labelArquivo1.TabIndex = 3;
            this.labelArquivo1.Text = "Nenhum arquivo selecionado";
            //
            // buttonArquivo2
            //
            this.buttonArquivo2.Location = new System.Drawing.Point(31, 282);
            this.buttonArquivo2.Name = "buttonArquivo2";
            this.buttonArquivo2.Size = new System.Drawing.Size(163, 23);
            this.buttonArquivo2.TabIndex = 4;
            this.buttonArquivo2.Text = "Selecione a Lista Nova";
            this.buttonArquivo2.UseVisualStyleBackColor = true;
            this.buttonArquivo2.Click += new System.EventHandler(this.buttonArquivo2_Click);
            //
            // labelArquivo2
            //
            this.labelArquivo2.AutoSize = true;
            this.labelArquivo2.Location = new System.Drawing.Point(38, 305);
            this.labelArquivo2.Name = "labelArquivo2";
            this.labelArquivo2.Size = new System.Drawing.Size(145, 13);
            this.labelArquivo2.TabIndex = 5;
            this.labelArquivo2.Text = "Nenhum arquivo selecionado";
            //
            // buttonSelecionaPasta
            //
            this.buttonSelecionaPasta.Location = new System.Drawing.Point(272, 187);
            this.buttonSelecionaPasta.Name = "buttonSelecionaPasta";
            this.buttonSelecionaPasta.Size = new System.Drawing.Size(234, 23);
            this.buttonSelecionaPasta.TabIndex = 6;
            this.buttonSelecionaPasta.Text = "Selecione a pasta para salvar o resultado";
            this.buttonSelecionaPasta.UseVisualStyleBackColor = true;
            this.buttonSelecionaPasta.Click += new System.EventHandler(this.buttonSelecionaPasta_Click);
            //
            // labelPastaResultado
            //
            this.labelPastaResultado.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPastaResultado.Location = new System.Drawing.Point(324, 213);
            this.labelPastaResultado.Name = "labelPastaResultado";
            this.labelPastaResultado.Size = new System.Drawing.Size(182, 24);
            this.labelPastaResultado.TabIndex = 7;
            this.labelPastaResultado.Text = "Nenhuma pasta selecionada";
            //
            // textBoxNomeResult
            //
            this.textBoxNomeResult.Location = new System.Drawing.Point(284, 284);
            this.textBoxNomeResult.Name = "textBoxNomeResult";
            this.textBoxNomeResult.Size = new System.Drawing.Size(199, 20);
            this.textBoxNomeResult.TabIndex = 8;
            this.textBoxNomeResult.TextChanged += new System.EventHandler(this.textBoxNomeResult_TextChanged);
            //
            // labelTextResult
            //
            this.labelTextResult.AutoSize = true;
            this.labelTextResult.Location = new System.Drawing.Point(281, 305);
            this.labelTextResult.Name = "labelTextResult";
            this.labelTextResult.Size = new System.Drawing.Size(212, 13);
            this.labelTextResult.TabIndex = 9;
            this.labelTextResult.Text = "Digite um nome para o arquivo de resultado";
            //
            // checkBoxTxt
            //
            this.checkBoxTxt.AutoSize = true;
            this.checkBoxTxt.Location = new System.Drawing.Point(600, 187);
            this.checkBoxTxt.Name = "checkBoxTxt";
            this.checkBoxTxt.Size = new System.Drawing.Size(127, 17);
            this.checkBoxTxt.TabIndex = 10;
            this.checkBoxTxt.Text = "Salvar resultado em txt";
            this.checkBoxTxt.UseVisualStyleBackColor = true;
            this.checkBoxTxt.CheckedChanged += new System.EventHandler(this.checkBoxTxt_CheckedChanged);
            //
            // checkBoxExcel
            //
            this.checkBoxExcel.AutoSize = true;
            this.checkBoxExcel.Location = new System.Drawing.Point(600, 214);
            this.checkBoxExcel.Name = "checkBoxExcel";
            this.checkBoxExcel.Size = new System.Drawing.Size(132, 17);
            this.checkBoxExcel.TabIndex = 11;
            this.checkBoxExcel.Text = "Gerar Relatório de PMV";
            this.checkBoxExcel.UseVisualStyleBackColor = true;
            this.checkBoxExcel.CheckedChanged += new System.EventHandler(this.checkBoxExcel_CheckedChanged);
            //
            // buttonComparar
            //
            this.buttonComparar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonComparar.Location = new System.Drawing.Point(631, 282);
            this.buttonComparar.Name = "buttonComparar";
            this.buttonComparar.Size = new System.Drawing.Size(122, 56);
            this.buttonComparar.TabIndex = 12;
            this.buttonComparar.Text = "Comparar";
            this.buttonComparar.UseVisualStyleBackColor = true;
            this.buttonComparar.Click += new System.EventHandler(this.buttonComparar_Click);
            //
            // tabPageCAM
            //
            this.tabPageCAM.Controls.Add(this.labelCAMInstrucoes);
            this.tabPageCAM.Controls.Add(this.buttonPastaCamA);
            this.tabPageCAM.Controls.Add(this.labelPastaCamA);
            this.tabPageCAM.Controls.Add(this.buttonPastaCamB);
            this.tabPageCAM.Controls.Add(this.labelPastaCamB);
            this.tabPageCAM.Controls.Add(this.labelFabricanteA);
            this.tabPageCAM.Controls.Add(this.comboFabricanteA);
            this.tabPageCAM.Controls.Add(this.labelFabricanteB);
            this.tabPageCAM.Controls.Add(this.comboFabricanteB);
            this.tabPageCAM.Controls.Add(this.labelParafusoTipo);
            this.tabPageCAM.Controls.Add(this.comboParafusoTipo);
            this.tabPageCAM.Controls.Add(this.checkBoxConsiderarDiametro);
            this.tabPageCAM.Controls.Add(this.buttonPastaResultadoCam);
            this.tabPageCAM.Controls.Add(this.labelPastaResultadoCam);
            this.tabPageCAM.Controls.Add(this.labelNomeResultadoCam);
            this.tabPageCAM.Controls.Add(this.textBoxNomeResultadoCam);
            this.tabPageCAM.Controls.Add(this.buttonCompararCam);
            this.tabPageCAM.Location = new System.Drawing.Point(4, 22);
            this.tabPageCAM.Name = "tabPageCAM";
            this.tabPageCAM.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCAM.Size = new System.Drawing.Size(878, 444);
            this.tabPageCAM.TabIndex = 1;
            this.tabPageCAM.Text = "Comparar Arquivos CAM";
            this.tabPageCAM.UseVisualStyleBackColor = true;
            //
            // labelCAMInstrucoes
            //
            this.labelCAMInstrucoes.Location = new System.Drawing.Point(15, 15);
            this.labelCAMInstrucoes.Name = "labelCAMInstrucoes";
            this.labelCAMInstrucoes.Size = new System.Drawing.Size(760, 45);
            this.labelCAMInstrucoes.TabIndex = 0;
            this.labelCAMInstrucoes.Text = "Compara os arquivos .CAM de duas pastas (A e B). Selecione a pasta de cada fábric" +
    "a, o tipo de parafuso e se deve considerar o diâmetro dos furos. O resultado é sa" +
    "lvo em um arquivo .txt.";
            //
            // buttonPastaCamA
            //
            this.buttonPastaCamA.Location = new System.Drawing.Point(18, 70);
            this.buttonPastaCamA.Name = "buttonPastaCamA";
            this.buttonPastaCamA.Size = new System.Drawing.Size(200, 28);
            this.buttonPastaCamA.TabIndex = 1;
            this.buttonPastaCamA.Text = "Selecionar pasta CAM (A)";
            this.buttonPastaCamA.UseVisualStyleBackColor = true;
            this.buttonPastaCamA.Click += new System.EventHandler(this.buttonPastaCamA_Click);
            //
            // labelPastaCamA
            //
            this.labelPastaCamA.AutoSize = true;
            this.labelPastaCamA.Location = new System.Drawing.Point(230, 78);
            this.labelPastaCamA.Name = "labelPastaCamA";
            this.labelPastaCamA.Size = new System.Drawing.Size(145, 13);
            this.labelPastaCamA.TabIndex = 2;
            this.labelPastaCamA.Text = "Nenhuma pasta selecionada";
            //
            // buttonPastaCamB
            //
            this.buttonPastaCamB.Location = new System.Drawing.Point(18, 110);
            this.buttonPastaCamB.Name = "buttonPastaCamB";
            this.buttonPastaCamB.Size = new System.Drawing.Size(200, 28);
            this.buttonPastaCamB.TabIndex = 3;
            this.buttonPastaCamB.Text = "Selecionar pasta CAM (B)";
            this.buttonPastaCamB.UseVisualStyleBackColor = true;
            this.buttonPastaCamB.Click += new System.EventHandler(this.buttonPastaCamB_Click);
            //
            // labelPastaCamB
            //
            this.labelPastaCamB.AutoSize = true;
            this.labelPastaCamB.Location = new System.Drawing.Point(230, 118);
            this.labelPastaCamB.Name = "labelPastaCamB";
            this.labelPastaCamB.Size = new System.Drawing.Size(145, 13);
            this.labelPastaCamB.TabIndex = 4;
            this.labelPastaCamB.Text = "Nenhuma pasta selecionada";
            //
            // labelFabricanteA
            //
            this.labelFabricanteA.AutoSize = true;
            this.labelFabricanteA.Location = new System.Drawing.Point(15, 165);
            this.labelFabricanteA.Name = "labelFabricanteA";
            this.labelFabricanteA.Size = new System.Drawing.Size(76, 13);
            this.labelFabricanteA.TabIndex = 5;
            this.labelFabricanteA.Text = "Fabricante (A):";
            //
            // comboFabricanteA
            //
            this.comboFabricanteA.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboFabricanteA.FormattingEnabled = true;
            this.comboFabricanteA.Location = new System.Drawing.Point(120, 162);
            this.comboFabricanteA.Name = "comboFabricanteA";
            this.comboFabricanteA.Size = new System.Drawing.Size(150, 21);
            this.comboFabricanteA.TabIndex = 6;
            //
            // labelFabricanteB
            //
            this.labelFabricanteB.AutoSize = true;
            this.labelFabricanteB.Location = new System.Drawing.Point(300, 165);
            this.labelFabricanteB.Name = "labelFabricanteB";
            this.labelFabricanteB.Size = new System.Drawing.Size(76, 13);
            this.labelFabricanteB.TabIndex = 7;
            this.labelFabricanteB.Text = "Fabricante (B):";
            //
            // comboFabricanteB
            //
            this.comboFabricanteB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboFabricanteB.FormattingEnabled = true;
            this.comboFabricanteB.Location = new System.Drawing.Point(405, 162);
            this.comboFabricanteB.Name = "comboFabricanteB";
            this.comboFabricanteB.Size = new System.Drawing.Size(150, 21);
            this.comboFabricanteB.TabIndex = 8;
            //
            // labelParafusoTipo
            //
            this.labelParafusoTipo.AutoSize = true;
            this.labelParafusoTipo.Location = new System.Drawing.Point(15, 205);
            this.labelParafusoTipo.Name = "labelParafusoTipo";
            this.labelParafusoTipo.Size = new System.Drawing.Size(91, 13);
            this.labelParafusoTipo.TabIndex = 9;
            this.labelParafusoTipo.Text = "Tipo de parafuso:";
            //
            // comboParafusoTipo
            //
            this.comboParafusoTipo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboParafusoTipo.FormattingEnabled = true;
            this.comboParafusoTipo.Location = new System.Drawing.Point(120, 202);
            this.comboParafusoTipo.Name = "comboParafusoTipo";
            this.comboParafusoTipo.Size = new System.Drawing.Size(150, 21);
            this.comboParafusoTipo.TabIndex = 10;
            //
            // checkBoxConsiderarDiametro
            //
            this.checkBoxConsiderarDiametro.AutoSize = true;
            this.checkBoxConsiderarDiametro.Location = new System.Drawing.Point(303, 204);
            this.checkBoxConsiderarDiametro.Name = "checkBoxConsiderarDiametro";
            this.checkBoxConsiderarDiametro.Size = new System.Drawing.Size(217, 17);
            this.checkBoxConsiderarDiametro.TabIndex = 11;
            this.checkBoxConsiderarDiametro.Text = "Considerar diâmetro dos furos na comparação";
            this.checkBoxConsiderarDiametro.UseVisualStyleBackColor = true;
            //
            // buttonPastaResultadoCam
            //
            this.buttonPastaResultadoCam.Location = new System.Drawing.Point(18, 255);
            this.buttonPastaResultadoCam.Name = "buttonPastaResultadoCam";
            this.buttonPastaResultadoCam.Size = new System.Drawing.Size(252, 28);
            this.buttonPastaResultadoCam.TabIndex = 12;
            this.buttonPastaResultadoCam.Text = "Selecione a pasta para salvar o resultado";
            this.buttonPastaResultadoCam.UseVisualStyleBackColor = true;
            this.buttonPastaResultadoCam.Click += new System.EventHandler(this.buttonPastaResultadoCam_Click);
            //
            // labelPastaResultadoCam
            //
            this.labelPastaResultadoCam.AutoSize = true;
            this.labelPastaResultadoCam.Location = new System.Drawing.Point(285, 262);
            this.labelPastaResultadoCam.Name = "labelPastaResultadoCam";
            this.labelPastaResultadoCam.Size = new System.Drawing.Size(145, 13);
            this.labelPastaResultadoCam.TabIndex = 13;
            this.labelPastaResultadoCam.Text = "Nenhuma pasta selecionada";
            //
            // labelNomeResultadoCam
            //
            this.labelNomeResultadoCam.AutoSize = true;
            this.labelNomeResultadoCam.Location = new System.Drawing.Point(15, 300);
            this.labelNomeResultadoCam.Name = "labelNomeResultadoCam";
            this.labelNomeResultadoCam.Size = new System.Drawing.Size(160, 13);
            this.labelNomeResultadoCam.TabIndex = 14;
            this.labelNomeResultadoCam.Text = "Nome do arquivo de resultado:";
            //
            // textBoxNomeResultadoCam
            //
            this.textBoxNomeResultadoCam.Location = new System.Drawing.Point(181, 297);
            this.textBoxNomeResultadoCam.Name = "textBoxNomeResultadoCam";
            this.textBoxNomeResultadoCam.Size = new System.Drawing.Size(200, 20);
            this.textBoxNomeResultadoCam.TabIndex = 15;
            //
            // buttonCompararCam
            //
            this.buttonCompararCam.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCompararCam.Location = new System.Drawing.Point(600, 250);
            this.buttonCompararCam.Name = "buttonCompararCam";
            this.buttonCompararCam.Size = new System.Drawing.Size(150, 56);
            this.buttonCompararCam.TabIndex = 16;
            this.buttonCompararCam.Text = "Comparar CAM";
            this.buttonCompararCam.UseVisualStyleBackColor = true;
            this.buttonCompararCam.Click += new System.EventHandler(this.buttonCompararCam_Click);
            //
            // tabPageZip
            //
            this.tabPageZip.Controls.Add(this.checkBoxZip);
            this.tabPageZip.Controls.Add(this.buttonCriarZips);
            this.tabPageZip.Controls.Add(this.buttonSelecionaArquivosZIP);
            this.tabPageZip.Controls.Add(this.labelArquivosSelecionadosZIP);
            this.tabPageZip.Location = new System.Drawing.Point(4, 22);
            this.tabPageZip.Name = "tabPageZip";
            this.tabPageZip.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageZip.Size = new System.Drawing.Size(878, 444);
            this.tabPageZip.TabIndex = 2;
            this.tabPageZip.Text = "ZIP Arquivos";
            this.tabPageZip.UseVisualStyleBackColor = true;
            //
            // buttonSelecionaArquivosZIP
            //
            this.buttonSelecionaArquivosZIP.Location = new System.Drawing.Point(94, 172);
            this.buttonSelecionaArquivosZIP.Name = "buttonSelecionaArquivosZIP";
            this.buttonSelecionaArquivosZIP.Size = new System.Drawing.Size(234, 23);
            this.buttonSelecionaArquivosZIP.TabIndex = 0;
            this.buttonSelecionaArquivosZIP.Text = "Selecione os arquivos";
            this.buttonSelecionaArquivosZIP.UseVisualStyleBackColor = true;
            this.buttonSelecionaArquivosZIP.Click += new System.EventHandler(this.buttonSelecionaArquivosZIP_Click);
            //
            // labelArquivosSelecionadosZIP
            //
            this.labelArquivosSelecionadosZIP.AutoSize = true;
            this.labelArquivosSelecionadosZIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelArquivosSelecionadosZIP.Location = new System.Drawing.Point(166, 198);
            this.labelArquivosSelecionadosZIP.Name = "labelArquivosSelecionadosZIP";
            this.labelArquivosSelecionadosZIP.Size = new System.Drawing.Size(145, 13);
            this.labelArquivosSelecionadosZIP.TabIndex = 1;
            this.labelArquivosSelecionadosZIP.Text = "Nenhum arquivo selecionado";
            //
            // buttonCriarZips
            //
            this.buttonCriarZips.Location = new System.Drawing.Point(626, 172);
            this.buttonCriarZips.Name = "buttonCriarZips";
            this.buttonCriarZips.Size = new System.Drawing.Size(100, 50);
            this.buttonCriarZips.TabIndex = 2;
            this.buttonCriarZips.Text = "Criar ZIPs";
            this.buttonCriarZips.UseVisualStyleBackColor = true;
            this.buttonCriarZips.Click += new System.EventHandler(this.buttonCriarZips_Click);
            //
            // checkBoxZip
            //
            this.checkBoxZip.AutoSize = true;
            this.checkBoxZip.Location = new System.Drawing.Point(579, 251);
            this.checkBoxZip.Name = "checkBoxZip";
            this.checkBoxZip.Size = new System.Drawing.Size(166, 17);
            this.checkBoxZip.TabIndex = 3;
            this.checkBoxZip.Text = "Todos arquivos no mesmo zip";
            this.checkBoxZip.UseVisualStyleBackColor = true;
            //
            // openFileDialog1
            //
            this.openFileDialog1.FileName = "openFileDialog1";
            //
            // Form1
            //
            this.AcceptButton = this.buttonComparar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(910, 480);
            this.Controls.Add(this.tabControlPrincipal);
            this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Comparador de listas de materiais";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabControlPrincipal.ResumeLayout(false);
            this.tabPageListas.ResumeLayout(false);
            this.tabPageListas.PerformLayout();
            this.tabPageCAM.ResumeLayout(false);
            this.tabPageCAM.PerformLayout();
            this.tabPageZip.ResumeLayout(false);
            this.tabPageZip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlPrincipal;
        private System.Windows.Forms.TabPage tabPageListas;
        private System.Windows.Forms.TabPage tabPageCAM;
        private System.Windows.Forms.TabPage tabPageZip;

        // Aba Listas
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelDescricaoPrograma;
        private System.Windows.Forms.Button buttonArquivo1;
        private System.Windows.Forms.Label labelArquivo1;
        private System.Windows.Forms.Button buttonArquivo2;
        private System.Windows.Forms.Label labelArquivo2;
        private System.Windows.Forms.Button buttonSelecionaPasta;
        private System.Windows.Forms.Label labelPastaResultado;
        private System.Windows.Forms.TextBox textBoxNomeResult;
        private System.Windows.Forms.Label labelTextResult;
        private System.Windows.Forms.CheckBox checkBoxTxt;
        private System.Windows.Forms.CheckBox checkBoxExcel;
        private System.Windows.Forms.Button buttonComparar;

        // Aba CAM
        private System.Windows.Forms.Label labelCAMInstrucoes;
        private System.Windows.Forms.Button buttonPastaCamA;
        private System.Windows.Forms.Label labelPastaCamA;
        private System.Windows.Forms.Button buttonPastaCamB;
        private System.Windows.Forms.Label labelPastaCamB;
        private System.Windows.Forms.Label labelFabricanteA;
        private System.Windows.Forms.ComboBox comboFabricanteA;
        private System.Windows.Forms.Label labelFabricanteB;
        private System.Windows.Forms.ComboBox comboFabricanteB;
        private System.Windows.Forms.Label labelParafusoTipo;
        private System.Windows.Forms.ComboBox comboParafusoTipo;
        private System.Windows.Forms.CheckBox checkBoxConsiderarDiametro;
        private System.Windows.Forms.Button buttonPastaResultadoCam;
        private System.Windows.Forms.Label labelPastaResultadoCam;
        private System.Windows.Forms.Label labelNomeResultadoCam;
        private System.Windows.Forms.TextBox textBoxNomeResultadoCam;
        private System.Windows.Forms.Button buttonCompararCam;

        // Aba ZIP
        private System.Windows.Forms.Button buttonSelecionaArquivosZIP;
        private System.Windows.Forms.Label labelArquivosSelecionadosZIP;
        private System.Windows.Forms.Button buttonCriarZips;
        private System.Windows.Forms.CheckBox checkBoxZip;

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}
