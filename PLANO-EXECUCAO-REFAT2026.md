# Plano de Execução — refat2026 (branch `refat2026-claude01`)

Este documento descreve **o que foi feito**, **como buildar/testar** e o **processo TDD** para concluir
amanhã apenas colocando as instâncias de exemplo nas pastas indicadas.

> Resumo: o programa foi refatorado (abas separadas, lógica testável, uso limpo da HefestoCAM) e
> **migrado de .NET Framework 4.8 para .NET 8** — necessário porque a HefestoCAM 2026 é `.NET 8`.
> A solução **compila** e **os 37 testes unitários passam**. Faltam só as instâncias de exemplo
> para destravar os testes de integração.

---

## 1. Decisão-chave: migração para .NET 8

A `HefestoCAM.dll` e a `ModelagemTorre.ObjetosCore.dll` em
`%USERPROFILE%\Engetower\ModelagemTorre3D\` são **`.NETCoreApp,Version=v8.0`** (verificado no binário
e no `Hefesto.App.runtimeconfig.json` → `"tfm": "net8.0"`).

Um processo **.NET Framework 4.8 compila** contra essas DLLs (lê metadados) mas **não consegue
carregá-las em runtime** → a comparação de CAM lançava
`System.IO.FileNotFoundException: System.Runtime, Version=8.0.0.0`.

Por isso `ComparadorListasDeMateriais` e o projeto de testes foram migrados para **`net8.0-windows`**
(SDK-style, WinForms). Foi sua decisão (opção "Migrar p/ .NET 8 — recomendado").

**Pré-requisito de execução:** as máquinas precisam do **.NET 8 Desktop Runtime** instalado.

---

## 2. Como buildar

> Use o **MSBuild do Visual Studio** (não `dotnet build`): o projeto tem `COMReference` do Office,
> e o `dotnet` falha com `MSB4803` nesse cenário.

Caminho do MSBuild nesta máquina (VS 2026/18 Community):

```
"C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe"
```

Buildar a solução inteira:

```bash
MSBuild.exe ComparadorListasDeMateriais.sln -restore -t:Build -p:Configuration=Debug
```

Ou simplesmente **abrir a `.sln` no Visual Studio e dar Build** (recomendado).

Saídas:
- App:  `ComparadorListasDeMateriais\bin\Debug\net8.0-windows\ComparadorListasDeMateriais.exe`
- Testes: `ComparadorListasDeMateriais.Testes\bin\Debug\net8.0-windows\ComparadorListasDeMateriais.Testes.dll`

---

## 3. Como rodar os testes

### No Visual Studio (recomendado)
`Test > Test Explorer > Run All`. Os testes de integração aparecem como **Inconclusivos** até você
colocar os dados de exemplo (passo 4).

### Pela linha de comando
```
"C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" ^
  "ComparadorListasDeMateriais.Testes\bin\Debug\net8.0-windows\ComparadorListasDeMateriais.Testes.dll" /Platform:x64
```

Filtros úteis:
- Só unitários (sem integração): `/TestCaseFilter:"TestCategory!=Integracao&TestCategory!=RequerExcel"`
- Status atual: **37/37 unitários aprovados**.

---

## 4. Onde colocar as instâncias de exemplo (o que falta para amanhã)

Coloque os arquivos e **rebuild** (eles são copiados para a saída):

| Pasta (`ComparadorListasDeMateriais.Testes\DadosTeste\...`) | O que colocar | Destrava o teste |
|---|---|---|
| `Listas\` | 2 listas `.txt` (`01-original.txt`, `02-nova.txt`) | `ComparadorListasArquivosIntegracaoTests.CompararListasTxt_*` |
| `CAM\PastaA\` e `CAM\PastaB\` | arquivos `.CAM` correspondentes nas duas pastas | `ComparadorCAMIntegracaoTests.CompararCAM_*` |
| `Excel\` | 2 listas `.xls`/`.xlsx` (requer Excel instalado) | `ComparadorListasArquivosIntegracaoTests.CompararListasExcel_*` |

Cada pasta tem um `LEIAME.md` com detalhes. Enquanto vazias, os testes ficam **Inconclusivos** (não falham).

---

## 5. Ciclo TDD sugerido

1. Build da solução (passo 2) — deve ficar verde.
2. Run All — 37 unitários verdes; integração inconclusiva.
3. Coloque as instâncias de exemplo (passo 4) e rebuild.
4. Run All de novo — os testes de integração passam a executar de verdade.
5. Para cada nova regra/ajuste: **escreva o teste primeiro** (vermelho) → implemente → verde → refatore.
   - Regras puras de CAM: `RegrasComparacaoCAM` (cobertas por `RegrasComparacaoCAMTests`).
   - Relatórios: `RelatorioCAMTxt` / `RelatorioListasTxt`.
   - Comparação de listas: sobrecarga `Comparador.CompararListas(dic, dic, cabecalho, out)`.

---

## 6. Pontos de integração a conferir no 1º build/run

1. **ParafusoController** — `AtualizaParafusoController(...)` é `internal` em `ModelagemTorre.ObjetosCore`.
   Para o build não depender de `InternalsVisibleTo`, a chamada é feita por **reflexão** em
   `FuncoesComparadorCAM.ConfigurarParafusoController`. Se o método virar `public`, troque por chamada direta.
2. **Excel (Office Interop)** — via `COMReference` (`WrapperTool=primary`). Requer Excel + PIAs registrados.
3. **AutoCAD** — a leitura de `.CAM` é parsing de texto; se algum caminho da HefestoCAM exigir AutoCAD,
   valide com os arquivos reais de exemplo.

---

## 7. Arquivos novos/alterados (resumo)

**Camada CAM testável** (`ComparadorCAM\`): `OpcoesComparacaoCAM`, `RegrasComparacaoCAM`,
`ResultadoComparacaoCAM`, `ILeitorPastaCAM`, `LeitorPastaCAMHefesto`, `ComparadorCAMService`,
`RelatorioCAMTxt`, e `FuncoesComparadorCAM` (fachada, sem MessageBox, API 2026 corrigida).

**Listas/Relatórios**: sobrecarga testável em `Comparador.cs`; `Relatorios\RelatorioListasTxt.cs`
(extraído de `FuncoesUteis.EscreveTxt`); construtores sem parâmetro em `Chapa/CantoneiraComparacaoLista`.

**ZIP**: `Utils\ZipadorArquivos.cs` (extraído do `Form1`).

**UI**: `Form1` reorganizado em **3 abas** (Comparar Listas / Comparar Arquivos CAM / ZIP Arquivos);
removido o hack "TEMPORÁRIO" que disparava CAM pela extensão; aba CAM agora tem inputs de fabricante,
tipo de parafuso e "considerar diâmetro".

**Migração .NET 8**: `.csproj` SDK-style net8.0-windows; correções em `Excel.cs`
(`Marshal.GetActiveObject` via P/Invoke e alias para `Range`).

**Testes**: projeto `ComparadorListasDeMateriais.Testes` (MSTest) + pastas `DadosTeste`.

Detalhes visuais e estrutura completa em **`REFATORACAO-REFAT2026.html`**.
