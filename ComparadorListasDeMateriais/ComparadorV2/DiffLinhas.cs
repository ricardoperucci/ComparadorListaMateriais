using System;
using System.Collections.Generic;

namespace ComparadorListasDeMateriais.ComparadorV2
{
    /// <summary>O que aconteceu com uma linha na comparação lado a lado.</summary>
    public enum TipoLinhaDiff
    {
        Igual,
        Alterada,
        Removida,
        Inserida
    }

    /// <summary>
    /// Uma linha do lado a lado. <see cref="NumeroOriginal"/> e <see cref="NumeroNova"/> valem 0
    /// quando aquele lado não tem linha (inserção ou remoção).
    /// </summary>
    public class LinhaDiff
    {
        public TipoLinhaDiff Tipo { get; set; }

        public int NumeroOriginal { get; set; }
        public string TextoOriginal { get; set; } = string.Empty;

        public int NumeroNova { get; set; }
        public string TextoNova { get; set; } = string.Empty;
    }

    /// <summary>Resultado da comparação de dois textos linha a linha.</summary>
    public class ResultadoDiffLinhas
    {
        public List<LinhaDiff> Linhas { get; set; } = new List<LinhaDiff>();

        public int Iguais { get; set; }
        public int Alteradas { get; set; }
        public int Removidas { get; set; }
        public int Inseridas { get; set; }

        /// <summary>
        /// True quando as duas listas eram diferentes demais e o algoritmo caiu no alinhamento
        /// simples por índice. O relatório avisa quando isso acontece.
        /// </summary>
        public bool AlinhamentoDegradado { get; set; }

        public bool TemDiferenca => Alteradas > 0 || Removidas > 0 || Inseridas > 0;
    }

    /// <summary>
    /// Compara dois textos linha a linha com o algoritmo de Myers (O(ND)), produzindo linhas já
    /// alinhadas para exibição lado a lado.
    /// <para>
    /// Comparar por índice (linha 1 com linha 1, linha 2 com linha 2) desalinha tudo a partir da
    /// primeira linha inserida — por isso o Myers. Ele é rápido justamente no caso de uso aqui:
    /// duas revisões da mesma lista, que diferem em poucas linhas.
    /// </para>
    /// </summary>
    public static class DiffLinhas
    {
        /// <summary>Teto de diferenças antes de cair no alinhamento simples, para não estourar memória.</summary>
        private const int MaximoDiferencas = 3000;

        private enum TipoOperacao { Igual, Remove, Insere }

        private struct Operacao
        {
            public TipoOperacao Tipo;
            public int IndiceOriginal;
            public int IndiceNova;
        }

        public static ResultadoDiffLinhas Comparar(IList<string> pOriginal, IList<string> pNova)
        {
            IList<string> original = pOriginal ?? new List<string>();
            IList<string> nova = pNova ?? new List<string>();

            List<Operacao> operacoes = Operacoes(original, nova, out bool degradado);

            ResultadoDiffLinhas saida = MontarLinhas(operacoes, original, nova);
            saida.AlinhamentoDegradado = degradado;

            return saida;
        }

        // =====================================================================
        //  Myers
        // =====================================================================

        private static List<Operacao> Operacoes(IList<string> a, IList<string> b, out bool degradado)
        {
            degradado = false;

            var operacoes = new List<Operacao>();

            // Duas revisões da mesma lista compartilham prefixo e sufixo enormes. Aparar os dois
            // deixa para o Myers só o miolo que realmente mudou.
            int inicio = 0;
            while (inicio < a.Count && inicio < b.Count && a[inicio] == b[inicio])
                inicio++;

            int fimA = a.Count - 1;
            int fimB = b.Count - 1;
            while (fimA >= inicio && fimB >= inicio && a[fimA] == b[fimB])
            {
                fimA--;
                fimB--;
            }

            for (int i = 0; i < inicio; i++)
                operacoes.Add(new Operacao { Tipo = TipoOperacao.Igual, IndiceOriginal = i, IndiceNova = i });

            int nMiolo = fimA - inicio + 1;
            int mMiolo = fimB - inicio + 1;

            if (nMiolo > 0 || mMiolo > 0)
            {
                var mioloA = new List<string>();
                for (int i = inicio; i <= fimA; i++) mioloA.Add(a[i]);

                var mioloB = new List<string>();
                for (int i = inicio; i <= fimB; i++) mioloB.Add(b[i]);

                List<Operacao> doMiolo = MyersOuDegradado(mioloA, mioloB, out degradado);

                foreach (Operacao op in doMiolo)
                {
                    operacoes.Add(new Operacao
                    {
                        Tipo = op.Tipo,
                        IndiceOriginal = op.IndiceOriginal < 0 ? -1 : op.IndiceOriginal + inicio,
                        IndiceNova = op.IndiceNova < 0 ? -1 : op.IndiceNova + inicio
                    });
                }
            }

            for (int i = fimA + 1; i < a.Count; i++)
            {
                int deslocamento = i - (fimA + 1);
                operacoes.Add(new Operacao
                {
                    Tipo = TipoOperacao.Igual,
                    IndiceOriginal = i,
                    IndiceNova = fimB + 1 + deslocamento
                });
            }

            return operacoes;
        }

