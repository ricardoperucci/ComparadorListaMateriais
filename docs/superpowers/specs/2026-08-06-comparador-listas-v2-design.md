# Comparador de Listas de Materiais v2 — design

Data: 2026-08-06
Branch: `refat2026-claude01`

## Problema

A aba "Comparar Listas" atual tem três problemas que o usuário quer resolver:

1. **O relatório .txt é difícil de ler.** Não tem resumo, os rótulos de "posições
   só na original / só na nova" estão invertidos, "Melhorias" é um nome enganoso
   (só contém diferenças de comprimento/largura ≤ 2 mm) e os textos das
   divergências têm formatos inconsistentes entre si.
2. **Troca de material vira posição nova + posição eliminada.** O pareamento usa
   `NumeracaoComMaterial`, então `65H` na lista original e `65G` na nova são
   tratadas como duas posições diferentes, quando na verdade é a mesma posição
   com o material trocado.
3. **O diálogo de seleção de pasta é diferente do de seleção de arquivo.**
   `FolderBrowserDialog` (árvore) vs. `OpenFileDialog` (Explorer).

Além disso, durante o brainstorming foram acrescentados dois requisitos:

4. Gerar também um relatório **HTML** (além do .txt).
5. No fim do relatório HTML, uma **tela lado a lado com as duas listas cruas**,
   marcando as diferenças, como um comparador de texto tradicional.

## Escopo

Uma **aba nova** no programa, com uma camada de comparação própria e isolada. A
aba "Comparar Listas" atual **não é alterada** e continua servindo de rede de
segurança até a nova ser validada em obra real.

Fora de escopo: saída Excel/PMV na aba nova (continua só na aba antiga);
agregação de quantidades entre estruturas; qualquer mudança no parse de txt/Excel.

## Decisões tomadas

| # | Decisão | Motivo |
|---|---|---|
| D1 | Camada `ComparadorV2` nova e isolada, reaproveitando o parse e o motor `CompararComObjeto` existentes | Risco zero de regressão na aba que hoje funciona |
| D2 | Siglas de material = **G, H, F**, e só quando `Length >= 2` | Regra canônica do Hefesto (`FuncoesHefesto.RemoveSiglaMaterialDaPosicao`) |
| D3 | Caso ambíguo não é pareado; vai para uma seção "Revisar manualmente" | O programa não deve adivinhar qual peça virou qual |
| D4 | Saídas: TXT novo + HTML. Sem Excel | Escolha do usuário |
| D5 | Diff lado a lado sobre as **listas cruas**, dentro do relatório HTML | Escolha do usuário |
| D6 | `SelecionaPasta()` passa a usar `OpenFolderDialog` **no app inteiro** | É um helper único usado por todas as abas; mudança puramente visual |
| D7 | TXT escrito em UTF-8 **com BOM** | Garante acentos e box-drawing corretos em qualquer editor |

## Arquitetura

Namespace novo: `ComparadorListasDeMateriais.ComparadorV2`.

| Arquivo | Responsabilidade | Pureza |
|---|---|---|
| `SiglaMaterialPosicao.cs` | `RemoveSigla(pos)` e `Sigla(pos)` | pura |
| `PareadorPosicoes.cs` | Pareia as posições de uma estrutura entre as duas listas | pura |
| `ComparadorListasV2.cs` | Orquestra: estruturas comuns → pareador → `CompararComObjeto` → resultado | I/O só na sobrecarga que recebe caminhos |
| `ResultadoComparacaoV2.cs` | Modelo de saída (+ `EstruturaComparacaoV2`, `ParPosicoes`, `GrupoAmbiguo`, `ResumoComparacao`) | dados |
| `FormatadorDivergenciaV2.cs` | `ErroPosicao` → `(rótulo, valor original, valor novo, delta)` normalizado | pura |
| `RelatorioV2Txt.cs` | `ResultadoComparacaoV2` → string (txt) | pura |
| `RelatorioV2Html.cs` | `ResultadoComparacaoV2` + textos das listas → string (html) | pura |
| `DiffLinhas.cs` | Diff de Myers O(ND) sobre listas de linhas → sequência de blocos | pura |
| `RenderizadorListaParaTexto.cs` | Lista em Excel → texto tabulado, para o diff funcionar com .xlsx | I/O de Excel |

Reaproveitado sem alteração: `FuncoesUteis.CriaObjetosComparacaoListaByTxt` /
`ByExcel`, `FuncoesUteis.LerTxtLisaMateriais`, `FuncoesUteis.ComparaNomeEstruturas`,
`ObjetoComparacaoLista.CompararComObjeto` e as 17 classes de divergência.

