using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias
{
    public class DivergenciaEsquerda : ErroPosicao
    {
        public string NumeracaoDireitaOriginal { get; set; }
        public string NumeracaoDireitaNova { get; set; }

        public DivergenciaEsquerda(string pOriginal, string pNovo) : base(EnumErrosPosicao.DireitaEsquerda, true)
        {
            this.NumeracaoDireitaOriginal = pOriginal;
            this.NumeracaoDireitaNova = pNovo;
        }

        public override string EscreveErroExcel()
        {
            if (string.IsNullOrEmpty(NumeracaoDireitaOriginal))
            {
                return string.Format("Adicionado peça esquerda da posição {0}", NumeracaoDireitaNova);
            }

            else if (!string.IsNullOrEmpty(this.NumeracaoDireitaNova))
            {
                return string.Format("Informação peça esquerda alterada p/ posição {0}", NumeracaoDireitaNova);
            }

            else
            {
                return "Removida informação de peça esquerda";
            }
        }
    }
}
