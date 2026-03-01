namespace CadastroCartsys.Domain.Entities
{
    public class Cidade
    {
        public int Id { get; private set; }
        public string Nome { get; private set; }
        public int EstadoId { get; private set; }
        public Estado? Estado { get; private set; }

        public Cidade(int id, string nome, int estadoId, Estado? estado = null)
        {
            Id = id;
            Nome = nome;
            EstadoId = estadoId;
            Estado = estado;
        }

        public Cidade() { }
    }
}
