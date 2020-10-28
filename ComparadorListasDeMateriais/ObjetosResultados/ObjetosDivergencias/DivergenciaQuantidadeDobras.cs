using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias
{
    public class DivergenciaQuantidadeDobras : ErroPosicao
    {
        public int ValorOriginal { get; set; }
        public int ValorNovo { get; set; }

        public int DeltaValores { get; set; }

        public DivergenciaQuantidadeDobras(int pValorOriginal, int pValorNovo) : base(EnumErrosPosicao.Dobras, true)
        {
            this.ValorOriginal = pValorOriginal;
            this.ValorNovo = pValorNovo;

            this.DeltaValores = pValorNovo - pValorOriginal;
        }

        public override string EscreveErroExcel()
        {
            string template = "Quantidade de dobras alterada p/ {0} ({1} {2})";

            string acao = "";
            if (this.DeltaValores > 0)
            {
                acao = this.DeltaValores > 1 ? "dobras adicionadas" : "dobra adicionada";
            }
            else
            {
                acao = this.DeltaValores < -1 ? "dobras removidas" : "dobra removida";
            }

            return string.Format(template, this.ValorNovo.ToString(), Math.Abs(this.DeltaValores).ToString(), acao);
        }

        public override string EscreveErroTxt()
        {
            return string.Format("Dobras: {0} / {1}", this.ValorOriginal, this.ValorNovo);
        }
    }
}
