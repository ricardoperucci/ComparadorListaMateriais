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
        public double DiametroCalota { get; set; }
        public double DiametroFuroCalota { get; set; }
        public double RaioUsinagemCalota { get; set; }

        /// <summary>
        /// Construtor para lista em EXCEL
        /// </summary>
        /// <param name="pLinhaExcel"></param>
        /// <param name="pEstrutura"></param>
        public ChapaComparacaoLista(List<string> pLinhaExcel, string pEstrutura)
        {
            this.SalvarInformacoesObjetoByLinhaExcel(pLinhaExcel, pEstrutura);

            string[] perfilSplitX = pLinhaExcel[4].Split(new string[] { " x " }, StringSplitOptions.RemoveEmptyEntries);
            this.Espessura = System.Convert.ToDouble(perfilSplitX.First().Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries).Last().Replace(",","."));

            if (pLinhaExcel[4].ToLower().Contains("calota") || (perfilSplitX.Last().Contains("d") && perfilSplitX.Last().Contains("r")))
            {
                SalvarInformacoesCalota(perfilSplitX.Last());

                return;
            }

            this.Largura = System.Convert.ToDouble(pLinhaExcel[4].Split(new string[] { " x " }, StringSplitOptions.RemoveEmptyEntries).Last().Replace(",", "."));

            double comp = this.Comprimento;
            double larg = this.Largura;

            this.Comprimento = comp > larg ? comp : larg;

            this.Largura = comp > larg ? larg : comp;
        }

        /// <summary>
        /// Construtor para lista em Txt
        /// </summary>
        /// <param name="pLinhaTxt"></param>
        /// <param name="pEstrutura"></param>
        public ChapaComparacaoLista(string pLinhaTxt, string pEstrutura)
        {
            this.SalvarInformacoesObjetoByLinhaTxt(pLinhaTxt, pEstrutura);

            string perfil = pLinhaTxt.Substring(14, 24).Replace(" ", "");

            var perfilSplit = perfil.Split(new string[] { "x" }, StringSplitOptions.RemoveEmptyEntries);

            this.SalvarSiglaMaterial(perfilSplit[0].ToLower().Replace("c", ""));

            this.Espessura = System.Convert.ToDouble(perfilSplit[0].ToLower().Replace(this.SiglaMaterial.ToLower(), "").Replace("c", "").Replace(",", "."));

            if (perfil.Contains("/") && perfil.Contains("r"))
            {
                this.Calota = true;

                string perfilSplitDepoisX = perfil.Split(new string[] { "x" }, StringSplitOptions.RemoveEmptyEntries).Last().Replace(" ","");
                SalvarInformacoesCalota(perfilSplitDepoisX);

                return;
            }

            double comp = System.Convert.ToDouble(perfilSplit[1].Replace(",", "."));

            double larg = System.Convert.ToDouble(perfilSplit.Last().Replace(",", "."));

            this.Comprimento = comp > larg ? comp : larg;

            this.Largura = comp > larg ? larg : comp;
        }

        /// <summary>
        /// Compara o objeto com o outro
        /// </summary>
        /// <param name="pOutro"></param>
        /// <returns></returns>
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

                if (this.Calota)
                {
                    if (Math.Abs(this.DiametroCalota - outraChapaComparacao.DiametroCalota) > 0.1)
                    {
                        diferencas.Add(string.Format("DiametroCalota: {0}mm => {1}/{2}", (outraChapaComparacao.DiametroCalota - this.DiametroCalota), this.DiametroCalota, outraChapaComparacao.DiametroCalota));
                    }

                    if (Math.Abs(this.DiametroFuroCalota - outraChapaComparacao.DiametroFuroCalota) > 0.1)
                    {
                        diferencas.Add(string.Format("DiametroFuroCalota: {0}mm => {1}/{2}", (outraChapaComparacao.DiametroFuroCalota - this.DiametroFuroCalota), this.DiametroFuroCalota, outraChapaComparacao.DiametroFuroCalota));
                    }

                    if (Math.Abs(this.RaioUsinagemCalota - outraChapaComparacao.RaioUsinagemCalota) > 0.1)
                    {
                        diferencas.Add(string.Format("RaioUsinagemCalota: {0}mm => {1}/{2}", (outraChapaComparacao.RaioUsinagemCalota - this.RaioUsinagemCalota), this.RaioUsinagemCalota, outraChapaComparacao.RaioUsinagemCalota));
                    }
                }

                else
                {
                    if (Math.Abs(this.Largura - outraChapaComparacao.Largura) > 0.1)
                    {
                        diferencas.Add(string.Format("Largura: {0}mm => {1}/{2}", (outraChapaComparacao.Largura - this.Largura), this.Largura, outraChapaComparacao.Largura));
                    }

                    if (Math.Abs(this.Espessura - pOutro.Espessura) > 0.1)
                    {
                        diferencas.Add(string.Format("Espessura: {0}mm => {1}/{2}", (outraChapaComparacao.Espessura - this.Espessura), this.Espessura, outraChapaComparacao.Espessura));
                    }
                }
            }

            return diferencas;
        }

        /// <summary>
        /// Salva as informações da calota em lista txt
        /// </summary>
        /// <param name="pPerfilSplitX"></param>
        private void SalvarInformacoesCalota(string pPerfil)
        {
            this.Calota = true;

            string diametroString = pPerfil.Split('/').First().ToLower().Replace("d", "").Replace("Ø", "").Replace("ø", "");
            string diametroFuroString = pPerfil.Split('/').Last().ToLower().Split('r').First();
            string raioUsinagemString = pPerfil.ToLower().Split('r').Last().Split(' ').First().Split('(').First();

            SalvarInformacoesCalota(diametroString, diametroFuroString, raioUsinagemString);
        }

        /// <summary>
        /// Salva as informações da calota pela string
        /// </summary>
        /// <param name="pDiametroString"></param>
        /// <param name="pDiametroFuroString"></param>
        /// <param name="pRaioUsinagemString"></param>
        private void SalvarInformacoesCalota(string pDiametroString, string pDiametroFuroString, string pRaioUsinagemString)
        {
            try
            {
                this.DiametroCalota = System.Convert.ToDouble(pDiametroString);
            }
            catch
            {
                this.DiametroCalota = 0;
            }

            try
            {
                this.DiametroFuroCalota = System.Convert.ToDouble(pDiametroFuroString);
            }
            catch
            {
                this.DiametroFuroCalota = 0;
            }

            try
            {
                this.RaioUsinagemCalota = System.Convert.ToDouble(pRaioUsinagemString);
            }
            catch
            {
                this.RaioUsinagemCalota = 0;
            }
        }
    }
}
