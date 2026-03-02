using CadastroCartsys.Core.DTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CadastroCartsys.Presentation.Interfaces
{
    public interface IClientFormView
    {
        event EventHandler SearchCepEvent;
        event EventHandler LoadSearchClientEvent;
        event EventHandler SaveClientEvent;
        event EventHandler FilterCityEvent;

        void DisplaySuccessMessage(string message);
        void DisplayErrorMessage(string message);

        void PopularForm(ClientFormDto dto);  
        ClientFormDto GetForm();

        ComboBox ComboState { get; }
        ComboBox ComboCity { get; }

        string Cep { get; }
        string Endereco { get; set; }
        string Bairro { get; set; }
        string Cidade { get; set; }
        string Estado { get; set; }
    }
}
