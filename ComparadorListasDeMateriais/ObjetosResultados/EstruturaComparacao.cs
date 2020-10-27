using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ObjetosResultados
{
    public class EstruturaComparacao
    {
        public string NomeEstruturaSaida { get; set; }

        public List<string> PosicoesSomenteListaOriginal { get; set; }
        public List<string> PosicoesSomenteListaNova { get; set; }

        public List<PosicaoComparacao> ListaPosicoesComErros { get; set; }

        public EstruturaComparacao()
        {
            this.PosicoesSomenteListaOriginal = new List<string>();
            this.PosicoesSomenteListaNova = new List<string>();

            this.ListaPosicoesComErros = new List<PosicaoComparacao>();
        }
    }
}
