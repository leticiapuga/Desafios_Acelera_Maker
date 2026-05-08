using ProjetoContaBancaria.Controllers;
using ProjetoContaBancaria.Models;
using ProjetoContaBancaria.Utils;
using System.Globalization;
using System.Text;

namespace ProjetoContaBancaria
{
    internal class Program
    {
        private static readonly CultureInfo CulturaBr = new("pt-BR");
        private const int Largura = 78;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "Banco Essencial | Conta Bancária";

            var controller = new ContaController();
            var opcao = -1;

            while (opcao != 0)
            {
                MostrarMenuPrincipal();
                opcao = LerInteiro("Escolha uma opção", 0, 9);

                try
                {
                    switch (opcao)
                    {
                        case 1:
                            CriarContaCorrente(controller);
                            break;
                        case 2:
                            CriarContaPoupanca(controller);
                            break;
                        case 3:
                            ListarContas(controller);
                            break;
                        case 4:
                            ConsultarConta(controller);
                            break;
                        case 5:
                            Sacar(controller);
                            break;
                        case 6:
                            Depositar(controller);
                            break;
                        case 7:
                            Transferir(controller);
                            break;
                        case 8:
                            EncerrarConta(controller);
                            break;
                        case 9:
                            MostrarRelatorioFinanceiro(controller);
                            break;
                        case 0:
                            MostrarDespedida();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MostrarMensagem("Não foi possível concluir a operação.", ConsoleColor.Red);
                    Console.WriteLine("Verifique se o banco de dados está ativo e se a conexão do appsettings.json está correta.");
                    Console.WriteLine($"Detalhe técnico: {ex.Message}");
                }

                if (opcao != 0)
                    Pausar();
            }
        }

        private static void MostrarMenuPrincipal()
        {
            Console.Clear();
            MostrarCabecalho("Banco Essencial", "Gerencie contas, saldos e transferências de forma simples.");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("O que você deseja fazer hoje?\n");
            Console.ResetColor();

            MostrarOpcaoMenu("1", "Criar conta corrente", "Conta com limite para movimentações.");
            MostrarOpcaoMenu("2", "Criar conta poupança", "Conta com aniversário mensal de rendimento.");
            MostrarOpcaoMenu("3", "Listar contas", "Veja todas as contas cadastradas.");
            MostrarOpcaoMenu("4", "Consultar conta", "Busque uma conta pelo número.");
            MostrarOpcaoMenu("5", "Sacar", "Retire um valor da conta informada.");
            MostrarOpcaoMenu("6", "Depositar", "Adicione saldo em uma conta.");
            MostrarOpcaoMenu("7", "Transferir", "Envie valor de uma conta para outra.");
            MostrarOpcaoMenu("8", "Encerrar conta", "Remova uma conta cadastrada.");
            MostrarOpcaoMenu("9", "Relatório financeiro", "Resumo geral de saldos, limites e tipos de conta.");
            MostrarOpcaoMenu("0", "Sair", "Finalizar o atendimento.");

            Console.WriteLine();
        }

        private static void CriarContaCorrente(ContaController controller)
        {
            MostrarTela("Nova conta corrente", "Informe os dados abaixo. Campos vazios serão solicitados novamente.");

            var titular = LerTextoObrigatorio("Nome do titular");
            var agencia = LerAgencia();
            var saldo = LerValorOpcional("Saldo inicial", "pressione Enter para iniciar com R$ 0,00");
            var limite = LerValor("Limite disponível", 0);

            var conta = new ContaCorrente(0, agencia, titular, saldo, limite);
            var numeroConta = controller.Cadastrar(conta);
            conta.Numero = numeroConta;

            MostrarMensagem("Conta corrente criada com sucesso!", ConsoleColor.Green);
            MostrarConta(conta, "Resumo da nova conta");
            MostrarMensagem("Guarde o número da conta para futuras operações.", ConsoleColor.Yellow);
        }

