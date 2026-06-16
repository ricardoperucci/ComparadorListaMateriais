using ComparadorListasDeMateriais.ObjetosLista;
using System.Collections.Generic;

namespace ComparadorListasDeMateriais.Testes.Infra
{
    /// <summary>
    /// Fábricas de objetos para os testes de comparação de listas, montando as peças por
    /// propriedades (graças aos construtores sem parâmetro adicionados na refatoração), sem
    /// depender do parse frágil de Excel/txt.
    /// </summary>
    public static class ConstrutoresTeste
    {
        public static ChapaComparacaoLista Chapa(
            string posicao, double comprimento, double largura, double espessura,
            string sigla = "S", int qtdPecas = 1)
        {
            return new ChapaComparacaoLista
            {
                NumeracaoComMaterial = posicao,
                NumeracaoSemMaterial = posicao,
                Comprimento = comprimento,
                Largura = largura,
                Espessura = espessura,
                SiglaMaterial = sigla,
                QuantidadePecasNoComponente = qtdPecas
            };
        }

        public static CantoneiraComparacaoLista Cantoneira(
            string posicao, double comprimento, double aba, double espessura,
            string sigla = "S", int qtdPecas = 1)
        {
            return new CantoneiraComparacaoLista
            {
                NumeracaoComMaterial = posicao,
                NumeracaoSemMaterial = posicao,
                Comprimento = comprimento,
                Aba = aba,
                Espessura = espessura,
                SiglaMaterial = sigla,
                QuantidadePecasNoComponente = qtdPecas,
                PerfilSimples = $"L{aba}x{espessura}"
            };
        }

        public static Dictionary<string, List<ObjetoComparacaoLista>> Estrutura(string nome, params ObjetoComparacaoLista[] objetos)
        {
            return new Dictionary<string, List<ObjetoComparacaoLista>>
            {
                { nome, new List<ObjetoComparacaoLista>(objetos) }
            };
        }
    }
}
