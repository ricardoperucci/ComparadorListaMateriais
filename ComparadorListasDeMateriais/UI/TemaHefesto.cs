using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ComparadorListasDeMateriais.UI
{
    /// <summary>
    /// Espelha em WinForms os tokens do tema CLARO do Hefesto
    /// (Hefesto.Tools/WPF/Tema/Cores.Claro.xaml e Tipografia.xaml). Como este app é WinForms e o
    /// tema oficial é WPF, replicamos os mesmos hexadecimais/fontes aqui, mantendo a identidade visual.
    /// </summary>
    public static class TemaHefesto
    {
        // ---- Destaque ----
        public static readonly Color Accent = Hex("1A73C7");
        public static readonly Color AccentSoft = Hex("3B8FE0");
        public static readonly Color AccentForte = Hex("155A99");
        public static readonly Color Secundario = Hex("7C5CFA");

        // ---- Fundos (camadas, do mais "profundo" ao mais claro) ----
        public static readonly Color FundoJanela = Hex("FFFFFF");
        public static readonly Color FundoPainel = Hex("F3F4F6");
        public static readonly Color FundoCabecalho = Hex("EFF1F4");
        public static readonly Color FundoControle = Hex("FFFFFF");
        public static readonly Color FundoControleSeta = Hex("E9ECF0");

        // ---- Bordas ----
        public static readonly Color BordaControle = Hex("C6CCD4");
        public static readonly Color BordaSutil = Hex("E3E6EB");

        // ---- Textos ----
        public static readonly Color TextoPrimario = Hex("202225");
        public static readonly Color TextoSecundario = Hex("555A60");
        public static readonly Color TextoTerciario = Hex("7A8088");
        public static readonly Color TextoDesabilitado = Hex("AAB0B8");
        public static readonly Color TextoSobreAccent = Hex("FFFFFF");

        // ---- Tipografia (Segoe UI; px do tema -> pt no WinForms: pt = px * 0.75 a 96dpi) ----
        public const string Familia = "Segoe UI";
        public static Font Fonte(float px, FontStyle estilo = FontStyle.Regular) => new Font(Familia, px * 0.75f, estilo);
        public static Font FonteBase => Fonte(14);
        public static Font FonteTitulo => Fonte(17, FontStyle.Bold);
        public static Font FonteRotulo => Fonte(13);
        public static Font FontePequena => Fonte(11);

        private static Color Hex(string h) => ColorTranslator.FromHtml("#" + h);

        // =====================================================================
        //  Aplicadores de estilo
        // =====================================================================

        public static void AplicarNoFormulario(Form f)
        {
            f.BackColor = FundoJanela;
            f.ForeColor = TextoPrimario;
            f.Font = FonteBase;
        }

        /// <summary>Botão primário: fundo accent, texto sobre-accent, cantos arredondados e hover animado pelo WinForms.</summary>
        public static void BotaoPrimario(Button b)
        {
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.BackColor = Accent;
            b.ForeColor = TextoSobreAccent;
            b.Font = Fonte(14, FontStyle.Bold);
            b.Cursor = Cursors.Hand;
            b.UseVisualStyleBackColor = false;
            b.FlatAppearance.MouseOverBackColor = AccentSoft;
            b.FlatAppearance.MouseDownBackColor = AccentForte;
            Arredondar(b, 4);
        }

        /// <summary>Botão secundário (flat, fundo de controle, borda sutil).</summary>
        public static void BotaoSecundario(Button b)
        {
            b.FlatStyle = FlatStyle.Flat;
            b.BackColor = FundoPainel;
            b.ForeColor = TextoPrimario;
            b.Font = FonteRotulo;
            b.Cursor = Cursors.Hand;
            b.UseVisualStyleBackColor = false;
            b.FlatAppearance.BorderColor = BordaControle;
            b.FlatAppearance.BorderSize = 1;
            b.FlatAppearance.MouseOverBackColor = FundoControleSeta;
            b.FlatAppearance.MouseDownBackColor = BordaControle;
            Arredondar(b, 4);
        }

        public static void Campo(TextBox t)
        {
            t.BorderStyle = BorderStyle.FixedSingle;
            t.BackColor = FundoControle;
            t.ForeColor = TextoPrimario;
            t.Font = FonteBase;
        }

        /// <summary>ComboBox escuro com itens desenhados (accent na seleção, igual ao ComboBoxItem do tema).</summary>
        public static void Campo(ComboBox c)
        {
            c.FlatStyle = FlatStyle.Flat;
            c.BackColor = FundoControle;
            c.ForeColor = TextoPrimario;
            c.Font = FonteBase;
            c.DrawMode = DrawMode.OwnerDrawFixed;
            c.ItemHeight = 22;
            c.DrawItem += (s, e) =>
            {
                if (e.Index < 0) return;
                bool sel = (e.State & DrawItemState.Selected) != 0;
                using (var bg = new SolidBrush(sel ? Accent : FundoControle))
                    e.Graphics.FillRectangle(bg, e.Bounds);
                string txt = c.GetItemText(c.Items[e.Index]);
                TextRenderer.DrawText(e.Graphics, txt, c.Font, e.Bounds,
                    sel ? TextoSobreAccent : TextoPrimario,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            };
        }

        public static void CheckTema(CheckBox c)
        {
            c.FlatStyle = FlatStyle.Flat;
            c.ForeColor = TextoPrimario;
            c.BackColor = Color.Transparent;
            c.Font = FonteRotulo;
            c.Cursor = Cursors.Hand;
            c.FlatAppearance.CheckedBackColor = Accent;
            c.FlatAppearance.BorderColor = BordaControle;
        }

        public static void Rotulo(Label l) { Aplicar(l, TextoSecundario, FonteRotulo); }
        public static void Texto(Label l) { Aplicar(l, TextoPrimario, FonteBase); }
        public static void Terciario(Label l) { Aplicar(l, TextoTerciario, FontePequena); }
        public static void Titulo(Label l) { Aplicar(l, TextoPrimario, FonteTitulo); }
        public static void Subtitulo(Label l) { Aplicar(l, TextoTerciario, FontePequena); }

        private static void Aplicar(Label l, Color cor, Font fonte)
        {
            l.ForeColor = cor;
            l.Font = fonte;
            l.BackColor = Color.Transparent;
        }

        /// <summary>
        /// Estiliza um TabControl em modo escuro (owner-draw): aba selecionada com fundo do painel e
        /// sublinhado accent; abas inativas em fundo de cabeçalho.
        /// </summary>
        public static void EstilizarTabControl(TabControl tab)
        {
            tab.DrawMode = TabDrawMode.OwnerDrawFixed;
            tab.SizeMode = TabSizeMode.Fixed;
            tab.ItemSize = new Size(190, 34);
            tab.BackColor = FundoCabecalho;

            foreach (TabPage pagina in tab.TabPages)
            {
                pagina.BackColor = FundoJanela;
                pagina.ForeColor = TextoPrimario;
                pagina.UseVisualStyleBackColor = false;
            }

            tab.DrawItem += (s, e) =>
            {
                TabPage pagina = tab.TabPages[e.Index];
                bool selecionada = e.Index == tab.SelectedIndex;
                Rectangle r = tab.GetTabRect(e.Index);

                using (var bg = new SolidBrush(selecionada ? FundoJanela : FundoCabecalho))
                    e.Graphics.FillRectangle(bg, r);

                if (selecionada)
                    using (var ac = new SolidBrush(Accent))
                        e.Graphics.FillRectangle(ac, r.X, r.Bottom - 3, r.Width, 3);

                TextRenderer.DrawText(e.Graphics, pagina.Text,
                    Fonte(13, selecionada ? FontStyle.Bold : FontStyle.Regular), r,
                    selecionada ? TextoPrimario : TextoSecundario,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };
        }

        /// <summary>Aplica um <see cref="Region"/> de cantos arredondados ao controle (e mantém ao redimensionar).</summary>
        public static void Arredondar(Control c, int raio = 4)
        {
            void Aplica()
            {
                if (c.Width <= 0 || c.Height <= 0) return;
                using (var caminho = CaminhoArredondado(new Rectangle(0, 0, c.Width, c.Height), raio))
                    c.Region = new Region(caminho);
            }
            Aplica();
            c.Resize += (s, e) => Aplica();
        }

        private static GraphicsPath CaminhoArredondado(Rectangle r, int raio)
        {
            int d = raio * 2;
            var caminho = new GraphicsPath();
            caminho.AddArc(r.X, r.Y, d, d, 180, 90);
            caminho.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            caminho.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            caminho.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            caminho.CloseFigure();
            return caminho;
        }
    }
}
