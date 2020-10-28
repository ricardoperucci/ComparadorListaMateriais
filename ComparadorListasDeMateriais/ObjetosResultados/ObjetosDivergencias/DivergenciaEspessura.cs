using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias
{
    public class DivergenciaEspessura : ErroPosicao
    {
        public double ValorOriginal { get; set; }
        public double ValorNovo { get; set; }

        public DivergenciaEspessura(double pOriginal, double pNovo) : base(EnumErrosPosicao.Espessura, true)
        {
            this.ValorOriginal = pOriginal;
            this.ValorNovo = pNovo;
        }

        public override string EscreveErroExcel()
        {
            return string.Format("Espessura alterada de {0}mm p/ {1}mm", this.ValorOriginal, this.ValorNovo);
        }

        public override string EscreveErroTxt()
        {
            return string.Format("Espessura: {0}/{1}", this.ValorOriginal, this.ValorNovo);
        }
    }
}
