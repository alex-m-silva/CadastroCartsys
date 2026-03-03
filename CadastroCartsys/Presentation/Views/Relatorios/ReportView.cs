using CadastroCartsys.Presentation.Interfaces.Cadastro.Relatorios;
using CadastroCartsys.Presentation.Presenters.Relatorios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        }

        public int? IdInitial => int.TryParse(txtIdInicial.Text, out var i) ? i : null;

        public int? IdEnd => int.TryParse(txtIdFinal.Text, out var i) ? i : null;

        public bool All => chTodos.Checked;

        public ComboBox ComboState => cbxEstado;

        public ComboBox ComboCity => cbxCidade;

        public event EventHandler GenerateEventReport;
        public event EventHandler FilterCityEvent;

        private void ToggleFiltros(bool habilitado)
        {
            txtIdInicial.Enabled = habilitado;
            txtIdFinal.Enabled = habilitado;
            cbxEstado.Enabled = habilitado;
            cbxCidade.Enabled = habilitado;
        }

        private void AssociateEventsHandler()
        {
            btnGerar.Click += delegate
            {
                GenerateEventReport?.Invoke(this, EventArgs.Empty);
            };
            chTodos.CheckedChanged += delegate
            {
                ToggleFiltros(!chTodos.Checked);
            };

            cbxEstado.SelectionChangeCommitted += delegate
            {
                FilterCityEvent?.Invoke(this, EventArgs.Empty);
            };
        }

        public void DisplayErrorMessage(string message)
        {
            MessageBox.Show(message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
