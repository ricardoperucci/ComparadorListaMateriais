using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias
{
    public class DivergenciaPecasTipo : ErroPosicao
    {
        public DivergenciaPecasTipo() : base(EnumErrosPosicao.PecasDiferentes, true)
        {

        }

        public override string EscreveErroExcel()
        {
            return "Peças diferentes em cada lista (cantoneira/chapa)";
        }
    }
}
