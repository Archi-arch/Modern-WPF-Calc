using System;
using System.Collections.Generic;

namespace Calculator.Core.Services
{
    public class CalculatorEngine : ICalculatorEngine
    {
        public double ExecuteBinaryOperation(double left, double right, string op)
        {
            return op switch
            {
                "+" => left + right,
                "-" => left - right,
                "×" or "*" => left * right,
                "÷" or "/" => right == 0 ? throw new DivideByZeroException("Cannot divide by zero") : left / right,
                "%" or "mod" => right == 0 ? throw new DivideByZeroException("Cannot divide by zero") : left % right,
                "^" or "x^y" => Math.Pow(left, right),
                _ => throw new InvalidOperationException($"Unknown operation: {op}")
            };
        }

        public double ExecuteUnaryOperation(double operand, string op)
        {
            return op switch
            {
                "x²" or "sqr" => operand * operand,
                "√" or "sqrt" => operand < 0 ? throw new ArgumentException("Invalid input for square root") : Math.Sqrt(operand),
                "1/x" => operand == 0 ? throw new DivideByZeroException("Cannot divide by zero") : 1.0 / operand,
                "±" => -operand,
                "%" => operand / 100.0,
                "sin" => Math.Sin(operand * Math.PI / 180.0),
                "cos" => Math.Cos(operand * Math.PI / 180.0),
                "tan" => Math.Tan(operand * Math.PI / 180.0),
                "ln" => operand <= 0 ? throw new ArgumentException("Invalid input for log") : Math.Log(operand),
                "log" => operand <= 0 ? throw new ArgumentException("Invalid input for log") : Math.Log10(operand),
                "n!" => CalculateFactorial(operand),
                _ => operand
            };
        }

        public double EvaluateExpression(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression)) return 0;
            
            // Clean up visual math symbols
            string cleanExpr = expression.Replace("×", "*").Replace("÷", "/");
            return ParseInfix(cleanExpr);
        }

        public bool IsValidNumber(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        private static double CalculateFactorial(double n)
        {
            if (n < 0 || n != Math.Floor(n) || n > 170)
                throw new ArgumentException("Invalid factorial argument");

            if (n == 0 || n == 1) return 1;

            double result = 1;
            for (int i = 2; i <= (int)n; i++)
            {
                result *= i;
            }
            return result;
        }

        private static double ParseInfix(string expression)
        {
            // Simple Shunting-Yard evaluator for infix expressions with +, -, *, /, ^
            List<string> tokens = Tokenize(expression);
            List<string> outputQueue = new();
            Stack<string> operatorStack = new();

            Dictionary<string, int> precedence = new()
            {
                { "+", 1 }, { "-", 1 },
                { "*", 2 }, { "/", 2 }, { "%", 2 },
                { "^", 3 }
            };

            foreach (var token in tokens)
            {
                if (double.TryParse(token, System.Globalization.CultureInfo.InvariantCulture, out _))
                {
                    outputQueue.Add(token);
                }
                else if (precedence.ContainsKey(token))
                {
                    while (operatorStack.Count > 0 && precedence.ContainsKey(operatorStack.Peek()) &&
                           precedence[operatorStack.Peek()] >= precedence[token])
                    {
                        outputQueue.Add(operatorStack.Pop());
                    }
                    operatorStack.Push(token);
                }
                else if (token == "(")
                {
                    operatorStack.Push(token);
                }
                else if (token == ")")
                {
                    while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
                    {
                        outputQueue.Add(operatorStack.Pop());
                    }
                    if (operatorStack.Count > 0 && operatorStack.Peek() == "(")
                    {
                        operatorStack.Pop();
                    }
                }
            }

            while (operatorStack.Count > 0)
            {
                outputQueue.Add(operatorStack.Pop());
            }

            // Evaluate RPN
            Stack<double> evalStack = new();
            foreach (var token in outputQueue)
            {
                if (double.TryParse(token, System.Globalization.CultureInfo.InvariantCulture, out double num))
                {
                    evalStack.Push(num);
                }
                else if (precedence.ContainsKey(token))
                {
                    if (evalStack.Count < 2) return 0;
                    double b = evalStack.Pop();
                    double a = evalStack.Pop();

                    double res = token switch
                    {
                        "+" => a + b,
                        "-" => a - b,
                        "*" => a * b,
                        "/" => b == 0 ? throw new DivideByZeroException("Cannot divide by zero") : a / b,
                        "%" => a % b,
                        "^" => Math.Pow(a, b),
                        _ => 0
                    };
                    evalStack.Push(res);
                }
            }

            return evalStack.Count > 0 ? evalStack.Pop() : 0;
        }

        private static List<string> Tokenize(string expr)
        {
            List<string> tokens = new();
            string currentNum = "";

            for (int i = 0; i < expr.Length; i++)
            {
                char c = expr[i];

                if (char.IsDigit(c) || c == '.' || (c == '-' && (i == 0 || expr[i - 1] == '(' || "+-*/%^".Contains(expr[i - 1]))))
                {
                    currentNum += c;
                }
                else
                {
                    if (!string.IsNullOrEmpty(currentNum))
                    {
                        tokens.Add(currentNum);
                        currentNum = "";
                    }

                    if ("+-*/%^()".Contains(c))
                    {
                        tokens.Add(c.ToString());
                    }
                }
            }

            if (!string.IsNullOrEmpty(currentNum))
            {
                tokens.Add(currentNum);
            }

            return tokens;
        }
    }
}
