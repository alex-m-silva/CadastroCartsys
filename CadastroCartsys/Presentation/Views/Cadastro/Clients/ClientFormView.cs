using CadastroCartsys.Core.DTOs;
using CadastroCartsys.Presentation.Interfaces.Cadastro.Clientes;
using CadastroCartsys.Presentation.Presenters.Cadastro.Clientes;

namespace CadastroCartsys.Presentation.Views.Clients
{
    public partial class ClientFormView : Form, IClientFormView
    {
        private readonly ClientFormPresenter _presenter;

        public ClientFormView(ClientFormPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;
            _presenter.SetView(this);
            AssociateEventsHandler();
        }
        public ComboBox ComboState => cbxEstado;
        public ComboBox ComboCity => cbxCidade;

        public string Endereco
        {
            get => txtEndereco.Text;
            set => txtEndereco.Text = value;
        }
        public string Bairro
        {
            get => txtBairro.Text;
            set => txtBairro.Text = value;
        }
        public string Cidade
        {
            get => cbxCidade.Text;
            set => cbxCidade.Text = value;
        }
        public string Estado
        {
            get => cbxEstado.Text;
            set => cbxEstado.Text = value;
        }

        public string CpfCnpj
        {
            get => txtCpfCnpj.Text.Trim();
            set => txtCpfCnpj.Text = value;
        }
        public string Cep
        {
            get => txtCep.Text.Trim();
            set => txtCep.Text = value;
        }

        public event EventHandler SearchCepEvent;
        public event EventHandler LoadSearchClientEvent;
        public event EventHandler SaveClientEvent;
        public event EventHandler FilterCityEvent;
        public event EventHandler DeleteClientEvent;
        public event EventHandler FormatCpfCnpjEvent;

        private void AssociateEventsHandler()
        {
            btnSearch.Click += delegate
            {
                LoadSearchClientEvent?.Invoke(this, EventArgs.Empty);
            };

            btnClose.Click += delegate
            {
                this.Close();
            };

            txtCep.Leave += delegate
            {
                SearchCepEvent?.Invoke(this, EventArgs.Empty);
            };
            txtCep.KeyPress += (s, e) =>
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                    e.Handled = true;
            };
            txtCpfCnpj.KeyPress += (s, e) =>
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                    e.Handled = true;
            };
            txtCpfCnpj.Leave += delegate
            {
                FormatCpfCnpjEvent?.Invoke(this, EventArgs.Empty);
            };

            btnSave.Click += delegate
            {
                SaveClientEvent?.Invoke(this, EventArgs.Empty);
            };

            cbxEstado.SelectionChangeCommitted += delegate
            {
                FilterCityEvent?.Invoke(this, EventArgs.Empty);
            };

            btnCancelar.Click += delegate
            {
                ClearFields();
            };

            btnDelete.Click += delegate
            {
                DeleteClientEvent?.Invoke(this, EventArgs.Empty);
            };
        }

        public void PopularForm(ClientFormDto dto)
        {
            txtCodigo.Text = dto.Id.ToString();
            txtNome.Text = dto.Nome;
            txtCpfCnpj.Text = dto.CpfCnpj;
            txtCep.Text = dto.Cep ?? string.Empty;
            txtEndereco.Text = dto.Endereco ?? string.Empty;
            txtNumero.Text = dto.Numero ?? string.Empty;
            txtComplemento.Text = dto.Complemento ?? string.Empty;
            txtBairro.Text = dto.Bairro ?? string.Empty;
            cbxCidade.Text = dto.CidadeNome;
            cbxEstado.Text = dto.EstadoNome;
            dtpDataNascimento.Value = dto.DataNascimento ?? DateTime.Now.Date;
        }

        public void ClearFields()
        {
            txtCodigo.Clear();
            txtNome.Clear();
            txtCpfCnpj.Clear();
            txtCep.Clear();
            txtEndereco.Clear();
            txtNumero.Clear();
            txtComplemento.Clear();
            txtBairro.Clear();
            dtpDataNascimento.Value = DateTime.Today;

            cbxCidade.DataSource = null;
            cbxCidade.Items.Clear();
            cbxCidade.Text = string.Empty;
            cbxEstado.SelectedIndex = -1;
        }

        public ClientFormDto GetForm() => new()
        {
            Id = int.TryParse(txtCodigo.Text, out var id) ? id : 0,
            Nome = txtNome.Text.Trim(),
            CpfCnpj = txtCpfCnpj.Text.Trim(),
            Cep = txtCep.Text.Trim(),
            Endereco = txtEndereco.Text.Trim(),
            Numero = txtNumero.Text.Trim(),
            Complemento = txtComplemento.Text.Trim(),
            Bairro = txtBairro.Text.Trim(),
            CidadeNome = cbxCidade.Text.Trim(),
            CidadeId = cbxCidade.SelectedValue is int cidadeId ? cidadeId : 0,
            DataNascimento = dtpDataNascimento.Value
        };

        public void DisplaySuccessMessage(string message)
        {
            MessageBox.Show(message, "Sucesso",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void DisplayErrorMessage(string message)
        {
            MessageBox.Show(message, "Erro",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void DisplayAttentionMessage(string message)
        {
            MessageBox.Show(message, "Atenção",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public bool ShowConfirmation(string message, string title)
        {
            var result = MessageBox.Show(message, title,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            return result == DialogResult.Yes;
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
    }
}
