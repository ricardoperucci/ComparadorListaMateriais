using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias
{
    public class DivergenciaPerfilCantoneira : ErroPosicao
    {
        public string ValorOriginal { get; set; }
        public string ValorNovo { get; set; }

        public DivergenciaPerfilCantoneira(string pOriginal, string pNovo) : base(EnumErrosPosicao.PerfilCantoneira, true)
        {
            this.ValorOriginal = pOriginal;
            this.ValorNovo = pNovo;
        }

        public override string EscreveErroExcel()
        {
            return string.Format("Perfil alterado de {0} p/ {1}", this.ValorOriginal, this.ValorNovo);
        }

        public override string EscreveErroTxt()
        {
            return string.Format("Perfil: {0} / {1}", this.ValorOriginal, this.ValorNovo);
        }
    }
}
