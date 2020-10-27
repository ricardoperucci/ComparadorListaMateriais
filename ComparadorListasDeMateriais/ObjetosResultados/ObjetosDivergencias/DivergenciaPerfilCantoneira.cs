using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias
{
    public class DivergenciaPerfilCantoneira : ErroPosicao
    {
        public string PerfilOriginal { get; set; }
        public string PerfilNovo { get; set; }

        public DivergenciaPerfilCantoneira(string pOriginal, string pNovo) : base(EnumErrosPosicao.PerfilCantoneira, true)
        {
            this.PerfilOriginal = pOriginal;
            this.PerfilNovo = pNovo;
        }

        public override string EscreveErroExcel()
        {
            return string.Format("Perfil alterado de {0} p/ {1}", this.PerfilOriginal, this.PerfilNovo);
        }
    }
}
