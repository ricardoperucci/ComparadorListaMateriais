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
            //string template = "Quantidade de recortes alterada p/ {0} ({1} {2})";
            string template = "{0} {1} {2}";

            string acao = "";
            if(this.DeltaValores > 0)
            {
                acao = this.DeltaValores > 1 ? "Acrescentados recortes" : "Acrescentado recorte";
            }
            else
            {
                acao = this.DeltaValores < -1 ? "Removidos recortes" : "Removido recorte";
            }

            string qtde = "XXX=0000";
            string justif = "XXXXXXX";
            for (int i = 0; i < Math.Abs(this.DeltaValores) - 1; i++)
            {
                if (i == Math.Abs(this.DeltaValores) - 2)
                {
                    qtde = qtde + " e XXX=0000";
                    justif = justif + " e XXXXXXX";
                }
                else
                {
                    qtde = qtde + ", XXX=0000";
                    justif = justif + ", XXXXXXX";
                }
            }

            string finalTexto = this.DeltaValores > 0 ? string.Format("para evitar colisão com {0}", justif) : "";

            return string.Format(template, acao, qtde, finalTexto);
        }

        public override string EscreveErroTxt()
        {
            return string.Format("Recortes: {0} / {1}", this.ValorOriginal, this.ValorNovo);
        }

    }
}
