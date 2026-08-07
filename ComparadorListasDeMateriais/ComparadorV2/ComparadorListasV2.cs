using ComparadorListasDeMateriais.ObjetosLista;
using ComparadorListasDeMateriais.ObjetosResultados;
using ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias;
using System.Collections.Generic;
using System.Linq;

namespace ComparadorListasDeMateriais.ComparadorV2
{
    /// <summary>
    /// Versão 2 do comparador de listas de materiais.
    /// <para>
    /// Diferença essencial para o <see cref="Comparador"/> antigo: uma posição que só trocou de
    /// material (65H → 65G) é reconhecida como a MESMA posição, e não como uma posição eliminada
    /// somada a uma posição nova.
    /// </para>
    /// <para>
    /// Reaproveita o parse de txt/Excel e o motor de divergências (<c>CompararComObjeto</c>) que já
    /// existiam; o que muda é a chave de pareamento e o modelo de resultado.
    /// </para>
    /// </summary>
    public class ComparadorListasV2
    {
        /// <summary>Compara duas listas a partir dos caminhos dos arquivos (Excel ou txt).</summary>
        public ResultadoComparacaoV2 Comparar(string pCaminhoOriginal, string pCaminhoNova)
        {
            var leitor = new Comparador();

            Dictionary<string, List<ObjetoComparacaoLista>> original = leitor.CriaObjetosComparacaoLista(pCaminhoOriginal);
            Dictionary<string, List<ObjetoComparacaoLista>> nova = leitor.CriaObjetosComparacaoLista(pCaminhoNova);

            ResultadoComparacaoV2 resultado = Comparar(original, nova);

            resultado.CaminhoListaOriginal = pCaminhoOriginal;
            resultado.CaminhoListaNova = pCaminhoNova;

            return resultado;
        }

        /// <summary>
        /// Sobrecarga testável: compara estruturas já carregadas em memória, sem tocar em arquivos.
        /// </summary>
        public ResultadoComparacaoV2 Comparar(
            Dictionary<string, List<ObjetoComparacaoLista>> pOriginal,
            Dictionary<string, List<ObjetoComparacaoLista>> pNova)
        {
            var resultado = new ResultadoComparacaoV2();

            var leitor = new Comparador();

            resultado.DivergenciasMesmaLista.AddRange(leitor.ComparaEstruturasUmaLista(pOriginal, "Lista Original"));
            resultado.DivergenciasMesmaLista.AddRange(leitor.ComparaEstruturasUmaLista(pNova, "Lista Nova"));

            List<string> comuns = leitor.EstruturasComuns(pOriginal, pNova, out List<string> soNaOriginal, out List<string> soNaNova);

            resultado.EstruturasSomenteOriginal.AddRange(soNaOriginal.Select(FuncoesUteis.NomeEstruturaMaiusculo));
            resultado.EstruturasSomenteNova.AddRange(soNaNova.Select(FuncoesUteis.NomeEstruturaMaiusculo));

            foreach (string estrutura in comuns)
            {
                List<ObjetoComparacaoLista> objetosOriginal = pOriginal
                    .FirstOrDefault(x => FuncoesUteis.ComparaNomeEstruturas(x.Key, estrutura)).Value;

                List<ObjetoComparacaoLista> objetosNova = pNova
                    .FirstOrDefault(x => FuncoesUteis.ComparaNomeEstruturas(x.Key, estrutura)).Value;

                resultado.Estruturas.Add(CompararEstrutura(objetosOriginal, objetosNova, estrutura));
            }

            return resultado;
        }

        /// <summary>Compara uma estrutura presente nas duas listas.</summary>
        public EstruturaComparacaoV2 CompararEstrutura(
            List<ObjetoComparacaoLista> pOriginais,
            List<ObjetoComparacaoLista> pNovas,
            string pNomeEstrutura)
        {
            var saida = new EstruturaComparacaoV2
            {
                NomeEstrutura = FuncoesUteis.NomeEstruturaMaiusculo(pNomeEstrutura)
            };

            ResultadoPareamento pareamento = PareadorPosicoes.Parear(pOriginais, pNovas);

            foreach (ParObjetos par in OrdenadorPosicoes.Ordenar(pareamento.Pares, x => x.Original.NumeracaoComMaterial))
            {
                saida.Pares.Add(MontarPar(par));
            }

            saida.PosicoesEliminadas.AddRange(
                OrdenadorPosicoes.Ordenar(pareamento.SomenteOriginal.Select(x => x.NumeracaoComMaterial).Distinct()));

            saida.PosicoesNovas.AddRange(
                OrdenadorPosicoes.Ordenar(pareamento.SomenteNova.Select(x => x.NumeracaoComMaterial).Distinct()));

            saida.Ambiguos.AddRange(OrdenadorPosicoes.Ordenar(pareamento.Ambiguos, x => x.ChaveBase));

            return saida;
        }

        /// <summary>
        /// Roda o motor de divergências sobre um par e separa não conformidades de diferenças pequenas.
        /// </summary>
        private static ParPosicoes MontarPar(ParObjetos pPar)
        {
            List<ErroPosicao> erros = pPar.Original.CompararComObjeto(pPar.Nova);

            var saida = new ParPosicoes
            {
                NumeracaoOriginal = pPar.Original.NumeracaoComMaterial,
                NumeracaoNova = pPar.Nova.NumeracaoComMaterial,
                Tipo = pPar.Tipo
            };

            // Ordenado para leitura (perfil antes de comprimento), não na ordem em que o motor
            // de comparação encontrou os erros. Ver FormatadorDivergenciaV2.Ordem.
            foreach (ErroPosicao erro in erros.OrderBy(FormatadorDivergenciaV2.Ordem))
            {
                // No par por troca de material o cabeçalho já mostra "65H → 65G"; repetir uma linha
                // "Material" logo abaixo seria dizer a mesma coisa duas vezes.
                if (pPar.Tipo == TipoPar.MaterialTrocado && erro is DivergenciaMaterial)
                    continue;

                if (erro.NcOuMelhoria)
                    saida.Divergencias.Add(erro);
                else
                    saida.DiferencasPequenas.Add(erro);
            }

            return saida;
        }
    }
}
