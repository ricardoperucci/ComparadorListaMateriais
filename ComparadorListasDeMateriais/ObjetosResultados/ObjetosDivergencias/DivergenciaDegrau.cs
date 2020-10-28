using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias
{
    public class DivergenciaDegrau : ErroPosicao
    {
        public bool TemDegrauOriginal { get; set; }
        public bool TemDegrauNovo { get; set; }

        public DivergenciaDegrau(bool pOriginal, bool pNovo) : base(EnumErrosPosicao.Degrau, true)
        {
            this.TemDegrauOriginal = pOriginal;
            this.TemDegrauNovo = pNovo;
        }

        public override string EscreveErroExcel()
        {
            string template = "{0} parafuso degrau";
            string acao = "";
            if (this.TemDegrauNovo)
                acao = "Adicionado";
            else
                acao = "Removido";

            return string.Format(template, acao);
        }

        public override string EscreveErroTxt()
        {
            return EscreveErroExcel();
        }
    }
}