### Fluxo

```
caminho1, caminho2
   → CriaObjetosComparacaoLista (existente)      → Dictionary<estrutura, List<ObjetoComparacaoLista>>
   → EstruturasComuns (existente)
   → para cada estrutura comum: PareadorPosicoes → pares + sobras + ambíguos
   → para cada par: CompararComObjeto (existente) → List<ErroPosicao>
   → ResultadoComparacaoV2
   → RelatorioV2Txt.Gerar(resultado)             → .txt
   → RelatorioV2Html.Gerar(resultado, textos)    → .html  (abre no navegador)
```

## Regra de pareamento (requisito 2)

Porta literal do Hefesto (`HefestoCore/Uteis/FuncoesHefesto.cs:3886`):

```csharp
public static string RemoveSigla(string pPosicao)
{
    if (pPosicao.Length < 2) return pPosicao;
    char ultima = char.ToUpperInvariant(pPosicao[pPosicao.Length - 1]);
    if (ultima == 'G' || ultima == 'H' || ultima == 'F')
        return pPosicao.Substring(0, pPosicao.Length - 1);
    return pPosicao;
}
```

Só G, H e F são siglas de material. Qualquer outra letra final continua fazendo
parte da numeração e **não** é removida.

Algoritmo, aplicado por estrutura:

1. **Pareamento exato** por `NumeracaoComMaterial` normalizada (minúsculas, sem
   espaços) — igual ao comportamento atual.
2. As sobras dos dois lados são agrupadas por `ChaveBase = RemoveSigla(numeração)`
   (normalizada da mesma forma):
   - chave com **exatamente 1 sobra de cada lado** → par de **material trocado**;
   - chave com **2 ou mais** de algum lado → **grupo ambíguo**, não pareado.
3. O que restar vira **posição eliminada** (só na original) ou **posição nova**
   (só na nova).

Todo par — exato ou por troca de material — passa por `CompararComObjeto`, então
uma posição que trocou material *e* mudou de comprimento aparece com as duas
informações no mesmo item.

Os itens de um grupo ambíguo aparecem **duas vezes** no relatório, de propósito:
em novas/eliminadas (porque é isso que factualmente são) e em "Revisar
manualmente" (como pista de que pode ter havido troca de material ali).

### Renderização do material

- Par por **troca de material**: o cabeçalho do item mostra `65H → 65G` e a linha
  genérica "Material" vinda de `CompararComObjeto` é **suprimida** naquele item,
  para não dizer a mesma coisa duas vezes.
- Par **exato** em que o material do perfil mudou sem mexer na numeração: a linha
  "Material" aparece normalmente.

### Correção de bug herdado

`ObjetoComparacaoLista.NumeracaoSemMaterial` trata só `h` e `g` — falta o `F`
(`ObjetosLista/ObjetoComparacaoLista.cs:107` e `:144`). O v2 não usa essa
propriedade; usa `SiglaMaterialPosicao.RemoveSigla`, que já nasce com os três.
A propriedade antiga fica como está (a aba antiga não é tocada).

## Relatório TXT (requisito 1)

80 colunas, UTF-8 com BOM, **seções vazias são omitidas**.

