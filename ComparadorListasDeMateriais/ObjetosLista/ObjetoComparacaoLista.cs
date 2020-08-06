using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ObjetosLista
{
    public abstract class ObjetoComparacaoLista
    {
        public string Estrutura { get; set; }

        public string NumeracaoComMaterial { get; set; }

        public string NumeracaoSemMaterial { get; set; }

        public string SiglaMaterial { get; set; }

        public int QuantidadePecasNoComponente { get; set; }

        public int QuantidadeRecortes { get; set; }
        public int QuantidadeDobras { get; set; }
        public int QuantidadeChanfros { get; set; }

        public bool TemDegrau { get; set; }

        public string NumeracaoDireitaComSigla { get; set; }

        public string NumeracaoConjuntoSoldado { get; set; }

        public double Comprimento { get; set; }

        public int QuantidadeFurosModelo { get; set; }

        public double Espessura { get; set; }

        public ObjetoComparacaoLista()
        {

        }

        public abstract List<string> CompararComObjeto(ObjetoComparacaoLista pOutro);

        protected  List<string> CompararObjeto(ObjetoComparacaoLista pOutro)
        {
            List<string> diferencas = new List<string>();

            if (Math.Abs(this.Comprimento - pOutro.Comprimento) > 0.1)
            {
                diferencas.Add(string.Format("Comprimento: {0}mm => {1}/{2}", (pOutro.Comprimento - this.Comprimento), this.Comprimento, pOutro.Comprimento));
            }

            if (this.SiglaMaterial != pOutro.SiglaMaterial)
            {
                diferencas.Add("Material");
            }

            if (this.QuantidadePecasNoComponente != pOutro.QuantidadePecasNoComponente)
            {
                diferencas.Add(string.Format("Quantidade: {0}/{1}", this.QuantidadePecasNoComponente, pOutro.QuantidadePecasNoComponente));
            }

            if (this.QuantidadeRecortes != pOutro.QuantidadeRecortes)
            {
                diferencas.Add(string.Format("Recortes: {0}/{1}", this.QuantidadeRecortes, pOutro.QuantidadeRecortes));
            }

            if (this.QuantidadeDobras != pOutro.QuantidadeDobras)
            {
                diferencas.Add(string.Format("Dobras: {0}/{1}", this.QuantidadeDobras, pOutro.QuantidadeDobras));
            }

            if (this.QuantidadeChanfros != pOutro.QuantidadeChanfros)
            {
                diferencas.Add(string.Format("Chanfros: {0}/{1}", this.QuantidadeChanfros, pOutro.QuantidadeChanfros));
            }

            if (this.TemDegrau != pOutro.TemDegrau)
            {
                diferencas.Add(string.Format("Parafuso Degrau"));
            }

            if (this.NumeracaoDireitaComSigla != pOutro.NumeracaoDireitaComSigla)
            {
                diferencas.Add(string.Format("Peça direita"));
            }

            if (this.NumeracaoConjuntoSoldado != pOutro.NumeracaoConjuntoSoldado)
            {
                diferencas.Add(string.Format("Conjunto Soldado"));
            }

            return diferencas;
        }

        public void SalvarInformacoesObjetoByLinhaExcel(List<string> pLinhaExcel, string pEstrutura)
        {
            this.Estrutura = pEstrutura;

            this.NumeracaoComMaterial = pLinhaExcel[1].ToString().Split(new string[] { ".0" }, StringSplitOptions.RemoveEmptyEntries).First().Split(new string[] { ",0" }, StringSplitOptions.RemoveEmptyEntries).First().Replace(" ", "");

            this.NumeracaoSemMaterial = this.NumeracaoComMaterial.ToLower().Last().Equals('h') || this.NumeracaoComMaterial.ToLower().Last().Equals('g') ? this.NumeracaoComMaterial.Remove(this.NumeracaoComMaterial.Count() - 1) : this.NumeracaoComMaterial;

            this.QuantidadePecasNoComponente = System.Convert.ToInt32(pLinhaExcel[3]);

            this.QuantidadeRecortes = (pLinhaExcel[9] == "" || pLinhaExcel[9] == " ") ? 0 : System.Convert.ToInt32(pLinhaExcel[9]);
            this.QuantidadeDobras = (pLinhaExcel[10] == "" || pLinhaExcel[10] == " ") ? 0 : System.Convert.ToInt32(pLinhaExcel[10]);
            this.QuantidadeChanfros = (pLinhaExcel[11] == "" || pLinhaExcel[11] == " ") ? 0 : System.Convert.ToInt32(pLinhaExcel[11]);

            this.NumeracaoConjuntoSoldado = pLinhaExcel[0];

            this.Comprimento = string.IsNullOrEmpty(pLinhaExcel[6]) ? 0 : System.Convert.ToDouble(pLinhaExcel[6].Replace(",", "."));

            this.SiglaMaterial = pLinhaExcel[5];

            this.TemDegrau = pLinhaExcel[14].Contains("X");

            this.NumeracaoDireitaComSigla = pLinhaExcel[13];
        }

        public void SalvarInformacoesObjetoByLinhaTxt(string pLinhaTxt, string pEstrutura)
        {
            string linhaCorrigida = pLinhaTxt.PadRight(70);

            this.Estrutura = pEstrutura;

            this.QuantidadePecasNoComponente = System.Convert.ToInt32(linhaCorrigida.Substring(0, 4).Replace(" ",""));

            this.NumeracaoComMaterial = linhaCorrigida.Substring(4,10).Replace(" ", "");

            this.NumeracaoSemMaterial = this.NumeracaoComMaterial.ToLower().Last().Equals('h') || this.NumeracaoComMaterial.ToLower().Last().Equals('g') ? this.NumeracaoComMaterial.Remove(this.NumeracaoComMaterial.Count() - 1) : this.NumeracaoComMaterial;

            this.QuantidadeRecortes = System.Convert.ToInt32(linhaCorrigida.Substring(38, 2).Replace(" ", ""));
            this.QuantidadeDobras = System.Convert.ToInt32(linhaCorrigida.Substring(40, 2).Replace(" ", ""));
            //this.QuantidadeChanfros = System.Convert.ToInt32(pLinhaTxt.Substring(42, 5).Replace(" ", ""));
            this.QuantidadeChanfros = System.Convert.ToInt32(linhaCorrigida.Substring(42, 2).Replace(" ", ""));

            if (this.NumeracaoComMaterial.Contains("439"))
            {
                var k = 0;
            }

            this.NumeracaoDireitaComSigla = linhaCorrigida.Substring(47, 7).Replace(" ", "");

            this.TemDegrau = linhaCorrigida.Substring(54, 5).Replace(" ", "").ToLower().Contains("x");

            this.NumeracaoConjuntoSoldado = linhaCorrigida.Substring(59).Replace(" ", "");

            return;
        }

        protected void SalvarSiglaMaterial(string pParteEspessura)
        {
            if (pParteEspessura.ToLower().Contains("h"))
            {
                this.SiglaMaterial = "H";
            }
            else if (pParteEspessura.ToLower().Contains("*"))
            {
                this.SiglaMaterial = "*";
            }
            else if (char.IsDigit(pParteEspessura.Last()))
            {
                this.SiglaMaterial = "S";
            }
            else if (pParteEspessura.ToLower().Contains("g"))
            {
                this.SiglaMaterial = "G";
            }
        }


    }
}
