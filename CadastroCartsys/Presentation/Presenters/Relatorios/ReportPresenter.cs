using CadastroCartsys.Core.DTOs;
using CadastroCartsys.Data.Repositories.Interfaces;
using CadastroCartsys.Domain.Entities;
using CadastroCartsys.Presentation.Interfaces.Cadastro.Relatorios;

namespace CadastroCartsys.Presentation.Presenters.Relatorios
{
    public class ReportPresenter
    {
        private readonly IClientRepository _clientRepository;
        private readonly IStateRepository _stateRepository;
        private readonly ICityRepository _cityRepository;
        private IReportView _view = null!;

        private IEnumerable<Estado> _stateCache = [];
        private IEnumerable<Cidade> _citiesCache = [];

        public ReportPresenter(
            IClientRepository clientRepository,
            IStateRepository stateRepository,
            ICityRepository cityRepository
            )
        {
            _clientRepository = clientRepository;
            _stateRepository = stateRepository;
            _cityRepository = cityRepository;
        }

        public void SetView(IReportView view)
        {
            _view = view;
            AssociateEventHandlers();
            LoadRegion();
        }

        private void AssociateEventHandlers()
        {
            _view.GenerateEventReport += GenerateEvent;
            _view.FilterCityEvent += FilterCity;
        }

        private async void LoadRegion()
        {
            _stateCache = await _stateRepository.GetAllAsync();
            _citiesCache = await _cityRepository.GetAllAsync();

            _view.ComboState.DataSource = _stateCache.ToList();
            _view.ComboState.DisplayMember = nameof(Estado.Nome);
            _view.ComboState.ValueMember = nameof(Estado.Id);
            _view.ComboState.SelectedIndex = -1;
        }

        private void FilterCity(object? sender, EventArgs e)
        {
            if (_view.ComboState.SelectedValue is not int estadoId) return;

            var cities = _citiesCache
                .Where(c => c.EstadoId == estadoId)
                .ToList();

            _view.ComboCity.DataSource = cities;
            _view.ComboCity.DisplayMember = nameof(Cidade.Nome);
            _view.ComboCity.ValueMember = nameof(Cidade.Id);
            _view.ComboCity.SelectedIndex = -1;
        }

        private async void GenerateEvent(object? sender, EventArgs e)
        {
            try
            {
                var filter = GetFilter();
                if (!ValidateFilter(filter)) return;

                var dataClient = await _clientRepository.GetReportAsync(filter);
                var dataClientList = dataClient.ToList();

                if (!dataClientList.Any())
                {
                    _view.DisplayErrorMessage("Nenhum cliente encontrado com os filtros informados.");
                    return;
                }

                DisplayReport(dataClientList, filter);
            }
            catch (Exception ex)
            {
                _view.DisplayErrorMessage($"Erro ao gerar relatório: {ex.Message}");
            }
        }

        private ClientReportFilterDto GetFilter()
        {
            return new ClientReportFilterDto
            {
                Todos = _view.All,
                IdInicial = _view.IdInitial,
                IdFinal = _view.IdEnd,
                CidadeId = _view.ComboCity.SelectedValue as int?,
                EstadoId = _view.ComboState.SelectedValue as int?
            };
        }

        private bool ValidateFilter(ClientReportFilterDto filter)
        {
            if (filter.Todos) return true;

            if (filter.IdInicial.HasValue && filter.IdFinal.HasValue
                && filter.IdInicial > filter.IdFinal)
            {
                _view.DisplayErrorMessage("Código Inicial não pode ser maior que código Final.");
                return false;
            }

            return true;
        }
        private void DisplayReport(List<ClientReportDto> dataClient, ClientReportFilterDto filter)
        {
            var reportPath = Path.Combine(AppContext.BaseDirectory, "Reports", "RelClientes.frx");

            var report = new FastReport.Report();

            report.Load(reportPath);

            // 25Mil clientes não abriu o pdf Take para testes
            report.RegisterData(dataClient, "Clientes");
            report.GetDataSource("Clientes").Enabled = true;

            report.SetParameterValue("FiltroDescricao", GetDescriptionFilter(filter));
            report.SetParameterValue("IdInicial", filter.IdInicial?.ToString() ?? "Todos");
            report.SetParameterValue("IdFinal", filter.IdFinal?.ToString() ?? "Todos");

            report.Prepare();

            using var saveDialog = new SaveFileDialog
            {
                Filter = "PDF|*.pdf",
                FileName = $"Relatorio_Clientes_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                Title = "Salvar Relatório"
            };

            if (saveDialog.ShowDialog() != DialogResult.OK) return;

            var export = new FastReport.Export.PdfSimple.PDFSimpleExport();
            report.Export(export, saveDialog.FileName);

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = saveDialog.FileName,
                UseShellExecute = true
            });
        }

        private string GetDescriptionFilter(ClientReportFilterDto filter)
        {
            if (filter.Todos) return "Todos os clientes";

            var partes = new List<string>();

            if (filter.IdInicial.HasValue || filter.IdFinal.HasValue)
                partes.Add($"ID: {filter.IdInicial} até {filter.IdFinal}");

            if (_view.ComboState.SelectedValue is int)
                partes.Add($"Estado: {_view.ComboState.Text}");

            if (_view.ComboCity.SelectedValue is int)
                partes.Add($"Cidade: {_view.ComboCity.Text}");

            return string.Join(" | ", partes);
        }
    }
}
