using ComparadorListasDeMateriais.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.IO.Compression;

namespace ComparadorListasDeMateriais.Testes.Unitarios
{
    /// <summary>
    /// Testes do serviço de compactação. Usa uma pasta temporária — não precisa de UI nem dados externos.
    /// </summary>
    [TestClass]
    public class ZipadorArquivosTests
    {
        private string _dir;

        [TestInitialize]
        public void Init()
        {
            _dir = Path.Combine(Path.GetTempPath(), "ZipTeste_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_dir);
        }

        [TestCleanup]
        public void Limpa()
        {
            try { Directory.Delete(_dir, true); } catch { /* best effort */ }
        }

        [TestMethod]
        public void CaminhoZipPara_TrocaApenasUltimaExtensao()
        {
            string entrada = Path.Combine("X", "a.b.cam");
            string esperado = Path.Combine("X", "a.b.zip");
            Assert.AreEqual(esperado, ZipadorArquivos.CaminhoZipPara(entrada));
        }

        [TestMethod]
        public void CriarZipPorArquivo_GeraUmZipParaCadaArquivo()
        {
            string a = CriaArquivo("a.txt");
            string b = CriaArquivo("b.txt");

            ResultadoZip resultado = ZipadorArquivos.CriarZipPorArquivo(new[] { a, b });

            Assert.AreEqual(2, resultado.Sucesso);
            Assert.AreEqual(0, resultado.Erros);
            Assert.IsTrue(File.Exists(Path.ChangeExtension(a, ".zip")));
            Assert.IsTrue(File.Exists(Path.ChangeExtension(b, ".zip")));
        }

        [TestMethod]
        public void CriarZipUnico_AgrupaTodosOsArquivosNoMesmoZip()
        {
            string a = CriaArquivo("a.txt");
            string b = CriaArquivo("b.txt");

            ResultadoZip resultado = ZipadorArquivos.CriarZipUnico(new[] { a, b });

            Assert.AreEqual(2, resultado.Sucesso);
            Assert.AreEqual(1, resultado.ArquivosGerados.Count);

            using (ZipArchive zip = ZipFile.OpenRead(resultado.ArquivosGerados[0]))
            {
                Assert.AreEqual(2, zip.Entries.Count);
            }
        }

        [TestMethod]
        public void CriarZipPorArquivo_ListaVazia_NaoFazNada()
        {
            ResultadoZip resultado = ZipadorArquivos.CriarZipPorArquivo(new string[0]);
            Assert.AreEqual(0, resultado.Sucesso);
            Assert.AreEqual(0, resultado.Erros);
        }

        private string CriaArquivo(string nome)
        {
            string caminho = Path.Combine(_dir, nome);
            File.WriteAllText(caminho, "conteudo de teste");
            return caminho;
        }
    }
}