        private static List<Operacao> MyersOuDegradado(List<string> a, List<string> b, out bool degradado)
        {
            degradado = false;

            List<Operacao> caminho = Myers(a, b);

            if (caminho != null)
                return caminho;

            degradado = true;

            return AlinhamentoPorIndice(a, b);
        }

        /// <summary>
        /// Myers greedy com trace. Devolve null quando o número de diferenças passa do teto —
        /// aí o chamador cai no alinhamento simples em vez de consumir memória sem limite.
        /// </summary>
        private static List<Operacao> Myers(List<string> a, List<string> b)
        {
            int n = a.Count;
            int m = b.Count;
            int max = n + m;

            if (max == 0)
                return new List<Operacao>();

            int teto = Math.Min(max, MaximoDiferencas);

            var v = new int[2 * max + 1];
            var trace = new List<int[]>();

            for (int d = 0; d <= teto; d++)
            {
                // Só a faixa [-d, d] importa em cada passo; guardar a fatia mantém a memória em O(D²).
                var fatia = new int[2 * d + 1];
                for (int k = -d; k <= d; k += 1)
                    fatia[k + d] = v[k + max];
                trace.Add(fatia);

                for (int k = -d; k <= d; k += 2)
                {
                    int x;
                    if (k == -d || (k != d && v[k - 1 + max] < v[k + 1 + max]))
                        x = v[k + 1 + max];
                    else
                        x = v[k - 1 + max] + 1;

                    int y = x - k;

                    while (x < n && y < m && a[x] == b[y])
                    {
                        x++;
                        y++;
                    }

                    v[k + max] = x;

                    if (x >= n && y >= m)
                        return Backtrack(trace, a, b, d);
                }
            }

            return null;
        }

        private static List<Operacao> Backtrack(List<int[]> pTrace, List<string> a, List<string> b, int pD)
        {
            var operacoes = new List<Operacao>();

            int x = a.Count;
            int y = b.Count;

            for (int d = pD; d > 0; d--)
            {
                int[] v = pTrace[d];
                int k = x - y;

                int kAnterior;
                if (k == -d || (k != d && Valor(v, d, k - 1) < Valor(v, d, k + 1)))
                    kAnterior = k + 1;
                else
                    kAnterior = k - 1;

                int xAnterior = Valor(v, d, kAnterior);
                int yAnterior = xAnterior - kAnterior;

                while (x > xAnterior && y > yAnterior)
                {
                    x--;
                    y--;
                    operacoes.Add(new Operacao { Tipo = TipoOperacao.Igual, IndiceOriginal = x, IndiceNova = y });
                }

                if (x == xAnterior)
                    operacoes.Add(new Operacao { Tipo = TipoOperacao.Insere, IndiceOriginal = -1, IndiceNova = yAnterior });
                else
                    operacoes.Add(new Operacao { Tipo = TipoOperacao.Remove, IndiceOriginal = xAnterior, IndiceNova = -1 });

                x = xAnterior;
                y = yAnterior;
            }

            while (x > 0 && y > 0)
            {
                x--;
                y--;
                operacoes.Add(new Operacao { Tipo = TipoOperacao.Igual, IndiceOriginal = x, IndiceNova = y });
            }

            operacoes.Reverse();

            return operacoes;
        }

