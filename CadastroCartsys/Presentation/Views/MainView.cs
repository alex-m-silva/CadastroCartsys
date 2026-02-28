using CadastroCartsys.Presentation.Interfaces;
using CadastroCartsys.Presentation.Presenters;

namespace CadastroCartsys
{
    public partial class MainView : Form, IMainView
    {
        private readonly MainPresenter _presenter;

        public MainView(MainPresenter presenter)
        {
            InitializeComponent();
            AssociateEventsHandler();
            _presenter = presenter;
            _presenter.SetView(this);
        }

        public event EventHandler LoadCustomerRegistrationFormEvent;
        public event EventHandler LoadCustomerReportFormEvent;

        private void AssociateEventsHandler()
        {
            sairToolStripMenuItem.Click += delegate
            {
                Close();
            };

            clienteToolStripMenuItem.Click += delegate
            {
                LoadCustomerRegistrationFormEvent?.Invoke(this, EventArgs.Empty);
            };

            relatorioToolStripMenuItem.Click += delegate
            {
                LoadCustomerReportFormEvent?.Invoke(this, EventArgs.Empty);
            };
        }
    }
}
