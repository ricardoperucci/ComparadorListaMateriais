using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias
{
    public class DivergenciaLargura : ErroPosicao
    {
        public double ValorOriginal { get; set; }
        public double ValorNovo { get; set; }

        public DivergenciaLargura(double pOriginal, double pNovo) : base(EnumErrosPosicao.Largura, true)
        {
            this.ValorOriginal = pOriginal;
            this.ValorNovo = pNovo;
        }

        public override string EscreveErroExcel()
        {
            return string.Format("Largura alterada de {0}mm p/ {1}mm", this.ValorOriginal, this.ValorNovo);
        }
    }
}
