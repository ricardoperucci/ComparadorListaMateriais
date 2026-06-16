using Hefesto.CAM.ComparacaoArquivos;
using ModelagemTorre;
using System.Collections.Generic;

namespace ComparadorListasDeMateriais.ComparadorCAM
{
    /// <summary>
    /// Porta (abstração) de leitura de uma pasta de arquivos CAM.
    /// <para>
    /// Isola a dependência da HefestoCAM para que a orquestração da comparação
    /// (<see cref="ComparadorCAMService"/>) possa ser testada com leitores falsos,
    /// sem precisar de arquivos reais nem do ambiente AutoCAD.
    /// </para>
    /// </summary>
    public interface ILeitorPastaCAM
    {
        /// <summary>
        /// Lê todos os arquivos <c>.CAM</c> de uma pasta e devolve os objetos de leitura
        /// da HefestoCAM indexados pela posição (sem o sufixo de material).
        /// </summary>
        Dictionary<string, ObjetoLeituraCAMTxt> LerPasta(string pCaminhoPasta, FabricanteEnum pFabricante);
    }
}
