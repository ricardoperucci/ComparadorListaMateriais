using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias
{
    public class DivergenciaFuroCalota : ErroPosicao
    {
        public double ValorOriginal { get; set; }
        public double ValorNovo { get; set; }

        public DivergenciaFuroCalota(double pOriginal, double pNovo) : base(EnumErrosPosicao.DiametroFuroCalota, true)
        {
            this.ValorOriginal = pOriginal;
            this.ValorNovo = pNovo;
        }

        public override string EscreveErroExcel()
        {
            return string.Format("Diâmetro do furo da calota alterado de {0}mm p/ {1}mm", this.ValorOriginal, this.ValorNovo);
        }

        public override string EscreveErroTxt()
        {
            return string.Format("Diâmetro furo calota: {0}/{1}", this.ValorOriginal, this.ValorNovo);
        }
    }
}
