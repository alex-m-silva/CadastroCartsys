using CadastroCartsys.Core.DTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CadastroCartsys.Presentation.Interfaces.Cadastro.Clientes
{
    public interface IClientFormView
    {
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

        void PopularForm(ClientFormDto dto);
        void ClearFields();
        ClientFormDto GetForm();

        ComboBox ComboState { get; }
        ComboBox ComboCity { get; }

        string Cep { get; set; }
        string CpfCnpj { get; set; }

        string Endereco { get; set; }
        string Bairro { get; set; }
        string Cidade { get; set; }
        string Estado { get; set; }
    }
}
