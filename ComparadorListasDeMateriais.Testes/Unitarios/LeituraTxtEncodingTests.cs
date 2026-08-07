using ComparadorListasDeMateriais;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;

namespace ComparadorListasDeMateriais.Testes.Unitarios
{
    /// <summary>
    /// Regressão da migração .NET 8: listas legadas em Windows-1252 (ANSI) devem ser lidas com os
    /// acentos corretos. No .NET Framework isso funcionava via Encoding.Default; no .NET 8 Default
    /// virou UTF-8, então a leitura ANSI passou a ser explícita em FuncoesUteis.LerTxtLisaMateriais.
    /// </summary>
    [TestClass]
    public class LeituraTxtEncodingTests
    {
        [TestMethod]
        public void LerTxtLisaMateriais_ArquivoAnsiWindows1252_DecodificaAcentos()
        {
            // "3 - CABEÇA" em Windows-1252 (Ç = 0xC7), sem BOM — caso que quebrava após a migração.
            byte[] ansi = { 0x33, 0x20, 0x2D, 0x20, 0x43, 0x41, 0x42, 0x45, 0xC7, 0x41, 0x0D, 0x0A };
            string caminho = Path.Combine(Path.GetTempPath(), "lista_ansi_" + Guid.NewGuid().ToString("N") + ".txt");
            File.WriteAllBytes(caminho, ansi);

            try
            {
                string texto = FuncoesUteis.LerTxtLisaMateriais(caminho);
                StringAssert.Contains(texto, "CABEÇA");
            }
            finally
            {
                File.Delete(caminho);
            }
        }

        [TestMethod]
        public void LerTxtLisaMateriais_AnsiComPalavraForaDaHeuristica_DecodificaAcentos()
        {
            // "PÁRA- RAIOS EM SUSPENSÃO" em Windows-1252 (Á = 0xC1, Ã = 0xC3). Não contém "cabe"
            // nem "extens", então nenhuma das palavras especiais da heurística salva o caso: a
            // detecção precisa reconhecer o caractere de substituição (U+FFFD) por si só.
            byte[] ansi =
            {
                0x50, 0xC1, 0x52, 0x41, 0x2D, 0x20, 0x52, 0x41, 0x49, 0x4F, 0x53, 0x20,
                0x45, 0x4D, 0x20, 0x53, 0x55, 0x53, 0x50, 0x45, 0x4E, 0x53, 0xC3, 0x4F, 0x0D, 0x0A
            };

            string caminho = Path.Combine(Path.GetTempPath(), "lista_para_" + Guid.NewGuid().ToString("N") + ".txt");
            File.WriteAllBytes(caminho, ansi);

            try
            {
                string texto = FuncoesUteis.LerTxtLisaMateriais(caminho);

                Assert.IsFalse(texto.Contains('�'), "o texto não pode voltar com caracteres de substituição");
                StringAssert.Contains(texto, "PÁRA- RAIOS EM SUSPENSÃO");
            }
            finally
            {
                File.Delete(caminho);
            }
        }

        [TestMethod]
        public void LerTxtLisaMateriais_ArquivoUtf8_DecodificaAcentos()
        {
            string caminho = Path.Combine(Path.GetTempPath(), "lista_utf8_" + Guid.NewGuid().ToString("N") + ".txt");
            File.WriteAllText(caminho, "3 - CABEÇA\r\n", new UTF8Encoding(false));

            try
            {
                string texto = FuncoesUteis.LerTxtLisaMateriais(caminho);
                StringAssert.Contains(texto, "CABEÇA");
            }
            finally
            {
                File.Delete(caminho);
            }
        }
    }
}
