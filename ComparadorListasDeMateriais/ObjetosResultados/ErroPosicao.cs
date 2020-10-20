using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ObjetosResultados
{
    public class ErroPosicao
    {
        public string ValorOriginal { get; set; }
        public string ValorNovo { get; set; }

        public string DeltaValores { get; set; }

        public EnumErrosPosicao TipoErroEnum { get; set; }

        public bool NcOuMelhoria { get; set; }

        public ErroPosicao(EnumErrosPosicao pTipoEnum, string pValorOriginal, string pValorNovo, string pDeltaValores, bool pNcOuMelhoria)
        {
            this.TipoErroEnum = pTipoEnum;
            this.ValorOriginal = pValorOriginal;
            this.ValorNovo = pValorNovo;
            this.NcOuMelhoria = pNcOuMelhoria;
            this.DeltaValores = pDeltaValores;
        }

        /// <summary>
        /// Escreve o texto do erro
        /// </summary>
        /// <returns></returns>
        public string EscreveErro()
        {
            if (this.TipoErroEnum.Equals(EnumErrosPosicao.Comprimento))
            {
                return string.Format("Comprimento: {0}mm => {1}/{2}", this.DeltaValores, this.ValorOriginal, this.ValorNovo);
            }

            if (this.TipoErroEnum.Equals(EnumErrosPosicao.Material))
            {
                return "Material";
            }

            if (this.TipoErroEnum.Equals(EnumErrosPosicao.Quantidade))
            {
                return string.Format("Quantidade: {0}/{1}", this.ValorOriginal, this.ValorNovo);
            }

            if (this.TipoErroEnum.Equals(EnumErrosPosicao.Recortes))
            {
                return string.Format("Recortes: {0}/{1}", this.ValorOriginal, this.ValorNovo);
            }

            if (this.TipoErroEnum.Equals(EnumErrosPosicao.Dobras))
            {
                return string.Format("Dobras: {0}/{1}", this.ValorOriginal, this.ValorNovo);
            }

            if (this.TipoErroEnum.Equals(EnumErrosPosicao.Chanfros))
            {
                return string.Format("Chanfros: {0}/{1}", this.ValorOriginal, this.ValorNovo);
            }

            if (this.TipoErroEnum.Equals(EnumErrosPosicao.Degrau))
            {
                return string.Format("Parafuso Degrau");
            }

            if (this.TipoErroEnum.Equals(EnumErrosPosicao.DireitaEsquerda))
            {
                return string.Format("Peça direita");
            }

            if (this.TipoErroEnum.Equals(EnumErrosPosicao.ConjuntoSoldado))
            {
                return string.Format("Conjunto Soldado");
            }

            if (this.TipoErroEnum.Equals(EnumErrosPosicao.PecasDiferentes))
            {
                return "Objetos diferentes cantoneira/chapa";
            }

            if (this.TipoErroEnum.Equals(EnumErrosPosicao.PerfilCantoneira))
            {
                return string.Format("Perfil: {0} / {1}", this.ValorOriginal, this.ValorNovo);
            }

            if (this.TipoErroEnum.Equals(EnumErrosPosicao.DiametroCalota))
            {
               return string.Format("DiametroCalota: {0}mm => {1}/{2}", this.DeltaValores, this.ValorOriginal, this.ValorNovo);
            }

            if (this.TipoErroEnum.Equals(EnumErrosPosicao.DiametroFuroCalota))
            {
                return string.Format("DiametroFuroCalota: {0}mm => {1}/{2}", this.DeltaValores, this.ValorOriginal, this.ValorNovo);
            }

            if (this.TipoErroEnum.Equals(EnumErrosPosicao.RaioUsinagemCalota))
            {
                return string.Format("RaioUsinagemCalota: {0}mm => {1}/{2}", this.DeltaValores, this.ValorOriginal, this.ValorNovo);
            }

            if (this.TipoErroEnum.Equals(EnumErrosPosicao.Largura))
            {
                return string.Format("Largura: {0}mm => {1}/{2}", this.DeltaValores, this.ValorOriginal, this.ValorNovo);
            }

            if (this.TipoErroEnum.Equals(EnumErrosPosicao.Espessura))
            {
                return string.Format("Espessura: {0}mm => {1}/{2}", this.DeltaValores, this.ValorOriginal, this.ValorNovo);
            }

            return "";
        }



    }
}
