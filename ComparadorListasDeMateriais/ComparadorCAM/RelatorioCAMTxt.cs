using System.Text;

namespace ComparadorListasDeMateriais.ComparadorCAM
{
    /// <summary>
    /// Renderiza um <see cref="ResultadoComparacaoCAM"/> como texto (formato do relatório .txt da comparação CAM).
    /// <para>É uma função pura (string in / string out), portanto totalmente testável sem UI nem arquivos.</para>
    /// </summary>
    public static class RelatorioCAMTxt
    {
        public static string Gerar(ResultadoComparacaoCAM pResultado)
        {
            var sb = new StringBuilder();

            sb.AppendLine("RELATÓRIO COMPARAÇÂO ARQUIVOS CAM");
            sb.AppendLine($"Pasta A: {pResultado.CaminhoPastaA}");
            sb.AppendLine($"Pasta B: {pResultado.CaminhoPastaB}");
            sb.AppendLine();
            sb.AppendLine();

            if (pResultado.PosicoesFaltandoNaA.Count > 0)
            {
                sb.AppendLine("Posições não encontradas nos arquivos da Pasta A");
                sb.AppendLine(string.Join(", ", pResultado.PosicoesFaltandoNaA));
                sb.AppendLine();
            }

            if (pResultado.PosicoesFaltandoNaB.Count > 0)
            {
                sb.AppendLine("Posições não encontradas nos arquivos da Pasta B");
                sb.AppendLine(string.Join(", ", pResultado.PosicoesFaltandoNaB));
                sb.AppendLine();
            }

            if (pResultado.TodasPosicoesPresentes)
            {
                sb.AppendLine("Todas posições existem nas duas pastas.");
                sb.AppendLine();
            }

            if (pResultado.Divergencias.Count > 0)
            {
                sb.AppendLine("Divergências encontradas entre os arquivos:");

                foreach (DivergenciaPosicaoCAM divergencia in pResultado.Divergencias)
                {
                    sb.AppendLine($"POS. {divergencia.Posicao}: {string.Join(" / ", divergencia.Diferencas)}");
                }

                sb.AppendLine();
            }

            EscreveBlocoCantoneiras(sb, pResultado.CantoneirasSemAcoGNaA, "A");
            EscreveBlocoCantoneiras(sb, pResultado.CantoneirasSemAcoGNaB, "B");

            EscreveBlocoFuros(sb, pResultado.FurosForaPadraoNaA, "A");
            EscreveBlocoFuros(sb, pResultado.FurosForaPadraoNaB, "B");

            return sb.ToString();
        }

        private static void EscreveBlocoCantoneiras(StringBuilder pSb, System.Collections.Generic.List<string> pCantoneirasSemG, string pPasta)
        {
            if (pCantoneirasSemG.Count > 0)
            {
                pSb.AppendLine($"Cantoneiras sem aço G nos arquivos da Pasta {pPasta}");
                pSb.AppendLine(string.Join(", ", pCantoneirasSemG));
            }
            else
            {
                pSb.AppendLine($"Todas cantoneiras com aço G nos arquivos da Pasta {pPasta}");
            }

            pSb.AppendLine();
        }

        private static void EscreveBlocoFuros(StringBuilder pSb, System.Collections.Generic.List<string> pFurosFora, string pPasta)
        {
            if (pFurosFora.Count > 0)
            {
                pSb.AppendLine($"Peças com furo diferente do padrão da fábrica nos arquivos da Pasta {pPasta}");
                pSb.AppendLine(string.Join(", ", pFurosFora));
            }
            else
            {
                pSb.AppendLine($"Todas peças com furo dentro do padrão da fábrica nos arquivos da Pasta {pPasta}");
            }

            pSb.AppendLine();
        }
    }
}
