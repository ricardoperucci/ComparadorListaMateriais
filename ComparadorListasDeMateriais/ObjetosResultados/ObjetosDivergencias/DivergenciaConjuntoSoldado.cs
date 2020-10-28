using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias
{
    public class DivergenciaConjuntoSoldado : ErroPosicao
    {
        public string ValorOriginal { get; set; }
        public string ValorNovo { get; set; }

        public DivergenciaConjuntoSoldado(string pOriginal, string pNovo) : base(EnumErrosPosicao.ConjuntoSoldado, true)
        {
            this.ValorOriginal = pOriginal;
            this.ValorNovo = pNovo;
        }

        public override string EscreveErroExcel()
        {
            if (string.IsNullOrEmpty(ValorOriginal))
            {
                return string.Format("Adicionada numeração do conjunto soldado {0}", ValorNovo);
            }

            else if (!string.IsNullOrEmpty(this.ValorNovo))
            {
                return string.Format("Numeração do conjunto soldado alterada p/ {0}", ValorNovo);
            }

            else
            {
                return "Removida numeração do conjunto soldado";
            }
        }

        public override string EscreveErroTxt()
        {
            return string.Format("Conjunto soldado: {0} / {1}", this.ValorOriginal, this.ValorNovo);
        }
    }
}
