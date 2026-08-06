using ComparadorListasDeMateriais.ObjetosLista;
using System.Collections.Generic;
using System.Linq;

namespace ComparadorListasDeMateriais.ComparadorV2
{
    /// <summary>Uma posição da lista original casada com a sua correspondente na lista nova.</summary>
    public class ParObjetos
    {
        public ObjetoComparacaoLista Original { get; set; }
        public ObjetoComparacaoLista Nova { get; set; }

        public TipoPar Tipo { get; set; }
    }

    /// <summary>Saída do pareamento de uma estrutura.</summary>
    public class ResultadoPareamento
    {
        public List<ParObjetos> Pares { get; set; } = new List<ParObjetos>();

        public List<ObjetoComparacaoLista> SomenteOriginal { get; set; } = new List<ObjetoComparacaoLista>();
        public List<ObjetoComparacaoLista> SomenteNova { get; set; } = new List<ObjetoComparacaoLista>();

        public List<GrupoAmbiguo> Ambiguos { get; set; } = new List<GrupoAmbiguo>();
    }

    /// <summary>
    /// Decide quais posições das duas listas são "a mesma posição".
    /// <para>
    /// É aqui que mora a correção do caso 65H → 65G: no comparador antigo a chave era a numeração
    /// COM a sigla de material, então uma troca de material virava uma posição eliminada mais uma
    /// posição nova. Aqui, o que sobra do pareamento exato ainda tem uma segunda chance de casar
    /// pela numeração base (sem a sigla).
    /// </para>
    /// <para>
    /// Função pura: não lê arquivo nem depende de UI.
    /// </para>
    /// </summary>
    public static class PareadorPosicoes
    {
        public static ResultadoPareamento Parear(
            List<ObjetoComparacaoLista> pOriginais,
            List<ObjetoComparacaoLista> pNovas)
        {
            var saida = new ResultadoPareamento();

            List<ObjetoComparacaoLista> originais = pOriginais ?? new List<ObjetoComparacaoLista>();
            List<ObjetoComparacaoLista> novas = pNovas ?? new List<ObjetoComparacaoLista>();

            var novasDisponiveis = new List<ObjetoComparacaoLista>(novas);
            var sobrasOriginal = new List<ObjetoComparacaoLista>();

            // 1) Pareamento exato pela numeração com material — o comportamento de sempre.
            foreach (ObjetoComparacaoLista original in originais)
            {
                string chave = SiglaMaterialPosicao.Normaliza(original.NumeracaoComMaterial);

                ObjetoComparacaoLista correspondente =
                    novasDisponiveis.FirstOrDefault(x => SiglaMaterialPosicao.Normaliza(x.NumeracaoComMaterial) == chave);

                if (correspondente == null)
                {
                    sobrasOriginal.Add(original);
                    continue;
                }

                novasDisponiveis.Remove(correspondente);

                saida.Pares.Add(new ParObjetos
                {
                    Original = original,
                    Nova = correspondente,
                    Tipo = TipoPar.Exato
                });
            }

            // 2) O que sobrou ganha uma segunda chance pela numeração base (sem a sigla de material).
            var porBaseOriginal = sobrasOriginal
                .GroupBy(x => SiglaMaterialPosicao.ChaveBase(x.NumeracaoComMaterial))
                .ToDictionary(g => g.Key, g => g.ToList());

            var porBaseNova = novasDisponiveis
                .GroupBy(x => SiglaMaterialPosicao.ChaveBase(x.NumeracaoComMaterial))
                .ToDictionary(g => g.Key, g => g.ToList());

            var pareadosOriginal = new HashSet<ObjetoComparacaoLista>();
            var pareadosNova = new HashSet<ObjetoComparacaoLista>();

            foreach (string chaveBase in porBaseOriginal.Keys)
            {
                if (!porBaseNova.ContainsKey(chaveBase))
                    continue;

                List<ObjetoComparacaoLista> candidatosOriginal = porBaseOriginal[chaveBase];
                List<ObjetoComparacaoLista> candidatosNova = porBaseNova[chaveBase];

                // Mais de um candidato de algum lado: o programa não adivinha qual peça virou qual.
                if (candidatosOriginal.Count > 1 || candidatosNova.Count > 1)
                {
                    saida.Ambiguos.Add(new GrupoAmbiguo
                    {
                        ChaveBase = chaveBase,
                        CandidatosOriginal = candidatosOriginal.Select(x => x.NumeracaoComMaterial).ToList(),
                        CandidatosNova = candidatosNova.Select(x => x.NumeracaoComMaterial).ToList()
                    });

                    continue;
                }

                ObjetoComparacaoLista original = candidatosOriginal[0];
                ObjetoComparacaoLista nova = candidatosNova[0];

                pareadosOriginal.Add(original);
                pareadosNova.Add(nova);

                saida.Pares.Add(new ParObjetos
                {
                    Original = original,
                    Nova = nova,
                    Tipo = TipoPar.MaterialTrocado
                });
            }

            // 3) O que não casou de jeito nenhum é posição eliminada ou posição nova.
            saida.SomenteOriginal.AddRange(sobrasOriginal.Where(x => !pareadosOriginal.Contains(x)));
            saida.SomenteNova.AddRange(novasDisponiveis.Where(x => !pareadosNova.Contains(x)));

            return saida;
        }
    }
}
