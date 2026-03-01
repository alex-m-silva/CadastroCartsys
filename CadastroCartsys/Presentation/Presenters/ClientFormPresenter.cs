using CadastroCartsys.Data.Repositories.Interfaces;
using CadastroCartsys.Infrastructure.ViaCep.Interfaces;
using CadastroCartsys.Presentation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroCartsys.Presentation.Presenters
{
    public class ClientFormPresenter
    {
        private readonly IClientRepository _clientRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IStateRepository _stateRepository;

        private readonly ICepService _cepService;
        private IClientFormView _view = null!;

        public ClientFormPresenter(
            IClientRepository clientRepository, 
            ICityRepository cityRepository, 
            IStateRepository stateRepository, 
            ICepService cepService)
        {
            _clientRepository = clientRepository;
            _cityRepository = cityRepository;
            _stateRepository = stateRepository;
            _cepService = cepService;
        }

        public void SetView(IClientFormView view)
        {
            _view = view;
            AssociateEventHandlers();
        }

        private void AssociateEventHandlers()
        {
            LoadCombos();
        }

        private async void BuscarCep(object? sender, EventArgs e)
        {
            try
            {
                var cep = _view.Cep;

                if (string.IsNullOrWhiteSpace(cep)) return;

                var resultado = await _cepService.GetCepAsync(cep);

                if (resultado is null)
                {
                    MessageBox.Show("CEP não encontrado.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _view.Endereco = resultado.Logradouro ?? string.Empty;
                _view.Bairro = resultado.Bairro ?? string.Empty;
                _view.Cidade = resultado.Localidade ?? string.Empty;
                _view.Estado = resultado.Uf ?? string.Empty;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "CEP Inválido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao consultar CEP. Verifique sua conexão.",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCombos()
        {
            try
            {
                 var estados = _stateRepository.GetAll().ToList();
                 var cidades = _cityRepository.GetAll().ToList();

                 _view.SetComboState(estados);
                 _view.SetComboCity(cidades);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar filtros: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
