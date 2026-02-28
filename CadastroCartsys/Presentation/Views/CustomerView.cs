using CadastroCartsys.Core.DTOs;
using CadastroCartsys.Presentation.Interfaces;
using CadastroCartsys.Presentation.Presenters;

namespace CadastroCartsys.Presentation.Views
{
    public partial class CustomerView : Form, ICustomerView
    {
        private readonly CustomerPresenter _presenter;

        public event EventHandler SearchCustomersEvent;

        public CustomerFilterDto GetFilters() => new()
        {
            Id = txtCodigo.Text.Trim(),
            Nome = txtNome.Text.Trim(),
            CpfCnpj = txtCpfCnpj.Text.Trim(),
            Cep = txtCep.Text.Trim(),
            Cidade = txtCidade.Text.Trim(),
            Estado = txtEstado.Text.Trim(),
            DataNascimento = txtDataNascimento.Text.Trim()
        };

        public CustomerView(CustomerPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;
            _presenter.SetView(this);
            AssociateEventsHandler();
        }

        private void AssociateEventsHandler()
        {
            btnBuscar.Click += delegate
            {
                SearchCustomersEvent?.Invoke(this, EventArgs.Empty);
            };
            
        }

        public void SetCustomerListBindingSource(BindingSource source)
        {
            dtgvCustomers.DataSource = source;
        }
    }
}
