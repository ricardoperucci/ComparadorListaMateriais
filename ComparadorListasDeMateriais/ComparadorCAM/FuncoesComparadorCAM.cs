﻿using Hefesto.CAM;
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

                dicObjetosLeituraCamA.Add(posicaoCAMSemMaterial, objetoLeituraCAM);
            }

            foreach (string arquivo in arquivosCamB)
            {
                string camSalvo = System.IO.File.ReadAllText(arquivo);

                string posicaoCAM = FuncoesLeituraCAMTxt.PosicaoNomeArquivoCAM(arquivo, out string posicaoCAMSemMaterial);

                ObjetoLeituraCAMTxt objetoLeituraCAM = FuncoesLeituraCAMTxt.CriaObjetoLeituraCAM(camSalvo, posicaoCAM, pFabricante);

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

            foreach (string pos in dicObjetosLeituraCamA.Keys)
            {
                if (!dicObjetosLeituraCamB.ContainsKey(pos))
                    continue;

                if(dicObjetosLeituraCamA[pos] is CantoneiraLeituraCAMTxt cant && !cant.Material.Contains("GR60"))
                {
                    cantoneirasSemGlistaA.Add(pos);
                }
                if (dicObjetosLeituraCamB[pos] is CantoneiraLeituraCAMTxt cantb && !cantb.Material.Contains("GR60"))
                {
                    cantoneirasSemGlistaB.Add(pos);
                }

                var listaDifsPosicao = dicObjetosLeituraCamA[pos].CompararComObjeto(dicObjetosLeituraCamB[pos]);

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
