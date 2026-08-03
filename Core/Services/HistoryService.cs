using System.Collections.Generic;
using Calculator.Core.Models;

namespace Calculator.Core.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly List<CalculationItem> _history = new();

        public IReadOnlyList<CalculationItem> History => _history.AsReadOnly();

        public void AddItem(string expression, string result)
        {
            if (string.IsNullOrWhiteSpace(expression) || string.IsNullOrWhiteSpace(result))
                return;

            _history.Insert(0, new CalculationItem(expression, result));
        }

        public void ClearHistory()
        {
            _history.Clear();
        }
    }
}
