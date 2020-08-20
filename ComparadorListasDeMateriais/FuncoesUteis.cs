using ComparadorListasDeMateriais.ObjetosLista;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ComparadorListasDeMateriais
{
    public static class FuncoesUteis
    {

        public static int _primeiraLinhaNCExcel = 21;

        public static int _primeiraLinhaMelhoriaExcel = 8;

        public static string _tituloErroEstruturaSoNa1 = "Estruturas que estão na Lista Original e não foram encontradas na Lista Nova:";
        public static string _tituloErroEstruturaSoNa2 = "Estruturas que estão na Lista Nova e não foram encontradas na Lista Original:";

        public static string _tituloPosicoesSoNa1 = "Posições que estão na Lista Original e não estão na Lista Nova";
        public static string _tituloPosicoesSoNa2 = "Posições que estão na Lista Nova e não estão na Lista Original";

        public static string _prefixoErroEstrutura = "ENE_";

        public static bool VerificaMelhoria(List<string> pDivergencias)
        {
            if (pDivergencias.Count == 1 && (pDivergencias[0].Contains(" 1mm") || pDivergencias[0].Contains(" -1mm")))
            {
                return true;
            }

            if(pDivergencias.Count == 2 && (pDivergencias[0].Contains(" 1mm") || pDivergencias[0].Contains(" -1mm")) && (pDivergencias[1].Contains(" 1mm") || pDivergencias[1].Contains(" -1mm")))
            {
                return true;
            }

            return false;
        }

        public static string NomeEstruturaMaiusculo(string pNome)
        {
            string nomeUpper = pNome.ToUpper();

            string saida = "";

            for (int i = 0; i < nomeUpper.Count(); i++)
            {
                if(i > 1 && nomeUpper[i].Equals('M') && char.IsDigit(nomeUpper[i - 1]))
                {
                    saida = saida + "m";
                }
                else
                {
                    saida = saida + nomeUpper[i];
                }
            }

            return saida;
        }

        /// <summary>
        /// A partir das listas de string que representam cada linha da lista de material em Excel, cria os objetosComparacaoLista para cada estrutura
        /// </summary>
        /// <param name="pListaLinhasListaMateriais"></param>
        /// <returns></returns>
        public static Dictionary<string, List<ObjetoComparacaoLista>> CriaObjetosComparacaoListaByExcel(List<List<string>> pListaLinhasListaMateriais)
        {
            string estruturaVigenteLista = "";

            Dictionary<string, List<ObjetoComparacaoLista>> dicObjetosPorEstrutura = new Dictionary<string, List<ObjetoComparacaoLista>>();

            for (int i = 0; i < pListaLinhasListaMateriais.Count; i++)
            {
                List<string> objetoLista = pListaLinhasListaMateriais[i];
               
                if (i > 10 && pListaLinhasListaMateriais[i - 4][0].Equals("L I S T A   D E   M A T E R I A I S") && pListaLinhasListaMateriais[i + 1][0].Contains("Conj. Soldado"))
                {
                    estruturaVigenteLista = objetoLista[0];
                }

                bool eCalota = objetoLista[4].ToLower().Contains("calota") || (objetoLista[4].Contains("d") && objetoLista[4].Contains("r"));

                if (objetoLista.Count < 9 || string.IsNullOrEmpty(objetoLista[1]) || string.IsNullOrEmpty(objetoLista[4]) || (!eCalota && string.IsNullOrEmpty(objetoLista[6])) || string.IsNullOrEmpty(objetoLista[7]) || string.IsNullOrEmpty(objetoLista[8]))
                {
                    continue;
                }

                string posicao = objetoLista[1].ToString().Split(new string[] { ".0" }, StringSplitOptions.RemoveEmptyEntries).First().Split(new string[] { ",0" }, StringSplitOptions.RemoveEmptyEntries).First().Replace(" ", "");

                
                string posicaoSemMaterial = posicao.Last().Equals('H') || posicao.Last().Equals('G') || posicao.Last().Equals('h') || posicao.Last().Equals('g') ? posicao.Remove(posicao.Count() - 1) : posicao;

                //posicaoStringNumero so serve pra testar converter pra double no try e garantir que é uma linha de uma peça na lista
                string posicaoStringNumero = posicaoSemMaterial;
                if (Char.IsLetter(posicaoSemMaterial.Last()))
                {
                    posicaoStringNumero = posicaoStringNumero.Remove(posicaoStringNumero.Count() - 1);
                }
                if (Char.IsLetter(posicaoSemMaterial.First()))
                {
                    posicaoStringNumero = posicaoStringNumero.Remove(0, 1);
                }
                try
                {
                    System.Convert.ToDouble(posicaoStringNumero);
                }

                catch
                {
                    continue;
                }

                ObjetoComparacaoLista objetoCriado = null;

                //CHAPA
                if (objetoLista[4].Contains("C"))
                {
                    objetoCriado = new ChapaComparacaoLista(objetoLista, estruturaVigenteLista);
                }
                //CANTONEIRA
                else
                {
                    objetoCriado = new CantoneiraComparacaoLista(objetoLista, estruturaVigenteLista);
                }

                if (!dicObjetosPorEstrutura.ContainsKey(estruturaVigenteLista))
                {
                    dicObjetosPorEstrutura.Add(estruturaVigenteLista, new List<ObjetoComparacaoLista>());
                }

                dicObjetosPorEstrutura[estruturaVigenteLista].Add(objetoCriado);

            }

            return dicObjetosPorEstrutura;
        }

        /// <summary>
        /// A partir das listas de string que representam cada linha da lista de material em Excel, cria os objetosComparacaoLista para cada estrutura
        /// </summary>
        /// <param name="pListaLinhasListaMateriais"></param>
        /// <returns></returns>
        public static Dictionary<string, List<ObjetoComparacaoLista>> CriaObjetosComparacaoListaByTxt(List<string> pListaLinhasListaMateriais)
        {
            string estruturaVigenteLista = "";

            Dictionary<string, List<ObjetoComparacaoLista>> dicObjetosPorEstrutura = new Dictionary<string, List<ObjetoComparacaoLista>>();

            for (int i = 0; i < pListaLinhasListaMateriais.Count; i++)
            {
                string objetoLista = pListaLinhasListaMateriais[i];

                if (objetoLista.Count() < 2)
                    continue;

                if (VerificaLinhaTXTEstrutura(objetoLista))
                {
                    estruturaVigenteLista = objetoLista.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries).Last();
                    continue;
                }

                ObjetoComparacaoLista objetoCriado = null;

                //CHAPA
                if (objetoLista.Substring(14, 24).Replace(" ", "").Contains("C"))
                {
                    objetoCriado = new ChapaComparacaoLista(objetoLista, estruturaVigenteLista);
                }
                //CANTONEIRA
                else
                {
                    objetoCriado = new CantoneiraComparacaoLista(objetoLista, estruturaVigenteLista);
                }

                if (!dicObjetosPorEstrutura.ContainsKey(estruturaVigenteLista))
                {
                    dicObjetosPorEstrutura.Add(estruturaVigenteLista, new List<ObjetoComparacaoLista>());
                }

                dicObjetosPorEstrutura[estruturaVigenteLista].Add(objetoCriado);

            }

            return dicObjetosPorEstrutura;
        }

        public static Stream GetResourceFileStream(string fileName)
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            // Get all embedded resources
            string[] arrResources = currentAssembly.GetManifestResourceNames();

            foreach (string resourceName in arrResources)
            {
                if (resourceName.Contains(fileName))
                {
                    return currentAssembly.GetManifestResourceStream(resourceName);
                }
            }

            return null;
        }

        public static bool VerificaLinhaTXTEstrutura(string pLinha)
        {
            bool isEstrutura = false;

            if (pLinha.Count() < 43)
            {
                isEstrutura = true;
            }
            else
            {
                try
                {
                    System.Convert.ToInt32(pLinha.Substring(38, 2).Replace(" ", ""));
                }
                catch
                {
                    if (pLinha.Contains("-"))
                    {
                        isEstrutura = true;
                    }
                }
                try
                {
                    System.Convert.ToInt32(pLinha.Substring(40, 2).Replace(" ", ""));
                }
                catch
                {
                    if (pLinha.Contains("-"))
                    {
                        isEstrutura = true;
                    }
                }
                try
                {
                    System.Convert.ToInt32(pLinha.Substring(40, 2).Replace(" ", ""));
                }
                catch
                {
                    if (pLinha.Contains("-"))
                    {
                        isEstrutura = true;
                    }
                }
            }

            return isEstrutura;
        }

        public static List<List<string>>[] LerExcelVariasAbas(string pCaminho, string[] pAba)
        {
            WorkBook wb = WorkBook.ReadExcelFile(pCaminho);

            List<List<string>>[] saida = new List<List<string>>[pAba.Length];

            for (int i = 0; i < pAba.Length; i++)
            {
                WorkSheet ws = wb.GetWorksheetByName(pAba[i]);

                saida[i] = ws.Data.Select(x => x.Select(k => k == null ? "" : k.ToString()).ToList()).ToList();
            }


            return saida;
        }

        public static void EscreveExcel(string pNomeArquivo, string pResultadoComparacao)
        {
            //object[][] saida = new object[3][] { new string[2] { "A", "B" }, new string[2] { "C", "D" }, new string[2] { "E", "F" } };

            string nomeComExt = pNomeArquivo;

            Stream k = GetResourceFileStream("Template");
            using (var fileStream = File.Create(nomeComExt))
            {
                k.Seek(0, SeekOrigin.Begin);
                k.CopyTo(fileStream);
            }

            WorkBook wb = WorkBook.ReadExcelFile(nomeComExt);

            WorkSheet abaNcs = wb.GetWorksheetByName("PMV-NCs");
            WorkSheet abaMelhorias = wb.GetWorksheetByName("PMV-Melhorias");

            //dados = lista de listas onde cada lista é uma linha
            var dados = abaNcs.Data;
            //aba.WriteData(0, 0, saida);
            EscreveDivergenciasExcel(abaNcs, ListaNCsMelhoriasPosicao(pResultadoComparacao, true), _primeiraLinhaNCExcel);

            EscreveDivergenciasExcel(abaMelhorias, ListaNCsMelhoriasPosicao(pResultadoComparacao, false), _primeiraLinhaMelhoriaExcel);

            wb.Save();
        }

        public static void EscreveDivergenciasExcel(WorkSheet pAbaNcs, Dictionary<string, List<string>> pErrosPorPosicao, int pIndexPrimeiraLinha)
        {
            List<string[]> listaTextosLinhas = new List<string[]>();

            foreach(var kvpPosicaoErros in pErrosPorPosicao)
            {
                List<string> linha = new List<string>();

                linha.Add(kvpPosicaoErros.Key);

                string descricaoErro = string.Join("; ", kvpPosicaoErros.Value);

                linha.Add(descricaoErro);

                listaTextosLinhas.Add(linha.ToArray());
            }
            if(pIndexPrimeiraLinha == _primeiraLinhaNCExcel)
            {
                pAbaNcs.WriteData(pIndexPrimeiraLinha, 2, listaTextosLinhas.ToArray());
            }
            else
            {
                pAbaNcs.WriteData(pIndexPrimeiraLinha, 0, listaTextosLinhas.ToArray());
            }

        }

        public static Dictionary<string, List<string>> ListaNCsMelhoriasPosicao(string pResultado, bool pNCOuMelhoria)
        {
            List<string> linhasResultado = pResultado.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();

            Dictionary<string, List<string>> dicErrosPosicao = new Dictionary<string, List<string>>();

            string erroPrimeiro = pNCOuMelhoria ? "#" : "$";
            string erroSecundarios = pNCOuMelhoria ? "%" : "@";

            foreach (string linha in linhasResultado)
            {
                string erro = LimpaLinhaErro(linha);

                if (linha.Contains(erroPrimeiro))
                {
                    string posicao = linha.Split(erroPrimeiro.ToCharArray()).Last().Split(':').First();

                    if (dicErrosPosicao.ContainsKey(posicao))
                    {
                        if(!ErroIgualNaLista(dicErrosPosicao[posicao], erro))
                            dicErrosPosicao[posicao].Add(erro);
                    }
                    else
                    {
                        dicErrosPosicao.Add(posicao, new List<string> { erro });
                    }
                }

                if (linha.Contains(erroSecundarios))
                {
                    if(!ErroIgualNaLista(dicErrosPosicao.Last().Value, erro))
                        dicErrosPosicao.Last().Value.Add(linha.Replace("%", "").Replace("@", "").Replace("   ", " "));
                }
            }

            if (pNCOuMelhoria)
            {
                foreach (var erro in EscreverPosicoesNaoEncontradasExcel(linhasResultado))
                {
                    dicErrosPosicao.Add(erro.Key, erro.Value);
                }

                foreach (var erro in EscreverEstruturasNaoEncontradasExcel(linhasResultado))
                {
                    dicErrosPosicao.Add(erro.Key, erro.Value);
                }
            }

            return dicErrosPosicao;
        }

        public static bool ErroIgualNaLista(List<string> pLista, string pErro)
        {
            string erroCorrigido = pErro.ToLower().Replace(" ", "");

            foreach(string erroLista in pLista)
            {
                if(erroLista.ToLower().Replace(" ", "").Equals(erroCorrigido))
                {
                    return true;
                }
            }

            return false;
        }

        public static Dictionary<string, List<string>> EscreverEstruturasNaoEncontradasExcel(List<string> pLinhasResultado)
        {
            Dictionary<string, List<string>> dicErrosEstrutura = new Dictionary<string, List<string>>();

            int numErro = 0;

            bool errosLista1 = false;
            bool errosLista2 = false;

            foreach (string linha in pLinhasResultado)
            {
                if(linha.Contains(_tituloErroEstruturaSoNa1))
                {
                    errosLista1 = true;
                    continue;
                }

                if (errosLista1)
                {
                    if (linha.Contains(_tituloErroEstruturaSoNa2))
                    {
                        errosLista2 = true;
                        errosLista1 = false;
                        continue;
                    }

                    string posErro = _prefixoErroEstrutura + CorrigeZerosPrefixo(numErro);

                    string erro = string.Format("Est. {0} só na LisOrig", linha.Replace("   ","").Replace("•",""));

                    dicErrosEstrutura.Add(posErro, new List<string> { erro });

                    numErro++;
                }

                if (errosLista2)
                {
                    if (!linha.Contains("•"))
                    {
                        errosLista2 = false;
                        break;
                    }
                    string posErro = _prefixoErroEstrutura + CorrigeZerosPrefixo(numErro);

                    string erro = string.Format("Est. {0} só na LisNova", linha.Replace("   ", "").Replace("•", ""));

                    dicErrosEstrutura.Add(posErro, new List<string> { erro });

                    numErro++;
                }

            }

            return dicErrosEstrutura;
        }

        public static Dictionary<string, List<string>> EscreverPosicoesNaoEncontradasExcel(List<string> pLinhasResultado)
        {
            Dictionary<string, List<string>> dicErrosPosicoes = new Dictionary<string, List<string>>();

            string erroSoNaLista = "Posição contida somente na Lista {0}";

            foreach (string linha in pLinhasResultado)
            {
                if (linha.Contains(_tituloPosicoesSoNa1))
                {
                    var posicoes = linha.Replace(_tituloPosicoesSoNa1, "").Replace(" ", "").Replace(":", "").Split(',');

                    foreach(var pos in posicoes)
                    {
                        dicErrosPosicoes.Add(pos, new List<string> { string.Format(erroSoNaLista, "Original") });
                    }
                }

                if (linha.Contains(_tituloPosicoesSoNa2))
                {
                    var posicoes = linha.Replace(_tituloPosicoesSoNa2, "").Replace(" ", "").Replace(":", "").Split(',');

                    foreach (var pos in posicoes)
                    {
                        dicErrosPosicoes.Add(pos, new List<string> { string.Format(erroSoNaLista, "Nova") });
                    }
                }
            }

            return dicErrosPosicoes;
        }


        private static string CorrigeZerosPrefixo(int valor)
        {
            if(valor < 10)
            {
                return "0" + valor.ToString();
            }
            else
            {
                return valor.ToString();
            }
        }

        public static string LimpaLinhaErro (string pLinha)
        {
            var linhaSplit = pLinha.Split(':');

            string erro = "";
            for (int i = 1; i < linhaSplit.Count(); i++)
            {
                erro = erro + linhaSplit[i];
            }

            //string erro = linhaSplit.Last();

            //if (pLinha.Contains("Comprimento"))
            //{
            //    erro = "Comprimento " + erro;
            //}
            //else if (pLinha.Contains("Largura"))
            //{
            //    erro = "Largura " + erro;
            //}
            //else if (pLinha.Contains("Quantidade"))
            //{
            //    erro = "Quantidade " + erro;
            //}

            return erro;
        }

        public static void EscreveTxt(string pNomeArquivo, string pResultadoComparacao)
        {
            string nome = pNomeArquivo + ".txt";

            string resultado = pResultadoComparacao.Replace("@", "").Replace("#", "").Replace("$", "").Replace("%", "");

            using (StreamWriter outputFile = new StreamWriter(nome, false))
            {
                outputFile.Write(resultado);
            }
        }


        /// <summary>
        /// Tira os acentos de uma string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveAcentos(string text, bool removeHifenEspacos = false)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            string saida = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

            if (removeHifenEspacos)
            {
                saida = Regex.Replace(saida, "-", "");
                saida = Regex.Replace(saida, " ", "");
            }

            return saida;
        }

        public static string RemoveComunsEstrutura(string pEstruturaToLower)
        {
            string saida = pEstruturaToLower.Replace("gab.", "gabarito").Replace("gabaritos", "gabarito").Replace("gabarito p/ montagem", "gabarito").Replace("ext.", "extensao").Replace("c/", "").Replace("p/", "").Replace("h=", "").Replace(" em ", "").Replace(" de ", "").Replace(" da ", "").Replace("(1 torre)", "");

            if (char.IsDigit(saida[0]))
            {
                saida = saida.Remove(0, 1);
            }

            saida = RemoveAcentos(saida, true);

            int indexInicioReal = 0;
            for (int i = 0; i < saida.Count(); i++)
            {
                if (char.IsLetter(saida[i]))
                {
                    indexInicioReal = i;
                    break;
                }
            }

            return saida.Substring(indexInicioReal);
        }

        public static bool ComparaNomeEstruturas(string pEstrutura1, string pEstrutura2)
        {
            return RemoveComunsEstrutura(pEstrutura1.ToLower()).Equals(RemoveComunsEstrutura(pEstrutura2.ToLower()));
        }

        public static bool ComparaPosicoes(string pPosicao1, string pPosicao2)
        {
            return pPosicao1.ToLower().Replace(" ","").Equals(pPosicao2.ToLower().Replace(" ",""));
        }
    }
}
