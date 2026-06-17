#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ComparadorListasDeMateriais.DiffCamTextual
{
    /// <summary>
    /// Resumo numérico da comparação textual de pastas CAM.
    /// </summary>
    public sealed record ResultadoDiffCam(int Comuns, int Alterados, int SoNoAntes, int SoNoDepois);

    /// <summary>
    /// Compara duas pastas de arquivos .CAM <b>linha a linha</b> e gera um relatório HTML com
    /// resumos por tag, inventários e destaque de alterações com delta &gt; 0.8 mm.
    /// <para>
    /// Reescrita (portada) do estudo <c>ESTUDOS/CAMUSA</c> dentro deste comparador — abordagem
    /// puramente textual/regex, sem qualquer dependência da HefestoCAM ou de DLLs externas.
    /// </para>
    /// </summary>
    public static class GeradorHtmlDiffCam
    {
        private const double LimiarDelta = 0.8;

        private static readonly Regex RegexTag = new(@"^([A-Z]+_[A-Z]+):", RegexOptions.Compiled);
        private static readonly Regex RegexNumeros = new(@"\d+(?:\.\d+)?", RegexOptions.Compiled);
        private static readonly Regex RegexChapa = new(@"^\s*CH\.?\s+(\d+(?:\.\d+)?)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RegexCantoneira = new(@"^\s*(?:EA|L)\s+(\d+(?:\.\d+)?)\s*x\s*(\d+(?:\.\d+)?)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private sealed record Mudanca(int Linha, string Antes, string Depois);

        public static ResultadoDiffCam Gerar(string dirAntes, string dirDepois, string saidaHtml)
        {
            string[] arquivosAntes = NomesCam(dirAntes);
            string[] arquivosDepois = NomesCam(dirDepois);

            var setAntes = new HashSet<string>(arquivosAntes, StringComparer.OrdinalIgnoreCase);
            var setDepois = new HashSet<string>(arquivosDepois, StringComparer.OrdinalIgnoreCase);

            List<string> nomesComuns = arquivosAntes.Where(setDepois.Contains).OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToList();
            List<string> soNoAntes = arquivosAntes.Where(x => !setDepois.Contains(x)).OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToList();
            List<string> soNoDepois = arquivosDepois.Where(x => !setAntes.Contains(x)).OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToList();

            var diffsPorArquivo = new SortedDictionary<string, List<Mudanca>>(StringComparer.OrdinalIgnoreCase);
            var tagsAlteradas = new Dictionary<string, int>();
            var valoresNomPro = new Dictionary<string, int>();
            var valoresSpePro = new Dictionary<string, int>();
            var valoresLunPro = new Dictionary<string, int>();
            var valoresLarPro = new Dictionary<string, int>();
            int shapeAlteradoCount = 0;

            foreach (string nome in nomesComuns)
            {
                string[] linhasAntes = File.ReadAllLines(Path.Combine(dirAntes, nome));
                string[] linhasDepois = File.ReadAllLines(Path.Combine(dirDepois, nome));

                int maxLinhas = Math.Max(linhasAntes.Length, linhasDepois.Length);
                var mudancas = new List<Mudanca>();
                bool temShape = false;

                for (int i = 0; i < maxLinhas; i++)
                {
                    string a = i < linhasAntes.Length ? linhasAntes[i] : "";
                    string d = i < linhasDepois.Length ? linhasDepois[i] : "";

                    if (a == d)
                        continue;

                    mudancas.Add(new Mudanca(i + 1, a, d));

                    string tag;
                    Match mTag = RegexTag.Match(a);
                    if (!mTag.Success)
                        mTag = RegexTag.Match(d);

                    if (mTag.Success)
                    {
                        tag = mTag.Groups[1].Value + ":";
                    }
                    else
                    {
                        tag = "(SHAPE/conteúdo)";
                        temShape = true;
                    }

                    Incrementa(tagsAlteradas, tag);

                    string par = $"{a} -> {d}";
                    switch (tag)
                    {
                        case "NOM_PRO:":
                            if (a.Contains("CH.") || d.Contains("CH."))
                                Incrementa(valoresNomPro, par);
                            break;
                        case "SPE_PRO:":
                            Incrementa(valoresSpePro, par);
                            break;
                        case "LUN_PRO:":
                            Incrementa(valoresLunPro, par);
                            break;
                        case "LAR_PRO:":
                            Incrementa(valoresLarPro, par);
                            break;
                    }
                }

                if (mudancas.Count > 0)
                {
                    diffsPorArquivo[nome] = mudancas;
                    if (temShape)
                        shapeAlteradoCount++;
                }
            }

            // Inventário de espessuras de chapa e perfis de cantoneira
            var chapasAntes = new Dictionary<string, int>();
            var chapasDepois = new Dictionary<string, int>();
            var cantoneirasAntes = new Dictionary<string, int>();
            var cantoneirasDepois = new Dictionary<string, int>();

            ExtraiInventario(dirAntes, arquivosAntes, chapasAntes, cantoneirasAntes);
            ExtraiInventario(dirDepois, arquivosDepois, chapasDepois, cantoneirasDepois);

            List<string> todasChapas = chapasAntes.Keys.Union(chapasDepois.Keys)
                .OrderBy(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            List<string> todasCantoneiras = cantoneirasAntes.Keys.Union(cantoneirasDepois.Keys)
                .OrderBy(x => double.Parse(x.Split(" x ")[0], CultureInfo.InvariantCulture))
                .ThenBy(x => double.Parse(x.Split(" x ")[1], CultureInfo.InvariantCulture)).ToList();

            string html = MontaHtml(dirAntes, dirDepois, arquivosAntes.Length, arquivosDepois.Length,
                nomesComuns.Count, soNoAntes, soNoDepois, diffsPorArquivo, shapeAlteradoCount,
                tagsAlteradas, valoresNomPro, valoresSpePro, valoresLunPro, valoresLarPro,
                todasChapas, chapasAntes, chapasDepois, todasCantoneiras, cantoneirasAntes, cantoneirasDepois);

            File.WriteAllText(saidaHtml, html, new UTF8Encoding(true));

            return new ResultadoDiffCam(nomesComuns.Count, diffsPorArquivo.Count, soNoAntes.Count, soNoDepois.Count);
        }

        private static string[] NomesCam(string dir) =>
            Directory.GetFiles(dir, "*.CAM", SearchOption.TopDirectoryOnly).Select(Path.GetFileName).Where(x => x != null).Cast<string>().ToArray();

        private static void Incrementa(Dictionary<string, int> dic, string chave)
        {
            dic[chave] = dic.TryGetValue(chave, out int v) ? v + 1 : 1;
        }

        private static void ExtraiInventario(string dir, string[] nomes, Dictionary<string, int> chapas, Dictionary<string, int> cantoneiras)
        {
            foreach (string nome in nomes)
            {
                string? linhaNomPro = File.ReadLines(Path.Combine(dir, nome))
                    .FirstOrDefault(l => l.StartsWith("NOM_PRO:", StringComparison.Ordinal));

                if (linhaNomPro == null)
                    continue;

                string conteudo = linhaNomPro.Substring("NOM_PRO:".Length);

                Match mChapa = RegexChapa.Match(conteudo);
                if (mChapa.Success)
                {
                    Incrementa(chapas, mChapa.Groups[1].Value);
                    continue;
                }

                Match mCant = RegexCantoneira.Match(conteudo);
                if (mCant.Success)
                    Incrementa(cantoneiras, $"{mCant.Groups[1].Value} x {mCant.Groups[2].Value}");
            }
        }

        // ---- Cálculo de delta ----

        private static List<double> ExtraiNumeros(string s) =>
            RegexNumeros.Matches(s).Select(m => double.Parse(m.Value, CultureInfo.InvariantCulture)).ToList();

        /// <summary>Maior |Δ| entre números pareados por posição; null se não der pra parear.</summary>
        private static double? ComputaDeltaMax(string antes, string depois)
        {
            List<double> nA = ExtraiNumeros(antes);
            List<double> nD = ExtraiNumeros(depois);

            if (nA.Count == 0 || nD.Count == 0 || nA.Count != nD.Count)
                return null;

            double maxDelta = 0.0;
            for (int i = 0; i < nA.Count; i++)
            {
                double delta = Math.Abs(nA[i] - nD[i]);
                if (delta > maxDelta)
                    maxDelta = delta;
            }

            return maxDelta;
        }

        private static string FormataDelta(double? delta) =>
            delta == null ? "-" : delta.Value.ToString("0.00", CultureInfo.InvariantCulture);

        private static string HtmlEscape(string? s) =>
            s == null ? "" : s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

        // ---- Montagem do HTML ----

        private static string MontaHtml(
            string dirAntes, string dirDepois, int qtdAntes, int qtdDepois, int qtdComuns,
            List<string> soNoAntes, List<string> soNoDepois,
            SortedDictionary<string, List<Mudanca>> diffsPorArquivo, int shapeAlteradoCount,
            Dictionary<string, int> tagsAlteradas,
            Dictionary<string, int> valoresNomPro, Dictionary<string, int> valoresSpePro,
            Dictionary<string, int> valoresLunPro, Dictionary<string, int> valoresLarPro,
            List<string> todasChapas, Dictionary<string, int> chapasAntes, Dictionary<string, int> chapasDepois,
            List<string> todasCantoneiras, Dictionary<string, int> cantoneirasAntes, Dictionary<string, int> cantoneirasDepois)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"pt-BR\">");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset=\"UTF-8\">");
            sb.AppendLine("<title>Diff de pastas CAM</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("  body { font-family: -apple-system, Segoe UI, sans-serif; max-width: 1400px; margin: 0 auto; padding: 24px; color: #222; }");
            sb.AppendLine("  h1 { color: #1a4d8f; border-bottom: 3px solid #1a4d8f; padding-bottom: 8px; }");
            sb.AppendLine("  h2 { color: #555; margin-top: 32px; border-bottom: 1px solid #ccc; padding-bottom: 4px; }");
            sb.AppendLine("  h3 { background: #eef3f9; padding: 6px 10px; font-family: monospace; font-size: 14px; margin-top: 16px; border-left: 4px solid #1a4d8f; }");
            sb.AppendLine("  table { border-collapse: collapse; width: 100%; margin-bottom: 12px; font-size: 12px; }");
            sb.AppendLine("  th, td { padding: 4px 8px; border: 1px solid #ddd; text-align: left; vertical-align: top; }");
            sb.AppendLine("  th { background: #f5f5f5; font-weight: 600; }");
            sb.AppendLine("  td.linha { text-align: right; color: #888; width: 50px; }");
            sb.AppendLine("  td.antes  { background: #fff5f5; color: #c0392b; font-family: monospace; white-space: pre; }");
            sb.AppendLine("  td.depois { background: #f1faf4; color: #1e7c3a; font-family: monospace; white-space: pre; }");
            sb.AppendLine("  td.delta  { font-family: monospace; text-align: right; }");
            sb.AppendLine("  tr.alteracao-grande td.antes  { background: #ffcdd2; font-weight: bold; }");
            sb.AppendLine("  tr.alteracao-grande td.depois { background: #ffcdd2; font-weight: bold; }");
            sb.AppendLine("  tr.alteracao-grande td.delta  { background: #b71c1c; color: white; font-weight: bold; }");
            sb.AppendLine("  tr.alteracao-grande td.linha  { background: #ffcdd2; font-weight: bold; color: #b71c1c; }");
            sb.AppendLine("  .summary { background: #fffde7; padding: 16px; border-left: 4px solid #fdd835; margin-bottom: 20px; }");
            sb.AppendLine("  .summary ul { margin: 8px 0; }");
            sb.AppendLine("  details { margin-bottom: 8px; }");
            sb.AppendLine("  summary { cursor: pointer; font-weight: 600; padding: 4px; }");
            sb.AppendLine("  .toc { columns: 3; column-gap: 30px; font-size: 13px; }");
            sb.AppendLine("  .toc a { text-decoration: none; color: #1a4d8f; }");
            sb.AppendLine("  .toc a:hover { text-decoration: underline; }");
            sb.AppendLine("  .summary table { font-size: 13px; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<h1>Diff de pastas CAM</h1>");

            sb.AppendLine("<div class=\"summary\">");
            sb.AppendLine("  <strong>Comparação:</strong>");
            sb.AppendLine("  <ul>");
            sb.AppendLine($"    <li><strong>Antes:</strong> {HtmlEscape(dirAntes)} ({qtdAntes} arquivos)</li>");
            sb.AppendLine($"    <li><strong>Depois:</strong> {HtmlEscape(dirDepois)} ({qtdDepois} arquivos)</li>");
            sb.AppendLine($"    <li>Arquivos comuns: {qtdComuns}</li>");
            sb.AppendLine($"    <li>Só no Antes: {soNoAntes.Count}</li>");
            sb.AppendLine($"    <li>Só no Depois: {soNoDepois.Count}</li>");
            sb.AppendLine($"    <li><strong>Arquivos alterados: {diffsPorArquivo.Count}</strong></li>");
            sb.AppendLine($"    <li>Arquivos com SHAPE alterado: {shapeAlteradoCount}</li>");
            sb.AppendLine("  </ul>");
            sb.AppendLine("</div>");

            sb.AppendLine("<h2>Resumo de tags alteradas</h2>");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>Tag</th><th>Ocorrências (linhas alteradas)</th></tr>");
            foreach (string k in tagsAlteradas.Keys.OrderBy(x => x, StringComparer.Ordinal))
                sb.AppendLine($"<tr><td>{HtmlEscape(k)}</td><td>{tagsAlteradas[k]}</td></tr>");
            sb.AppendLine("</table>");

            // Diferenças por arquivo (logo após o resumo de tags)
            EscreveDiffPorArquivo(sb, diffsPorArquivo);

            sb.AppendLine("<h2>Valores únicos por tag</h2>");
            EscreveResumoValores(sb, "NOM_PRO (chapas)", valoresNomPro);
            EscreveResumoValores(sb, "SPE_PRO", valoresSpePro);
            EscreveResumoValores(sb, "LUN_PRO", valoresLunPro);
            EscreveResumoValores(sb, "LAR_PRO", valoresLarPro);

            // Inventário espessuras de chapa
            sb.AppendLine("<h2>Inventário de espessuras de chapa existentes</h2>");
            sb.AppendLine($"<p>Espessuras únicas extraídas do NOM_PRO de todas as chapas - {todasChapas.Count} valores únicos.</p>");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>Espessura</th><th>Qtd. em antes</th><th>Qtd. em depois</th></tr>");
            foreach (string k in todasChapas)
            {
                int cA = chapasAntes.TryGetValue(k, out int va) ? va : 0;
                int cD = chapasDepois.TryGetValue(k, out int vd) ? vd : 0;
                sb.AppendLine($"<tr><td>{HtmlEscape(k)}</td><td>{cA}</td><td>{cD}</td></tr>");
            }
            sb.AppendLine("</table>");

            // Inventário perfis de cantoneira
            sb.AppendLine("<h2>Inventário de perfis de cantoneira existentes</h2>");
            sb.AppendLine($"<p>Perfis (aba x espessura) únicos extraídos do NOM_PRO - {todasCantoneiras.Count} valores únicos.</p>");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>Aba x Espessura</th><th>Qtd. em antes</th><th>Qtd. em depois</th></tr>");
            foreach (string k in todasCantoneiras)
            {
                int cA = cantoneirasAntes.TryGetValue(k, out int va) ? va : 0;
                int cD = cantoneirasDepois.TryGetValue(k, out int vd) ? vd : 0;
                sb.AppendLine($"<tr><td>{HtmlEscape(k)}</td><td>{cA}</td><td>{cD}</td></tr>");
            }
            sb.AppendLine("</table>");

            if (soNoAntes.Count > 0 || soNoDepois.Count > 0)
            {
                sb.AppendLine("<h2>Arquivos não-comuns</h2>");
                if (soNoAntes.Count > 0)
                {
                    sb.AppendLine("<h3>Só no Antes</h3><ul>");
                    foreach (string n in soNoAntes)
                        sb.AppendLine($"<li>{HtmlEscape(n)}</li>");
                    sb.AppendLine("</ul>");
                }
                if (soNoDepois.Count > 0)
                {
                    sb.AppendLine("<h3>Só no Depois</h3><ul>");
                    foreach (string n in soNoDepois)
                        sb.AppendLine($"<li>{HtmlEscape(n)}</li>");
                    sb.AppendLine("</ul>");
                }
            }

            sb.AppendLine("</body></html>");
            return sb.ToString();
        }

        private static void EscreveResumoValores(StringBuilder sb, string titulo, Dictionary<string, int> dic)
        {
            if (dic.Count == 0)
                return;

            sb.AppendLine($"<h3>{HtmlEscape(titulo)}</h3>");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>Antes</th><th>Depois</th><th>Δ (max)</th><th>Ocorrências</th></tr>");
            foreach (KeyValuePair<string, int> kvp in dic.OrderByDescending(x => x.Value))
            {
                string[] partes = kvp.Key.Split(" -> ", 2);
                string antes = partes[0];
                string depois = partes.Length > 1 ? partes[1] : "";
                double? delta = ComputaDeltaMax(antes, depois);
                string cls = delta != null && delta > LimiarDelta ? " class='alteracao-grande'" : "";
                sb.AppendLine($"<tr{cls}><td class='antes'>{HtmlEscape(antes)}</td><td class='depois'>{HtmlEscape(depois)}</td><td class='delta'>{FormataDelta(delta)}</td><td>{kvp.Value}</td></tr>");
            }
            sb.AppendLine("</table>");
        }

        private static void EscreveDiffPorArquivo(StringBuilder sb, SortedDictionary<string, List<Mudanca>> diffsPorArquivo)
        {
            sb.AppendLine("<h2>Arquivos alterados (índice)</h2>");
            sb.AppendLine("<div class=\"toc\">");
            foreach (string nome in diffsPorArquivo.Keys)
            {
                string anchor = Anchor(nome);
                sb.AppendLine($"<a href='#{anchor}'>{HtmlEscape(nome)}</a><br>");
            }
            sb.AppendLine("</div>");

            sb.AppendLine("<h2>Diferenças por arquivo</h2>");
            foreach (KeyValuePair<string, List<Mudanca>> kvp in diffsPorArquivo)
            {
                string anchor = Anchor(kvp.Key);
                sb.AppendLine($"<details id='{anchor}'>");
                sb.AppendLine($"<summary>{HtmlEscape(kvp.Key)} ({kvp.Value.Count} linha(s))</summary>");
                sb.AppendLine("<table>");
                sb.AppendLine("<tr><th>Linha</th><th>Antes</th><th>Depois</th><th>Δ (max)</th></tr>");
                foreach (Mudanca m in kvp.Value)
                {
                    double? delta = ComputaDeltaMax(m.Antes, m.Depois);
                    string cls = delta != null && delta > LimiarDelta ? " class='alteracao-grande'" : "";
                    sb.AppendLine($"<tr{cls}><td class='linha'>{m.Linha}</td><td class='antes'>{HtmlEscape(m.Antes)}</td><td class='depois'>{HtmlEscape(m.Depois)}</td><td class='delta'>{FormataDelta(delta)}</td></tr>");
                }
                sb.AppendLine("</table>");
                sb.AppendLine("</details>");
            }
        }

        private static string Anchor(string nome) => Regex.Replace(nome, "[^A-Za-z0-9_]", "_");
    }
}
