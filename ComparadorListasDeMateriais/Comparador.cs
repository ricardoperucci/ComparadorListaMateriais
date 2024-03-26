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
using ComparadorListasDeMateriais.ObjetosResultados;
using ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias;

namespace ComparadorListasDeMateriais
{
    public class Comparador
    {

        /// <summary>
        /// Compara as duas listas
        /// </summary>
        /// <param name="pCaminho1"></param>
        /// <param name="pCaminho2"></param>
        /// <param name="resultado"></param>
        /// <returns></returns>
        public bool CompararListas(string pCaminho1, string pCaminho2, out ObjetoResultadoComparacao resultado)
        {
            ObjetoCabecalho cabecalho = new ObjetoCabecalho(pCaminho1, pCaminho2);

            Dictionary<string, List<ObjetoComparacaoLista>> listaMateriais1 = CriaObjetosComparacaoLista(pCaminho1);
            Dictionary<string, List<ObjetoComparacaoLista>> listaMateriais2 = CriaObjetosComparacaoLista(pCaminho2);

            List<DivergenciaEntreEstruturaLista> divergenciasMesmaLista = new List<DivergenciaEntreEstruturaLista>();

            divergenciasMesmaLista.AddRange(ComparaEstruturasUmaLista(listaMateriais1, "Lista Original"));
            divergenciasMesmaLista.AddRange(ComparaEstruturasUmaLista(listaMateriais2, "Lista Nova"));

            List<string> estruturasComuns = EstruturasComuns(listaMateriais1, listaMateriais2, out List<string> soNa1, out List<string> soNa2);

            resultado = new ObjetoResultadoComparacao(cabecalho, soNa1, soNa2, divergenciasMesmaLista);

            foreach (string estrutura in estruturasComuns)
            {
                var objeto1 = listaMateriais1.FirstOrDefault(x => FuncoesUteis.ComparaNomeEstruturas(x.Key, estrutura));

                var objeto2 = listaMateriais2.FirstOrDefault(x => FuncoesUteis.ComparaNomeEstruturas(x.Key, estrutura));

                resultado.ListaEstruturasComparadas.Add(ComparaEstruturas(objeto1.Value, objeto2.Value, estrutura));
            }

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

                string textoLista = FuncoesUteis.LerTxtLisaMateriais(pCaminho);

                List<string> linhasLista = FuncoesUteis.DivideTextoPorLinhas(textoLista);

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

        /// <summary>
        /// Compara a estrutura nas duas listas
        /// </summary>
        /// <param name="pObjetosLista1"></param>
        /// <param name="pObjetosLista2"></param>
        /// <param name="pEstrutura"></param>
        /// <returns></returns>
        public EstruturaComparacao ComparaEstruturas(List<ObjetoComparacaoLista> pObjetosLista1, List<ObjetoComparacaoLista> pObjetosLista2, string pEstrutura)
        {
            EstruturaComparacao saida = new EstruturaComparacao();

            saida.NomeEstruturaSaida = FuncoesUteis.NomeEstruturaMaiusculo(pEstrutura);

            saida.PosicoesSomenteListaOriginal = pObjetosLista1.Where(x => !pObjetosLista2.Any(y => FuncoesUteis.ComparaPosicoes(x.NumeracaoComMaterial,y.NumeracaoComMaterial))).Select(x => x.NumeracaoComMaterial).ToList();

            saida.PosicoesSomenteListaNova = pObjetosLista2.Where(x => !pObjetosLista1.Any(y => FuncoesUteis.ComparaPosicoes(x.NumeracaoComMaterial, y.NumeracaoComMaterial))).Select(x => x.NumeracaoComMaterial).ToList();

            List<string> posicoesNasDuas = pObjetosLista1.Where(x => pObjetosLista2.Any(y => FuncoesUteis.ComparaPosicoes(x.NumeracaoComMaterial, y.NumeracaoComMaterial))).Select(x => x.NumeracaoComMaterial).ToList();

            if (posicoesNasDuas.Count > 0)
            {
                foreach (string posicao in posicoesNasDuas)
                {
                    ObjetoComparacaoLista objetoLista1 = pObjetosLista1.FirstOrDefault(x => FuncoesUteis.ComparaPosicoes(x.NumeracaoComMaterial, posicao));
                    ObjetoComparacaoLista objetoLista2 = pObjetosLista2.FirstOrDefault(x => FuncoesUteis.ComparaPosicoes(x.NumeracaoComMaterial, posicao));

                    List<ErroPosicao> divergencias = objetoLista1.CompararComObjeto(objetoLista2);

                    if(divergencias.Count > 0)
                    {
                        saida.ListaPosicoesComErros.Add(new PosicaoComparacao(divergencias, posicao));
                    }
                }
            }

            return saida;
        }

        /// <summary>
        /// Compara a estrutura nas duas listas
        /// </summary>
        /// <param name="pObjetosLista1"></param>
        /// <param name="pObjetosLista2"></param>
        /// <param name="pQualLista"></param>
        /// <returns></returns>
        public List<DivergenciaEntreEstruturaLista> ComparaEstruturasUmaLista(Dictionary<string, List<ObjetoComparacaoLista>> pEstruturasLista, string pQualLista)
        {
            List<DivergenciaEntreEstruturaLista> saida = new List<DivergenciaEntreEstruturaLista>();

            for (int i = 0; i < pEstruturasLista.Count; i++)
            {
                string nomeEstrutura = pEstruturasLista.Keys.ToList()[i];
                List<ObjetoComparacaoLista> estrutura1 = pEstruturasLista[nomeEstrutura];

                foreach (ObjetoComparacaoLista objetoEstrutura1 in estrutura1)
                {
                    DivergenciaEntreEstruturaLista jaEncontrouErro = saida.FirstOrDefault(x => x.Posicao.Equals(objetoEstrutura1.NumeracaoComMaterial));

                    if(jaEncontrouErro != null)
                    {
                        jaEncontrouErro.EstruturasQueContem.Add(nomeEstrutura);
                    }
                    else
                    {
                        for (int k = i + 1; k < pEstruturasLista.Count; k++)
                        {
                            string nomeEstrutura2 = pEstruturasLista.Keys.ToList()[k];
                            List<ObjetoComparacaoLista> estrutura2 = pEstruturasLista[nomeEstrutura2];

                            ObjetoComparacaoLista objetoEstrutura2 = estrutura2.FirstOrDefault(x => FuncoesUteis.ComparaPosicoes(x.NumeracaoComMaterial, objetoEstrutura1.NumeracaoComMaterial));

                            if(objetoEstrutura2 != null)
                            {
                                List<ErroPosicao> divergencias = objetoEstrutura1.CompararComObjeto(objetoEstrutura2);

                                if (divergencias.Count > 0)
                                {
                                    if (divergencias.Count == 1 && divergencias[0].TipoErroEnum.Equals(EnumErrosPosicao.Quantidade))
                                        continue;

                                    saida.Add(new DivergenciaEntreEstruturaLista(new List<string> { nomeEstrutura }, pQualLista, objetoEstrutura1.NumeracaoComMaterial));
                                }
                            }
                           
                        }
                    }
                }

            }

            return saida;
        }

    }
}
