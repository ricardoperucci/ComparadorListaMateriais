using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias
{
    public class DivergenciaQuantidadePecas : ErroPosicao
    {
        public int ValorOriginal { get; set; }
        public int ValorNovo { get; set; }

        public int DeltaValores { get; set; }

        public DivergenciaQuantidadePecas(int pValorOriginal, int pValorNovo) : base(EnumErrosPosicao.Quantidade, true)
        {
            this.ValorOriginal = pValorOriginal;
            this.ValorNovo = pValorNovo;

            this.DeltaValores = pValorOriginal - pValorNovo;
        }

        public override string EscreveErroExcel()
        {
            return string.Format("Quantidade de peças alterada de {0} p/ {1}", this.ValorOriginal, this.ValorNovo);
        }

    }
}
