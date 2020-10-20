using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using ComparadorListasDeMateriais.ObjetosLista;

namespace ComparadorListasDeMateriais
{
    public class Comparador
    {

        public bool CompararListas(string pCaminho1, string pCaminho2, out string resultado)
        {
            resultado = "";

            Dictionary<string, List<ObjetoComparacaoLista>> listaMateriais1 = CriaObjetosComparacaoLista(pCaminho1);
            Dictionary<string, List<ObjetoComparacaoLista>> listaMateriais2 = CriaObjetosComparacaoLista(pCaminho2);

            StringBuilder strBuilder = new StringBuilder();

            strBuilder.AppendLine("Comparação listas de materiais");

            strBuilder.AppendLine(string.Format("Arquivos: {0} e {1}", pCaminho1.Split('/').Last().Split('\\').Last(), pCaminho2.Split('/').Last().Split('\\').Last()));

            string data = "Data: " + DateTime.Today.Day.ToString() + "/" + (DateTime.Today.Month.ToString().Count() == 1 ? "0" + DateTime.Today.Month.ToString() : DateTime.Today.Month.ToString()) + "/" + DateTime.Today.Year;
            string horario = "Horário: " + DateTime.Now.ToLongTimeString();

            strBuilder.AppendLine(data);
            strBuilder.AppendLine();

            strBuilder.AppendLine(horario);
            strBuilder.AppendLine();

            List<string> estruturasComuns = EstruturasComuns(listaMateriais1, listaMateriais2, out List<string> soNa1, out List<string> soNa2);

            foreach(string estrutura in estruturasComuns)
            {
                var objeto1 = listaMateriais1.FirstOrDefault(x => FuncoesUteis.ComparaNomeEstruturas(x.Key, estrutura));

                var objeto2 = listaMateriais2.FirstOrDefault(x => FuncoesUteis.ComparaNomeEstruturas(x.Key, estrutura));

                strBuilder.AppendLine(ComparaEstruturaNasListas(objeto1.Value, objeto2.Value, estrutura));
            }

            if(soNa1.Count > 0)
            {
                strBuilder.AppendLine(FuncoesUteis._tituloErroEstruturaSoNa1);
                foreach(string est in soNa1)
                {
                    strBuilder.AppendLine(string.Format("   • {0}", est));
                }
            }

            if (soNa2.Count > 0)
            {
                strBuilder.AppendLine(FuncoesUteis._tituloErroEstruturaSoNa2);
                foreach (string est in soNa2)
                {
                    strBuilder.AppendLine(string.Format("   • {0}", est));
                }
            }

            resultado = strBuilder.ToString();

            return true;
        }

        /// <summary>
        /// Reconhece se o caminho escolhido era de excel ou txt e retorna as listas dos objetos comparacao
        /// </summary>
        /// <param name="pCaminho"></param>
        /// <param name="excel"></param>
        /// <param name="parafusosComparacaoPorEstrutura"></param>
        /// <returns></returns>
        public Dictionary<string, List<ObjetoComparacaoLista>> CriaObjetosComparacaoLista(string pCaminho)
        {
            Dictionary<string, List<ObjetoComparacaoLista>> dicMateriais = null;


            if (pCaminho.ToLower().Contains(".xls") || pCaminho.ToLower().Contains(".xlsx"))
            {
                List<List<string>>[] linhas1 = FuncoesUteis.LerExcelVariasAbas(pCaminho, new string[] { "MATERIAIS", "PARAFUSOS" });

                List<List<string>> listaLinhasListaMateriais1 = linhas1[0];

                dicMateriais = FuncoesUteis.CriaObjetosComparacaoListaByExcel(listaLinhasListaMateriais1);

                //List<List<string>> listaLinhasListaParafusos1 = linhas1[1];
            }
            else if (pCaminho.ToLower().Contains(".txt"))
            {
                string textoLista = System.IO.File.ReadAllText(pCaminho);

                List<string> linhasLista = textoLista.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();

                dicMateriais = FuncoesUteis.CriaObjetosComparacaoListaByTxt(linhasLista);
            }

            else
            {
                throw new Exception("Formato de lista não reconhecido");
            }
             
            return dicMateriais;
        }

        public List<string> EstruturasComuns(Dictionary<string, List<ObjetoComparacaoLista>> pLista1, Dictionary<string, List<ObjetoComparacaoLista>> pLista2, out List<string> estruturasSoNa1, out List<string> estruturasSoNa2)
        {
            List<string> estruturasComuns = new List<string>();

            estruturasSoNa1 = new List<string>();

            estruturasSoNa2 = new List<string>();

            foreach (string estrutura1 in pLista1.Keys)
            {
                var tem = pLista2.FirstOrDefault(x => FuncoesUteis.ComparaNomeEstruturas(x.Key, estrutura1));

                if (tem.Value == null)
                {
                    estruturasSoNa1.Add(estrutura1);
                }
                else
                {
                    estruturasComuns.Add(estrutura1);
                }
            }

            foreach (string estrutura2 in pLista2.Keys)
            {
                var tem = pLista1.FirstOrDefault(x => FuncoesUteis.ComparaNomeEstruturas(x.Key, estrutura2));

                if (tem.Value == null)
                {
                    estruturasSoNa2.Add(estrutura2);
                }
            }

            return estruturasComuns;
        }

