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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonArquivo1
            // 
            this.buttonArquivo1.Location = new System.Drawing.Point(50, 181);
            this.buttonArquivo1.Name = "buttonArquivo1";
            this.buttonArquivo1.Size = new System.Drawing.Size(163, 23);
            this.buttonArquivo1.TabIndex = 0;
            this.buttonArquivo1.Text = "Selecione a Lista Original";
            this.buttonArquivo1.UseVisualStyleBackColor = true;
            this.buttonArquivo1.Click += new System.EventHandler(this.buttonArquivo1_Click);
            // 
            // buttonArquivo2
            // 
            this.buttonArquivo2.Location = new System.Drawing.Point(50, 275);
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
            this.buttonComparar.Location = new System.Drawing.Point(650, 275);
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
            this.labelArquivo1.Location = new System.Drawing.Point(57, 207);
            this.labelArquivo1.Name = "labelArquivo1";
            this.labelArquivo1.Size = new System.Drawing.Size(145, 13);
            this.labelArquivo1.TabIndex = 5;
            this.labelArquivo1.Text = "Nenhum arquivo selecionado";
            // 
            // labelArquivo2
            // 
            this.labelArquivo2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.labelArquivo2.AutoSize = true;
            this.labelArquivo2.Location = new System.Drawing.Point(57, 301);
            this.labelArquivo2.Name = "labelArquivo2";
            this.labelArquivo2.Size = new System.Drawing.Size(145, 13);
            this.labelArquivo2.TabIndex = 6;
            this.labelArquivo2.Text = "Nenhum arquivo selecionado";
            // 
            // buttonSelecionaPasta
            // 
            this.buttonSelecionaPasta.Location = new System.Drawing.Point(291, 180);
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
            this.labelPastaResultado.Location = new System.Drawing.Point(331, 206);
            this.labelPastaResultado.Name = "labelPastaResultado";
            this.labelPastaResultado.Size = new System.Drawing.Size(142, 13);
            this.labelPastaResultado.TabIndex = 8;
            this.labelPastaResultado.Text = "Nenhuma pasta selecionada";
            // 
            // labelDescricaoPrograma
            // 
            this.labelDescricaoPrograma.AutoSize = true;
            this.labelDescricaoPrograma.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDescricaoPrograma.Location = new System.Drawing.Point(157, 44);
            this.labelDescricaoPrograma.Name = "labelDescricaoPrograma";
            this.labelDescricaoPrograma.Size = new System.Drawing.Size(71, 17);
            this.labelDescricaoPrograma.TabIndex = 9;
            this.labelDescricaoPrograma.Text = "Descricao";
            // 
            // textBoxNomeResult
            // 
            this.textBoxNomeResult.Location = new System.Drawing.Point(303, 277);
            this.textBoxNomeResult.Name = "textBoxNomeResult";
            this.textBoxNomeResult.Size = new System.Drawing.Size(199, 20);
            this.textBoxNomeResult.TabIndex = 10;
            this.textBoxNomeResult.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // labelTextResult
            // 
            this.labelTextResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.labelTextResult.AutoSize = true;
            this.labelTextResult.Location = new System.Drawing.Point(300, 300);
            this.labelTextResult.Name = "labelTextResult";
            this.labelTextResult.Size = new System.Drawing.Size(212, 13);
            this.labelTextResult.TabIndex = 11;
            this.labelTextResult.Text = "Digite um nome para o arquivo de resultado";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ComparadorListasDeMateriais.Properties.Resources.LogoEEC;
            this.pictureBox1.Location = new System.Drawing.Point(25, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(98, 84);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            // 
            // checkBoxTxt
            // 
            this.checkBoxTxt.AutoSize = true;
            this.checkBoxTxt.Location = new System.Drawing.Point(619, 180);
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
            this.checkBoxExcel.Location = new System.Drawing.Point(619, 207);
            this.checkBoxExcel.Name = "checkBoxExcel";
            this.checkBoxExcel.Size = new System.Drawing.Size(147, 17);
            this.checkBoxExcel.TabIndex = 14;
            this.checkBoxExcel.Text = "Gerar Relatório de PMV";
            this.checkBoxExcel.UseVisualStyleBackColor = true;
            this.checkBoxExcel.CheckedChanged += new System.EventHandler(this.checkBoxExcel_CheckedChanged);
            // 
            // Form1
            // 
            this.AcceptButton = this.buttonComparar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(798, 372);
            this.Controls.Add(this.checkBoxExcel);
            this.Controls.Add(this.checkBoxTxt);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.labelTextResult);
            this.Controls.Add(this.textBoxNomeResult);
            this.Controls.Add(this.labelDescricaoPrograma);
            this.Controls.Add(this.labelPastaResultado);
            this.Controls.Add(this.buttonSelecionaPasta);
            this.Controls.Add(this.labelArquivo2);
            this.Controls.Add(this.labelArquivo1);
            this.Controls.Add(this.buttonComparar);
            this.Controls.Add(this.buttonArquivo2);
            this.Controls.Add(this.buttonArquivo1);
            this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Comparador de listas de materiais";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}

