using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias
{
    public class DivergenciaEntreEstruturaLista : ErroPosicao
    {
        public List<string> EstruturasQueContem { get; set; }

        public string ListaErrada { get; set; }

        public string Posicao { get; set; }

        public DivergenciaEntreEstruturaLista(List<string> pListaEstruturas, string pQualLista, string pPosicao) : base(EnumErrosPosicao.EntreEstruturasLista, true)
        {
            this.EstruturasQueContem = pListaEstruturas;
            this.ListaErrada = pQualLista;
            this.Posicao = pPosicao;
        }

        public override string EscreveErroExcel()
        {
            return string.Format("{0} - Informações da posição divergentes entre as estruturas {1}", this.ListaErrada, string.Join(", ", this.EstruturasQueContem));
        }

        public override string EscreveErroTxt()
        {
            return string.Format("Infos diferentes entre estruturas na {0}", this.ListaErrada);
        }

    }
}
