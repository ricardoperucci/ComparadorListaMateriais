using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorListasDeMateriais.ObjetosResultados
{
    public class ObjetoCabecalho
    {
        public string TituloRelatorio = "Comparação listas de materiais";

        public string CaminhoArquivoListaOriginal { get; set; }
        public string CaminhoArquivoListaNova { get; set; }

        public string Data { get; set; }
        public string Horario { get; set; }

        public ObjetoCabecalho(string pCaminhoArquivoListaOriginal, string pCaminhoArquivoListaNova)
        {
            this.CaminhoArquivoListaOriginal = pCaminhoArquivoListaOriginal;
            this.CaminhoArquivoListaNova = pCaminhoArquivoListaNova;

            this.Data = "Data: " + DateTime.Today.Day.ToString() + "/" + (DateTime.Today.Month.ToString().Count() == 1 ? "0" + DateTime.Today.Month.ToString() : DateTime.Today.Month.ToString()) + "/" + DateTime.Today.Year;
            this.Horario = "Horário: " + DateTime.Now.ToLongTimeString();
        }

        /// <summary>
        /// Escreve o cabecalho do relatório
        /// </summary>
        /// <returns></returns>
        public string EscreveCabecalho()
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.AppendLine(this.TituloRelatorio);

            strBuilder.AppendLine(string.Format("Arquivos: {0} e {1}", CaminhoArquivoListaOriginal.Split('/').Last().Split('\\').Last(), CaminhoArquivoListaNova.Split('/').Last().Split('\\').Last()));
            
            strBuilder.AppendLine(this.Data);
            strBuilder.AppendLine();

            strBuilder.AppendLine(this.Horario);
            strBuilder.AppendLine();

            return strBuilder.ToString();
        }
    }
}
