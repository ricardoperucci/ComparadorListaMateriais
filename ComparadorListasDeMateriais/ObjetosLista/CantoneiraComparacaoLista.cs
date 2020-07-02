using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ObjetosLista
{
    public class CantoneiraComparacaoLista : ObjetoComparacaoLista
    {
        public double Aba { get; set; }

        public string PerfilSimples { get; set; }

        public bool PinoEstaiada { get; set; }
        //Construtor pra lista em EXCEL
        public CantoneiraComparacaoLista(List<string> pLinhaExcel, string pEstrutura)
        {
            this.SalvarInformacoesObjetoByLinhaExcel(pLinhaExcel, pEstrutura);

            PinoEstaiada = false;

            if (pLinhaExcel[4].Contains("BR"))
            {
                PinoEstaiada = true;
                this.PerfilSimples = pLinhaExcel[4];
                return;
            }

            this.Aba = System.Convert.ToDouble(pLinhaExcel[4].Split(new string[] { "x" }, StringSplitOptions.RemoveEmptyEntries).First().Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries).Last().Replace(",","."));

            this.Espessura = System.Convert.ToDouble(pLinhaExcel[4].Split(new string[] { "x" }, StringSplitOptions.RemoveEmptyEntries).Last().Replace(",", "."));

            this.PerfilSimples = string.Format("L{0}x{1}", this.Aba, this.Espessura);
        }

        //Construtor pra lista em TXT
        public CantoneiraComparacaoLista(string pLinhaTxt, string pEstrutura)
        {
            string perfil = pLinhaTxt.Substring(14, 24).Replace(" ", "");

            this.Comprimento = System.Convert.ToDouble(perfil.Split('x').Last().Replace(",", "."));

            //FALTA IMPLEMENTAR A CRIACAO POR LINHA TXT
            this.SalvarInformacoesObjetoByLinhaTxt(pLinhaTxt, pEstrutura);

            if (perfil.Contains("BR"))
            {
                this.PinoEstaiada = true;
                this.PerfilSimples = perfil;
                return;
            }

            var perfilSplit = perfil.Split(new string[] { "x" }, StringSplitOptions.RemoveEmptyEntries);

            this.Aba = System.Convert.ToDouble(perfilSplit[1].Replace(",", "."));

            this.SalvarSiglaMaterial(perfilSplit[2]);

            this.Espessura = System.Convert.ToDouble(perfilSplit[2].ToLower().Replace(this.SiglaMaterial.ToLower(), "").Replace(",", "."));

            this.PerfilSimples = string.Format("L{0}x{1}", this.Aba, this.Espessura);
        }

        public override List<string> CompararComObjeto(ObjetoComparacaoLista pOutro)
        {
            List<string> diferencas = new List<string>();

            if (pOutro is ChapaComparacaoLista)
            {
                diferencas.Add("Objetos diferentes cantoneira/chapa");
            }
            else
            {
                diferencas.AddRange(this.CompararObjeto(pOutro));

                CantoneiraComparacaoLista outraCantoneiraComparacao = pOutro as CantoneiraComparacaoLista;

                if (Math.Abs(this.Aba - outraCantoneiraComparacao.Aba) > 0.1 || Math.Abs(this.Espessura - pOutro.Espessura) > 0.1)
                {
                    diferencas.Add(string.Format("Perfil: {0} / {1}", this.PerfilSimples, outraCantoneiraComparacao.PerfilSimples));
                }
            }

            return diferencas;
        }
    }
}
