using CadastroCartsys.Core.DTOs;
using CadastroCartsys.Data.Repositories.Interfaces;
using CadastroCartsys.Domain.Entities;
using CadastroCartsys.Presentation.Interfaces.Cadastro.Relatorios;
using FastReport;

namespace CadastroCartsys.Presentation.Presenters.Relatorios
{
    public class ReportPresenter
    {
        private readonly IClientRepository _clientRepository;
        private readonly IStateRepository _stateRepository;
        private readonly ICityRepository _cityRepository;
        private IReportView _view = null!;

        private List<Estado> _stateCache = [];
        private List<Cidade> _citiesCache = [];

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

        private void LoadRegion()
        {
            _stateCache = _stateRepository.GetAll().ToList();
            _citiesCache = _cityRepository.GetAll().ToList();

            _view.ComboState.DataSource = _stateCache;
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
                var filtro = ObterFiltro();
                if (!ValidarFiltro(filtro)) return;

                var dados = await _clientRepository.GetReportAsync(filtro);
                var lista = dados.ToList();

                if (!lista.Any())
                {
                    _view.DisplayErrorMessage("Nenhum cliente encontrado com os filtros informados.");
                    return;
                }

                ExibirRelatorio(lista, filtro);
            }
            catch (Exception ex)
            {
                _view.DisplayErrorMessage($"Erro ao gerar relatório: {ex.Message}");
            }
        }

        private ClientReportFilterDto ObterFiltro()
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

        private bool ValidarFiltro(ClientReportFilterDto filtro)
        {
            if (filtro.Todos) return true;

            if (filtro.IdInicial.HasValue && filtro.IdFinal.HasValue
                && filtro.IdInicial > filtro.IdFinal)
            {
                _view.DisplayErrorMessage("ID Inicial não pode ser maior que ID Final.");
                return false;
            }

            return true;
        }
        private void ExibirRelatorio(List<ClientReportDto> dados, ClientReportFilterDto filtro)
        {
            var reportPath = Path.Combine(AppContext.BaseDirectory, "Reports", "RelClientes.frx");

            var report = new FastReport.Report();

            report.Load(reportPath);

            report.RegisterData(dados.Take(100), "Clientes");
            report.GetDataSource("Clientes").Enabled = true;

            report.SetParameterValue("FiltroDescricao", ObterDescricaoFiltro(filtro));
            report.SetParameterValue("IdInicial", filtro.IdInicial?.ToString() ?? "Todos");
            report.SetParameterValue("IdFinal", filtro.IdFinal?.ToString() ?? "Todos");

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

        private string ObterDescricaoFiltro(ClientReportFilterDto filtro)
        {
            if (filtro.Todos) return "Todos os clientes";

            var partes = new List<string>();

            if (filtro.IdInicial.HasValue || filtro.IdFinal.HasValue)
                partes.Add($"ID: {filtro.IdInicial} até {filtro.IdFinal}");

            if (_view.ComboState.SelectedValue is int)
                partes.Add($"Estado: {_view.ComboState.Text}");

            if (_view.ComboCity.SelectedValue is int)
                partes.Add($"Cidade: {_view.ComboCity.Text}");

            return string.Join(" | ", partes);
        }
    }
}
