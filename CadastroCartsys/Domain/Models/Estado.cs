namespace CadastroCartsys.Domain.Entities
{
    public class Estado
    {
        public int Id { get; private set; }
        public string Nome { get; private set; }
        public string Uf { get; private set; }

        public Estado(int id, string nome, string uf)
        {
            Id = id;
            Nome = nome;
            Uf = uf;
        }

        private Estado() { }
    }
}