        private static int Valor(int[] pFatia, int pD, int pK)
        {
            int indice = pK + pD;

            return indice >= 0 && indice < pFatia.Length ? pFatia[indice] : 0;
        }

        /// <summary>Plano B: alinha linha a linha por índice. Só é usado se o Myers estourar o teto.</summary>
        private static List<Operacao> AlinhamentoPorIndice(List<string> a, List<string> b)
        {
            var operacoes = new List<Operacao>();

            int comum = Math.Min(a.Count, b.Count);

            for (int i = 0; i < comum; i++)
            {
                if (a[i] == b[i])
                {
                    operacoes.Add(new Operacao { Tipo = TipoOperacao.Igual, IndiceOriginal = i, IndiceNova = i });
                    continue;
                }

                operacoes.Add(new Operacao { Tipo = TipoOperacao.Remove, IndiceOriginal = i, IndiceNova = -1 });
                operacoes.Add(new Operacao { Tipo = TipoOperacao.Insere, IndiceOriginal = -1, IndiceNova = i });
            }

            for (int i = comum; i < a.Count; i++)
                operacoes.Add(new Operacao { Tipo = TipoOperacao.Remove, IndiceOriginal = i, IndiceNova = -1 });

            for (int i = comum; i < b.Count; i++)
                operacoes.Add(new Operacao { Tipo = TipoOperacao.Insere, IndiceOriginal = -1, IndiceNova = i });

            return operacoes;
        }

        // =====================================================================
        //  Montagem das linhas lado a lado
        // =====================================================================

        /// <summary>
        /// Converte a sequência de operações em linhas alinhadas. Remoções e inserções seguidas
        /// são casadas duas a duas, que é o que faz uma linha modificada aparecer lado a lado em
        /// vez de virar uma linha vermelha em cima e uma verde embaixo.
        /// </summary>
        private static ResultadoDiffLinhas MontarLinhas(List<Operacao> pOperacoes, IList<string> a, IList<string> b)
        {
            var saida = new ResultadoDiffLinhas();

            int i = 0;
            while (i < pOperacoes.Count)
            {
                if (pOperacoes[i].Tipo == TipoOperacao.Igual)
                {
                    Operacao op = pOperacoes[i];

                    saida.Linhas.Add(new LinhaDiff
                    {
                        Tipo = TipoLinhaDiff.Igual,
                        NumeroOriginal = op.IndiceOriginal + 1,
                        TextoOriginal = a[op.IndiceOriginal],
                        NumeroNova = op.IndiceNova + 1,
                        TextoNova = b[op.IndiceNova]
                    });

                    saida.Iguais++;
                    i++;
                    continue;
                }

                var removidas = new List<int>();
                var inseridas = new List<int>();

                while (i < pOperacoes.Count && pOperacoes[i].Tipo == TipoOperacao.Remove)
                {
                    removidas.Add(pOperacoes[i].IndiceOriginal);
                    i++;
                }

                while (i < pOperacoes.Count && pOperacoes[i].Tipo == TipoOperacao.Insere)
                {
                    inseridas.Add(pOperacoes[i].IndiceNova);
                    i++;
                }

                int pares = Math.Min(removidas.Count, inseridas.Count);

                for (int p = 0; p < pares; p++)
                {
                    saida.Linhas.Add(new LinhaDiff
                    {
                        Tipo = TipoLinhaDiff.Alterada,
                        NumeroOriginal = removidas[p] + 1,
                        TextoOriginal = a[removidas[p]],
                        NumeroNova = inseridas[p] + 1,
                        TextoNova = b[inseridas[p]]
                    });

                    saida.Alteradas++;
                }

                for (int p = pares; p < removidas.Count; p++)
                {
                    saida.Linhas.Add(new LinhaDiff
                    {
                        Tipo = TipoLinhaDiff.Removida,
                        NumeroOriginal = removidas[p] + 1,
                        TextoOriginal = a[removidas[p]]
                    });

                    saida.Removidas++;
                }

                for (int p = pares; p < inseridas.Count; p++)
                {
                    saida.Linhas.Add(new LinhaDiff
                    {
                        Tipo = TipoLinhaDiff.Inserida,
                        NumeroNova = inseridas[p] + 1,
                        TextoNova = b[inseridas[p]]
                    });

                    saida.Inseridas++;
                }
            }

            return saida;
        }
    }
}