        public string ComparaEstruturaNasListas(List<ObjetoComparacaoLista> pObjetosLista1, List<ObjetoComparacaoLista> pObjetosLista2, string pEstrutura)
        {
            bool teveErro = false;

            StringBuilder strBuilder = new StringBuilder();

            strBuilder.AppendLine(FuncoesUteis.NomeEstruturaMaiusculo(pEstrutura));
            strBuilder.AppendLine();

            List<string> posicoesSomenteNa1 = pObjetosLista1.Where(x => !pObjetosLista2.Any(y => FuncoesUteis.ComparaPosicoes(x.NumeracaoComMaterial,y.NumeracaoComMaterial))).Select(x => x.NumeracaoComMaterial).ToList();

            List<string> posicoesSomenteNa2 = pObjetosLista2.Where(x => !pObjetosLista1.Any(y => FuncoesUteis.ComparaPosicoes(x.NumeracaoComMaterial, y.NumeracaoComMaterial))).Select(x => x.NumeracaoComMaterial).ToList();

            List<string> posicoesNasDuas = pObjetosLista1.Where(x => pObjetosLista2.Any(y => FuncoesUteis.ComparaPosicoes(x.NumeracaoComMaterial, y.NumeracaoComMaterial))).Select(x => x.NumeracaoComMaterial).ToList();

            if (posicoesSomenteNa1.Count > 0)
            {
                teveErro = true;

                strBuilder.AppendLine(string.Format("   {0}: {1}", FuncoesUteis._tituloPosicoesSoNa1, string.Join(", ", posicoesSomenteNa1)));
                strBuilder.AppendLine();
            }

            if (posicoesSomenteNa2.Count > 0)
            {
                teveErro = true;

                strBuilder.AppendLine(string.Format("   {0}: {1}", FuncoesUteis._tituloPosicoesSoNa2, string.Join(", ", posicoesSomenteNa2)));
                strBuilder.AppendLine();
            }

            if (posicoesNasDuas.Count > 0)
            {
                StringBuilder strDivergenciasEst = new StringBuilder();
                StringBuilder strMelhoriasEst = new StringBuilder();

                bool teveDivergencia = false;
                bool teveMelhoria = false;

                foreach (string posicao in posicoesNasDuas)
                {
                    ObjetoComparacaoLista objetoLista1 = pObjetosLista1.FirstOrDefault(x => FuncoesUteis.ComparaPosicoes(x.NumeracaoComMaterial, posicao));
                    ObjetoComparacaoLista objetoLista2 = pObjetosLista2.FirstOrDefault(x => FuncoesUteis.ComparaPosicoes(x.NumeracaoComMaterial, posicao));

                    List<string> divergencias = objetoLista1.CompararComObjeto(objetoLista2);

                    if(divergencias.Count > 0)
                    {

                        if (FuncoesUteis.VerificaMelhoria(divergencias))
                        {
                            teveErro = true;
                            teveMelhoria = true;

                            for (int i = 0; i < divergencias.Count; i++)
                            {
                                string div = divergencias[i];
                                if (i == 0)
                                {
                                    strMelhoriasEst.AppendLine(string.Format("      ${0}: • {1}", posicao, div));
                                }
                                else
                                {
                                    strMelhoriasEst.AppendLine(string.Format("           @• {0}", div));
                                }
                            }
                        }

                        else
                        {
                            teveDivergencia = true;

                            teveErro = true;

                            for (int i = 0; i < divergencias.Count; i++)
                            {
                                string div = divergencias[i];
                                if (i == 0)
                                {
                                    strDivergenciasEst.AppendLine(string.Format("      #{0}: • {1}", posicao, div));
                                }
                                else
                                {
                                    strDivergenciasEst.AppendLine(string.Format("           %• {0}", div));
                                }
                            }

                            //strDivergenciasEst.AppendLine();
                        }
                        
                    }
                }

                if (teveDivergencia)
                {
                    strBuilder.AppendLine("   Divergências:");
                    strBuilder.AppendLine();
                    strBuilder.AppendLine(strDivergenciasEst.ToString());
                }
                if (teveMelhoria)
                {
                    strBuilder.AppendLine("   Melhorias:");
                    strBuilder.AppendLine();
                    strBuilder.AppendLine(strMelhoriasEst.ToString());
                }
            }

            if (!teveErro)
            {
                strBuilder.AppendLine("    Todas as posições na estrutura estão iguais");
                strBuilder.AppendLine();
            }

            return strBuilder.ToString();
        }

        

    }
}
