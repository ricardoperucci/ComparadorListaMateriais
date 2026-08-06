using ComparadorListasDeMateriais.ObjetosResultados;
using ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparadorListasDeMateriais.ComparadorV2
{
    /// <summary>
    /// Renderiza o relatório .txt da comparação v2.
    /// <para>
    /// Função pura (recebe o resultado, devolve string): testável sem UI e sem disco. A gravação
    /// em arquivo fica em <see cref="ArquivosRelatorioV2"/>, que grava em UTF-8 com BOM — o
    /// relatório usa ═ ─ ■ → ≤, que não existem em Windows-1252.
    /// </para>
    /// </summary>
    public static class RelatorioV2Txt
    {
        private const int Largura = 80;

        private static readonly string ReguaDupla = new string('═', Largura);
        private static readonly string ReguaSimples = new string('─', Largura);

        public static string Gerar(ResultadoComparacaoV2 pResultado)
        {
            var sb = new StringBuilder();

            EscreveCabecalho(sb, pResultado);
            EscreveResumo(sb, pResultado.Resumo());

            foreach (EstruturaComparacaoV2 estrutura in pResultado.Estruturas)
                EscreveEstrutura(sb, estrutura);

            EscreveAvisosGerais(sb, pResultado);

            return sb.ToString();
        }

        // =====================================================================
        //  Blocos
        // =====================================================================

        private static void EscreveCabecalho(StringBuilder sb, ResultadoComparacaoV2 pResultado)
        {
            sb.AppendLine(ReguaDupla);
            sb.AppendLine("  COMPARAÇÃO DE LISTAS DE MATERIAIS");
            sb.AppendLine(ReguaDupla);
            sb.AppendLine();
            sb.AppendLine("  Lista original   " + pResultado.NomeListaOriginal);
            sb.AppendLine("  Lista nova       " + pResultado.NomeListaNova);
            sb.AppendLine("  Gerado em        " + pResultado.GeradoEm.ToString("dd/MM/yyyy HH:mm"));
            sb.AppendLine();
        }

        private static void EscreveResumo(StringBuilder sb, ResumoComparacao pResumo)
        {
            sb.AppendLine(ReguaSimples);
            sb.AppendLine("  RESUMO");
            sb.AppendLine(ReguaSimples);
            sb.AppendLine();

            var linhas = new[]
            {
                new[] { Contador("Estruturas comparadas", pResumo.EstruturasComparadas), Contador("Posições novas", pResumo.PosicoesNovas) },
                new[] { Contador("Posições conferidas", pResumo.PosicoesConferidas), Contador("Posições eliminadas", pResumo.PosicoesEliminadas) },
                new[] { Contador("Posições iguais", pResumo.PosicoesIguais), Contador("Estruturas só na original", pResumo.EstruturasSomenteOriginal) },
                new[] { Contador("Posições alteradas", pResumo.PosicoesAlteradas), Contador("Estruturas só na nova", pResumo.EstruturasSomenteNova) },
                new[] { Contador("Material trocado", pResumo.MaterialTrocado), Contador("Revisar manualmente", pResumo.GruposAmbiguos) },
                new[] { Contador("Diferenças pequenas", pResumo.SomenteDiferencasPequenas), string.Empty }
            };

            foreach (string[] linha in linhas)
                sb.AppendLine(("  " + linha[0].PadRight(38) + linha[1]).TrimEnd());

            sb.AppendLine();

            sb.AppendLine(pResumo.PrecisamAtencao == 0
                ? "  >> As duas listas estão iguais nas estruturas comparadas."
                : "  >> " + pResumo.PrecisamAtencao + " posições precisam de atenção nesta comparação.");

            sb.AppendLine();
        }

        private static void EscreveEstrutura(StringBuilder sb, EstruturaComparacaoV2 pEstrutura)
        {
            sb.AppendLine();
            sb.AppendLine(ReguaDupla);
            sb.AppendLine("  " + pEstrutura.NomeEstrutura);
            sb.AppendLine(ReguaDupla);
            sb.AppendLine();

            if (pEstrutura.SemDiferencas)
            {
                sb.AppendLine("  Nenhuma diferença. " + PosicoesIguais(pEstrutura.Pares.Count));
                sb.AppendLine();
                return;
            }

            EscreveSecaoPares(sb, "MATERIAL TROCADO", pEstrutura.MaterialTrocado);
            EscreveSecaoPares(sb, "POSIÇÕES ALTERADAS", pEstrutura.Alteradas);
            EscreveSecaoDiferencasPequenas(sb, pEstrutura.ComDiferencasPequenas);
            EscreveSecaoLista(sb, "POSIÇÕES NOVAS", pEstrutura.PosicoesNovas);
            EscreveSecaoLista(sb, "POSIÇÕES ELIMINADAS", pEstrutura.PosicoesEliminadas);
            EscreveSecaoAmbiguos(sb, pEstrutura.Ambiguos);
        }

        private static void EscreveSecaoPares(StringBuilder sb, string pTitulo, List<ParPosicoes> pPares)
        {
            if (pPares.Count == 0)
                return;

            EscreveTituloSecao(sb, pTitulo, pPares.Count);
            sb.AppendLine();

            int larguraNumeracao = pPares.Max(x => x.NumeracaoOriginal.Length);

            foreach (ParPosicoes par in pPares)
            {
                sb.AppendLine(par.Tipo == TipoPar.MaterialTrocado
                    ? "      " + par.NumeracaoOriginal.PadRight(larguraNumeracao) + "  →  " + par.NumeracaoNova
                    : "      " + par.NumeracaoOriginal);

                foreach (ErroPosicao erro in par.Divergencias)
                    sb.AppendLine(LinhaDivergencia(erro));
            }

            sb.AppendLine();
        }

        private static void EscreveSecaoDiferencasPequenas(StringBuilder sb, List<ParPosicoes> pPares)
        {
            if (pPares.Count == 0)
                return;

            EscreveTituloSecao(sb, "DIFERENÇAS PEQUENAS (≤ 2 mm)", pPares.Count);
            sb.AppendLine();

            foreach (ParPosicoes par in pPares)
            {
                sb.AppendLine("      " + par.NumeracaoNova);

                foreach (ErroPosicao erro in par.DiferencasPequenas)
                    sb.AppendLine(LinhaDivergencia(erro));
            }

            sb.AppendLine();
        }

        private static void EscreveSecaoLista(StringBuilder sb, string pTitulo, List<string> pItens)
        {
            if (pItens.Count == 0)
                return;

            string titulo = TituloSecao(pTitulo, pItens.Count);
            string juntos = string.Join(", ", pItens);

            // Lista curta cabe na mesma linha do título; lista longa vira um bloco quebrado.
            if (titulo.Length + 6 + juntos.Length <= Largura)
            {
                sb.AppendLine(titulo + "      " + juntos);
                sb.AppendLine();
                return;
            }

            sb.AppendLine(titulo);
            sb.AppendLine();

            foreach (string linha in QuebrarEmLinhas(pItens, Largura - 6))
                sb.AppendLine("      " + linha);

            sb.AppendLine();
        }

        private static void EscreveSecaoAmbiguos(StringBuilder sb, List<GrupoAmbiguo> pAmbiguos)
        {
            if (pAmbiguos.Count == 0)
                return;

            EscreveTituloSecao(sb, "REVISAR MANUALMENTE", pAmbiguos.Count);
            sb.AppendLine();

            foreach (GrupoAmbiguo grupo in pAmbiguos)
            {
                sb.AppendLine("      Numeração " + grupo.ChaveBase +
                              " — original tem " + string.Join(", ", grupo.CandidatosOriginal) +
                              " | nova tem " + string.Join(", ", grupo.CandidatosNova));
            }

            sb.AppendLine();
            sb.AppendLine("      Mais de um candidato para a mesma numeração; confira se houve troca de material.");
            sb.AppendLine();
        }

        private static void EscreveAvisosGerais(StringBuilder sb, ResultadoComparacaoV2 pResultado)
        {
            bool temAviso = pResultado.EstruturasSomenteOriginal.Count > 0 ||
                            pResultado.EstruturasSomenteNova.Count > 0 ||
                            pResultado.DivergenciasMesmaLista.Count > 0;

            if (!temAviso)
                return;

            sb.AppendLine();
            sb.AppendLine(ReguaDupla);
            sb.AppendLine("  AVISOS GERAIS");
            sb.AppendLine(ReguaDupla);
            sb.AppendLine();

            EscreveSecaoLista(sb, "ESTRUTURAS SÓ NA LISTA ORIGINAL", pResultado.EstruturasSomenteOriginal);
            EscreveSecaoLista(sb, "ESTRUTURAS SÓ NA LISTA NOVA", pResultado.EstruturasSomenteNova);

            if (pResultado.DivergenciasMesmaLista.Count == 0)
                return;

            EscreveTituloSecao(sb, "POSIÇÕES DIVERGENTES ENTRE ESTRUTURAS DA MESMA LISTA",
                pResultado.DivergenciasMesmaLista.Count);
            sb.AppendLine();

            foreach (DivergenciaEntreEstruturaLista divergencia in pResultado.DivergenciasMesmaLista)
            {
                sb.AppendLine("      " + divergencia.Posicao + "  na " + divergencia.ListaErrada + ": " +
                              string.Join(", ", divergencia.EstruturasQueContem));
            }

            sb.AppendLine();
        }

        // =====================================================================
        //  Auxiliares de formatação
        // =====================================================================

        private static string LinhaDivergencia(ErroPosicao pErro)
        {
            DivergenciaFormatada f = FormatadorDivergenciaV2.Formatar(pErro);

            if (!f.TemValores)
                return "            • " + f.Rotulo;

            string linha = "            • " + (f.Rotulo + " ").PadRight(20, '.') + "  " +
                           f.ValorOriginal.PadRight(9) + "→  " + f.ValorNovo.PadRight(9);

            return f.TemDelta ? (linha + " (" + f.Delta + ")") : linha.TrimEnd();
        }

        private static void EscreveTituloSecao(StringBuilder sb, string pTitulo, int pQuantidade)
        {
            sb.AppendLine(TituloSecao(pTitulo, pQuantidade));
        }

        private static string TituloSecao(string pTitulo, int pQuantidade)
        {
            return "  ■ " + pTitulo + "  (" + pQuantidade + ")";
        }

        /// <summary>"1 posição está igual" / "87 posições estão iguais" — concordância certa nos dois casos.</summary>
        internal static string PosicoesIguais(int pQuantidade)
        {
            return pQuantidade == 1
                ? "1 posição está igual nas duas listas."
                : pQuantidade + " posições estão iguais nas duas listas.";
        }

        private static string Contador(string pRotulo, int pValor)
        {
            return (pRotulo + " ").PadRight(29, '.') + pValor.ToString().PadLeft(4);
        }

        /// <summary>Junta os itens em linhas separadas por vírgula, sem passar da largura pedida.</summary>
        private static List<string> QuebrarEmLinhas(List<string> pItens, int pLargura)
        {
            var linhas = new List<string>();
            var atual = new StringBuilder();

            foreach (string item in pItens)
            {
                string proximo = atual.Length == 0 ? item : ", " + item;

                if (atual.Length + proximo.Length > pLargura && atual.Length > 0)
                {
                    linhas.Add(atual.ToString() + ",");
                    atual.Clear();
                    proximo = item;
                }

                atual.Append(proximo);
            }

            if (atual.Length > 0)
                linhas.Add(atual.ToString());

            return linhas;
        }
    }
}
