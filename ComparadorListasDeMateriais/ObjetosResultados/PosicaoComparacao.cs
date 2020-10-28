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


        public bool EscreveAlteracoes(bool pNCouMelhoria, out string alteracoes)
        {
            alteracoes = "";

            List<ErroPosicao> divergenciasPosicao = this.ListaErrosPosicao.Where(x => x.NcOuMelhoria == pNCouMelhoria).ToList();

            StringBuilder strBuilder = new StringBuilder();
            if (divergenciasPosicao.Count == 0)
            {
                return false;
            }

            else
            {
                for (int i = 0; i < divergenciasPosicao.Count(); i++)
                {
                    if (i == 0)
                        strBuilder.AppendLine(string.Format("       {0}: • {1}", this.NumeracaoString, divergenciasPosicao[i].EscreveErroTxt()));
                    else
                        strBuilder.AppendLine(string.Format("           • {1}", this.NumeracaoString, divergenciasPosicao[i].EscreveErroTxt()));
                }
            }

            alteracoes = strBuilder.ToString();
            return true;
        }
    }
}
