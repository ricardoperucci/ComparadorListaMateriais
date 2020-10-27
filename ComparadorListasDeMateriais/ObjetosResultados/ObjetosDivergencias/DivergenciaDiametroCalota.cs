using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias
{
    public class DivergenciaDiametroCalota : ErroPosicao
    {
        public double DiametroOriginal { get; set; }
        public double DiametroNovo { get; set; }

        public DivergenciaDiametroCalota(double pOriginal, double pNovo) : base(EnumErrosPosicao.DiametroCalota, true)
        {
            this.DiametroOriginal = pOriginal;
            this.DiametroNovo = pNovo;
        }

        public override string EscreveErroExcel()
        {
            return string.Format("Diâmetro da calota alterado de {0}mm p/ {1}mm", this.DiametroOriginal, this.DiametroNovo);
        }
    }
}
