using CadastroCartsys.Presentation.Interfaces;
using CadastroCartsys.Presentation.Views;

namespace CadastroCartsys.Presentation.Presenters
{
    public class MainPresenter
    {
        private IMainView _view = null!;
        private ClientView? _customerForm;

        private readonly Func<ClientView> _cadastroClienteFactory;

        public MainPresenter( Func<ClientView> cadastroClienteFactory)
        {
            _cadastroClienteFactory = cadastroClienteFactory;
        }

        public void SetView(IMainView view)
        {
            _view = view;                    
            AssociateEventHandlers();        
        }

        private void AssociateEventHandlers()
        {
            _view.LoadCustomerReportFormEvent += LoadCustomerReportForm;
            _view.LoadCustomerRegistrationFormEvent += LoadCustomerRegistrationForm;
        }

        private void LoadCustomerRegistrationForm(object? sender, EventArgs e)
        {
            AbrirClientes();
        }

        private void LoadCustomerReportForm(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }


        private void AbrirClientes()
        {
            if (_customerForm == null || _customerForm.IsDisposed)
            {
                _customerForm = _cadastroClienteFactory();
                _customerForm.FormClosed += (s, e) => _customerForm = null;
                _customerForm.Show();
            }
            else
            {
                _customerForm.BringToFront();
            }
        }
    }
}
