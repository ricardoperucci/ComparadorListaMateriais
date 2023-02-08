using ModelagemTorre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ComparadorCAM
{
    public static class EstaticosComparacaoCAM
    {
        private static List<double> _DiametrosPadraoBrametal = new List<double> { 14.7, 18, 21 };

        private static List<double> _DiametrosPadraoPadrao = new List<double> { 14.3, 17.5, 20.6 };

        public static List<double> GetListaDiametrosPadrao(FabricanteEnum pFabricante)
        {
            if (pFabricante == FabricanteEnum.Brametal)
                return _DiametrosPadraoBrametal;
            else
                return _DiametrosPadraoPadrao;
        }
    }
}
