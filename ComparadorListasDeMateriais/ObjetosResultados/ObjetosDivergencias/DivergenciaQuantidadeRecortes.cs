using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias
{
    public class DivergenciaQuantidadeRecortes : ErroPosicao
    {
        public int ValorOriginal { get; set; }
        public int ValorNovo { get; set; }

        public int DeltaValores { get; set; }

        public DivergenciaQuantidadeRecortes(int pValorOriginal, int pValorNovo) : base(EnumErrosPosicao.Recortes, true)
        {
            this.ValorOriginal = pValorOriginal;
            this.ValorNovo = pValorNovo;

            this.DeltaValores = pValorNovo - pValorOriginal;
        }

        public override string EscreveErroExcel()
        {
            string template = "Quantidade de recortes alterada p/ {0} ({1} {2})";

            string acao = "";
            if(this.DeltaValores > 0)
            {
                acao = this.DeltaValores > 1 ? "recortes adicionados" : "recorte adicionado";
            }
            else
            {
                acao = this.DeltaValores < -1 ? "recortes removidos" : "recorte removido";
            }

            return string.Format(template, this.ValorNovo.ToString(), Math.Abs(this.DeltaValores).ToString(), acao);
        }

    }
}
