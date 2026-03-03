using CadastroCartsys.Presentation.Interfaces;
using CadastroCartsys.Presentation.Views.Clients;
using CadastroCartsys.Presentation.Views.Relatorios;

namespace CadastroCartsys.Presentation.Presenters
{
    public class MainPresenter
    {
        private IMainView _view = null!;
        private ClientFormView? _customerForm;
        private ReportView? _clientReport;

        private readonly Func<ClientFormView> _clientFactoryRegistration;
        private readonly Func<ReportView> _clientReportFactory;

        public MainPresenter(
            Func<ClientFormView> clientFactoryRegistration,
            Func<ReportView> clientReportFactory)
        {
            _clientFactoryRegistration = clientFactoryRegistration;
            _clientReportFactory = clientReportFactory;
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
            OpenClientReportView();
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

        private void OpenClientReportView()
        {
            if (_clientReport == null || _clientReport.IsDisposed)
            {
                _clientReport = _clientReportFactory();
                _clientReport.FormClosed += (s, e) => _clientReport = null;
                _clientReport.Show();
            }
            else
            {
                _clientReport.BringToFront();
            }
        }
    }
}
