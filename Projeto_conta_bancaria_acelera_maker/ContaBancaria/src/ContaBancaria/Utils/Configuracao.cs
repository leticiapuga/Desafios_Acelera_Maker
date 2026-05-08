using Microsoft.Extensions.Configuration;
using System.IO;

namespace ProjetoContaBancaria.Utils
{
    public static class Configuracao
    {
        public static IConfigurationRoot GetConfiguration()
        {
            var caminhosPossiveis = new[]
            {
                Directory.GetCurrentDirectory(),
                AppContext.BaseDirectory,
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..")
            };

            foreach (var caminho in caminhosPossiveis.Select(Path.GetFullPath).Distinct())
            {
                var arquivo = Path.Combine(caminho, "appsettings.json");

                if (File.Exists(arquivo))
                {
                    return new ConfigurationBuilder()
                        .SetBasePath(caminho)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .Build();
                }
            }

            throw new FileNotFoundException("Arquivo appsettings.json não encontrado. Verifique se ele está na pasta src/ContaBancaria ou na pasta de execução do sistema.");
        }

        public static string GetConnectionString(string name)
        {
            var conn = GetConfiguration().GetSection("ConnectionStrings")[name];

            if (conn is null)
                throw new InvalidOperationException($"Connection string '{name}' não encontrada.");

            return conn;
        }
    }
}
