namespace Calculator.Core.Services
{
    public interface ICalculatorEngine
    {
        double ExecuteBinaryOperation(double left, double right, string op);
        double ExecuteUnaryOperation(double operand, string op);
        double EvaluateExpression(string expression);
        bool IsValidNumber(double value);
    }
}
