using System;

namespace ProjetoContaBancaria.Utils
{
    public static class Cores
    {
        public static void EscreverComCor(string texto, ConsoleColor cor)
        {
            var corOriginal = Console.ForegroundColor;
            Console.ForegroundColor = cor;
            Console.WriteLine(texto);
            Console.ForegroundColor = corOriginal;
        }
    }
}
