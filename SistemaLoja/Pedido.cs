namespace SistemaLoja.Lab12_ConexaoSQLServer;

public class Pedido
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public DateTime DataPedido { get; set; }
    public decimal ValorTotal { get; set; }
}
