using System;

namespace Calculator.Core.Models
{
    public class CalculationItem
    {
        public string Expression { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public string FormattedTime => Timestamp.ToString("HH:mm:ss");

        public CalculationItem() { }

        public CalculationItem(string expression, string result)
        {
            Expression = expression;
            Result = result;
            Timestamp = DateTime.Now;
        }
    }
}