```
════════════════════════════════════════════════════════════════════════════════
  COMPARAÇÃO DE LISTAS DE MATERIAIS
════════════════════════════════════════════════════════════════════════════════

  Lista original   LM-TORRE-XPTO-R0.txt
  Lista nova       LM-TORRE-XPTO-R1.txt
  Gerado em        06/08/2026 14:32

────────────────────────────────────────────────────────────────────────────────
  RESUMO
────────────────────────────────────────────────────────────────────────────────

  Estruturas comparadas .......  12      Posições novas ..............   7
  Posições conferidas .........  446     Posições eliminadas .........   3
  Posições iguais .............  418     Estruturas só na original ...   1
  Posições alteradas ..........  23      Estruturas só na nova .......   0
  Material trocado ............   5      Revisar manualmente .........   1

  >> 39 posições precisam de atenção nesta comparação.


════════════════════════════════════════════════════════════════════════════════
  01-CABEÇA
════════════════════════════════════════════════════════════════════════════════

  ■ MATERIAL TROCADO  (2)

      65H   →  65G
      107H  →  107F
            • Comprimento ......  1250,0   →  1268,0    (+18,0 mm)

  ■ POSIÇÕES ALTERADAS  (2)

      42G
            • Comprimento ......  980,0    →  995,0     (+15,0 mm)
            • Quantidade .......  4        →  6
      88H
            • Espessura ........  4,75     →  6,35

  ■ DIFERENÇAS PEQUENAS (≤ 2 mm)  (1)

      51G
            • Comprimento ......  2000,0   →  2001,2    (+1,2 mm)

  ■ POSIÇÕES NOVAS  (1)      201G

  ■ REVISAR MANUALMENTE  (1)

      Numeração 65 — original tem 65H, 65F | nova tem 65G
      Mais de um candidato para a mesma numeração; confira se houve troca de material.


════════════════════════════════════════════════════════════════════════════════
  02-TRONCO
════════════════════════════════════════════════════════════════════════════════

  Nenhuma diferença. As 87 posições estão iguais nas duas listas.


════════════════════════════════════════════════════════════════════════════════
  AVISOS GERAIS
════════════════════════════════════════════════════════════════════════════════

  ■ ESTRUTURAS SÓ NA LISTA ORIGINAL  (1)      04-MÍSULA 2

  ■ POSIÇÕES DIVERGENTES ENTRE ESTRUTURAS DA MESMA LISTA  (1)
      65H  na Lista Original: 01-CABEÇA, 02-TRONCO
```

### Definição dos contadores

Sem isso os números do resumo ficam ambíguos.

| Contador | Definição |
|---|---|
| Posições conferidas | Total de pares formados (exatos + material trocado). No exemplo: 418 + 23 + 5 = 446 |
| Posições iguais | Pares sem nenhuma divergência |
| Posições alteradas | Pares **exatos** com pelo menos uma divergência de `NcOuMelhoria = true` |
| Material trocado | Pares formados por chave base (`65H → 65G`), tendo ou não outras divergências |
| Posições novas / eliminadas | Sobras não pareadas, incluindo as que estão em grupos ambíguos |
| Revisar manualmente | Quantidade de **grupos** ambíguos (não de posições dentro deles) |
| Precisam de atenção | Alteradas + material trocado + novas + eliminadas + revisar manualmente |

Uma posição pode aparecer em duas seções: se ela tem uma divergência NC e também
uma diferença ≤ 2 mm, sai em "Posições alteradas" com as linhas NC e em
"Diferenças pequenas" com as linhas ≤ 2 mm — mesmo comportamento do relatório
atual, que separa divergências de melhorias sobre a mesma lista de erros. Ela
conta **uma vez só** em "Posições alteradas".

Uma posição cujas únicas divergências são ≤ 2 mm **não** conta como alterada:
sai apenas em "Diferenças pequenas".

Mudanças de fundo em relação ao relatório atual:

- Resumo com contadores no topo.
- Rótulos corretos. Hoje `PosicoesSomenteListaOriginal` (posições que só existem
  na original, ou seja, **eliminadas**) é impresso como "Posições não encontradas
  na lista original" — está invertido (`Relatorios/RelatorioListasTxt.cs:30` e `:36`).
- "Melhorias" → "Diferenças pequenas (≤ 2 mm)", que é o que a seção realmente
  contém: só `DivergenciaComprimento` e `DivergenciaLargura` têm
  `NcOuMelhoria = false`, e só quando o delta é ≤ 2 mm.
- Valores sempre no formato `original → novo (delta)`, com alinhamento único,
  via `FormatadorDivergenciaV2` — hoje cada classe escreve do seu jeito
  (`"Comprimento: 18mm => 1250 / 1268"` vs. `"Espessura: 4,75/6,35"`).
  O formatador é um `switch` sobre os tipos concretos de `ErroPosicao`, dentro da
  camada v2; as 17 classes compartilhadas não são alteradas.
- Estrutura sem diferença ocupa uma linha em vez de um bloco.

## Relatório HTML (requisito 4)

Arquivo único, sem CSS/JS externo, abre offline. Mesmas seções do TXT em formato
web (cartões de resumo, seção por estrutura, badges por tipo de item) e, no fim,
o diff lado a lado.

Ao terminar a comparação a aba abre o HTML no navegador com
`Process.Start(new ProcessStartInfo(caminho) { UseShellExecute = true })`,
igual à aba "Diff CAM (HTML)" faz hoje (`Form1.cs:250`).

## Diff lado a lado (requisito 5)

