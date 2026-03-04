namespace CadastroCartsys.Presentation.Interfaces.Cadastro.Clientes
{
    public interface IClientView
    {
        DataGridViewColumnCollection Columns { get; }

        string SearchTerm { get; }
        string FieldResearch { get; }
        int? SelectedId { get; set; }

        event EventHandler SearchClientsEvent;
        event EventHandler FilterAlteredEvent;
        event EventHandler ClientSelectionEvent;

        void SetCustomerListBindingSource(BindingSource source);

        void FillFilterComboBox(DataGridViewColumnCollection Columns);

        void DisplayErrorMessage(string message);
    }
}
