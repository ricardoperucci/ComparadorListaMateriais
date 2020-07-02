using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ObjetosLista
{
    public class ChapaComparacaoLista : ObjetoComparacaoLista
    {
        public double Largura { get; set; }

        public bool Calota { get; set; }

        //Construtor para lista em EXCEL
        public ChapaComparacaoLista(List<string> pLinhaExcel, string pEstrutura)
        {
            this.SalvarInformacoesObjetoByLinhaExcel(pLinhaExcel, pEstrutura);

            var perfilSplitX = pLinhaExcel[4].Split(new string[] { " x " }, StringSplitOptions.RemoveEmptyEntries);
            this.Espessura = System.Convert.ToDouble(perfilSplitX.First().Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries).Last().Replace(",","."));

            if (perfilSplitX.Last().Contains("d") && perfilSplitX.Last().Contains("r"))
            {
                this.Calota = true;
                return;
            }

            this.Largura = System.Convert.ToDouble(pLinhaExcel[4].Split(new string[] { " x " }, StringSplitOptions.RemoveEmptyEntries).Last().Replace(",", "."));

            double comp = this.Comprimento;
            double larg = this.Largura;

            this.Comprimento = comp > larg ? comp : larg;

            this.Largura = comp > larg ? larg : comp;
        }

        //Construtor para lista em Txt
        public ChapaComparacaoLista(string pLinhaTxt, string pEstrutura)
        {
            //FALTA IMPLEMENTAR A CRIACAO POR LINHA TXT
            this.SalvarInformacoesObjetoByLinhaTxt(pLinhaTxt, pEstrutura);

            string perfil = pLinhaTxt.Substring(14, 24).Replace(" ", "");

            var perfilSplit = perfil.Split(new string[] { "x" }, StringSplitOptions.RemoveEmptyEntries);

            this.SalvarSiglaMaterial(perfilSplit[0].ToLower().Replace("c", ""));

            this.Espessura = System.Convert.ToDouble(perfilSplit[0].ToLower().Replace(this.SiglaMaterial.ToLower(), "").Replace("c", "").Replace(",", "."));

            if (perfil.Contains("/") && perfil.Contains("r"))
            {
                this.Calota = true;
                return;
            }

            double comp = System.Convert.ToDouble(perfilSplit[1].Replace(",", "."));

            double larg = System.Convert.ToDouble(perfilSplit.Last().Replace(",", "."));

            this.Comprimento = comp > larg ? comp : larg;

            this.Largura = comp > larg ? larg : comp;
        }

        public override List<string> CompararComObjeto(ObjetoComparacaoLista pOutro)
        {
            List<string> diferencas = new List<string>();

            if (pOutro is CantoneiraComparacaoLista)
            {
                diferencas.Add("Objetos diferentes cantoneira/chapa");
            }
            else
            {
                diferencas.AddRange(this.CompararObjeto(pOutro));

                ChapaComparacaoLista outraChapaComparacao = pOutro as ChapaComparacaoLista;

                if (Math.Abs(this.Largura - outraChapaComparacao.Largura) > 0.1)
                {
                    diferencas.Add(string.Format("Largura: {0}mm => {1}/{2}", (outraChapaComparacao.Largura - this.Largura), this.Largura, outraChapaComparacao.Largura));
                }

                if (Math.Abs(this.Espessura - pOutro.Espessura) > 0.1)
                {
                    diferencas.Add(string.Format("Espessura: {0}mm => {1}/{2}", (outraChapaComparacao.Espessura - this.Espessura), this.Espessura, outraChapaComparacao.Espessura));
                }
            }

            return diferencas;
        }

    }
}
