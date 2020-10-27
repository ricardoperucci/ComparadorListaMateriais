using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ObjetosResultados
{
    public class ObjetoResultadoComparacao
    {
        public ObjetoCabecalho ObjetoCabecalho { get; set; }

        public List<string> EstruturasSomenteListaOriginal { get; set; }
        public List<string> EstruturasSomenteListaNova { get; set; }

        public List<EstruturaComparacao> ListaEstruturasComparadas { get; set; }

        public ObjetoResultadoComparacao(ObjetoCabecalho pObjetoCabecalho, List<string> pEstruturasSomenteListaOriginal, List<string> pEstruturasSomenteListaNova)
        {
            this.ObjetoCabecalho = pObjetoCabecalho;

            this.ListaEstruturasComparadas = new List<EstruturaComparacao>();

            this.EstruturasSomenteListaOriginal = pEstruturasSomenteListaOriginal;
            this.EstruturasSomenteListaNova = pEstruturasSomenteListaNova;
        }
    }
}
