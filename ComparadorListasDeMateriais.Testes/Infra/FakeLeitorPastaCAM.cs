using ComparadorListasDeMateriais.ComparadorCAM;
using Hefesto.CAM.ComparacaoArquivos;
using ModelagemTorre;
using System.Collections.Generic;

namespace ComparadorListasDeMateriais.Testes.Infra
{
    /// <summary>
    /// Leitor falso de pastas CAM — demonstra o seam (porta <see cref="ILeitorPastaCAM"/>):
    /// permite exercitar a orquestração de <see cref="ComparadorCAMService"/> sem ler arquivos reais.
    /// </summary>
    public class FakeLeitorPastaCAM : ILeitorPastaCAM
    {
        private readonly Dictionary<string, ObjetoLeituraCAMTxt> _conteudo;

        public FakeLeitorPastaCAM(Dictionary<string, ObjetoLeituraCAMTxt> conteudo = null)
        {
            _conteudo = conteudo ?? new Dictionary<string, ObjetoLeituraCAMTxt>();
        }

        public Dictionary<string, ObjetoLeituraCAMTxt> LerPasta(string pCaminhoPasta, FabricanteEnum pFabricante)
        {
            return _conteudo;
        }
    }
}
