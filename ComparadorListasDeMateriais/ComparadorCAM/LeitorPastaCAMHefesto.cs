using Hefesto.CAM.ComparacaoArquivos;
using ModelagemTorre;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ComparadorListasDeMateriais.ComparadorCAM
{
    /// <summary>
    /// Implementação real da leitura de pastas CAM usando os objetos da <b>HefestoCAM</b>.
    /// <para>
    /// É o <b>único ponto de acoplamento com a HefestoCAM na etapa de leitura</b> — concentrado
    /// aqui de propósito, para manter o resto da comparação testável e desacoplado.
    /// </para>
    /// </summary>
    public class LeitorPastaCAMHefesto : ILeitorPastaCAM
    {
        public Dictionary<string, ObjetoLeituraCAMTxt> LerPasta(string pCaminhoPasta, FabricanteEnum pFabricante)
        {
            var dicionario = new Dictionary<string, ObjetoLeituraCAMTxt>();

            List<string> arquivosCam = Directory.GetFiles(pCaminhoPasta)
                .Where(arquivo => arquivo.Split('.').Last().ToUpper() == "CAM")
                .ToList();

            foreach (string arquivo in arquivosCam)
            {
                string camSalvo = File.ReadAllText(arquivo);

                // API HefestoCAM 2026: PosicaoNomeArquivoCAM passou a exigir o FabricanteEnum
                // (antes a chamada usava só 2 argumentos, o que quebrava o build contra a DLL nova).
                string posicaoCAM = FuncoesLeituraCAMTxt.PosicaoNomeArquivoCAM(arquivo, pFabricante, out string posicaoSemMaterial);

                ObjetoLeituraCAMTxt objetoLeitura = FuncoesLeituraCAMTxt.CriaObjetoLeituraCAM(camSalvo, posicaoCAM, pFabricante, arquivo);

                // Indexador (em vez de Add) evita exceção caso haja posições repetidas na pasta.
                dicionario[posicaoSemMaterial] = objetoLeitura;
            }

            return dicionario;
        }
    }
}
