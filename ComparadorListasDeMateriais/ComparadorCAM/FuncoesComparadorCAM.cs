using Hefesto.CAM;
using Hefesto.CAM.ComparacaoArquivos;
using ModelagemTorre;
using ModelagemTorre.ObjetosCore;
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
            FabricanteEnum pFabricanteA,
            FabricanteEnum pFabricanteB,
            string pCaminhoPastaCamA,
            string pCaminhoPastaCamB,
            out StringBuilder stringBuilder)
        {
            DialogResult dr = MessageBox.Show("Comparar diametros furos?", "Atenção", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            bool considerarDiametro = dr == DialogResult.Yes;

            List<string> arquivosCamA = Directory.GetFiles(pCaminhoPastaCamA).Where(x => x.Split(".".ToCharArray()).Last().Equals("CAM")).ToList();
            List<string> arquivosCamB = Directory.GetFiles(pCaminhoPastaCamB).Where(x => x.Split(".".ToCharArray()).Last().Equals("CAM")).ToList();

            Dictionary<string, ObjetoLeituraCAMTxt> dicObjetosLeituraCamA = new Dictionary<string, ObjetoLeituraCAMTxt>();
            Dictionary<string, ObjetoLeituraCAMTxt> dicObjetosLeituraCamB = new Dictionary<string, ObjetoLeituraCAMTxt>();

            foreach (string arquivo in arquivosCamA)
            {
                string camSalvo = System.IO.File.ReadAllText(arquivo);

                string posicaoCAM = FuncoesLeituraCAMTxt.PosicaoNomeArquivoCAM(arquivo, out string posicaoCAMSemMaterial);

                ObjetoLeituraCAMTxt objetoLeituraCAM = FuncoesLeituraCAMTxt.CriaObjetoLeituraCAM(camSalvo, posicaoCAM, pFabricanteA, arquivo);

                dicObjetosLeituraCamA.Add(posicaoCAMSemMaterial, objetoLeituraCAM);
            }

            foreach (string arquivo in arquivosCamB)
            {
                string camSalvo = System.IO.File.ReadAllText(arquivo);

                string posicaoCAM = FuncoesLeituraCAMTxt.PosicaoNomeArquivoCAM(arquivo, out string posicaoCAMSemMaterial);

                ObjetoLeituraCAMTxt objetoLeituraCAM = FuncoesLeituraCAMTxt.CriaObjetoLeituraCAM(camSalvo, posicaoCAM, pFabricanteB, arquivo);

                dicObjetosLeituraCamB.Add(posicaoCAMSemMaterial, objetoLeituraCAM);
            }

            var posicoesFaltandoA = dicObjetosLeituraCamB.Keys.Where(x => !dicObjetosLeituraCamA.Keys.Contains(x));
            var posicoesFaltandoB = dicObjetosLeituraCamA.Keys.Where(x => !dicObjetosLeituraCamB.Keys.Contains(x));

            stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("RELATÓRIO COMPARAÇÂO ARQUIVOS CAM");

            stringBuilder.AppendLine($"Pasta A: {pCaminhoPastaCamA}");
            stringBuilder.AppendLine($"Pasta B: {pCaminhoPastaCamB}");

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

            List<string> cantoneirasSemGlistaA = new List<string>();
            List<string> cantoneirasSemGlistaB = new List<string>();

            List<string> furosForaPadraolistaA = new List<string>();
            List<string> furosForaPadraolistaB = new List<string>();

            bool furosNoPadrao(List<FuroCAM> ppListaFuros, FabricanteEnum ppFabricante, out List<double> ppDiametrosFora)
            {
                ppDiametrosFora = new List<double>();

                var listaDiametrosPadrao = EstaticosComparacaoCAM.GetListaDiametrosPadrao(ppFabricante);

                bool estaNoPadrao = true;

                foreach(FuroCAM furo in ppListaFuros)
                {
                    if (!listaDiametrosPadrao.Contains(furo.Diametro))
                    {
                        ppDiametrosFora.Add(furo.Diametro);
                        estaNoPadrao = false;
                    }
                }

                ppDiametrosFora = ppDiametrosFora.Distinct().ToList();

                return estaNoPadrao;
            }

            foreach (string pos in dicObjetosLeituraCamA.Keys)
            {
                if (!dicObjetosLeituraCamB.ContainsKey(pos))
                    continue;

                if(dicObjetosLeituraCamA[pos] is CantoneiraLeituraCAMTxt cant)
                {
                    if(!cant.Material.Contains("GR60"))
                        cantoneirasSemGlistaA.Add(pos);

                    if (!furosNoPadrao(cant.ListaFurosAba1, pFabricanteA, out List<double> diametrosFora))
                        furosForaPadraolistaA.Add($"{pos} ({string.Join(";", diametrosFora)})");
                    else if (!furosNoPadrao(cant.ListaFurosAba2, pFabricanteA, out List<double> diametrosFora2))
                        furosForaPadraolistaA.Add($"{pos} ({string.Join(";", diametrosFora2)})");
                }

                else if(dicObjetosLeituraCamA[pos] is ChapaLeituraCAMTxt chapa)
                {
                    if (!furosNoPadrao(chapa.ListaFurosCAM, pFabricanteA, out List<double> diametrosFora))
                        furosForaPadraolistaA.Add($"{pos} ({string.Join(";", diametrosFora)})");
                }

                if (dicObjetosLeituraCamB[pos] is CantoneiraLeituraCAMTxt cantb)
                {
                    if (!cantb.Material.Contains("GR60"))
                        cantoneirasSemGlistaB.Add(pos);

                    if (!furosNoPadrao(cantb.ListaFurosAba1, pFabricanteB, out List<double> diametrosFora))
                        furosForaPadraolistaB.Add($"{pos} ({string.Join(";", diametrosFora)})");

                    else if (!furosNoPadrao(cantb.ListaFurosAba2, pFabricanteB, out List<double> diametrosFora2))
                        furosForaPadraolistaB.Add($"{pos} ({string.Join(";", diametrosFora2)})");
                }

                else if (dicObjetosLeituraCamB[pos] is ChapaLeituraCAMTxt chapaB)
                {
                    if (!furosNoPadrao(chapaB.ListaFurosCAM, pFabricanteB, out List<double> diametrosFora))
                        furosForaPadraolistaB.Add($"{pos} ({string.Join(";", diametrosFora)})");
                }

                var listaDifsPosicao = dicObjetosLeituraCamA[pos].CompararComObjeto(dicObjetosLeituraCamB[pos], considerarDiametro);

                if (listaDifsPosicao.Count == 0)
                    continue;


                divergencias.Add(pos, listaDifsPosicao);
            }

            if(divergencias.Count > 0)
            {
                stringBuilder.AppendLine("Divergências encontradas entre os arquivos:");

                var divergenciasSort = divergencias.OrderBy(x => FuncoesHefestoUteis.PosicaoNumeroParaOrdem(x.Key)).ToList();

                foreach (var kvpdiv in divergenciasSort)
                {
                    stringBuilder.AppendLine($"POS. {kvpdiv.Key}: {string.Join(" / ", kvpdiv.Value)}");
                }

                stringBuilder.AppendLine();
            }

            if (cantoneirasSemGlistaA.Count > 0)
            {
                stringBuilder.AppendLine("Cantoneiras sem aço G nos arquivos da Pasta A");
                stringBuilder.AppendLine(string.Join(", ", cantoneirasSemGlistaA));
                stringBuilder.AppendLine();
            }
            else
            {
                stringBuilder.AppendLine("Todas cantoneiras com aço G nos arquivos da Pasta A");
                stringBuilder.AppendLine();
            }

            if (cantoneirasSemGlistaB.Count > 0)
            {
                stringBuilder.AppendLine("Cantoneiras sem aço G nos arquivos da Pasta B");
                stringBuilder.AppendLine(string.Join(", ", cantoneirasSemGlistaB));
                stringBuilder.AppendLine();
            }
            else
            {
                stringBuilder.AppendLine("Todas cantoneiras com aço G nos arquivos da Pasta B");
                stringBuilder.AppendLine();
            }

            if (furosForaPadraolistaA.Count > 0)
            {
                stringBuilder.AppendLine("Peças com furo diferente do padrão da fábrica nos arquivos da Pasta A");
                stringBuilder.AppendLine(string.Join(", ", furosForaPadraolistaA));
                stringBuilder.AppendLine();
            }
            else
            {
                stringBuilder.AppendLine("Todas peças com furo dentro do padrão da fábrica nos arquivos da Pasta A");
                stringBuilder.AppendLine();
            }

            if (furosForaPadraolistaB.Count > 0)
            {
                stringBuilder.AppendLine("Peças com furo diferente do padrão da fábrica nos arquivos da Pasta B");
                stringBuilder.AppendLine(string.Join(", ", furosForaPadraolistaB));
                stringBuilder.AppendLine();
            }
            else
            {
                stringBuilder.AppendLine("Todas peças com furo dentro do padrão da fábrica nos arquivos da Pasta B");
                stringBuilder.AppendLine();
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
