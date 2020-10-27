using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias
{
    public class DivergenciaComprimento : ErroPosicao
    {
        public double ValorOriginal { get; set; }
        public double ValorNovo { get; set; }

        public double DeltaValores { get; set; }

        public DivergenciaComprimento(double pValorOriginal, double pValorNovo) : base(EnumErrosPosicao.Comprimento, Math.Abs(pValorOriginal - pValorNovo) > 2)
        {
            this.ValorOriginal = pValorOriginal;
            this.ValorNovo = pValorNovo;

            this.DeltaValores = pValorOriginal - pValorNovo;
        }

        public override string EscreveErroExcel()
        {
            return string.Format("Comprimento alterado de {0}mm p/ {1}mm", this.ValorOriginal, this.ValorNovo);
        }
    }
}
