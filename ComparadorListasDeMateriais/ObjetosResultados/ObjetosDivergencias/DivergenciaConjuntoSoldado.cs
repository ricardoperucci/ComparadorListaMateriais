using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias
{
    public class DivergenciaConjuntoSoldado : ErroPosicao
    {
        public string NumeracaoCSOriginal { get; set; }
        public string NumeracaoCSNova { get; set; }

        public DivergenciaConjuntoSoldado(string pOriginal, string pNovo) : base(EnumErrosPosicao.ConjuntoSoldado, true)
        {
            this.NumeracaoCSOriginal = pOriginal;
            this.NumeracaoCSNova = pNovo;
        }

        public override string EscreveErroExcel()
        {
            if (string.IsNullOrEmpty(NumeracaoCSOriginal))
            {
                return string.Format("Adicionada numeração do conjunto soldado {0}", NumeracaoCSNova);
            }

            else if (!string.IsNullOrEmpty(this.NumeracaoCSNova))
            {
                return string.Format("Numeração do conjunto soldado alterada p/ {0}", NumeracaoCSNova);
            }

            else
            {
                return "Removida numeração do conjunto soldado";
            }
        }
    }
}
