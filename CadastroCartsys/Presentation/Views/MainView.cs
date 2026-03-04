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

        public event EventHandler LoadClientRegistrationFormEvent;
        public event EventHandler LoadClientReportFormEvent;
        private void AssociateEventsHandler()
        {
            sairToolStripMenuItem.Click += delegate
            {
                Close();
            };

            clienteToolStripMenuItem.Click += delegate
            {
                LoadClientRegistrationFormEvent?.Invoke(this, EventArgs.Empty);
            };

            relatorioToolStripMenuItem.Click += delegate
            {
                LoadClientReportFormEvent?.Invoke(this, EventArgs.Empty);
            };
        }
    }
}
