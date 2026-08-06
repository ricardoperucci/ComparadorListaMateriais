using ComparadorListasDeMateriais.ComparadorV2;
using ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ComparadorListasDeMateriais.Testes.Unitarios
{
    /// <summary>
    /// Testes do renderizador de texto do relatório v2 (função pura, sem UI nem arquivos).
    /// </summary>
    [TestClass]
    public class RelatorioV2TxtTests
    {
        private static ResultadoComparacaoV2 Resultado(EstruturaComparacaoV2 pEstrutura = null)
        {
            var resultado = new ResultadoComparacaoV2
            {
                CaminhoListaOriginal = @"C:\obras\LM-TORRE-R0.txt",
                CaminhoListaNova = @"C:\obras\LM-TORRE-R1.txt"
            };

            if (pEstrutura != null)
                resultado.Estruturas.Add(pEstrutura);

            return resultado;
        }

        [TestMethod]
        public void Gerar_Cabecalho_TrazSoOsNomesDosArquivos()
        {
            string texto = RelatorioV2Txt.Gerar(Resultado());

            StringAssert.Contains(texto, "COMPARAÇÃO DE LISTAS DE MATERIAIS");
            StringAssert.Contains(texto, "LM-TORRE-R0.txt");
            StringAssert.Contains(texto, "LM-TORRE-R1.txt");
            Assert.IsFalse(texto.Contains(@"C:\obras"), "o caminho completo polui o cabeçalho");
        }

        [TestMethod]
        public void Gerar_MaterialTrocado_MostraDeParaNaMesmaLinha()
        {
            var estrutura = new EstruturaComparacaoV2 { NomeEstrutura = "01-CABEÇA" };
            estrutura.Pares.Add(new ParPosicoes
            {
                NumeracaoOriginal = "65H",
                NumeracaoNova = "65G",
                Tipo = TipoPar.MaterialTrocado
            });

            string texto = RelatorioV2Txt.Gerar(Resultado(estrutura));

            StringAssert.Contains(texto, "MATERIAL TROCADO");
            StringAssert.Contains(texto, "65H");
            StringAssert.Contains(texto, "→");
            StringAssert.Contains(texto, "65G");
        }

        [TestMethod]
        public void Gerar_PosicoesEliminadasENovas_UsamRotulosCorretos()
        {
            var estrutura = new EstruturaComparacaoV2 { NomeEstrutura = "01-CABEÇA" };
            estrutura.PosicoesEliminadas.Add("40H");
            estrutura.PosicoesNovas.Add("50G");

            string texto = RelatorioV2Txt.Gerar(Resultado(estrutura));

            StringAssert.Contains(texto, "POSIÇÕES ELIMINADAS");
            StringAssert.Contains(texto, "40H");
            StringAssert.Contains(texto, "POSIÇÕES NOVAS");
            StringAssert.Contains(texto, "50G");

            // O relatório antigo dizia "não encontradas na lista original" para as que só existiam
            // na original — rótulo invertido que não pode voltar.
            Assert.IsFalse(texto.Contains("não encontradas na lista original"));
        }

        [TestMethod]
        public void Gerar_SecaoVazia_NaoAparece()
        {
            var estrutura = new EstruturaComparacaoV2 { NomeEstrutura = "01-CABEÇA" };
            estrutura.PosicoesNovas.Add("50G");

            string texto = RelatorioV2Txt.Gerar(Resultado(estrutura));

            StringAssert.Contains(texto, "POSIÇÕES NOVAS");
            Assert.IsFalse(texto.Contains("POSIÇÕES ELIMINADAS"));
            Assert.IsFalse(texto.Contains("MATERIAL TROCADO"));
        }

        [TestMethod]
        public void Gerar_EstruturaSemDiferenca_OcupaUmaLinha()
        {
            var estrutura = new EstruturaComparacaoV2 { NomeEstrutura = "02-TRONCO" };
            estrutura.Pares.Add(new ParPosicoes { NumeracaoOriginal = "1H", NumeracaoNova = "1H", Tipo = TipoPar.Exato });

            string texto = RelatorioV2Txt.Gerar(Resultado(estrutura));

            StringAssert.Contains(texto, "Nenhuma diferença. 1 posição está igual nas duas listas.");
        }

        [TestMethod]
        public void Gerar_Divergencia_MostraOriginalNovoEDelta()
        {
            var estrutura = new EstruturaComparacaoV2 { NomeEstrutura = "01-CABEÇA" };
            var par = new ParPosicoes { NumeracaoOriginal = "42G", NumeracaoNova = "42G", Tipo = TipoPar.Exato };
            par.Divergencias.Add(new DivergenciaComprimento(980, 995));
            estrutura.Pares.Add(par);

            string texto = RelatorioV2Txt.Gerar(Resultado(estrutura));

            StringAssert.Contains(texto, "Comprimento");
            StringAssert.Contains(texto, "980,0");
            StringAssert.Contains(texto, "995,0");
            StringAssert.Contains(texto, "(+15,0 mm)");
        }

        [TestMethod]
        public void Gerar_CasoAmbiguo_ListaOsCandidatosDosDoisLados()
        {
            var estrutura = new EstruturaComparacaoV2 { NomeEstrutura = "01-CABEÇA" };
            estrutura.Ambiguos.Add(new GrupoAmbiguo
            {
                ChaveBase = "65",
                CandidatosOriginal = new List<string> { "65H", "65F" },
                CandidatosNova = new List<string> { "65G" }
            });

            string texto = RelatorioV2Txt.Gerar(Resultado(estrutura));

            StringAssert.Contains(texto, "REVISAR MANUALMENTE");
            StringAssert.Contains(texto, "Numeração 65");
            StringAssert.Contains(texto, "65H, 65F");
            StringAssert.Contains(texto, "65G");
        }

        [TestMethod]
        public void Gerar_ListasIguais_DizQueEstaoIguais()
        {
            string texto = RelatorioV2Txt.Gerar(Resultado());

            StringAssert.Contains(texto, "As duas listas estão iguais nas estruturas comparadas.");
        }

        [TestMethod]
        public void Gerar_EstruturasSoNumaLista_SaemNosAvisosGerais()
        {
            ResultadoComparacaoV2 resultado = Resultado();
            resultado.EstruturasSomenteOriginal.Add("04-MÍSULA 2");

            string texto = RelatorioV2Txt.Gerar(resultado);

            StringAssert.Contains(texto, "AVISOS GERAIS");
            StringAssert.Contains(texto, "ESTRUTURAS SÓ NA LISTA ORIGINAL");
            StringAssert.Contains(texto, "04-MÍSULA 2");
        }
    }
}
