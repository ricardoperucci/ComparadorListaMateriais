using ComparadorListasDeMateriais.ComparadorV2;
using ComparadorListasDeMateriais.ObjetosLista;
using ComparadorListasDeMateriais.ObjetosResultados;
using ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias;
using ComparadorListasDeMateriais.Testes.Infra;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace ComparadorListasDeMateriais.Testes.Unitarios
{
    /// <summary>
    /// Testes do comparador v2 ligando pareamento e motor de divergências.
    /// </summary>
    [TestClass]
    public class ComparadorListasV2Tests
    {
        private static Dictionary<string, List<ObjetoComparacaoLista>> Estrutura(params ObjetoComparacaoLista[] objetos)
        {
            return ConstrutoresTeste.Estrutura("01-CABECA", objetos);
        }

        [TestMethod]
        public void Comparar_TrocaDeMaterialComMudancaDeComprimento_UmUnicoParComAsDuasInformacoes()
        {
            ResultadoComparacaoV2 resultado = new ComparadorListasV2().Comparar(
                Estrutura(ConstrutoresTeste.Chapa("65H", 1250, 60, 4.75, "H")),
                Estrutura(ConstrutoresTeste.Chapa("65G", 1268, 60, 4.75, "G")));

            EstruturaComparacaoV2 estrutura = resultado.Estruturas.Single();

            Assert.AreEqual(0, estrutura.PosicoesNovas.Count, "não pode virar posição nova");
            Assert.AreEqual(0, estrutura.PosicoesEliminadas.Count, "não pode virar posição eliminada");

            ParPosicoes par = estrutura.MaterialTrocado.Single();
            Assert.AreEqual("65H", par.NumeracaoOriginal);
            Assert.AreEqual("65G", par.NumeracaoNova);
            Assert.AreEqual("H", par.SiglaOriginal);
            Assert.AreEqual("G", par.SiglaNova);

            // A mudança de comprimento vem junto, no mesmo item.
            Assert.AreEqual(1, par.Divergencias.Count);
            Assert.IsInstanceOfType(par.Divergencias.Single(), typeof(DivergenciaComprimento));
        }

        [TestMethod]
        public void Comparar_TrocaDeMaterial_NaoRepeteALinhaMaterial()
        {
            ResultadoComparacaoV2 resultado = new ComparadorListasV2().Comparar(
                Estrutura(ConstrutoresTeste.Chapa("65H", 1000, 60, 4.75, "H")),
                Estrutura(ConstrutoresTeste.Chapa("65G", 1000, 60, 4.75, "G")));

            ParPosicoes par = resultado.Estruturas.Single().MaterialTrocado.Single();

            // O cabeçalho do item já diz "65H → 65G"; repetir "Material" abaixo seria redundante.
            Assert.IsFalse(par.Divergencias.Any(x => x is DivergenciaMaterial));
            Assert.AreEqual(0, par.Divergencias.Count);
        }

        [TestMethod]
        public void Comparar_ParExatoComMaterialDoPerfilDiferente_MantemALinhaMaterial()
        {
            // Numeração igual dos dois lados, mas o material do perfil mudou: aí a linha faz falta.
            ResultadoComparacaoV2 resultado = new ComparadorListasV2().Comparar(
                Estrutura(ConstrutoresTeste.Chapa("65", 1000, 60, 4.75, "S")),
                Estrutura(ConstrutoresTeste.Chapa("65", 1000, 60, 4.75, "G")));

            ParPosicoes par = resultado.Estruturas.Single().Alteradas.Single();

            Assert.IsTrue(par.Divergencias.Any(x => x is DivergenciaMaterial));
        }

        [TestMethod]
        public void Comparar_DiferencaMenorQueDoisMilimetros_NaoContaComoAlterada()
        {
            ResultadoComparacaoV2 resultado = new ComparadorListasV2().Comparar(
                Estrutura(ConstrutoresTeste.Chapa("51G", 2000.0, 60, 3.0, "G")),
                Estrutura(ConstrutoresTeste.Chapa("51G", 2001.2, 60, 3.0, "G")));

            EstruturaComparacaoV2 estrutura = resultado.Estruturas.Single();

            Assert.AreEqual(0, estrutura.Alteradas.Count);
            Assert.AreEqual(1, estrutura.SomenteDiferencasPequenas.Count);
            Assert.AreEqual(0, resultado.Resumo().PosicoesAlteradas);
            Assert.AreEqual(1, resultado.Resumo().SomenteDiferencasPequenas);
        }

        [TestMethod]
        public void Comparar_PosicoesIguais_EstruturaSemDiferencas()
        {
            ResultadoComparacaoV2 resultado = new ComparadorListasV2().Comparar(
                Estrutura(ConstrutoresTeste.Chapa("65H", 1000, 60, 4.75, "H")),
                Estrutura(ConstrutoresTeste.Chapa("65H", 1000, 60, 4.75, "H")));

            EstruturaComparacaoV2 estrutura = resultado.Estruturas.Single();

            Assert.IsTrue(estrutura.SemDiferencas);
            Assert.AreEqual(1, estrutura.PosicoesIguais);
            Assert.AreEqual(0, resultado.Resumo().PrecisamAtencao);
        }

        [TestMethod]
        public void Comparar_CasoAmbiguo_ViraAvisoENaoPareia()
        {
            ResultadoComparacaoV2 resultado = new ComparadorListasV2().Comparar(
                Estrutura(ConstrutoresTeste.Chapa("65H", 1000, 60, 4.75, "H"),
                          ConstrutoresTeste.Chapa("65F", 1000, 60, 4.75, "F")),
                Estrutura(ConstrutoresTeste.Chapa("65G", 1000, 60, 4.75, "G")));

            EstruturaComparacaoV2 estrutura = resultado.Estruturas.Single();

            Assert.AreEqual(0, estrutura.MaterialTrocado.Count);
            Assert.AreEqual(1, estrutura.Ambiguos.Count);
            Assert.AreEqual(2, estrutura.PosicoesEliminadas.Count);
            Assert.AreEqual(1, estrutura.PosicoesNovas.Count);
            Assert.AreEqual(1, resultado.Resumo().GruposAmbiguos);
        }

        [TestMethod]
        public void Comparar_Resumo_ContadoresFecham()
        {
            var original = Estrutura(
                ConstrutoresTeste.Chapa("10H", 1000, 60, 4.75, "H"),   // igual
                ConstrutoresTeste.Chapa("20H", 1000, 60, 4.75, "H"),   // alterada
                ConstrutoresTeste.Chapa("30H", 1000, 60, 4.75, "H"),   // material trocado
                ConstrutoresTeste.Chapa("40H", 1000, 60, 4.75, "H"));  // eliminada

            var nova = Estrutura(
                ConstrutoresTeste.Chapa("10H", 1000, 60, 4.75, "H"),
                ConstrutoresTeste.Chapa("20H", 1500, 60, 4.75, "H"),
                ConstrutoresTeste.Chapa("30G", 1000, 60, 4.75, "G"),
                ConstrutoresTeste.Chapa("50G", 1000, 60, 4.75, "G"));  // nova

            ResumoComparacao resumo = new ComparadorListasV2().Comparar(original, nova).Resumo();

            Assert.AreEqual(1, resumo.EstruturasComparadas);
            Assert.AreEqual(3, resumo.PosicoesConferidas);
            Assert.AreEqual(1, resumo.PosicoesIguais);
            Assert.AreEqual(1, resumo.PosicoesAlteradas);
            Assert.AreEqual(1, resumo.MaterialTrocado);
            Assert.AreEqual(1, resumo.PosicoesNovas);
            Assert.AreEqual(1, resumo.PosicoesEliminadas);
            Assert.AreEqual(4, resumo.PrecisamAtencao);

            Assert.AreEqual(
                resumo.PosicoesConferidas,
                resumo.PosicoesIguais + resumo.PosicoesAlteradas + resumo.SomenteDiferencasPequenas + resumo.MaterialTrocado,
                "os pares têm que se distribuir exatamente entre as quatro categorias");
        }

        [TestMethod]
        public void Comparar_VariasDivergencias_PerfilVemAntesDoComprimento()
        {
            // O motor de comparação produz o comprimento primeiro; a v2 reordena para leitura.
            ResultadoComparacaoV2 resultado = new ComparadorListasV2().Comparar(
                ConstrutoresTeste.Estrutura("01-CABECA", ConstrutoresTeste.Cantoneira("70H", 1000, 50, 4.75, "H")),
                ConstrutoresTeste.Estrutura("01-CABECA", ConstrutoresTeste.Cantoneira("70H", 1400, 65, 6.35, "H")));

            List<ErroPosicao> divergencias = resultado.Estruturas.Single().Alteradas.Single().Divergencias;

            int perfil = divergencias.FindIndex(x => x.TipoErroEnum == EnumErrosPosicao.PerfilCantoneira);
            int comprimento = divergencias.FindIndex(x => x.TipoErroEnum == EnumErrosPosicao.Comprimento);

            Assert.IsTrue(perfil >= 0, "o teste precisa produzir uma divergência de perfil");
            Assert.IsTrue(comprimento >= 0, "o teste precisa produzir uma divergência de comprimento");
            Assert.IsTrue(perfil < comprimento, "perfil tem que aparecer antes do comprimento");
        }

        [TestMethod]
        public void Comparar_EstruturaSoNumaDasListas_ApareceNoAviso()
        {
            var original = ConstrutoresTeste.Estrutura("01-CABECA", ConstrutoresTeste.Chapa("10H", 1000, 60, 4.75, "H"));
            var nova = ConstrutoresTeste.Estrutura("02-TRONCO", ConstrutoresTeste.Chapa("10H", 1000, 60, 4.75, "H"));

            ResultadoComparacaoV2 resultado = new ComparadorListasV2().Comparar(original, nova);

            Assert.AreEqual(0, resultado.Estruturas.Count);
            Assert.AreEqual(1, resultado.EstruturasSomenteOriginal.Count);
            Assert.AreEqual(1, resultado.EstruturasSomenteNova.Count);
        }
    }
}
