using CadastroCartsys.Presentation.Interfaces.Cadastro.Relatorios;
using CadastroCartsys.Presentation.Presenters.Relatorios;

namespace CadastroCartsys.Presentation.Views.Relatorios
{
    public partial class ReportView : Form, IReportView
    {
        private readonly ReportPresenter _presenter;
        public ReportView(ReportPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;
            _presenter.SetView(this);
            AssociateEventsHandler();
        }

        public int? IdInitial
            => int.TryParse(txtIdInicial.Text, out var i) ? i : null;

        public int? IdEnd
            => int.TryParse(txtIdFinal.Text, out var i) ? i : null;

        public bool All
            => chTodos.Checked;

        public ComboBox ComboState
            => cbxEstado;

        public ComboBox ComboCity
            => cbxCidade;

        public event EventHandler GenerateEventReport;
        public event EventHandler FilterCityEvent;

        private void AssociateEventsHandler()
        {
            txtIdInicial.KeyPress += (s, e) =>
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                    e.Handled = true;
            };
            txtIdFinal.KeyPress += (s, e) =>
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                    e.Handled = true;
            };
            
            chTodos.CheckedChanged += delegate
            {
                ToggleFiltros(!chTodos.Checked);
            };

            cbxEstado.SelectionChangeCommitted += delegate
            {
                FilterCityEvent?.Invoke(this, EventArgs.Empty);
            };

            btnClear.Click += delegate
            {
                ClearFields();
            };
            btnGerar.Click += delegate
            {
                GenerateEventReport?.Invoke(this, EventArgs.Empty);
            };
        }

        private void ClearFields()
        {
            txtIdInicial.Clear();
            txtIdFinal.Clear();
            cbxEstado.Text = "";
            cbxCidade.Text = "";
            cbxEstado.SelectedIndex = -1;
            cbxCidade.SelectedIndex = -1;
            chTodos.Checked = false;
            ToggleFiltros(true);
        }
        private void ToggleFiltros(bool habilitado)
        {
            txtIdInicial.Enabled = habilitado;
            txtIdFinal.Enabled = habilitado;
            cbxEstado.Enabled = habilitado;
            cbxCidade.Enabled = habilitado;
        }

        public void DisplayErrorMessage(string message)
        {
            MessageBox.Show(message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
