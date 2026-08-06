using System;

namespace ComparadorListasDeMateriais.ComparadorV2
{
    /// <summary>
    /// Trata a sigla de material que vem no fim da numeração da posição (65<b>H</b>, 105<b>G</b>, 70<b>F</b>).
    /// <para>
    /// Porta da regra canônica do Hefesto (<c>HefestoCore/Uteis/FuncoesHefesto.RemoveSiglaMaterialDaPosicao</c>):
    /// somente <b>G</b>, <b>H</b> e <b>F</b> são siglas de material. Qualquer outra letra no fim faz parte
    /// da numeração e não pode ser removida, senão posições distintas seriam confundidas.
    /// </para>
    /// </summary>
    public static class SiglaMaterialPosicao
    {
        /// <summary>Devolve a numeração sem a sigla de material, ou a própria numeração se não houver sigla.</summary>
        public static string RemoveSigla(string pPosicao)
        {
            if (string.IsNullOrEmpty(pPosicao) || pPosicao.Length < 2)
                return pPosicao;

            return TemSigla(pPosicao) ? pPosicao.Substring(0, pPosicao.Length - 1) : pPosicao;
        }

        /// <summary>Devolve a sigla de material da posição (em maiúscula), ou string vazia se não houver.</summary>
        public static string Sigla(string pPosicao)
        {
            if (string.IsNullOrEmpty(pPosicao) || pPosicao.Length < 2)
                return string.Empty;

            return TemSigla(pPosicao) ? char.ToUpperInvariant(pPosicao[pPosicao.Length - 1]).ToString() : string.Empty;
        }

        /// <summary>
        /// Chave usada para parear posições entre as duas listas: numeração sem sigla, normalizada
        /// (minúsculas e sem espaços), no mesmo espírito de <see cref="FuncoesUteis.ComparaPosicoes"/>.
        /// </summary>
        public static string ChaveBase(string pPosicao)
        {
            // Normalizar ANTES de remover a sigla: se a numeração vier com espaço no fim (acontece
            // em lista de Excel), a última letra não é a sigla e ela escaparia da remoção.
            return RemoveSigla(Normaliza(pPosicao ?? string.Empty));
        }

        /// <summary>Normaliza uma numeração para comparação: sem espaços e em minúsculas.</summary>
        public static string Normaliza(string pPosicao)
        {
            return (pPosicao ?? string.Empty).Replace(" ", "").ToLowerInvariant();
        }

        private static bool TemSigla(string pPosicao)
        {
            char ultima = char.ToUpperInvariant(pPosicao[pPosicao.Length - 1]);

            return ultima == 'G' || ultima == 'H' || ultima == 'F';
        }
    }
}
