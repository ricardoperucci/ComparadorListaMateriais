using Hefesto.CAM;
using Hefesto.CAM.ComparacaoArquivos;
using ModelagemTorre;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComparadorListasDeMateriais.ComparadorCAM
{
    public static class FuncoesComparadorCAM
    {
        public static bool ComparaArquivosCAM(
            FabricanteEnum pFabricante,
            string pCaminhoPastaCamA,
            string pCaminhoPastaCamB,
            out StringBuilder stringBuilder)
        {
            List<string> arquivosCamA = Directory.GetFiles(pCaminhoPastaCamA).Where(x => x.Split(".".ToCharArray()).Last().Equals("CAM")).ToList();
            List<string> arquivosCamB = Directory.GetFiles(pCaminhoPastaCamB).Where(x => x.Split(".".ToCharArray()).Last().Equals("CAM")).ToList();

            Dictionary<string, ObjetoLeituraCAMTxt> dicObjetosLeituraCamA = new Dictionary<string, ObjetoLeituraCAMTxt>();
            Dictionary<string, ObjetoLeituraCAMTxt> dicObjetosLeituraCamB = new Dictionary<string, ObjetoLeituraCAMTxt>();

            foreach (string arquivo in arquivosCamA)
            {
                string camSalvo = System.IO.File.ReadAllText(arquivo);

                string posicaoCAM = FuncoesLeituraCAMTxt.PosicaoNomeArquivoCAM(arquivo, out string posicaoCAMSemMaterial);

                ObjetoLeituraCAMTxt objetoLeituraCAM = FuncoesLeituraCAMTxt.CriaObjetoLeituraCAM(camSalvo, posicaoCAM, pFabricante);

                dicObjetosLeituraCamA.Add(posicaoCAM, objetoLeituraCAM);
            }

            foreach (string arquivo in arquivosCamB)
            {
                string camSalvo = System.IO.File.ReadAllText(arquivo);

                string posicaoCAM = FuncoesLeituraCAMTxt.PosicaoNomeArquivoCAM(arquivo, out string posicaoCAMSemMaterial);

                ObjetoLeituraCAMTxt objetoLeituraCAM = FuncoesLeituraCAMTxt.CriaObjetoLeituraCAM(camSalvo, posicaoCAM, pFabricante);

                dicObjetosLeituraCamB.Add(posicaoCAM, objetoLeituraCAM);
            }

            var posicoesFaltandoA = dicObjetosLeituraCamB.Keys.Where(x => !dicObjetosLeituraCamA.Keys.Contains(x));
            var posicoesFaltandoB = dicObjetosLeituraCamA.Keys.Where(x => !dicObjetosLeituraCamB.Keys.Contains(x));

            stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(FuncoesHefestoUteis.FormatarNegrito("RELATÓRIO COMPARAÇÂO ARQUIVOS CAM"));

            stringBuilder.AppendLine($"{FuncoesHefestoUteis.FormatarNegrito("Pasta A")}: {pCaminhoPastaCamA}");
            stringBuilder.AppendLine($"{FuncoesHefestoUteis.FormatarNegrito("Pasta B")}: {pCaminhoPastaCamB}");

            stringBuilder.AppendLine();
            stringBuilder.AppendLine();

            if (posicoesFaltandoA.Count() > 0)
            {
                stringBuilder.AppendLine("Posições não encontradas nos arquivos da Pasta A");
                stringBuilder.AppendLine(string.Join(", ", posicoesFaltandoA));
                stringBuilder.AppendLine();
            }

            if (posicoesFaltandoB.Count() > 0)
            {
                stringBuilder.AppendLine("Posições não encontradas nos arquivos da Pasta A");
                stringBuilder.AppendLine(string.Join(", ", posicoesFaltandoB));
                stringBuilder.AppendLine();
            }

            if (posicoesFaltandoA.Count() == 0 && posicoesFaltandoB.Count() == 0)
            {
                stringBuilder.AppendLine("Todas posições existem nas duas pastas.");
                stringBuilder.AppendLine();
            }

            Dictionary<string, List<string>> divergencias = new Dictionary<string, List<string>>();

            foreach(string pos in dicObjetosLeituraCamA.Keys)
            {
                if (!dicObjetosLeituraCamB.ContainsKey(pos))
                    continue;


                divergencias.Add(pos, dicObjetosLeituraCamA[pos].CompararComObjeto(dicObjetosLeituraCamB[pos]));
            }

            if(divergencias.Count > 0)
            {
                stringBuilder.AppendLine("Divergências encontradas entre os arquivos:");

                foreach(var kvpdiv in divergencias)
                {
                    stringBuilder.AppendLine($"POS. {kvpdiv.Key}: {string.Join(" / ", kvpdiv.Value)}");
                }
            }

            return true;
        }


        /// <summary>
        /// Pede a seleção de uma pasta para o usuário
        /// </summary>
        /// <param name="pTexto"></param>
        /// <returns></returns>
        public static string SelecionaPasta(string pTexto)
        {
            //string pTexto = pModelosOuCam ? "modelos" : "arquivos CAM";
            string caminhoPasta;

            using (var fbd = new FolderBrowserDialog() { Description = string.Format("Selecione a pasta com os {0}", pTexto) })
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    caminhoPasta = fbd.SelectedPath;
                }
                else
                {
                    caminhoPasta = "erro";
                }
            }

            return caminhoPasta;
        }
    }
}
