# DadosTeste / CAM / PastaA  (arquivos .CAM da fábrica A)

Coloque aqui os arquivos **`.CAM`** que representam a **Pasta A** da comparação.

- Use os mesmos nomes/posições correspondentes aos da `PastaB` para que as posições casem.
- O fabricante padrão usado no teste é **CAW** (ver o teste para ajustar, se necessário).

Teste que consome estes dados:
`Integracao/ComparadorCAMIntegracaoTests.CompararCAM_DuasPastas_GeraRelatorio`

Requer a **HefestoCAM.dll** disponível (chega ao bin via o projeto principal) e host de teste **x64**.
Enquanto não houver arquivos `.CAM` em PastaA **e** PastaB, o teste fica **inconclusivo**.
