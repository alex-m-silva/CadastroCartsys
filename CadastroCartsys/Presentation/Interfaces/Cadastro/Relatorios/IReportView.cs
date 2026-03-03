using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CadastroCartsys.Presentation.Interfaces.Cadastro.Relatorios
{
    public interface IReportView
    {
        event EventHandler GenerateEventReport;
        event EventHandler FilterCityEvent;

        int? IdInitial { get; }
        int? IdEnd { get; }
        bool All { get; }

        ComboBox ComboState { get; }
        ComboBox ComboCity { get; }

        void DisplayErrorMessage(string message);
    }
}
