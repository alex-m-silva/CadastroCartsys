using CadastroCartsys.Presentation.Interfaces;
using CadastroCartsys.Presentation.Views.Clients;
using CadastroCartsys.Presentation.Views.Relatorios;

namespace CadastroCartsys.Presentation.Presenters
{
    public class MainPresenter
    {
        private readonly Func<ClientFormView> _clientFactoryRegistration;
        private readonly Func<ReportView> _clientReportFactory;
        private ClientFormView? _clientForm;
        private ReportView? _clientReport;

        private IMainView _view = null!;

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
            _view.LoadClientRegistrationFormEvent += LoadClientRegistrationForm;
            _view.LoadClientReportFormEvent += LoadClientReportForm;
        }

        private void LoadClientRegistrationForm(object? sender, EventArgs e)
        {
            OpenClientFormView();
        }

        private void LoadClientReportForm(object? sender, EventArgs e)
        {
            OpenClientReportView();
        }

        private void OpenClientFormView()
        {
            if (_clientForm == null || _clientForm.IsDisposed)
            {
                _clientForm = _clientFactoryRegistration();
                _clientForm.FormClosed += (s, e) => _clientForm = null;
                _clientForm.Show();
            }
            else
            {
                _clientForm.BringToFront();
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
