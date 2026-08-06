using ComparadorListasDeMateriais.ObjetosResultados;
using ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComparadorListasDeMateriais.ComparadorV2
{
    /// <summary>Como um par de posições foi formado entre as duas listas.</summary>
    public enum TipoPar
    {
        /// <summary>Mesma numeração nas duas listas (65H ↔ 65H).</summary>
        Exato,

        /// <summary>Mesma numeração base, sigla de material diferente (65H ↔ 65G).</summary>
        MaterialTrocado
    }

    /// <summary>
    /// Uma posição pareada entre as duas listas, com as divergências encontradas já separadas
    /// entre "não conformidades" e "diferenças pequenas" (comprimento/largura com delta ≤ 2 mm).
    /// </summary>
    public class ParPosicoes
    {
        public string NumeracaoOriginal { get; set; }
        public string NumeracaoNova { get; set; }

        public TipoPar Tipo { get; set; }

        public List<ErroPosicao> Divergencias { get; set; } = new List<ErroPosicao>();
        public List<ErroPosicao> DiferencasPequenas { get; set; } = new List<ErroPosicao>();

        /// <summary>Sigla de material da numeração original (vazia quando a numeração não tem sigla).</summary>
        public string SiglaOriginal => SiglaMaterialPosicao.Sigla(NumeracaoOriginal);

        /// <summary>Sigla de material da numeração nova (vazia quando a numeração não tem sigla).</summary>
        public string SiglaNova => SiglaMaterialPosicao.Sigla(NumeracaoNova);

        public bool TemDivergencia => Divergencias.Count > 0;
        public bool TemDiferencaPequena => DiferencasPequenas.Count > 0;

        /// <summary>Par exato sem nenhuma diferença entre as listas.</summary>
        public bool Igual => Tipo == TipoPar.Exato && !TemDivergencia && !TemDiferencaPequena;
    }

    /// <summary>
    /// Numeração base com mais de um candidato em algum dos lados — o programa não tenta adivinhar
    /// qual peça virou qual, apenas avisa para conferência manual.
    /// </summary>
    public class GrupoAmbiguo
    {
        public string ChaveBase { get; set; }

        public List<string> CandidatosOriginal { get; set; } = new List<string>();
        public List<string> CandidatosNova { get; set; } = new List<string>();
    }

    /// <summary>Resultado da comparação de uma estrutura presente nas duas listas.</summary>
    public class EstruturaComparacaoV2
    {
        public string NomeEstrutura { get; set; }

        public List<ParPosicoes> Pares { get; set; } = new List<ParPosicoes>();

        public List<string> PosicoesNovas { get; set; } = new List<string>();
        public List<string> PosicoesEliminadas { get; set; } = new List<string>();

        public List<GrupoAmbiguo> Ambiguos { get; set; } = new List<GrupoAmbiguo>();

        public List<ParPosicoes> MaterialTrocado =>
            Pares.Where(x => x.Tipo == TipoPar.MaterialTrocado).ToList();

        /// <summary>Pares exatos com ao menos uma não conformidade.</summary>
        public List<ParPosicoes> Alteradas =>
            Pares.Where(x => x.Tipo == TipoPar.Exato && x.TemDivergencia).ToList();

        /// <summary>Pares exatos cuja única diferença é de comprimento/largura ≤ 2 mm.</summary>
        public List<ParPosicoes> SomenteDiferencasPequenas =>
            Pares.Where(x => x.Tipo == TipoPar.Exato && !x.TemDivergencia && x.TemDiferencaPequena).ToList();

        /// <summary>Pares (de qualquer tipo) que têm alguma diferença pequena a mostrar.</summary>
        public List<ParPosicoes> ComDiferencasPequenas =>
            Pares.Where(x => x.Tipo == TipoPar.Exato && x.TemDiferencaPequena).ToList();

        public int PosicoesIguais => Pares.Count(x => x.Igual);

        /// <summary>Nada a reportar nesta estrutura.</summary>
        public bool SemDiferencas =>
            Pares.All(x => x.Igual) &&
            PosicoesNovas.Count == 0 &&
            PosicoesEliminadas.Count == 0 &&
            Ambiguos.Count == 0;
    }

    /// <summary>Contadores do bloco de resumo do relatório.</summary>
    public class ResumoComparacao
    {
        public int EstruturasComparadas { get; set; }

        /// <summary>Total de pares formados (exatos + material trocado).</summary>
        public int PosicoesConferidas { get; set; }

        /// <summary>Pares exatos sem nenhuma diferença.</summary>
        public int PosicoesIguais { get; set; }

        /// <summary>Pares exatos com ao menos uma não conformidade.</summary>
        public int PosicoesAlteradas { get; set; }

        /// <summary>Pares exatos cuja única diferença é ≤ 2 mm.</summary>
        public int SomenteDiferencasPequenas { get; set; }

        public int MaterialTrocado { get; set; }

        public int PosicoesNovas { get; set; }
        public int PosicoesEliminadas { get; set; }

        public int EstruturasSomenteOriginal { get; set; }
        public int EstruturasSomenteNova { get; set; }

        /// <summary>Quantidade de grupos ambíguos (não de posições dentro deles).</summary>
        public int GruposAmbiguos { get; set; }

        public int PrecisamAtencao =>
            PosicoesAlteradas + MaterialTrocado + PosicoesNovas + PosicoesEliminadas + GruposAmbiguos;
    }

    /// <summary>
    /// Resultado completo da comparação de duas listas de materiais na versão 2 do comparador.
    /// É um objeto de dados puro: quem gera texto/HTML a partir dele são os renderizadores.
    /// </summary>
    public class ResultadoComparacaoV2
    {
        public string CaminhoListaOriginal { get; set; }
        public string CaminhoListaNova { get; set; }

        public DateTime GeradoEm { get; set; } = DateTime.Now;

        public List<EstruturaComparacaoV2> Estruturas { get; set; } = new List<EstruturaComparacaoV2>();

        public List<string> EstruturasSomenteOriginal { get; set; } = new List<string>();
        public List<string> EstruturasSomenteNova { get; set; } = new List<string>();

        public List<DivergenciaEntreEstruturaLista> DivergenciasMesmaLista { get; set; } =
            new List<DivergenciaEntreEstruturaLista>();

        public string NomeListaOriginal => NomeArquivo(CaminhoListaOriginal);
        public string NomeListaNova => NomeArquivo(CaminhoListaNova);

        public ResumoComparacao Resumo()
        {
            return new ResumoComparacao
            {
                EstruturasComparadas = Estruturas.Count,
                PosicoesConferidas = Estruturas.Sum(x => x.Pares.Count),
                PosicoesIguais = Estruturas.Sum(x => x.PosicoesIguais),
                PosicoesAlteradas = Estruturas.Sum(x => x.Alteradas.Count),
                SomenteDiferencasPequenas = Estruturas.Sum(x => x.SomenteDiferencasPequenas.Count),
                MaterialTrocado = Estruturas.Sum(x => x.MaterialTrocado.Count),
                PosicoesNovas = Estruturas.Sum(x => x.PosicoesNovas.Count),
                PosicoesEliminadas = Estruturas.Sum(x => x.PosicoesEliminadas.Count),
                EstruturasSomenteOriginal = EstruturasSomenteOriginal.Count,
                EstruturasSomenteNova = EstruturasSomenteNova.Count,
                GruposAmbiguos = Estruturas.Sum(x => x.Ambiguos.Count)
            };
        }

        private static string NomeArquivo(string pCaminho)
        {
            if (string.IsNullOrWhiteSpace(pCaminho))
                return string.Empty;

            return pCaminho.Split('/').Last().Split('\\').Last();
        }
    }
}
