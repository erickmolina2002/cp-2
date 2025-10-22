using System;
using Microsoft.Data.SqlClient;
using SistemaLoja.Lab12_ConexaoSQLServer;

namespace SistemaLoja
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== LAB 12 - CONEXÃO SQL SERVER ===\n");

            var repositorioProduto = new ProdutoRepository();
            var repositorioPedido = new PedidoRepository();

            bool continuar = true;

            while (continuar)
            {
                MostrarMenu();
                string opcao = Console.ReadLine();

                try
                {
                    switch (opcao)
                    {
                        case "1":
                            repositorioProduto.ListarTodosProdutos();
                            break;

                        case "2":
                            InserirNovoProduto(repositorioProduto);
                            break;

                        case "3":
                            AtualizarProdutoExistente(repositorioProduto);
                            break;

                        case "4":
                            DeletarProdutoExistente(repositorioProduto);
                            break;

                        case "5":
                            ListarPorCategoria(repositorioProduto);
                            break;

                        case "6":
                            CriarNovoPedido(repositorioPedido);
                            break;

                        case "7":
                            ListarPedidosDeCliente(repositorioPedido);
                            break;

                        case "8":
                            DetalhesDoPedido(repositorioPedido);
                            break;

                        case "0":
                            continuar = false;
                            break;

                        default:
                            Console.WriteLine("Opção inválida!");
                            break;
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"\nErro SQL: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nErro: {ex.Message}");
                }

                if (continuar)
                {
                    Console.WriteLine("\nPressione qualquer tecla para continuar...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            Console.WriteLine("\nPrograma finalizado!");
        }

        static void MostrarMenu()
        {
            Console.WriteLine("\n╔════════════════════════════════════╗");
            Console.WriteLine("║       MENU PRINCIPAL               ║");
            Console.WriteLine("╠════════════════════════════════════╣");
            Console.WriteLine("║  PRODUTOS                          ║");
            Console.WriteLine("║  1 - Listar todos os produtos      ║");
            Console.WriteLine("║  2 - Inserir novo produto          ║");
            Console.WriteLine("║  3 - Atualizar produto             ║");
            Console.WriteLine("║  4 - Deletar produto               ║");
            Console.WriteLine("║  5 - Listar por categoria          ║");
            Console.WriteLine("║                                    ║");
            Console.WriteLine("║  PEDIDOS                           ║");
            Console.WriteLine("║  6 - Criar novo pedido             ║");
            Console.WriteLine("║  7 - Listar pedidos de cliente     ║");
            Console.WriteLine("║  8 - Detalhes de um pedido         ║");
            Console.WriteLine("║                                    ║");
            Console.WriteLine("║  0 - Sair                          ║");
            Console.WriteLine("╚════════════════════════════════════╝");
            Console.Write("\nEscolha uma opção: ");
        }

        static void InserirNovoProduto(ProdutoRepository repositorio)
        {
            Console.WriteLine("\n=== INSERIR NOVO PRODUTO ===");

            Console.Write("Nome: ");
            string nome = Console.ReadLine();

            Console.Write("Preço: ");
            decimal preco = decimal.Parse(Console.ReadLine());

            Console.Write("Estoque: ");
            int estoque = int.Parse(Console.ReadLine());

            Console.Write("ID da Categoria (0-Eletrônicos, 1-Livros, 2-Roupas, 3-Alimentos, 4-Esportes): ");
            int categoriaId = int.Parse(Console.ReadLine());

            var produto = new Produto
            {
                Nome = nome,
                Preco = preco,
                Estoque = estoque,
                CategoriaId = categoriaId
            };

            repositorio.InserirProduto(produto);
        }

        static void AtualizarProdutoExistente(ProdutoRepository repositorio)
        {
            Console.WriteLine("\n=== ATUALIZAR PRODUTO ===");

            Console.Write("ID do produto: ");
            int id = int.Parse(Console.ReadLine());

            var produto = repositorio.BuscarPorId(id);

            if (produto == null)
            {
                Console.WriteLine("\nProduto não encontrado!");
                return;
            }

            Console.WriteLine($"\nProduto atual: {produto.Nome}");
            Console.WriteLine($"Preço: R$ {produto.Preco:F2}");
            Console.WriteLine($"Estoque: {produto.Estoque}");

            Console.Write("\nNovo nome (Enter para manter): ");
            string nome = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nome))
                produto.Nome = nome;

            Console.Write("Novo preço (Enter para manter): ");
            string precoStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(precoStr))
                produto.Preco = decimal.Parse(precoStr);

            Console.Write("Novo estoque (Enter para manter): ");
            string estoqueStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(estoqueStr))
                produto.Estoque = int.Parse(estoqueStr);

            repositorio.AtualizarProduto(produto);
        }

        static void DeletarProdutoExistente(ProdutoRepository repositorio)
        {
            Console.WriteLine("\n=== DELETAR PRODUTO ===");

            Console.Write("ID do produto: ");
            int id = int.Parse(Console.ReadLine());

            var produto = repositorio.BuscarPorId(id);

            if (produto == null)
            {
                Console.WriteLine("\nProduto não encontrado!");
                return;
            }

            Console.WriteLine($"\nProduto: {produto.Nome}");
            Console.Write("Confirma a exclusão? (S/N): ");
            string confirmacao = Console.ReadLine();

            if (confirmacao?.ToUpper() == "S")
            {
                repositorio.DeletarProduto(id);
            }
            else
            {
                Console.WriteLine("\nOperação cancelada!");
            }
        }

        static void ListarPorCategoria(ProdutoRepository repositorio)
        {
            Console.WriteLine("\n=== PRODUTOS POR CATEGORIA ===");
            Console.WriteLine("\n0 - Eletrônicos");
            Console.WriteLine("1 - Livros");
            Console.WriteLine("2 - Roupas");
            Console.WriteLine("3 - Alimentos");
            Console.WriteLine("4 - Esportes");

            Console.Write("\nEscolha a categoria: ");
            int categoriaId = int.Parse(Console.ReadLine());

            repositorio.ListarProdutosPorCategoria(categoriaId);
        }

        static void CriarNovoPedido(PedidoRepository repositorio)
        {
            Console.WriteLine("\n=== CRIAR NOVO PEDIDO ===");

            Console.Write("ID do Cliente: ");
            int clienteId = int.Parse(Console.ReadLine());

            var pedido = new Pedido
            {
                ClienteId = clienteId,
                DataPedido = DateTime.Now,
                ValorTotal = 0
            };

            var itens = new List<PedidoItem>();
            bool adicionarMaisItens = true;

            while (adicionarMaisItens)
            {
                Console.Write("\nID do Produto: ");
                int produtoId = int.Parse(Console.ReadLine());

                Console.Write("Quantidade: ");
                int quantidade = int.Parse(Console.ReadLine());

                Console.Write("Preço Unitário: ");
                decimal precoUnitario = decimal.Parse(Console.ReadLine());

                var item = new PedidoItem
                {
                    ProdutoId = produtoId,
                    Quantidade = quantidade,
                    PrecoUnitario = precoUnitario
                };

                itens.Add(item);
                pedido.ValorTotal += quantidade * precoUnitario;

                Console.Write("\nAdicionar mais itens? (S/N): ");
                adicionarMaisItens = Console.ReadLine()?.ToUpper() == "S";
            }

            repositorio.CriarPedido(pedido, itens);
        }

        static void ListarPedidosDeCliente(PedidoRepository repositorio)
        {
            Console.WriteLine("\n=== PEDIDOS DO CLIENTE ===");

            Console.Write("ID do Cliente: ");
            int clienteId = int.Parse(Console.ReadLine());

            repositorio.ListarPedidosCliente(clienteId);
        }

        static void DetalhesDoPedido(PedidoRepository repositorio)
        {
            Console.WriteLine("\n=== DETALHES DO PEDIDO ===");

            Console.Write("ID do Pedido: ");
            int pedidoId = int.Parse(Console.ReadLine());

            repositorio.ObterDetalhesPedido(pedidoId);
        }
    }
}
