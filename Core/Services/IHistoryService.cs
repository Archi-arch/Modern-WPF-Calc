using System.Collections.Generic;
using Calculator.Core.Models;

namespace Calculator.Core.Services
{
    public interface IHistoryService
    {
        IReadOnlyList<CalculationItem> History { get; }
        void AddItem(string expression, string result);
        void ClearHistory();
    }
}
