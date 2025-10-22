using Microsoft.Data.SqlClient;

namespace SistemaLoja.Lab12_ConexaoSQLServer;

public class PedidoRepository
{
    public void CriarPedido(Pedido pedido, List<PedidoItem> itens)
    {
        using (SqlConnection conexao = DatabaseConnection.ObterConexao())
        {
            conexao.Open();

            SqlTransaction transacao = conexao.BeginTransaction();

            try
            {
                string sqlPedido = "INSERT INTO Pedidos (ClienteId, DataPedido, ValorTotal, Status) " +
                                   "OUTPUT INSERTED.Id " +
                                   "VALUES (@ClienteId, @DataPedido, @ValorTotal, 'Pendente')";

                int pedidoId = 0;
                using (SqlCommand comando = new SqlCommand(sqlPedido, conexao, transacao))
                {
                    comando.Parameters.AddWithValue("@ClienteId", pedido.ClienteId);
                    comando.Parameters.AddWithValue("@DataPedido", pedido.DataPedido);
                    comando.Parameters.AddWithValue("@ValorTotal", pedido.ValorTotal);

                    pedidoId = (int)comando.ExecuteScalar();
                }

                string sqlItem = "INSERT INTO PedidoItens (PedidoId, ProdutoId, Quantidade, PrecoUnitario) " +
                                "VALUES (@PedidoId, @ProdutoId, @Quantidade, @PrecoUnitario)";

                foreach (var item in itens)
                {
                    using (SqlCommand comando = new SqlCommand(sqlItem, conexao, transacao))
                    {
                        comando.Parameters.AddWithValue("@PedidoId", pedidoId);
                        comando.Parameters.AddWithValue("@ProdutoId", item.ProdutoId);
                        comando.Parameters.AddWithValue("@Quantidade", item.Quantidade);
                        comando.Parameters.AddWithValue("@PrecoUnitario", item.PrecoUnitario);

                        comando.ExecuteNonQuery();
                    }

                    string sqlEstoque = "UPDATE Produtos SET Estoque = Estoque - @Quantidade WHERE Id = @ProdutoId";

                    using (SqlCommand comando = new SqlCommand(sqlEstoque, conexao, transacao))
                    {
                        comando.Parameters.AddWithValue("@Quantidade", item.Quantidade);
                        comando.Parameters.AddWithValue("@ProdutoId", item.ProdutoId);

                        comando.ExecuteNonQuery();
                    }
                }

                transacao.Commit();
                Console.WriteLine($"\nPedido #{pedidoId} criado com sucesso!");
            }
            catch (Exception ex)
            {
                transacao.Rollback();
                Console.WriteLine($"\nErro ao criar pedido: {ex.Message}");
                throw;
            }
        }
    }

    public void ListarPedidosCliente(int clienteId)
    {
        string sql = @"SELECT p.*, c.Nome as NomeCliente
                      FROM Pedidos p
                      INNER JOIN Clientes c ON p.ClienteId = c.Id
                      WHERE p.ClienteId = @ClienteId
                      ORDER BY p.DataPedido DESC";

        using (SqlConnection conexao = DatabaseConnection.ObterConexao())
        {
            conexao.Open();

            using (SqlCommand comando = new SqlCommand(sql, conexao))
            {
                comando.Parameters.AddWithValue("@ClienteId", clienteId);

                using (SqlDataReader leitor = comando.ExecuteReader())
                {
                    Console.WriteLine("\n=== PEDIDOS DO CLIENTE ===");
                    string nomeCliente = "";
                    bool temPedidos = false;

                    while (leitor.Read())
                    {
                        if (!temPedidos)
                        {
                            nomeCliente = leitor["NomeCliente"].ToString();
                            Console.WriteLine($"\nCliente: {nomeCliente}\n");
                            Console.WriteLine($"{"ID",-5} {"Data",-20} {"Valor Total",-15} {"Status",-15}");
                            Console.WriteLine(new string('-', 60));
                            temPedidos = true;
                        }

                        int id = Convert.ToInt32(leitor["Id"]);
                        DateTime dataPedido = Convert.ToDateTime(leitor["DataPedido"]);
                        decimal valorTotal = Convert.ToDecimal(leitor["ValorTotal"]);
                        string status = leitor["Status"].ToString();

                        Console.WriteLine($"{id,-5} {dataPedido,-20:dd/MM/yyyy HH:mm} R$ {valorTotal,-12:F2} {status,-15}");
                    }

                    if (!temPedidos)
                    {
                        Console.WriteLine("\nNenhum pedido encontrado para este cliente.");
                    }
                }
            }
        }
    }

