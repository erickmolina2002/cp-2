using Microsoft.Data.SqlClient;

namespace SistemaLoja.Lab12_ConexaoSQLServer;

public class DatabaseConnection
{
    private static string ObterStringConexao()
    {
        var servidor = Environment.GetEnvironmentVariable("SQL_SERVER") ?? "localhost";
        return $"Server={servidor},1433;" +
               "Database=LojaDB;" +
               "User Id=sa;" +
               "Password=SqlServer2024!;" +
               "TrustServerCertificate=True;";
    }

    public static SqlConnection ObterConexao()
    {
        return new SqlConnection(ObterStringConexao());
    }
}
