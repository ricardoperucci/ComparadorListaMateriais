using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias
{
    public class DivergenciaDiametroCalota : ErroPosicao
    {
        public double ValorOriginal { get; set; }
        public double ValorNovo { get; set; }

        public DivergenciaDiametroCalota(double pOriginal, double pNovo) : base(EnumErrosPosicao.DiametroCalota, true)
        {
            this.ValorOriginal = pOriginal;
            this.ValorNovo = pNovo;
        }

        public override string EscreveErroExcel()
        {
            return string.Format("Diâmetro da calota alterado de {0}mm p/ {1}mm", this.ValorOriginal, this.ValorNovo);
        }

        public override string EscreveErroTxt()
        {
            return string.Format("Diâmetro calota: {0} / {1}", this.ValorOriginal, this.ValorNovo);
        }
    }
}
