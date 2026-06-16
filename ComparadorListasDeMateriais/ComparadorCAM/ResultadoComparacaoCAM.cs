using System.Collections.Generic;

namespace ComparadorListasDeMateriais.ComparadorCAM
{
    /// <summary>
    /// Divergência encontrada em uma posição que existe nas duas pastas CAM.
    /// </summary>
    public class DivergenciaPosicaoCAM
    {
        public string Posicao { get; set; }

        public List<string> Diferencas { get; set; } = new List<string>();

        public DivergenciaPosicaoCAM()
        {
        }

        public DivergenciaPosicaoCAM(string pPosicao, List<string> pDiferencas)
        {
            Posicao = pPosicao;
            Diferencas = pDiferencas ?? new List<string>();
        }
    }

    /// <summary>
    /// Resultado estruturado da comparação de arquivos CAM entre duas pastas (A e B).
    /// <para>
    /// É um objeto de dados puro (sem UI), facilmente inspecionável em testes e
    /// renderizável em relatório (ver <see cref="RelatorioCAMTxt"/>).
    /// </para>
    /// </summary>
    public class ResultadoComparacaoCAM
    {
        public string CaminhoPastaA { get; set; }
        public string CaminhoPastaB { get; set; }

        /// <summary>Posições que existem em B mas não foram encontradas na pasta A.</summary>
        public List<string> PosicoesFaltandoNaA { get; set; } = new List<string>();

        /// <summary>Posições que existem em A mas não foram encontradas na pasta B.</summary>
        public List<string> PosicoesFaltandoNaB { get; set; } = new List<string>();

        /// <summary>Divergências por posição (já ordenadas pela ordem natural da posição).</summary>
        public List<DivergenciaPosicaoCAM> Divergencias { get; set; } = new List<DivergenciaPosicaoCAM>();

        public List<string> CantoneirasSemAcoGNaA { get; set; } = new List<string>();
        public List<string> CantoneirasSemAcoGNaB { get; set; } = new List<string>();

        /// <summary>Posições com furos fora do padrão da fábrica, no formato "POS (d1;d2)".</summary>
        public List<string> FurosForaPadraoNaA { get; set; } = new List<string>();
        public List<string> FurosForaPadraoNaB { get; set; } = new List<string>();

        public bool TodasPosicoesPresentes => PosicoesFaltandoNaA.Count == 0 && PosicoesFaltandoNaB.Count == 0;
    }
}
