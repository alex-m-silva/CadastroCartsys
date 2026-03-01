using CadastroCartsys.Core.DTOs;
using CadastroCartsys.Domain.Entities;
using CadastroCartsys.Presentation.Interfaces;
using CadastroCartsys.Presentation.Presenters;
using System.Data.Common;

namespace CadastroCartsys.Presentation.Views
{
    public partial class ClientView : Form, IClientView
    {
        private readonly ClientPresenter _presenter;

        public event EventHandler SearchClientsEvent;
        public event EventHandler FilterAlteredEvent;

        public ClientView(ClientPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;
            _presenter.SetView(this);
            AssociateEventsHandler();
        }

        public DataGridViewColumnCollection Columns => dtgvCustomers.Columns;

        public string SearchTerm => txtTerm.Text;
        public string FieldResearch => cbxFilter.SelectedItem is null
            ? string.Empty
            : ((dynamic)cbxFilter.SelectedItem).Value.ToString();

        private void AssociateEventsHandler()
        {

            txtTerm.TextChanged += delegate
            {
                SearchClientsEvent?.Invoke(this, EventArgs.Empty);
            };
            cbxFilter.SelectionChangeCommitted += delegate
            {
                if (!string.IsNullOrWhiteSpace(txtTerm.Text))
                    txtTerm.Text = string.Empty;

                FilterAlteredEvent?.Invoke(this, EventArgs.Empty);
            };
        }

        public void SetCustomerListBindingSource(BindingSource source)
        {
            FormatDatagrid(dtgvCustomers);
            dtgvCustomers.DataSource = source;
        }

        //public void SetComboState(IEnumerable<Estado> estados)
        //{
        //    cmbState.DataSource = estados.ToList();
        //    cmbState.DisplayMember = nameof(Estado.Nome);
        //    cmbState.ValueMember = nameof(Estado.Id);
        //    cmbState.SelectedIndex = -1; 
        //}

        //public void SetComboCity(IEnumerable<Cidade> cidades)
        //{
        //    cmbCity.DataSource = cidades.ToList();
        //    cmbCity.DisplayMember = nameof(Cidade.Nome);
        //    cmbCity.ValueMember = nameof(Cidade.Id);
        //    cmbCity.SelectedIndex = -1;
        //}

        private void FormatDatagrid(DataGridView gridView)
        {
            gridView.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            gridView.DefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Regular);
            gridView.DefaultCellStyle.Padding = new Padding(2);
        }

        // Sobrescreve Fun para enter = tab
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void FillFilterComboBox(DataGridViewColumnCollection columns)
        {
            cbxFilter.DisplayMember = "Display";
            cbxFilter.ValueMember = "Value";

            foreach (DataGridViewColumn column in columns)
            {
                if (column.Visible && column is DataGridViewTextBoxColumn)
                {
                    cbxFilter.Items.Add(new
                    {
                        Display = column.HeaderText,
                        Value = column.DataPropertyName
                    });
                }
            }

            if (cbxFilter.Items.Count > 0)
                cbxFilter.SelectedIndex = 0;
        }
    }
}
