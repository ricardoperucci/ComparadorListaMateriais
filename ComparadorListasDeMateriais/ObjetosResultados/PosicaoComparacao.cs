using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ObjetosResultados
{
    public class PosicaoComparacao
    {
        public string NumeracaoString { get; set; }

        public List<ErroPosicao> ListaErrosPosicao { get; set; }

        public bool TeveErro { get; set; }
        public bool TeveMelhoria { get; set; }

        public PosicaoComparacao(List<ErroPosicao> pListaDivergencias, string pNumeracao)
        {
            this.ListaErrosPosicao = pListaDivergencias;

            this.NumeracaoString = pNumeracao;
        }
    }
}