    public void ObterDetalhesPedido(int pedidoId)
    {
        string sqlPedido = @"SELECT p.*, c.Nome as NomeCliente, c.Email
                            FROM Pedidos p
                            INNER JOIN Clientes c ON p.ClienteId = c.Id
                            WHERE p.Id = @PedidoId";

        string sqlItens = @"SELECT
                              pi.*,
                              p.Nome as NomeProduto,
                              (pi.Quantidade * pi.PrecoUnitario) as Subtotal
                          FROM PedidoItens pi
                          INNER JOIN Produtos p ON pi.ProdutoId = p.Id
                          WHERE pi.PedidoId = @PedidoId";

        using (SqlConnection conexao = DatabaseConnection.ObterConexao())
        {
            conexao.Open();

            using (SqlCommand comando = new SqlCommand(sqlPedido, conexao))
            {
                comando.Parameters.AddWithValue("@PedidoId", pedidoId);

                using (SqlDataReader leitor = comando.ExecuteReader())
                {
                    if (leitor.Read())
                    {
                        Console.WriteLine("\n=== DETALHES DO PEDIDO ===");
                        Console.WriteLine($"\nPedido #: {leitor["Id"]}");
                        Console.WriteLine($"Cliente: {leitor["NomeCliente"]}");
                        Console.WriteLine($"Email: {leitor["Email"]}");
                        Console.WriteLine($"Data: {Convert.ToDateTime(leitor["DataPedido"]):dd/MM/yyyy HH:mm}");
                        Console.WriteLine($"Status: {leitor["Status"]}");
                        Console.WriteLine($"Valor Total: R$ {Convert.ToDecimal(leitor["ValorTotal"]):F2}");
                    }
                    else
                    {
                        Console.WriteLine("\nPedido não encontrado!");
                        return;
                    }
                }
            }

            using (SqlCommand comando = new SqlCommand(sqlItens, conexao))
            {
                comando.Parameters.AddWithValue("@PedidoId", pedidoId);

                using (SqlDataReader leitor = comando.ExecuteReader())
                {
                    Console.WriteLine("\n=== ITENS DO PEDIDO ===");
                    Console.WriteLine($"{"Produto",-30} {"Qtd",-5} {"Preço Unit.",-15} {"Subtotal",-15}");
                    Console.WriteLine(new string('-', 70));

                    while (leitor.Read())
                    {
                        string nomeProduto = leitor["NomeProduto"].ToString();
                        int quantidade = Convert.ToInt32(leitor["Quantidade"]);
                        decimal precoUnitario = Convert.ToDecimal(leitor["PrecoUnitario"]);
                        decimal subtotal = Convert.ToDecimal(leitor["Subtotal"]);

                        Console.WriteLine($"{nomeProduto,-30} {quantidade,-5} R$ {precoUnitario,-12:F2} R$ {subtotal,-12:F2}");
                    }
                }
            }
        }
    }

    public void TotalVendasPorPeriodo(DateTime dataInicio, DateTime dataFim)
    {
        string sql = @"SELECT
                        COUNT(*) as TotalPedidos,
                        SUM(ValorTotal) as ValorTotal,
                        AVG(ValorTotal) as TicketMedio
                      FROM Pedidos
                      WHERE DataPedido BETWEEN @DataInicio AND @DataFim";

        using (SqlConnection conexao = DatabaseConnection.ObterConexao())
        {
            conexao.Open();

            using (SqlCommand comando = new SqlCommand(sql, conexao))
            {
                comando.Parameters.AddWithValue("@DataInicio", dataInicio);
                comando.Parameters.AddWithValue("@DataFim", dataFim);

                using (SqlDataReader leitor = comando.ExecuteReader())
                {
                    if (leitor.Read())
                    {
                        Console.WriteLine("\n=== TOTAL DE VENDAS POR PERÍODO ===");
                        Console.WriteLine($"\nPeríodo: {dataInicio:dd/MM/yyyy} a {dataFim:dd/MM/yyyy}");
                        Console.WriteLine($"Total de Pedidos: {leitor["TotalPedidos"]}");
                        Console.WriteLine($"Valor Total: R$ {(leitor["ValorTotal"] != DBNull.Value ? Convert.ToDecimal(leitor["ValorTotal"]) : 0):F2}");
                        Console.WriteLine($"Ticket Médio: R$ {(leitor["TicketMedio"] != DBNull.Value ? Convert.ToDecimal(leitor["TicketMedio"]) : 0):F2}");
                    }
                }
            }
        }
    }
}
