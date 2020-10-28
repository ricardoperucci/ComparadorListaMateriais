using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias
{
    public class DivergenciaRaioUsinagemCalota : ErroPosicao
    {
        public double ValorOriginal { get; set; }
        public double ValorNovo { get; set; }

        public DivergenciaRaioUsinagemCalota(double pOriginal, double pNovo) : base(EnumErrosPosicao.RaioUsinagemCalota, true)
        {
            this.ValorOriginal = pOriginal;
            this.ValorNovo = pNovo;
        }

        public override string EscreveErroExcel()
        {
            return string.Format("Raio de usinagem da calota alterado de {0} p/ {1}", this.ValorOriginal, this.ValorNovo);
        }

        public override string EscreveErroTxt()
        {
            return string.Format("Raio de usinagem {0} / {1}", this.ValorOriginal, this.ValorNovo);
        }
    }
}
