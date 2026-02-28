using CadastroCartsys.Presentation.Interfaces;
using CadastroCartsys.Presentation.Presenters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CadastroCartsys.Presentation.Views
{
    public partial class CustomerFormView : Form, ICustomerFormView
    {
        private readonly CustomerFormPresenter _presenter;

        public CustomerFormView(CustomerFormPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;
            _presenter.SetView(this);
        }

        public string Cep => throw new NotImplementedException();

        public string Endereco { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Bairro { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Cidade { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Estado { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event EventHandler SearchCepEvent;
    }
}
