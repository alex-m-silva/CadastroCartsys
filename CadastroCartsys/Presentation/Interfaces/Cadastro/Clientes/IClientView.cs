namespace CadastroCartsys.Presentation.Interfaces.Cadastro.Clientes
{
    public interface IClientView
    {
        event EventHandler SearchClientsEvent;
        event EventHandler FilterAlteredEvent;
        event EventHandler ClientSelectionEvent;

        void SetCustomerListBindingSource(BindingSource source);
        void FillFilterComboBox(DataGridViewColumnCollection Columns);

        DataGridViewColumnCollection Columns { get; }

        string SearchTerm { get; }
        string FieldResearch { get; }
        int? SelectedId { get; set; }
    }
}
