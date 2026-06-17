using ComparadorListasDeMateriais.DiffCamTextual;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace ComparadorListasDeMateriais.Testes.Unitarios
{
    /// <summary>
    /// Testes do diff textual de pastas .CAM (engine portado do estudo CAMUSA). Usa pastas
    /// temporárias com arquivos .CAM sintéticos — não depende de HefestoCAM nem de dados externos.
    /// </summary>
    [TestClass]
    public class GeradorHtmlDiffCamTests
    {
        private string _raiz, _antes, _depois;

        [TestInitialize]
        public void Init()
        {
            _raiz = Path.Combine(Path.GetTempPath(), "DiffCam_" + Guid.NewGuid().ToString("N"));
            _antes = Path.Combine(_raiz, "antes");
            _depois = Path.Combine(_raiz, "depois");
            Directory.CreateDirectory(_antes);
            Directory.CreateDirectory(_depois);
        }

        [TestCleanup]
        public void Limpa()
        {
            try { Directory.Delete(_raiz, true); } catch { /* best effort */ }
        }

        [TestMethod]
        public void Gerar_ComAlteracaoEArquivosNaoComuns_ResumoCorretoEHtmlGerado()
        {
            File.WriteAllText(Path.Combine(_antes, "p1.CAM"), "NOM_PRO: CH. 6.35 x 100 x 200\r\nSPE_PRO: 6.35\r\nLUN_PRO: 200\r\n");
            File.WriteAllText(Path.Combine(_antes, "p2.CAM"), "NOM_PRO: L 75 x 75 x 8 x 1000\r\n");
            File.WriteAllText(Path.Combine(_depois, "p1.CAM"), "NOM_PRO: CH. 6.35 x 100 x 200\r\nSPE_PRO: 6.35\r\nLUN_PRO: 201\r\n");
            File.WriteAllText(Path.Combine(_depois, "p3.CAM"), "NOM_PRO: CH. 8.0 x 100 x 100\r\n");

            string html = Path.Combine(_raiz, "rel.html");
            ResultadoDiffCam r = GeradorHtmlDiffCam.Gerar(_antes, _depois, html);

            Assert.AreEqual(1, r.Comuns, "p1 é comum às duas pastas.");
            Assert.AreEqual(1, r.Alterados, "p1 teve a linha LUN_PRO alterada.");
            Assert.AreEqual(1, r.SoNoAntes, "p2 só existe no antes.");
            Assert.AreEqual(1, r.SoNoDepois, "p3 só existe no depois.");

            Assert.IsTrue(File.Exists(html));
            string conteudo = File.ReadAllText(html);
            StringAssert.Contains(conteudo, "Diff de pastas CAM");
            StringAssert.Contains(conteudo, "LUN_PRO");
            // Δ = |201-200| = 1.0 > 0.8 mm => deve marcar a alteração como grande.
            StringAssert.Contains(conteudo, "alteracao-grande");
        }

        [TestMethod]
        public void Gerar_PastasComMesmoConteudo_ZeroAlterados()
        {
            File.WriteAllText(Path.Combine(_antes, "p1.CAM"), "NOM_PRO: CH. 6.35 x 100 x 200\r\n");
            File.WriteAllText(Path.Combine(_depois, "p1.CAM"), "NOM_PRO: CH. 6.35 x 100 x 200\r\n");

            string html = Path.Combine(_raiz, "rel.html");
            ResultadoDiffCam r = GeradorHtmlDiffCam.Gerar(_antes, _depois, html);

            Assert.AreEqual(1, r.Comuns);
            Assert.AreEqual(0, r.Alterados);
            Assert.AreEqual(0, r.SoNoAntes);
            Assert.AreEqual(0, r.SoNoDepois);
        }
    }
}
