using Microsoft.Data.SqlClient;

namespace SistemaLoja.Lab12_ConexaoSQLServer;

public class ProdutoRepository
{
    public void ListarTodosProdutos()
    {
        string sql = "SELECT * FROM Produtos";

        using (SqlConnection conexao = DatabaseConnection.ObterConexao())
        {
            conexao.Open();

            using (SqlCommand comando = new SqlCommand(sql, conexao))
            {
                using (SqlDataReader leitor = comando.ExecuteReader())
                {
                    Console.WriteLine("\n=== LISTA DE PRODUTOS ===");
                    Console.WriteLine($"{"ID",-5} {"Nome",-30} {"Preço",-12} {"Estoque",-10}");
                    Console.WriteLine(new string('-', 60));

                    while (leitor.Read())
                    {
                        int id = Convert.ToInt32(leitor["Id"]);
                        string nome = leitor["Nome"].ToString();
                        decimal preco = Convert.ToDecimal(leitor["Preco"]);
                        int estoque = Convert.ToInt32(leitor["Estoque"]);

                        Console.WriteLine($"{id,-5} {nome,-30} R$ {preco,-9:F2} {estoque,-10}");
                    }
                }
            }
        }
    }

    public void InserirProduto(Produto produto)
    {
        string sql = "INSERT INTO Produtos (Nome, Preco, Estoque, CategoriaId) " +
                     "VALUES (@Nome, @Preco, @Estoque, @CategoriaId)";

        using (SqlConnection conexao = DatabaseConnection.ObterConexao())
        {
            conexao.Open();

            using (SqlCommand comando = new SqlCommand(sql, conexao))
            {
                comando.Parameters.AddWithValue("@Nome", produto.Nome);
                comando.Parameters.AddWithValue("@Preco", produto.Preco);
                comando.Parameters.AddWithValue("@Estoque", produto.Estoque);
                comando.Parameters.AddWithValue("@CategoriaId", produto.CategoriaId);

                int linhasAfetadas = comando.ExecuteNonQuery();

                if (linhasAfetadas > 0)
                {
                    Console.WriteLine("\nProduto inserido com sucesso!");
                }
            }
        }
    }

    public void AtualizarProduto(Produto produto)
    {
        string sql = "UPDATE Produtos SET " +
                     "Nome = @Nome, " +
                     "Preco = @Preco, " +
                     "Estoque = @Estoque " +
                     "WHERE Id = @Id";

        using (SqlConnection conexao = DatabaseConnection.ObterConexao())
        {
            conexao.Open();

            using (SqlCommand comando = new SqlCommand(sql, conexao))
            {
                comando.Parameters.AddWithValue("@Id", produto.Id);
                comando.Parameters.AddWithValue("@Nome", produto.Nome);
                comando.Parameters.AddWithValue("@Preco", produto.Preco);
                comando.Parameters.AddWithValue("@Estoque", produto.Estoque);

                int linhasAfetadas = comando.ExecuteNonQuery();

                if (linhasAfetadas > 0)
                {
                    Console.WriteLine("\nProduto atualizado com sucesso!");
                }
                else
                {
                    Console.WriteLine("\nProduto não encontrado!");
                }
            }
        }
    }

    public void DeletarProduto(int id)
    {
        string sql = "DELETE FROM Produtos WHERE Id = @Id";

        using (SqlConnection conexao = DatabaseConnection.ObterConexao())
        {
            conexao.Open();

            using (SqlCommand comando = new SqlCommand(sql, conexao))
            {
                comando.Parameters.AddWithValue("@Id", id);

                int linhasAfetadas = comando.ExecuteNonQuery();

                if (linhasAfetadas > 0)
                {
                    Console.WriteLine("\nProduto deletado com sucesso!");
                }
                else
                {
                    Console.WriteLine("\nProduto não encontrado!");
                }
            }
        }
    }

    public Produto BuscarPorId(int id)
    {
        string sql = "SELECT * FROM Produtos WHERE Id = @Id";
        Produto produto = null;

        using (SqlConnection conexao = DatabaseConnection.ObterConexao())
        {
            conexao.Open();

            using (SqlCommand comando = new SqlCommand(sql, conexao))
            {
                comando.Parameters.AddWithValue("@Id", id);

                using (SqlDataReader leitor = comando.ExecuteReader())
                {
                    if (leitor.Read())
                    {
                        produto = new Produto
                        {
                            Id = Convert.ToInt32(leitor["Id"]),
                            Nome = leitor["Nome"].ToString(),
                            Preco = Convert.ToDecimal(leitor["Preco"]),
                            Estoque = Convert.ToInt32(leitor["Estoque"]),
                            CategoriaId = Convert.ToInt32(leitor["CategoriaId"])
                        };
                    }
                }
            }
        }

        return produto;
    }

    public void ListarProdutosPorCategoria(int categoriaId)
    {
        string sql = @"SELECT p.*, c.Nome as NomeCategoria
                      FROM Produtos p
                      INNER JOIN Categorias c ON p.CategoriaId = c.Id
                      WHERE p.CategoriaId = @CategoriaId";

        using (SqlConnection conexao = DatabaseConnection.ObterConexao())
        {
            conexao.Open();

            using (SqlCommand comando = new SqlCommand(sql, conexao))
            {
                comando.Parameters.AddWithValue("@CategoriaId", categoriaId);

                using (SqlDataReader leitor = comando.ExecuteReader())
                {
                    Console.WriteLine("\n=== PRODUTOS POR CATEGORIA ===");
                    string nomeCategoria = "";
                    bool temProdutos = false;

                    while (leitor.Read())
                    {
                        if (!temProdutos)
                        {
                            nomeCategoria = leitor["NomeCategoria"].ToString();
                            Console.WriteLine($"\nCategoria: {nomeCategoria}\n");
                            Console.WriteLine($"{"ID",-5} {"Nome",-30} {"Preço",-12} {"Estoque",-10}");
                            Console.WriteLine(new string('-', 60));
                            temProdutos = true;
                        }

                        int id = Convert.ToInt32(leitor["Id"]);
                        string nome = leitor["Nome"].ToString();
                        decimal preco = Convert.ToDecimal(leitor["Preco"]);
                        int estoque = Convert.ToInt32(leitor["Estoque"]);

                        Console.WriteLine($"{id,-5} {nome,-30} R$ {preco,-9:F2} {estoque,-10}");
                    }

                    if (!temProdutos)
                    {
                        Console.WriteLine("\nNenhum produto encontrado nesta categoria.");
                    }
                }
            }
        }
    }
}
