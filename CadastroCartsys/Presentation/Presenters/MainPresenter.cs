using CadastroCartsys.Presentation.Interfaces;
using CadastroCartsys.Presentation.Views;

namespace CadastroCartsys.Presentation.Presenters
{
    public class MainPresenter
    {
        private IMainView _view = null!;

        private readonly Func<CustomerView> _cadastroClienteFactory;

        public MainPresenter( Func<CustomerView> cadastroClienteFactory)
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
            var formCustomerRegistration = _cadastroClienteFactory();
            formCustomerRegistration.ShowDialog();
        }

        private void LoadCustomerReportForm(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
