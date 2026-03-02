using CadastroCartsys.Presentation.Interfaces;
using CadastroCartsys.Presentation.Views.Clients;

namespace CadastroCartsys.Presentation.Presenters
{
    public class MainPresenter
    {
        private IMainView _view = null!;
        private ClientFormView? _customerForm;

        private readonly Func<ClientFormView> _clientFactoryRegistration;

        public MainPresenter(Func<ClientFormView> clientFactoryRegistration)
        {
            _clientFactoryRegistration = clientFactoryRegistration;
        }

        public void SetView(IMainView view)
        {
            _view = view;
            AssociateEventHandlers();
        }

        private void AssociateEventHandlers()
        {
            _view.LoadCustomerReportFormEvent += LoadCustomerReportForm;
            _view.LoadClientRegistrationFormEvent += LoadClientRegistrationForm;
        }

        private void LoadClientRegistrationForm(object? sender, EventArgs e)
        {
            OpenClientFormView();
        }

        private void LoadCustomerReportForm(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OpenClientFormView()
        {
            if (_customerForm == null || _customerForm.IsDisposed)
            {
                _customerForm = _clientFactoryRegistration();
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
