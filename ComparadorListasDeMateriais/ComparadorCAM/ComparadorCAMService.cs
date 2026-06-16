using Hefesto.CAM;
using Hefesto.CAM.ComparacaoArquivos;
using ModelagemTorre;
using System.Collections.Generic;
using System.Linq;

namespace ComparadorListasDeMateriais.ComparadorCAM
{
    /// <summary>
    /// Orquestra a comparação de arquivos CAM entre duas pastas, produzindo um <see cref="ResultadoComparacaoCAM"/>.
    /// <para>
    /// Não possui dependência de UI (WinForms/MessageBox) — todas as escolhas chegam por
    /// <see cref="OpcoesComparacaoCAM"/>. A leitura dos arquivos é delegada a um <see cref="ILeitorPastaCAM"/>,
    /// o que permite testar a orquestração com leitores falsos.
    /// </para>
    /// </summary>
    public class ComparadorCAMService
    {
        private readonly ILeitorPastaCAM _leitor;

        public ComparadorCAMService(ILeitorPastaCAM pLeitor)
        {
            _leitor = pLeitor;
        }

        /// <summary>
        /// Lê as duas pastas e compara seus arquivos CAM.
        /// </summary>
        public ResultadoComparacaoCAM Comparar(string pCaminhoPastaA, string pCaminhoPastaB, OpcoesComparacaoCAM pOpcoes)
        {
            Dictionary<string, ObjetoLeituraCAMTxt> dicA = _leitor.LerPasta(pCaminhoPastaA, pOpcoes.FabricanteA);
            Dictionary<string, ObjetoLeituraCAMTxt> dicB = _leitor.LerPasta(pCaminhoPastaB, pOpcoes.FabricanteB);

            return Comparar(dicA, dicB, pOpcoes, pCaminhoPastaA, pCaminhoPastaB);
        }

        /// <summary>
        /// Sobrecarga testável: compara dicionários já carregados (permite injetar dados de teste
        /// sem tocar no sistema de arquivos).
        /// </summary>
        public ResultadoComparacaoCAM Comparar(
            Dictionary<string, ObjetoLeituraCAMTxt> pDicA,
            Dictionary<string, ObjetoLeituraCAMTxt> pDicB,
            OpcoesComparacaoCAM pOpcoes,
            string pCaminhoPastaA = null,
            string pCaminhoPastaB = null)
        {
            var resultado = new ResultadoComparacaoCAM
            {
                CaminhoPastaA = pCaminhoPastaA,
                CaminhoPastaB = pCaminhoPastaB
            };

            RegrasComparacaoCAM.PosicoesFaltando(
                pDicA.Keys, pDicB.Keys,
                out List<string> faltandoA, out List<string> faltandoB);
            resultado.PosicoesFaltandoNaA = faltandoA;
            resultado.PosicoesFaltandoNaB = faltandoB;

            var divergencias = new List<DivergenciaPosicaoCAM>();

            foreach (string posicao in pDicA.Keys)
            {
                if (!pDicB.ContainsKey(posicao))
                    continue;

                ObjetoLeituraCAMTxt objA = pDicA[posicao];
                ObjetoLeituraCAMTxt objB = pDicB[posicao];

                AvaliarPeca(objA, posicao, pOpcoes.FabricanteA, resultado.CantoneirasSemAcoGNaA, resultado.FurosForaPadraoNaA);
                AvaliarPeca(objB, posicao, pOpcoes.FabricanteB, resultado.CantoneirasSemAcoGNaB, resultado.FurosForaPadraoNaB);

                List<string> diferencas = objA.CompararComObjeto(objB, pOpcoes.ConsiderarDiametroFuros);

                if (diferencas.Count > 0)
                    divergencias.Add(new DivergenciaPosicaoCAM(posicao, diferencas));
            }

            resultado.Divergencias = divergencias
                .OrderBy(divergencia => FuncoesHefestoUteis.PosicaoNumeroParaOrdem(divergencia.Posicao))
                .ToList();

            return resultado;
        }

        /// <summary>
        /// Aplica as verificações de aço G (cantoneiras) e furos fora do padrão para uma peça.
        /// Reproduz a regra original: para cantoneira, reporta os furos fora da aba 1; se a aba 1
        /// estiver no padrão, verifica a aba 2.
        /// </summary>
        private static void AvaliarPeca(
            ObjetoLeituraCAMTxt pObjeto,
            string pPosicao,
            FabricanteEnum pFabricante,
            List<string> pCantoneirasSemAcoG,
            List<string> pFurosForaPadrao)
        {
            if (pObjeto is CantoneiraLeituraCAMTxt cantoneira)
            {
                if (RegrasComparacaoCAM.CantoneiraSemAcoG(cantoneira.MaterialCAM))
                    pCantoneirasSemAcoG.Add(pPosicao);

                List<double> foraAba1 = RegrasComparacaoCAM.FurosForaDoPadrao(Diametros(cantoneira.ListaFurosAba1), pFabricante);
                if (foraAba1.Count > 0)
                {
                    pFurosForaPadrao.Add(FormataFurosFora(pPosicao, foraAba1));
                }
                else
                {
                    List<double> foraAba2 = RegrasComparacaoCAM.FurosForaDoPadrao(Diametros(cantoneira.ListaFurosAba2), pFabricante);
                    if (foraAba2.Count > 0)
                        pFurosForaPadrao.Add(FormataFurosFora(pPosicao, foraAba2));
                }
            }
            else if (pObjeto is ChapaLeituraCAMTxt chapa)
            {
                List<double> fora = RegrasComparacaoCAM.FurosForaDoPadrao(Diametros(chapa.ListaFurosCAM), pFabricante);
                if (fora.Count > 0)
                    pFurosForaPadrao.Add(FormataFurosFora(pPosicao, fora));
            }
        }

        private static IEnumerable<double> Diametros(List<FuroCAM> pFuros)
        {
            return pFuros == null ? Enumerable.Empty<double>() : pFuros.Select(furo => furo.Diametro);
        }

        private static string FormataFurosFora(string pPosicao, List<double> pDiametrosFora)
        {
            return $"{pPosicao} ({string.Join(";", pDiametrosFora)})";
        }
    }
}