        private static void CriarContaPoupanca(ContaController controller)
        {
            MostrarTela("Nova conta poupança", "Informe os dados abaixo. A data de aniversário deve ser um dia entre 1 e 31.");

            var titular = LerTextoObrigatorio("Nome do titular");
            var agencia = LerAgencia();
            var saldo = LerValorOpcional("Saldo inicial", "pressione Enter para iniciar com R$ 0,00");
            var aniversario = LerInteiro("Dia de aniversário da poupança", 1, 31);

            var conta = new ContaPoupanca(0, agencia, titular, saldo, aniversario);
            var numeroConta = controller.Cadastrar(conta);
            conta.Numero = numeroConta;

            MostrarMensagem("Conta poupança criada com sucesso!", ConsoleColor.Green);
            MostrarConta(conta, "Resumo da nova conta");
            MostrarMensagem("Guarde o número da conta para futuras operações.", ConsoleColor.Yellow);
        }

        private static void ListarContas(ContaController controller)
        {
            MostrarTela("Contas cadastradas", "Visualização organizada das contas encontradas no sistema.");

            var contas = controller.ListarTodas();
            if (contas.Count == 0)
            {
                MostrarEstadoVazio("Nenhuma conta cadastrada até o momento.", "Crie uma conta corrente ou poupança para começar.");
                return;
            }

            Console.WriteLine($"Total de contas: {contas.Count}");
            Console.WriteLine($"Saldo total em contas: {FormatarMoeda(contas.Sum(conta => conta.Saldo))}\n");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("┌────────────┬──────────┬──────────┬──────────────────────┬──────────────┬──────────────────────┐");
            Console.WriteLine("│ Tipo       │ Número   │ Agência  │ Titular              │ Saldo        │ Detalhes             │");
            Console.WriteLine("├────────────┼──────────┼──────────┼──────────────────────┼──────────────┼──────────────────────┤");
            Console.ResetColor();

            foreach (var conta in contas)
            {
                var tipo = conta is ContaCorrente ? "Corrente" : "Poupança";
                var detalhes = conta switch
                {
                    ContaCorrente cc => $"Limite {FormatarMoeda(cc.Limite)}",
                    ContaPoupanca cp => $"Aniversário dia {cp.Aniversario}",
                    _ => "-"
                };

                Console.WriteLine($"│ {Ajustar(tipo, 10)} │ {Ajustar(conta.Numero.ToString(), 8)} │ {Ajustar(conta.Agencia.ToString(), 8)} │ {Ajustar(conta.Titular, 20)} │ {Ajustar(FormatarMoeda(conta.Saldo), 12)} │ {Ajustar(detalhes, 20)} │");
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("└────────────┴──────────┴──────────┴──────────────────────┴──────────────┴──────────────────────┘");
            Console.ResetColor();
        }

        private static void ConsultarConta(ContaController controller)
        {
            MostrarTela("Consultar conta", "Digite o número da conta para visualizar os detalhes.");

            var numero = LerInteiro("Número da conta", 1, int.MaxValue);
            var conta = controller.ProcurarPorNumero(numero);

            if (conta is null)
            {
                MostrarMensagem("Conta não encontrada.", ConsoleColor.Red);
                Console.WriteLine("Confira o número informado e tente novamente.");
                return;
            }

            MostrarConta(conta, "Dados da conta");
        }

        private static void Sacar(ContaController controller)
        {
            MostrarTela("Saque", "Informe a conta e o valor que deseja retirar.");

            var conta = ObterContaOuExibirErro(controller, "Número da conta");
            if (conta is null) return;

            MostrarConta(conta, "Conta selecionada");
            var valor = LerValor("Valor do saque", 0.01f);

            if (!conta.Sacar(valor))
            {
                MostrarMensagem("Saque não realizado.", ConsoleColor.Red);
                Console.WriteLine("O valor precisa ser positivo e respeitar o saldo disponível. Em conta corrente, o limite também é considerado.");
                return;
            }

            controller.Atualizar(conta);
            MostrarMensagem("Saque realizado com sucesso!", ConsoleColor.Green);
            MostrarConta(conta, "Saldo atualizado");
        }

        private static void Depositar(ContaController controller)
        {
            MostrarTela("Depósito", "Informe a conta e o valor que deseja adicionar.");

            var conta = ObterContaOuExibirErro(controller, "Número da conta");
            if (conta is null) return;

            MostrarConta(conta, "Conta selecionada");
            var valor = LerValor("Valor do depósito", 0.01f);

            conta.Depositar(valor);
            controller.Atualizar(conta);

            MostrarMensagem("Depósito realizado com sucesso!", ConsoleColor.Green);
            MostrarConta(conta, "Saldo atualizado");
        }

        private static void Transferir(ContaController controller)
        {
            MostrarTela("Transferência", "Envie dinheiro de uma conta para outra com conferência antes da operação.");

            var origem = ObterContaOuExibirErro(controller, "Conta de origem");
            if (origem is null) return;

            var destino = ObterContaOuExibirErro(controller, "Conta de destino");
            if (destino is null) return;

            if (origem.Numero == destino.Numero)
            {
                MostrarMensagem("A conta de origem e a conta de destino não podem ser iguais.", ConsoleColor.Red);
                return;
            }

            Console.WriteLine();
            MostrarConta(origem, "Origem");
            MostrarConta(destino, "Destino");

            var valor = LerValor("Valor da transferência", 0.01f);

            if (!PossuiSaldoParaTransferir(origem, valor))
            {
                MostrarMensagem("Transferência não realizada.", ConsoleColor.Red);
                Console.WriteLine("A conta de origem não possui saldo/limite suficiente para essa operação.");
                return;
            }

            if (!Confirmar("Confirmar transferência?"))
            {
                MostrarMensagem("Transferência cancelada pelo usuário.", ConsoleColor.Yellow);
                return;
            }

            // A atualização das duas contas é feita pelo controller dentro de uma transação.
            // Assim, ou origem e destino são salvos juntos, ou nada é alterado no banco.
            controller.Transferir(origem.Numero, destino.Numero, valor);

            var origemAtualizada = controller.ProcurarPorNumero(origem.Numero);
            var destinoAtualizado = controller.ProcurarPorNumero(destino.Numero);

            MostrarMensagem("Transferência realizada com sucesso!", ConsoleColor.Green);

            if (origemAtualizada is not null)
                MostrarConta(origemAtualizada, "Origem atualizada");

            if (destinoAtualizado is not null)
                MostrarConta(destinoAtualizado, "Destino atualizado");
        }

        private static void EncerrarConta(ContaController controller)
        {
            MostrarTela("Encerrar conta", "Essa ação remove a conta do cadastro.");

            var conta = ObterContaOuExibirErro(controller, "Número da conta");
            if (conta is null) return;

            MostrarConta(conta, "Conta selecionada");

            if (!Confirmar("Tem certeza que deseja encerrar esta conta?"))
            {
                MostrarMensagem("Encerramento cancelado.", ConsoleColor.Yellow);
                return;
            }

            controller.Deletar(conta.Numero);
            MostrarMensagem("Conta encerrada com sucesso.", ConsoleColor.Green);
        }

        private static void MostrarRelatorioFinanceiro(ContaController controller)
        {
            MostrarTela("Relatório financeiro", "Resumo dos principais indicadores das contas cadastradas.");

            var contas = controller.ListarTodas();
            if (contas.Count == 0)
            {
                MostrarEstadoVazio("Não há dados para gerar o relatório.", "Cadastre contas e realize movimentações para visualizar indicadores.");
                return;
            }

            var contasCorrentes = contas.OfType<ContaCorrente>().ToList();
            var contasPoupanca = contas.OfType<ContaPoupanca>().ToList();
            var contaMaiorSaldo = contas.OrderByDescending(conta => conta.Saldo).First();
            var contasNegativas = contas.Count(conta => conta.Saldo < 0);

            var saldoTotal = contas.Sum(conta => conta.Saldo);
            var saldoCorrente = contasCorrentes.Sum(conta => conta.Saldo);
            var saldoPoupanca = contasPoupanca.Sum(conta => conta.Saldo);
            var limiteTotal = contasCorrentes.Sum(conta => conta.Limite);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("┌" + new string('─', Largura) + "┐");
            Console.WriteLine("│" + Centralizar("INDICADORES GERAIS", Largura) + "│");
            Console.WriteLine("├" + new string('─', Largura) + "┤");
            Console.ResetColor();

            MostrarLinhaRelatorio("Total de contas", contas.Count.ToString());
            MostrarLinhaRelatorio("Contas correntes", contasCorrentes.Count.ToString());
            MostrarLinhaRelatorio("Contas poupança", contasPoupanca.Count.ToString());
            MostrarLinhaRelatorio("Saldo total armazenado", FormatarMoeda(saldoTotal));
            MostrarLinhaRelatorio("Saldo em contas correntes", FormatarMoeda(saldoCorrente));
            MostrarLinhaRelatorio("Saldo em contas poupança", FormatarMoeda(saldoPoupanca));
            MostrarLinhaRelatorio("Limite total disponível", FormatarMoeda(limiteTotal));
            MostrarLinhaRelatorio("Contas com saldo negativo", contasNegativas.ToString());

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("└" + new string('─', Largura) + "┘");
            Console.ResetColor();
            Console.WriteLine();

            MostrarConta(contaMaiorSaldo, "Conta com maior saldo");
        }

        private static void MostrarLinhaRelatorio(string rotulo, string valor)
        {
            Console.WriteLine("│ " + Ajustar(rotulo, 35) + " " + Ajustar(valor, Largura - 38) + " │");
        }

        private static Conta? ObterContaOuExibirErro(ContaController controller, string rotulo)
        {
            var numero = LerInteiro(rotulo, 1, int.MaxValue);
            var conta = controller.ProcurarPorNumero(numero);

            if (conta is not null) return conta;

            MostrarMensagem("Conta não encontrada.", ConsoleColor.Red);
            Console.WriteLine("Confira o número informado e tente novamente.");
            return null;
        }

        private static bool PossuiSaldoParaTransferir(Conta conta, float valor)
        {
            if (valor <= 0)
                return false;

            return conta switch
            {
                ContaCorrente corrente => corrente.Saldo + corrente.Limite >= valor,
                ContaPoupanca poupanca => poupanca.Saldo >= valor,
                _ => false
            };
        }

        private static void MostrarCabecalho(string titulo, string subtitulo)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔" + new string('═', Largura) + "╗");
            Console.WriteLine("║" + Centralizar(titulo.ToUpperInvariant(), Largura) + "║");
            Console.WriteLine("╚" + new string('═', Largura) + "╝");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(Centralizar(subtitulo, Largura));
            Console.ResetColor();
            Console.WriteLine();
        }

