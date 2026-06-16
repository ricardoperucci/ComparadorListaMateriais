# DadosTeste / Excel  (listas em .xls/.xlsx)

Coloque **2 arquivos `.xls`/`.xlsx`** de lista de materiais nesta pasta (lidos em ordem alfabética).

- A planilha precisa ter as abas esperadas pelo leitor (ex.: `MATERIAIS`).
- **Requer Microsoft Excel instalado** (o leitor usa Office Interop e abre o Excel).
- O teste tem as categorias `Integracao` e `RequerExcel` — você pode filtrá-lo no Test Explorer.

Teste que consome estes dados:
`Integracao/ComparadorListasArquivosIntegracaoTests.CompararListasExcel_GeraResultadoSemLancar`

Enquanto não houver 2 arquivos, o teste fica **inconclusivo** (não falha).
