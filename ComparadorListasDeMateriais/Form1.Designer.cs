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
            this.buttonArquivo1 = new System.Windows.Forms.Button();
            this.buttonArquivo2 = new System.Windows.Forms.Button();
            this.buttonComparar = new System.Windows.Forms.Button();
            this.labelArquivo1 = new System.Windows.Forms.Label();
            this.labelArquivo2 = new System.Windows.Forms.Label();
            this.buttonSelecionaPasta = new System.Windows.Forms.Button();
            this.labelPastaResultado = new System.Windows.Forms.Label();
            this.labelDescricaoPrograma = new System.Windows.Forms.Label();
            this.textBoxNomeResult = new System.Windows.Forms.TextBox();
            this.labelTextResult = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.checkBoxTxt = new System.Windows.Forms.CheckBox();
            this.checkBoxExcel = new System.Windows.Forms.CheckBox();
            this.tabControlComparaLista = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.buttonSelecionaArquivosZIP = new System.Windows.Forms.Button();
            this.labelArquivosSelecionadosZIP = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonCriarZips = new System.Windows.Forms.Button();
            this.checkBoxZip = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabControlComparaLista.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonArquivo1
            // 
            this.buttonArquivo1.Location = new System.Drawing.Point(31, 188);
            this.buttonArquivo1.Name = "buttonArquivo1";
            this.buttonArquivo1.Size = new System.Drawing.Size(163, 23);
            this.buttonArquivo1.TabIndex = 0;
            this.buttonArquivo1.Text = "Selecione a Lista Original";
            this.buttonArquivo1.UseVisualStyleBackColor = true;
            this.buttonArquivo1.Click += new System.EventHandler(this.buttonArquivo1_Click);
            // 
            // buttonArquivo2
            // 
            this.buttonArquivo2.Location = new System.Drawing.Point(31, 282);
            this.buttonArquivo2.Name = "buttonArquivo2";
            this.buttonArquivo2.Size = new System.Drawing.Size(163, 23);
            this.buttonArquivo2.TabIndex = 1;
            this.buttonArquivo2.Text = "Selecione a Lista Nova";
            this.buttonArquivo2.UseVisualStyleBackColor = true;
            this.buttonArquivo2.Click += new System.EventHandler(this.buttonArquivo2_Click);
            // 
            // buttonComparar
            // 
            this.buttonComparar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonComparar.Location = new System.Drawing.Point(631, 282);
            this.buttonComparar.Name = "buttonComparar";
            this.buttonComparar.Size = new System.Drawing.Size(122, 56);
            this.buttonComparar.TabIndex = 4;
            this.buttonComparar.Text = "Comparar";
            this.buttonComparar.UseVisualStyleBackColor = true;
            this.buttonComparar.Click += new System.EventHandler(this.buttonComparar_Click);
            // 
            // labelArquivo1
            // 
            this.labelArquivo1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.labelArquivo1.AutoSize = true;
            this.labelArquivo1.Location = new System.Drawing.Point(38, 214);
            this.labelArquivo1.Name = "labelArquivo1";
            this.labelArquivo1.Size = new System.Drawing.Size(145, 13);
            this.labelArquivo1.TabIndex = 5;
            this.labelArquivo1.Text = "Nenhum arquivo selecionado";
            // 
            // labelArquivo2
            // 
            this.labelArquivo2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.labelArquivo2.AutoSize = true;
            this.labelArquivo2.Location = new System.Drawing.Point(38, 305);
            this.labelArquivo2.Name = "labelArquivo2";
            this.labelArquivo2.Size = new System.Drawing.Size(145, 13);
            this.labelArquivo2.TabIndex = 6;
            this.labelArquivo2.Text = "Nenhum arquivo selecionado";
            // 
            // buttonSelecionaPasta
            // 
            this.buttonSelecionaPasta.Location = new System.Drawing.Point(272, 187);
            this.buttonSelecionaPasta.Name = "buttonSelecionaPasta";
            this.buttonSelecionaPasta.Size = new System.Drawing.Size(234, 23);
            this.buttonSelecionaPasta.TabIndex = 7;
            this.buttonSelecionaPasta.Text = "Selecione a pasta para salvar o resultado";
            this.buttonSelecionaPasta.UseVisualStyleBackColor = true;
            this.buttonSelecionaPasta.Click += new System.EventHandler(this.buttonSelecionaPasta_Click);
            // 
            // labelPastaResultado
            // 
            this.labelPastaResultado.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.labelPastaResultado.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPastaResultado.Location = new System.Drawing.Point(324, 213);
            this.labelPastaResultado.Name = "labelPastaResultado";
            this.labelPastaResultado.Size = new System.Drawing.Size(142, 24);
            this.labelPastaResultado.TabIndex = 8;
            this.labelPastaResultado.Text = "Nenhuma pasta selecionada";
            // 
            // labelDescricaoPrograma
            // 
            this.labelDescricaoPrograma.AutoSize = true;
            this.labelDescricaoPrograma.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDescricaoPrograma.Location = new System.Drawing.Point(138, 51);
            this.labelDescricaoPrograma.Name = "labelDescricaoPrograma";
            this.labelDescricaoPrograma.Size = new System.Drawing.Size(71, 17);
            this.labelDescricaoPrograma.TabIndex = 9;
            this.labelDescricaoPrograma.Text = "Descricao";
            // 
            // textBoxNomeResult
            // 
            this.textBoxNomeResult.Location = new System.Drawing.Point(284, 284);
            this.textBoxNomeResult.Name = "textBoxNomeResult";
            this.textBoxNomeResult.Size = new System.Drawing.Size(199, 20);
            this.textBoxNomeResult.TabIndex = 10;
            this.textBoxNomeResult.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // labelTextResult
            // 
            this.labelTextResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.labelTextResult.AutoSize = true;
            this.labelTextResult.Location = new System.Drawing.Point(281, 305);
            this.labelTextResult.Name = "labelTextResult";
            this.labelTextResult.Size = new System.Drawing.Size(212, 13);
            this.labelTextResult.TabIndex = 11;
            this.labelTextResult.Text = "Digite um nome para o arquivo de resultado";
            this.labelTextResult.Click += new System.EventHandler(this.labelTextResult_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ComparadorListasDeMateriais.Properties.Resources.LogoEEC;
            this.pictureBox1.Location = new System.Drawing.Point(6, 19);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(98, 84);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            // 
            // checkBoxTxt
            // 
            this.checkBoxTxt.AutoSize = true;
            this.checkBoxTxt.Location = new System.Drawing.Point(600, 187);
            this.checkBoxTxt.Name = "checkBoxTxt";
            this.checkBoxTxt.Size = new System.Drawing.Size(133, 17);
            this.checkBoxTxt.TabIndex = 13;
            this.checkBoxTxt.Text = "Salvar resultado em txt";
            this.checkBoxTxt.UseVisualStyleBackColor = true;
            this.checkBoxTxt.CheckedChanged += new System.EventHandler(this.checkBoxTxt_CheckedChanged);
            // 
            // checkBoxExcel
            // 
            this.checkBoxExcel.AutoSize = true;
            this.checkBoxExcel.Location = new System.Drawing.Point(600, 214);
            this.checkBoxExcel.Name = "checkBoxExcel";
            this.checkBoxExcel.Size = new System.Drawing.Size(138, 17);
            this.checkBoxExcel.TabIndex = 14;
            this.checkBoxExcel.Text = "Gerar Relatório de PMV";
            this.checkBoxExcel.UseVisualStyleBackColor = true;
            this.checkBoxExcel.CheckedChanged += new System.EventHandler(this.checkBoxExcel_CheckedChanged);
            // 
            // tabControlComparaLista
            // 
            this.tabControlComparaLista.AccessibleName = "";
            this.tabControlComparaLista.Controls.Add(this.tabPage1);
            this.tabControlComparaLista.Controls.Add(this.tabPage2);
            this.tabControlComparaLista.Location = new System.Drawing.Point(12, 2);
            this.tabControlComparaLista.Multiline = true;
            this.tabControlComparaLista.Name = "tabControlComparaLista";
            this.tabControlComparaLista.SelectedIndex = 0;
            this.tabControlComparaLista.Size = new System.Drawing.Size(886, 446);
            this.tabControlComparaLista.TabIndex = 15;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.buttonSelecionaPasta);
            this.tabPage1.Controls.Add(this.checkBoxExcel);
            this.tabPage1.Controls.Add(this.buttonArquivo1);
            this.tabPage1.Controls.Add(this.checkBoxTxt);
            this.tabPage1.Controls.Add(this.buttonArquivo2);
            this.tabPage1.Controls.Add(this.pictureBox1);
            this.tabPage1.Controls.Add(this.buttonComparar);
            this.tabPage1.Controls.Add(this.labelTextResult);
            this.tabPage1.Controls.Add(this.labelArquivo1);
            this.tabPage1.Controls.Add(this.textBoxNomeResult);
            this.tabPage1.Controls.Add(this.labelArquivo2);
            this.tabPage1.Controls.Add(this.labelDescricaoPrograma);
            this.tabPage1.Controls.Add(this.labelPastaResultado);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(878, 420);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Comparador Listas";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.checkBoxZip);
            this.tabPage2.Controls.Add(this.buttonCriarZips);
            this.tabPage2.Controls.Add(this.flowLayoutPanel1);
            this.tabPage2.Controls.Add(this.buttonSelecionaArquivosZIP);
            this.tabPage2.Controls.Add(this.labelArquivosSelecionadosZIP);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(878, 420);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "ZIP Arquivos";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // buttonSelecionaArquivosZIP
            // 
            this.buttonSelecionaArquivosZIP.Location = new System.Drawing.Point(94, 172);
            this.buttonSelecionaArquivosZIP.Name = "buttonSelecionaArquivosZIP";
            this.buttonSelecionaArquivosZIP.Size = new System.Drawing.Size(234, 23);
            this.buttonSelecionaArquivosZIP.TabIndex = 9;
            this.buttonSelecionaArquivosZIP.Text = "Selecione os arquivos";
            this.buttonSelecionaArquivosZIP.UseVisualStyleBackColor = true;
            this.buttonSelecionaArquivosZIP.Click += new System.EventHandler(this.buttonSelecionaArquivosZIP_Click);
            // 
            // labelArquivosSelecionadosZIP
            // 
            this.labelArquivosSelecionadosZIP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.labelArquivosSelecionadosZIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelArquivosSelecionadosZIP.Location = new System.Drawing.Point(166, 198);
            this.labelArquivosSelecionadosZIP.Name = "labelArquivosSelecionadosZIP";
            this.labelArquivosSelecionadosZIP.Size = new System.Drawing.Size(142, 24);
            this.labelArquivosSelecionadosZIP.TabIndex = 10;
            this.labelArquivosSelecionadosZIP.Text = "Nenhum arquivo selecionado";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Location = new System.Drawing.Point(373, 159);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(200, 100);
            this.flowLayoutPanel1.TabIndex = 11;
            // 
            // buttonCriarZips
            // 
            this.buttonCriarZips.Location = new System.Drawing.Point(626, 172);
            this.buttonCriarZips.Name = "buttonCriarZips";
            this.buttonCriarZips.Size = new System.Drawing.Size(100, 50);
            this.buttonCriarZips.TabIndex = 12;
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
            this.checkBoxZip.TabIndex = 13;
            this.checkBoxZip.Text = "Todos arquivos no mesmo zip";
            this.checkBoxZip.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AcceptButton = this.buttonComparar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(910, 460);
            this.Controls.Add(this.tabControlComparaLista);
            this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Comparador de listas de materiais";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabControlComparaLista.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonArquivo1;
        private System.Windows.Forms.Button buttonArquivo2;
        private System.Windows.Forms.Button buttonComparar;
        private System.Windows.Forms.Label labelArquivo1;
        private System.Windows.Forms.Label labelArquivo2;
        private System.Windows.Forms.Button buttonSelecionaPasta;
        private System.Windows.Forms.Label labelPastaResultado;
        private System.Windows.Forms.Label labelDescricaoPrograma;
        private System.Windows.Forms.TextBox textBoxNomeResult;
        private System.Windows.Forms.Label labelTextResult;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox checkBoxTxt;
        private System.Windows.Forms.CheckBox checkBoxExcel;
        private System.Windows.Forms.TabControl tabControlComparaLista;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button buttonSelecionaArquivosZIP;
        private System.Windows.Forms.Label labelArquivosSelecionadosZIP;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button buttonCriarZips;
        private System.Windows.Forms.CheckBox checkBoxZip;
    }
}

