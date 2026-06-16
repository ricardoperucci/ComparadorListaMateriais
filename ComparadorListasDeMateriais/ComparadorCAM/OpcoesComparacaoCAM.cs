using ModelagemTorre;

namespace ComparadorListasDeMateriais.ComparadorCAM
{
    /// <summary>
    /// Opções escolhidas pelo usuário para a comparação de arquivos CAM.
    /// <para>
    /// Substitui os valores que antes eram fixos no código (CAW/CAW/A394CM) e o
    /// <c>MessageBox</c> que perguntava se deveria considerar o diâmetro dos furos.
    /// Mantendo tudo em um objeto de dados, a regra de comparação fica testável (sem UI).
    /// </para>
    /// </summary>
    public class OpcoesComparacaoCAM
    {
        public FabricanteEnum FabricanteA { get; }
        public FabricanteEnum FabricanteB { get; }
        public ParafusoTipoEnum ParafusoTipo { get; }
        public bool ConsiderarDiametroFuros { get; }

        public OpcoesComparacaoCAM(
            FabricanteEnum fabricanteA,
            FabricanteEnum fabricanteB,
            ParafusoTipoEnum parafusoTipo,
            bool considerarDiametroFuros)
        {
            FabricanteA = fabricanteA;
            FabricanteB = fabricanteB;
            ParafusoTipo = parafusoTipo;
            ConsiderarDiametroFuros = considerarDiametroFuros;
        }
    }
}
