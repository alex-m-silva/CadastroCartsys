using CadastroCartsys.Core.DTOs;
using CadastroCartsys.Domain.Entities;
using CadastroCartsys.Presentation.Interfaces.Cadastro.Clientes;
using CadastroCartsys.Presentation.Presenters.Cadastro.Clientes;
using System.Data.Common;

namespace CadastroCartsys.Presentation.Views
{
    public partial class ClientView : Form, IClientView
    {
        private readonly ClientPresenter _presenter;

        public event EventHandler SearchClientsEvent;
        public event EventHandler FilterAlteredEvent;
        public event EventHandler ClientSelectionEvent;

        public ClientView(ClientPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;
            _presenter.SetView(this);
            AssociateEventsHandler();
        }

        public DataGridViewColumnCollection Columns => dtgvCustomers.Columns;

        public int? SelectedId { get; set; }

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
                if (string.IsNullOrWhiteSpace(txtTerm.Text)) return;

                FilterAlteredEvent?.Invoke(this, EventArgs.Empty);
            };

            dtgvCustomers.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex < 0)
                    return;

                if (dtgvCustomers.CurrentRow?.DataBoundItem is not ClientFormDto cliente)
                    return;

                SelectedId = (int)dtgvCustomers.Rows[e.RowIndex].Cells["Column1"].Value;
                if (SelectedId == null) return;
                ClientSelectionEvent?.Invoke(this, EventArgs.Empty);
                this.Close();
            };

            btnClose.Click += delegate
            {
                this.Close();
            };
        }

        public void SetCustomerListBindingSource(BindingSource source)
        {
            FormatDatagrid(dtgvCustomers);
            dtgvCustomers.DataSource = source;
        }

        private void FormatDatagrid(DataGridView gridView)
        {
            gridView.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            gridView.DefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Regular);
            gridView.DefaultCellStyle.Padding = new Padding(5);
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

        public void DisplayErrorMessage(string message)
        {
            MessageBox.Show(message, "Erro",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
