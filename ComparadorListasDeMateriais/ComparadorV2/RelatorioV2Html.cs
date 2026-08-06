using ComparadorListasDeMateriais.ObjetosResultados;
using ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ComparadorListasDeMateriais.ComparadorV2
{
    /// <summary>
    /// Renderiza o relatório HTML da comparação v2: mesmas seções do .txt em formato web, mais o
    /// lado a lado das duas listas no fim.
    /// <para>
    /// Arquivo único, sem CSS nem JS externo — abre offline e pode ser mandado por e-mail.
    /// </para>
    /// </summary>
    public static class RelatorioV2Html
    {
        /// <summary>Linhas iguais mostradas em volta de cada trecho alterado.</summary>
        private const int LinhasContexto = 3;

        public static string Gerar(ResultadoComparacaoV2 pResultado, TextoLista pTextoOriginal, TextoLista pTextoNova)
        {
            var sb = new StringBuilder();

            ResumoComparacao resumo = pResultado.Resumo();

            EscreveAbertura(sb, pResultado);
            EscreveResumo(sb, resumo);

            foreach (EstruturaComparacaoV2 estrutura in pResultado.Estruturas)
                EscreveEstrutura(sb, estrutura);

            EscreveAvisosGerais(sb, pResultado);
            EscreveLadoALado(sb, pResultado, pTextoOriginal, pTextoNova);

            sb.AppendLine("</main>");
            sb.AppendLine("<script>");
            sb.AppendLine("function abrirDobra(id){document.querySelectorAll('tr.'+id).forEach(function(r){r.classList.remove('oculta');});");
            sb.AppendLine("var d=document.querySelector('tr[data-dobra=\"'+id+'\"]');if(d){d.style.display='none';}}");
            sb.AppendLine("function mostrarTudo(){document.querySelectorAll('tr.oculta').forEach(function(r){r.classList.remove('oculta');});");
            sb.AppendLine("document.querySelectorAll('tr.dobra').forEach(function(r){r.style.display='none';});}");
            sb.AppendLine("</script>");
            sb.AppendLine("</body></html>");

            return sb.ToString();
        }

        // =====================================================================
        //  Cabeçalho e estilo
        // =====================================================================

        private static void EscreveAbertura(StringBuilder sb, ResultadoComparacaoV2 pResultado)
        {
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"pt-br\"><head><meta charset=\"utf-8\">");
            sb.AppendLine("<title>Comparação de listas de materiais</title>");
            sb.AppendLine("<style>");
            sb.AppendLine(Estilo());
            sb.AppendLine("</style></head><body>");

            sb.AppendLine("<header>");
            sb.AppendLine("  <h1>Comparação de listas de materiais</h1>");
            sb.AppendLine("  <p class=\"sub\">" + E(pResultado.NomeListaOriginal) + " &rarr; " +
                          E(pResultado.NomeListaNova) + " &middot; " +
                          pResultado.GeradoEm.ToString("dd/MM/yyyy HH:mm") + "</p>");
            sb.AppendLine("</header>");
        }

        private static string Estilo()
        {
            return string.Join("\n", new[]
            {
                "*{box-sizing:border-box}",
                "body{margin:0;padding:0 0 60px;font-family:'Segoe UI',Arial,sans-serif;color:#202225;background:#fff;font-size:14px}",
                "header{background:#EFF1F4;border-bottom:2px solid #1A73C7;padding:20px 32px}",
                "h1{margin:0;font-size:21px;font-weight:600}",
                "h2{margin:32px 0 10px;font-size:17px;font-weight:600;border-bottom:1px solid #E3E6EB;padding-bottom:6px}",
                "h3{margin:18px 0 8px;font-size:14px;font-weight:600;color:#555A60;text-transform:uppercase;letter-spacing:.03em}",
                ".sub{margin:4px 0 0;color:#555A60;font-size:13px}",
                "main{padding:0 32px}",
                ".cards{display:flex;flex-wrap:wrap;gap:10px;margin:20px 0 8px}",
                ".card{background:#F3F4F6;border-radius:8px;padding:10px 16px;min-width:132px}",
                ".card .rot{font-size:12px;color:#555A60}",
                ".card .num{font-size:22px;font-weight:600;margin-top:2px}",
                ".card.alerta{background:#FFF4E5}.card.alerta .rot,.card.alerta .num{color:#8A5300}",
                ".card.nova{background:#E7F4EA}.card.nova .rot,.card.nova .num{color:#1E6B33}",
                ".card.saiu{background:#FDECEA}.card.saiu .rot,.card.saiu .num{color:#B3261E}",
                ".aviso{background:#FFF4E5;border-left:3px solid #BA7517;padding:8px 12px;margin:12px 0;color:#8A5300;font-size:13px}",
                ".ok{color:#1E6B33;margin:8px 0}",
                "table.itens{border-collapse:collapse;width:100%;margin-bottom:6px}",
                "table.itens td{border-bottom:1px solid #E3E6EB;padding:7px 8px;vertical-align:top}",
                "td.tag{width:118px}",
                "span.badge{display:inline-block;font-size:11px;padding:2px 8px;border-radius:8px;white-space:nowrap}",
                "span.badge.mat{background:#FFF4E5;color:#8A5300}",
                "span.badge.alt{background:#E6F1FB;color:#185FA5}",
                "span.badge.peq{background:#F3F4F6;color:#555A60}",
                "code{font-family:Consolas,'Courier New',monospace;font-size:13px}",
                ".troca code{font-weight:600}",
                ".det{color:#555A60;font-size:13px;margin-top:3px}",
                ".det b{font-weight:600;color:#202225}",
                ".delta{color:#B3261E}",
                ".chips code{background:#F3F4F6;border-radius:4px;padding:2px 6px;margin:0 4px 4px 0;display:inline-block}",
                ".chips.nova code{background:#E7F4EA;color:#1E6B33}",
                ".chips.saiu code{background:#FDECEA;color:#B3261E}",
                "table.diff{border-collapse:collapse;width:100%;font-family:Consolas,'Courier New',monospace;font-size:12px;line-height:1.55}",
                "table.diff td{padding:0 8px;white-space:pre;vertical-align:top}",
                "table.diff td.n{width:52px;text-align:right;color:#7A8088;background:#F3F4F6;user-select:none}",
                "table.diff td.dir{border-left:1px solid #C6CCD4}",
                "tr.rem td.esq{background:#FDECEA;color:#B3261E}tr.rem td.dir{background:#FAFAFA}",
                "tr.ins td.dir{background:#E7F4EA;color:#1E6B33}tr.ins td.esq{background:#FAFAFA}",
                "tr.mod td.esq{background:#FDECEA;color:#B3261E}tr.mod td.dir{background:#E7F4EA;color:#1E6B33}",
                "tr.oculta{display:none}",
                "tr.dobra td{background:#F3F4F6;color:#555A60;text-align:center;padding:4px}",
                "button{font-family:inherit;font-size:12px;color:#1A73C7;background:none;border:none;cursor:pointer;text-decoration:underline}",
                ".barra{display:flex;justify-content:space-between;align-items:center;margin:10px 0 6px;font-size:13px;color:#555A60}"
            });
        }

        // =====================================================================
        //  Resumo
        // =====================================================================

        private static void EscreveResumo(StringBuilder sb, ResumoComparacao pResumo)
        {
            sb.AppendLine("<main>");
            sb.AppendLine("<div class=\"cards\">");

            Card(sb, "", "Posições alteradas", pResumo.PosicoesAlteradas);
            Card(sb, "alerta", "Material trocado", pResumo.MaterialTrocado);
            Card(sb, "nova", "Posições novas", pResumo.PosicoesNovas);
            Card(sb, "saiu", "Posições eliminadas", pResumo.PosicoesEliminadas);
            Card(sb, "", "Estruturas", pResumo.EstruturasComparadas);
            Card(sb, "", "Posições iguais", pResumo.PosicoesIguais);

            sb.AppendLine("</div>");

            sb.AppendLine(pResumo.PrecisamAtencao == 0
                ? "<p class=\"ok\">As duas listas estão iguais nas estruturas comparadas.</p>"
                : "<p class=\"sub\">" + pResumo.PrecisamAtencao + " posições precisam de atenção · " +
                  pResumo.PosicoesConferidas + " posições conferidas · " +
                  pResumo.SomenteDiferencasPequenas + " com diferença pequena (≤ 2 mm) · " +
                  pResumo.GruposAmbiguos + " para revisar manualmente</p>");
        }

        private static void Card(StringBuilder sb, string pClasse, string pRotulo, int pValor)
        {
            sb.AppendLine("  <div class=\"card " + pClasse + "\"><div class=\"rot\">" + E(pRotulo) +
                          "</div><div class=\"num\">" + pValor + "</div></div>");
        }

        // =====================================================================
        //  Estruturas
        // =====================================================================

        private static void EscreveEstrutura(StringBuilder sb, EstruturaComparacaoV2 pEstrutura)
        {
            sb.AppendLine("<h2>" + E(pEstrutura.NomeEstrutura) + "</h2>");

            if (pEstrutura.SemDiferencas)
            {
                sb.AppendLine("<p class=\"ok\">Nenhuma diferença. " +
                              E(RelatorioV2Txt.PosicoesIguais(pEstrutura.Pares.Count)) + "</p>");
                return;
            }

            var itens = new List<ParPosicoes>();
            itens.AddRange(pEstrutura.MaterialTrocado);
            itens.AddRange(pEstrutura.Alteradas);

            if (itens.Count > 0)
            {
                sb.AppendLine("<table class=\"itens\">");

                foreach (ParPosicoes par in itens)
                    EscreveItem(sb, par, par.Divergencias, par.Tipo == TipoPar.MaterialTrocado ? "mat" : "alt",
                        par.Tipo == TipoPar.MaterialTrocado ? "material" : "alterada");

                sb.AppendLine("</table>");
            }

            List<ParPosicoes> pequenas = pEstrutura.ComDiferencasPequenas;
            if (pequenas.Count > 0)
            {
                sb.AppendLine("<h3>Diferenças pequenas (≤ 2 mm)</h3>");
                sb.AppendLine("<table class=\"itens\">");

                foreach (ParPosicoes par in pequenas)
                    EscreveItem(sb, par, par.DiferencasPequenas, "peq", "≤ 2 mm");

                sb.AppendLine("</table>");
            }

            EscreveChips(sb, "Posições novas", pEstrutura.PosicoesNovas, "nova");
            EscreveChips(sb, "Posições eliminadas", pEstrutura.PosicoesEliminadas, "saiu");

            if (pEstrutura.Ambiguos.Count == 0)
                return;

            sb.AppendLine("<h3>Revisar manualmente</h3>");

            foreach (GrupoAmbiguo grupo in pEstrutura.Ambiguos)
            {
                sb.AppendLine("<div class=\"aviso\">Numeração <b>" + E(grupo.ChaveBase) + "</b> — original tem " +
                              E(string.Join(", ", grupo.CandidatosOriginal)) + ", nova tem " +
                              E(string.Join(", ", grupo.CandidatosNova)) +
                              ". Mais de um candidato para a mesma numeração; confira se houve troca de material.</div>");
            }
        }

        private static void EscreveItem(StringBuilder sb, ParPosicoes pPar, List<ErroPosicao> pErros,
            string pClasseBadge, string pTextoBadge)
        {
            sb.AppendLine("<tr>");
            sb.AppendLine("  <td class=\"tag\"><span class=\"badge " + pClasseBadge + "\">" + E(pTextoBadge) + "</span></td>");
            sb.Append("  <td>");

            sb.Append(pPar.Tipo == TipoPar.MaterialTrocado
                ? "<span class=\"troca\"><code>" + E(pPar.NumeracaoOriginal) + "</code> &rarr; <code>" + E(pPar.NumeracaoNova) + "</code></span>"
                : "<code>" + E(pPar.NumeracaoNova) + "</code>");

            foreach (ErroPosicao erro in pErros)
            {
                DivergenciaFormatada f = FormatadorDivergenciaV2.Formatar(erro);

                sb.Append("<div class=\"det\">" + E(f.Rotulo));

                if (f.TemValores)
                    sb.Append(" <b>" + E(f.ValorOriginal) + "</b> &rarr; <b>" + E(f.ValorNovo) + "</b>");

                if (f.TemDelta)
                    sb.Append(" <span class=\"delta\">(" + E(f.Delta) + ")</span>");

                sb.Append("</div>");
            }

            sb.AppendLine("</td>");
            sb.AppendLine("</tr>");
        }

        private static void EscreveChips(StringBuilder sb, string pTitulo, List<string> pItens, string pClasse)
        {
            if (pItens.Count == 0)
                return;

            sb.AppendLine("<h3>" + E(pTitulo) + " (" + pItens.Count + ")</h3>");
            sb.AppendLine("<div class=\"chips " + pClasse + "\">");

            foreach (string item in pItens)
                sb.Append("<code>" + E(item) + "</code>");

            sb.AppendLine("</div>");
        }

        private static void EscreveAvisosGerais(StringBuilder sb, ResultadoComparacaoV2 pResultado)
        {
            bool temAviso = pResultado.EstruturasSomenteOriginal.Count > 0 ||
                            pResultado.EstruturasSomenteNova.Count > 0 ||
                            pResultado.DivergenciasMesmaLista.Count > 0;

            if (!temAviso)
                return;

            sb.AppendLine("<h2>Avisos gerais</h2>");

            EscreveChips(sb, "Estruturas só na lista original", pResultado.EstruturasSomenteOriginal, "saiu");
            EscreveChips(sb, "Estruturas só na lista nova", pResultado.EstruturasSomenteNova, "nova");

            if (pResultado.DivergenciasMesmaLista.Count == 0)
                return;

            sb.AppendLine("<h3>Posições divergentes entre estruturas da mesma lista</h3>");
            sb.AppendLine("<table class=\"itens\">");

            foreach (DivergenciaEntreEstruturaLista divergencia in pResultado.DivergenciasMesmaLista)
            {
                sb.AppendLine("<tr><td class=\"tag\"><code>" + E(divergencia.Posicao) + "</code></td><td>" +
                              E(divergencia.ListaErrada) + ": " +
                              E(string.Join(", ", divergencia.EstruturasQueContem)) + "</td></tr>");
            }

            sb.AppendLine("</table>");
        }

        // =====================================================================
        //  Lado a lado
        // =====================================================================

        private static void EscreveLadoALado(StringBuilder sb, ResultadoComparacaoV2 pResultado,
            TextoLista pTextoOriginal, TextoLista pTextoNova)
        {
            if (pTextoOriginal == null || pTextoNova == null)
                return;

            if (pTextoOriginal.Linhas.Count == 0 && pTextoNova.Linhas.Count == 0)
                return;

            sb.AppendLine("<h2>Listas lado a lado</h2>");

            if (pTextoOriginal.Renderizado || pTextoNova.Renderizado)
            {
                sb.AppendLine("<div class=\"aviso\">Uma das listas está em Excel, que não tem texto cru. " +
                              "O que aparece abaixo é uma renderização das linhas da aba MATERIAIS, não o arquivo original.</div>");
            }

            ResultadoDiffLinhas diff = DiffLinhas.Comparar(pTextoOriginal.Linhas, pTextoNova.Linhas);

            if (diff.AlinhamentoDegradado)
            {
                sb.AppendLine("<div class=\"aviso\">As duas listas são diferentes demais para o alinhamento fino. " +
                              "As linhas abaixo foram pareadas por posição, então trechos podem aparecer deslocados.</div>");
            }

            sb.AppendLine("<div class=\"barra\"><span>" + E(pResultado.NomeListaOriginal) + " &nbsp;·&nbsp; " +
                          E(pResultado.NomeListaNova) + " &nbsp;—&nbsp; " + diff.Alteradas + " alteradas, " +
                          diff.Inseridas + " inseridas, " + diff.Removidas + " removidas</span>" +
                          "<button onclick=\"mostrarTudo()\">mostrar todas as linhas</button></div>");

            sb.AppendLine("<table class=\"diff\">");

            foreach (string linha in LinhasDoDiff(diff))
                sb.AppendLine(linha);

            sb.AppendLine("</table>");
        }

        /// <summary>
        /// Monta as linhas da tabela, colapsando trechos longos de linhas iguais para o arquivo não
        /// virar um paredão. Cada trecho colapsado pode ser aberto individualmente.
        /// </summary>
        private static List<string> LinhasDoDiff(ResultadoDiffLinhas pDiff)
        {
            var saida = new List<string>();

            int i = 0;
            int dobra = 0;

            while (i < pDiff.Linhas.Count)
            {
                if (pDiff.Linhas[i].Tipo != TipoLinhaDiff.Igual)
                {
                    saida.Add(LinhaTabela(pDiff.Linhas[i], string.Empty));
                    i++;
                    continue;
                }

                int inicio = i;
                while (i < pDiff.Linhas.Count && pDiff.Linhas[i].Tipo == TipoLinhaDiff.Igual)
                    i++;

                int quantidade = i - inicio;

                // Trecho curto: mostra inteiro, colapsar só atrapalharia.
                if (quantidade <= 2 * LinhasContexto + 1)
                {
                    for (int k = inicio; k < i; k++)
                        saida.Add(LinhaTabela(pDiff.Linhas[k], string.Empty));

                    continue;
                }

                dobra++;
                string id = "dobra" + dobra;

                int contextoInicial = inicio == 0 ? 0 : LinhasContexto;
                int contextoFinal = i == pDiff.Linhas.Count ? 0 : LinhasContexto;

                for (int k = inicio; k < inicio + contextoInicial; k++)
                    saida.Add(LinhaTabela(pDiff.Linhas[k], string.Empty));

                int primeiraOculta = inicio + contextoInicial;
                int ultimaOculta = i - contextoFinal;

                saida.Add("<tr class=\"dobra\" data-dobra=\"" + id + "\"><td colspan=\"4\">" +
                          "<button onclick=\"abrirDobra('" + id + "')\">… " + (ultimaOculta - primeiraOculta) +
                          " linhas iguais …</button></td></tr>");

                for (int k = primeiraOculta; k < ultimaOculta; k++)
                    saida.Add(LinhaTabela(pDiff.Linhas[k], "oculta " + id));

                for (int k = ultimaOculta; k < i; k++)
                    saida.Add(LinhaTabela(pDiff.Linhas[k], string.Empty));
            }

            return saida;
        }

        private static string LinhaTabela(LinhaDiff pLinha, string pClasseExtra)
        {
            string classe = ClasseLinha(pLinha.Tipo);

            string todas = string.Join(" ", new[] { classe, pClasseExtra }.Where(x => x.Length > 0));
            string atributoClasse = todas.Length > 0 ? " class=\"" + todas + "\"" : string.Empty;

            return "<tr" + atributoClasse + ">" +
                   "<td class=\"n\">" + Numero(pLinha.NumeroOriginal) + "</td>" +
                   "<td class=\"esq\">" + E(pLinha.TextoOriginal) + "</td>" +
                   "<td class=\"n\">" + Numero(pLinha.NumeroNova) + "</td>" +
                   "<td class=\"dir\">" + E(pLinha.TextoNova) + "</td>" +
                   "</tr>";
        }

        private static string ClasseLinha(TipoLinhaDiff pTipo)
        {
            switch (pTipo)
            {
                case TipoLinhaDiff.Alterada: return "mod";
                case TipoLinhaDiff.Removida: return "rem";
                case TipoLinhaDiff.Inserida: return "ins";
                default: return string.Empty;
            }
        }

        private static string Numero(int pNumero)
        {
            return pNumero > 0 ? pNumero.ToString() : "&nbsp;";
        }

        private static string E(string pTexto)
        {
            return WebUtility.HtmlEncode(pTexto ?? string.Empty);
        }
    }
}
