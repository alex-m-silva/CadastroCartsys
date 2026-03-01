namespace CadastroCartsys.Domain.Entities
{
    public class Cliente
    {
        public int Id { get; private set; }
        public string Nome { get; private set; }
        public string? Cep { get; private set; }
        public string CpfCnpj { get; private set; }
        public string? Endereco { get; private set; }
        public string? Numero { get; private set; }
        public string? Complemento { get; private set; }
        public string? Bairro { get; private set; }
        public int CidadeId { get; private set; }
        public DateOnly? DataNascimento { get; private set; }
        public Cidade? Cidade { get; private set; }

        public Cliente(
            int id,
            string nome,
            string cpfCnpj,
            int cidadeId,
            string? cep = null,
            string? endereco = null,
            string? numero = null,
            string? complemento = null,
            string? bairro = null,
            DateOnly? dataNascimento = null,
            Cidade? cidade = null)
        {
            Id = id;
            Nome = nome;
            CpfCnpj = cpfCnpj;
            CidadeId = cidadeId;
            Cep = cep;
            Endereco = endereco;
            Numero = numero;
            Complemento = complemento;
            Bairro = bairro;
            DataNascimento = dataNascimento;
            Cidade = cidade;
        }

        public bool CanBeDeleted()
        {
            int[] idsBlocked = { 1, 5, 8, 10, 15 };
            return !idsBlocked.Contains(Id);
        }

        public Cliente() { }
    }
}