- **Entrada:** o texto cru dos dois arquivos de lista.
  - `.txt` → lido com `FuncoesUteis.LerTxtLisaMateriais` (Windows-1252, como já é).
  - `.xls`/`.xlsx` → não existe texto cru; `RenderizadorListaParaTexto` gera uma
    renderização tabulada das linhas da aba MATERIAIS, e a seção exibe um aviso
    dizendo que é uma renderização, não o arquivo original.
- **Algoritmo:** Myers O(ND) sobre as linhas, produzindo blocos
  `Igual` / `Alterado` / `SoNaOriginal` / `SoNaNova`. Comparação por índice
  (como o `GeradorHtmlDiffCam` faz hoje) não serve: ela desalinha tudo a partir da
  primeira linha inserida.
  Myers é a escolha certa aqui porque o custo cresce com o **número de
  diferenças**, e duas revisões da mesma lista diferem em poucas linhas.
- **Apresentação:** duas colunas com número de linha, fonte monoespaçada, linhas
  removidas em vermelho à esquerda e inseridas em verde à direita, linha alterada
  lado a lado nas duas cores.
- **Colapso:** sequências longas de linhas iguais aparecem como
  `… 214 linhas iguais …` com 3 linhas de contexto de cada lado, e um botão
  "mostrar tudo" em JavaScript inline (sem biblioteca).

## Diálogo de pasta (requisito 3)

`Form1.SelecionaPasta()` (`Form1.cs:662`) troca `FolderBrowserDialog` por
`System.Windows.Forms.OpenFolderDialog`, disponível a partir do .NET 8 — o projeto
já é `net8.0-windows`. É o mesmo diálogo estilo Explorer do `OpenFileDialog`,
com barra de locais, caminho digitável e busca.

Como o helper é único e usado por todas as abas, a correção vale para o app
inteiro. É mudança puramente visual, sem efeito em lógica de comparação.

## Interface da aba nova

`MontarAbaComparadorV2()`, montada em código como as abas "Diff CAM (HTML)" e
"Renomear Arquivos", com o tema Hefesto aplicado. Nome da aba: **"Comparar Listas v2"**.

Controles: selecionar lista original · selecionar lista nova · selecionar pasta de
saída · nome do arquivo · checkbox TXT · checkbox HTML · botão comparar · label de
status.

Validações, no mesmo padrão das abas existentes (`MessageBox` e retorno):
duas listas selecionadas; pasta de saída selecionada se alguma saída estiver
marcada; pelo menos uma saída marcada; erro de comparação capturado e mostrado
com a mensagem da exceção.

## Testes

No projeto `ComparadorListasDeMateriais.Testes` que já existe (MSTest).

| Alvo | Casos |
|---|---|
| `SiglaMaterialPosicao` | `65H`→`65`; `105G`→`105`; `70F`→`70`; `65A`→`65A` (A não é sigla); `65`→`65`; `H`→`H` (Length < 2); minúsculas |
| `PareadorPosicoes` | par exato; `65H`↔`65G` vira par de material trocado; `65H`+`65F` vs `65G` vira grupo ambíguo e não pareia; sobras viram novas/eliminadas; posição sem letra pareia com `65H` |
| `ComparadorListasV2` | posição que trocou material *e* comprimento aparece como um único par com as duas divergências; contadores do resumo |
| `FormatadorDivergenciaV2` | cada tipo de divergência produz rótulo e valores esperados |
| `RelatorioV2Txt` | rótulos de eliminadas/novas corretos; seção vazia omitida; estrutura sem diferença em uma linha; resumo com os números certos |
| `DiffLinhas` | arquivos iguais; inserção no meio; remoção no meio; linha alterada; arquivo vazio de um lado |

Os testes de relatório seguem o padrão de `RelatorioListasTxtTests`: montam o
resultado em memória e conferem o texto, sem tocar em arquivo nem em UI.

## Riscos

- **Falso pareamento.** Se a numeração de uma posição legitimamente terminar em
  G, H ou F sem que a letra seja material, duas posições distintas podem ser
  pareadas. Mitigado por usar exatamente a mesma regra que o Hefesto já usa em
  produção, e por só parear quando há um único candidato de cada lado.
- **Encoding do TXT.** O relatório usa `═ ─ ■ → ≤`, que não existem em
  Windows-1252. Por isso o TXT é escrito em UTF-8 com BOM, e não com o
  `StreamWriter` sem encoding explícito que a aba antiga usa (`FuncoesUteis.cs:646`).
- **Tamanho do HTML.** Lista grande gera diff grande. Mitigado pelo colapso de
  trechos iguais.
