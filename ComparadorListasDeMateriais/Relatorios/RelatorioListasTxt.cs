using ComparadorListasDeMateriais.ObjetosResultados;
using ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias;
using System.Linq;
using System.Text;

namespace ComparadorListasDeMateriais.Relatorios
{
    /// <summary>
    /// Gera o texto do relatório de comparação de listas de materiais.
    /// <para>
    /// Função pura (recebe o <see cref="ObjetoResultadoComparacao"/> e devolve string), portanto
    /// totalmente testável sem UI nem arquivos. A escrita em disco fica em <see cref="FuncoesUteis.EscreveTxt"/>.
    /// </para>
    /// </summary>
    public static class RelatorioListasTxt
    {
        public static string Gerar(ObjetoResultadoComparacao pResultadoComparacao)
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.AppendLine(pResultadoComparacao.ObjetoCabecalho.EscreveCabecalho());

            foreach (EstruturaComparacao estrutura in pResultadoComparacao.ListaEstruturasComparadas)
            {
                strBuilder.AppendLine(estrutura.NomeEstruturaSaida);
                strBuilder.AppendLine();

                if (estrutura.PosicoesSomenteListaOriginal.Count > 0)
                {
                    strBuilder.AppendLine(string.Format("   Posições não encontradas na lista original: {0}", string.Join(", ", estrutura.PosicoesSomenteListaOriginal.Distinct())));
                    strBuilder.AppendLine();
                }

                if (estrutura.PosicoesSomenteListaNova.Count > 0)
                {
                    strBuilder.AppendLine(string.Format("   Posições não encontradas na lista nova: {0}", string.Join(", ", estrutura.PosicoesSomenteListaNova.Distinct())));
                    strBuilder.AppendLine();
                }

                StringBuilder divergencias = new StringBuilder();

                StringBuilder melhorias = new StringBuilder();

                bool temDivergencia = false;
                bool temMelhoria = false;

                foreach (PosicaoComparacao posicao in estrutura.ListaPosicoesComErros)
                {
                    if (posicao.EscreveAlteracoes(true, out string alteracoes))
                    {
                        divergencias.AppendLine(alteracoes);
                        temDivergencia = true;
                    }
                }

                foreach (PosicaoComparacao posicao in estrutura.ListaPosicoesComErros)
                {
                    if (posicao.EscreveAlteracoes(false, out string alteracoes))
                    {
                        melhorias.AppendLine(alteracoes);
                        temMelhoria = true;
                    }
                }

                if (temDivergencia)
                {
                    strBuilder.AppendLine("   Divergências:");
                    strBuilder.AppendLine();

                    strBuilder.Append(divergencias.ToString());
                }

                if (temMelhoria)
                {
                    strBuilder.AppendLine("   Melhorias:");
                    strBuilder.AppendLine();

                    strBuilder.AppendLine(melhorias.ToString());
                }

                if (!temMelhoria && !temDivergencia)
                {
                    strBuilder.AppendLine("   Todas as posições na estrutura estão iguais entre as listas");
                }
            }

            if (pResultadoComparacao.ListaDivergenciasMesmaLista.Count > 0)
            {
                strBuilder.AppendLine();
                strBuilder.AppendLine("Posições que estão com informações divergente entre estruturas de uma mesma lista:");

                foreach (DivergenciaEntreEstruturaLista divergencia in pResultadoComparacao.ListaDivergenciasMesmaLista)
                {
                    strBuilder.AppendLine(string.Format("   • {0} na {1} : {2}", divergencia.Posicao, divergencia.ListaErrada, string.Join(", ", divergencia.EstruturasQueContem)));
                }
            }

            if (pResultadoComparacao.EstruturasSomenteListaOriginal.Count > 0)
            {
                strBuilder.AppendLine("Estruturas que estão na Lista Original e não foram encontradas na Lista Nova:");

                foreach (string estrutura in pResultadoComparacao.EstruturasSomenteListaOriginal)
                {
                    strBuilder.AppendLine(string.Format("   • {0}", estrutura));
                }
            }

            if (pResultadoComparacao.EstruturasSomenteListaNova.Count > 0)
            {
                strBuilder.AppendLine("Estruturas que estão na Lista Nova e não foram encontradas na Lista Original:");

                foreach (string estrutura in pResultadoComparacao.EstruturasSomenteListaNova)
                {
                    strBuilder.AppendLine(string.Format("   • {0}", estrutura));
                }
            }

            return strBuilder.ToString();
        }
    }
}
