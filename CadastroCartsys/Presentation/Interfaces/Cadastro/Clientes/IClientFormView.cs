using CadastroCartsys.Core.DTOs;

namespace CadastroCartsys.Presentation.Interfaces.Cadastro.Clientes
{
    public interface IClientFormView
    {
        void PopularForm(ClientFormDto dto);
        void ClearFields();
        ClientFormDto GetForm();

        string CpfCnpj { get; set; }

        string Cep { get; set; }
        string Estado { get; set; }
        string Cidade { get; set; }
        string Endereco { get; set; }
        string Bairro { get; set; }

        ComboBox ComboState { get; }
        ComboBox ComboCity { get; }

        event EventHandler SearchCepEvent;
        event EventHandler LoadSearchClientEvent;
        event EventHandler SaveClientEvent;
        event EventHandler FilterCityEvent;
        event EventHandler DeleteClientEvent;
        event EventHandler FormatCpfCnpjEvent;

        void DisplaySuccessMessage(string message);
        void DisplayErrorMessage(string message);
        void DisplayAttentionMessage(string message);
        bool ShowConfirmation(string message, string title);
    }
}
