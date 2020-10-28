using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias
{
    public class DivergenciaMaterial : ErroPosicao
    {
        public DivergenciaMaterial() : base(EnumErrosPosicao.Material, true)
        {

        }

        public override string EscreveErroExcel()
        {
            return "Material alterado";
        }

        public override string EscreveErroTxt()
        {
            return "Material";
        }
    }
}
