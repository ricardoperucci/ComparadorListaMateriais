# DadosTeste / Listas  (listas em .txt)

Coloque **2 arquivos `.txt`** de lista de materiais nesta pasta.

- Os arquivos são lidos em ordem alfabética. Nomeie por exemplo:
  - `01-original.txt`  (lista original)
  - `02-nova.txt`      (lista nova)
- Não precisa de Excel instalado para este caso (parse de txt é puro).

Teste que consome estes dados:
`Integracao/ComparadorListasArquivosIntegracaoTests.CompararListasTxt_GeraResultadoSemLancar`

Enquanto não houver 2 arquivos, o teste fica **inconclusivo** (não falha).
Depois de adicionar os arquivos, **rebuild** do projeto de testes para copiá-los para a saída.