        private static void MostrarTela(string titulo, string subtitulo)
        {
            Console.Clear();
            MostrarCabecalho(titulo, subtitulo);
        }

        private static void MostrarOpcaoMenu(string numero, string titulo, string descricao)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"  {numero.PadLeft(2, '0')}  ");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(titulo.PadRight(24));
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(descricao);
            Console.ResetColor();
        }

        private static void MostrarMensagem(string mensagem, ConsoleColor cor)
        {
            Console.WriteLine();
            Console.ForegroundColor = cor;
            Console.WriteLine($"● {mensagem}");
            Console.ResetColor();
            Console.WriteLine();
        }

        private static void MostrarEstadoVazio(string titulo, string descricao)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("┌" + new string('─', Largura) + "┐");
            Console.WriteLine("│" + Centralizar(titulo, Largura) + "│");
            Console.WriteLine("│" + Centralizar(descricao, Largura) + "│");
            Console.WriteLine("└" + new string('─', Largura) + "┘");
            Console.ResetColor();
        }

        private static void MostrarConta(Conta conta, string titulo)
        {
            var tipo = conta is ContaCorrente ? "Conta corrente" : "Conta poupança";
            var detalhe = conta switch
            {
                ContaCorrente cc => $"Limite disponível: {FormatarMoeda(cc.Limite)}",
                ContaPoupanca cp => $"Aniversário da poupança: dia {cp.Aniversario}",
                _ => "Detalhes não informados"
            };

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("┌" + new string('─', Largura) + "┐");
            Console.WriteLine("│" + Centralizar(titulo, Largura) + "│");
            Console.WriteLine("├" + new string('─', Largura) + "┤");
            Console.ResetColor();

            Console.WriteLine("│ " + Ajustar($"Tipo: {tipo}", Largura - 2) + " │");
            Console.WriteLine("│ " + Ajustar($"Número: {conta.Numero}    Agência: {conta.Agencia}", Largura - 2) + " │");
            Console.WriteLine("│ " + Ajustar($"Titular: {conta.Titular}", Largura - 2) + " │");
            Console.WriteLine("│ " + Ajustar($"Saldo atual: {FormatarMoeda(conta.Saldo)}", Largura - 2) + " │");
            Console.WriteLine("│ " + Ajustar(detalhe, Largura - 2) + " │");

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("└" + new string('─', Largura) + "┘");
            Console.ResetColor();
            Console.WriteLine();
        }

        private static void MostrarDespedida()
        {
            Console.Clear();
            MostrarCabecalho("Até logo!", "Obrigado por utilizar o Banco Essencial.");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Centralizar("Sua sessão foi finalizada com segurança.", Largura));
            Console.ResetColor();
        }

        private static string LerTextoObrigatorio(string rotulo)
        {
            while (true)
            {
                Console.Write($"{rotulo}: ");
                var valor = Console.ReadLine()?.Trim() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(valor))
                    return valor;

                MostrarMensagem("Esse campo é obrigatório.", ConsoleColor.Red);
            }
        }

        private static int LerAgencia()
        {
            while (true)
            {
                Console.Write("Agência (ex.: 1234 ou 5678-9): ");
                var entrada = Console.ReadLine()?.Trim() ?? string.Empty;
                var somenteNumeros = entrada.Replace("-", string.Empty);

                if (string.IsNullOrWhiteSpace(entrada) || entrada.Length > 6 || !somenteNumeros.All(char.IsDigit))
                {
                    MostrarMensagem("Agência inválida. Use até 6 caracteres, com números e hífen opcional.", ConsoleColor.Red);
                    continue;
                }

                if (int.TryParse(somenteNumeros, out var agencia) && agencia > 0)
                    return agencia;

                MostrarMensagem("Agência inválida. Digite um número maior que zero.", ConsoleColor.Red);
            }
        }

        private static int LerInteiro(string rotulo, int minimo, int maximo)
        {
            while (true)
            {
                Console.Write($"{rotulo}: ");
                var entrada = Console.ReadLine();

                if (int.TryParse(entrada, out var valor) && valor >= minimo && valor <= maximo)
                    return valor;

                MostrarMensagem($"Digite um número entre {minimo} e {maximo}.", ConsoleColor.Red);
            }
        }

        private static float LerValor(string rotulo, float minimo)
        {
            while (true)
            {
                Console.Write($"{rotulo}: R$ ");
                var entrada = Console.ReadLine()?.Trim();

                if (TentarConverterValor(entrada, out var valor) && valor >= minimo)
                    return valor;

                MostrarMensagem($"Digite um valor igual ou maior que {FormatarMoeda(minimo)}.", ConsoleColor.Red);
            }
        }

        private static float LerValorOpcional(string rotulo, string ajuda)
        {
            while (true)
            {
                Console.Write($"{rotulo} ({ajuda}): R$ ");
                var entrada = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(entrada))
                    return 0;

                if (TentarConverterValor(entrada, out var valor) && valor >= 0)
                    return valor;

                MostrarMensagem("Digite um valor válido ou pressione Enter para deixar zerado.", ConsoleColor.Red);
            }
        }

        private static bool TentarConverterValor(string? entrada, out float valor)
        {
            if (float.TryParse(entrada, NumberStyles.Number, CulturaBr, out valor))
                return true;

            return float.TryParse(entrada, NumberStyles.Number, CultureInfo.InvariantCulture, out valor);
        }

        private static bool Confirmar(string pergunta)
        {
            Console.Write($"{pergunta} Digite SIM para confirmar: ");
            var resposta = Console.ReadLine()?.Trim();
            return string.Equals(resposta, "SIM", StringComparison.OrdinalIgnoreCase);
        }

        private static string FormatarMoeda(float valor)
        {
            return valor.ToString("C", CulturaBr);
        }

        private static string Ajustar(string texto, int largura)
        {
            texto ??= string.Empty;

            if (texto.Length > largura)
                return texto[..Math.Max(0, largura - 1)] + "…";

            return texto.PadRight(largura);
        }

        private static string Centralizar(string texto, int largura)
        {
            if (texto.Length >= largura)
                return Ajustar(texto, largura);

            var esquerda = (largura - texto.Length) / 2;
            var direita = largura - texto.Length - esquerda;
            return new string(' ', esquerda) + texto + new string(' ', direita);
        }

        private static void Pausar()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\nPressione Enter para voltar ao menu principal...");
            Console.ResetColor();
            Console.ReadLine();
        }
    }
}
