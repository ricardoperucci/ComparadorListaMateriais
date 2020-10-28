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

            List<string> estruturasComuns = EstruturasComuns(listaMateriais1, listaMateriais2, out List<string> soNa1, out List<string> soNa2);

            resultado = new ObjetoResultadoComparacao(cabecalho, soNa1, soNa2);

            foreach (string estrutura in estruturasComuns)
            {
                var objeto1 = listaMateriais1.FirstOrDefault(x => FuncoesUteis.ComparaNomeEstruturas(x.Key, estrutura));

                var objeto2 = listaMateriais2.FirstOrDefault(x => FuncoesUteis.ComparaNomeEstruturas(x.Key, estrutura));

                resultado.ListaEstruturasComparadas.Add(ComparaEstruturaNasListas(objeto1.Value, objeto2.Value, estrutura));
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

        /// <summary>
        /// Compara a estrutura nas duas listas
        /// </summary>
        /// <param name="pObjetosLista1"></param>
        /// <param name="pObjetosLista2"></param>
        /// <param name="pEstrutura"></param>
        /// <returns></returns>
        public EstruturaComparacao ComparaEstruturaNasListas(List<ObjetoComparacaoLista> pObjetosLista1, List<ObjetoComparacaoLista> pObjetosLista2, string pEstrutura)
        {
            EstruturaComparacao saida = new EstruturaComparacao();

            saida.NomeEstruturaSaida = FuncoesUteis.NomeEstruturaMaiusculo(pEstrutura);

            saida.PosicoesSomenteListaOriginal = pObjetosLista1.Where(x => !pObjetosLista2.Any(y => FuncoesUteis.ComparaPosicoes(x.NumeracaoComMaterial,y.NumeracaoComMaterial))).Select(x => x.NumeracaoComMaterial).ToList();

            saida.PosicoesSomenteListaNova = pObjetosLista2.Where(x => !pObjetosLista1.Any(y => FuncoesUteis.ComparaPosicoes(x.NumeracaoComMaterial, y.NumeracaoComMaterial))).Select(x => x.NumeracaoComMaterial).ToList();

            List<string> posicoesNasDuas = pObjetosLista1.Where(x => pObjetosLista2.Any(y => FuncoesUteis.ComparaPosicoes(x.NumeracaoComMaterial, y.NumeracaoComMaterial))).Select(x => x.NumeracaoComMaterial).ToList();

            if (posicoesNasDuas.Count > 0)
            {
                StringBuilder strDivergenciasEst = new StringBuilder();
                StringBuilder strMelhoriasEst = new StringBuilder();
                
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

    }
}
