using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Calculadora.Models;

public class CalculatorEngine
{
    public double Evaluate(string expression)
    {
        expression = expression.Replace(" ", "").Replace(",", ".");
        return EvaluateExpression(expression);
    }

    private double EvaluateExpression(string expr)
    {
        // Manejar paréntesis primero
        while (expr.Contains("("))
        {
            int lastOpen = expr.LastIndexOf("(", StringComparison.Ordinal);
            int nextClose = expr.IndexOf(")", lastOpen, StringComparison.Ordinal);

            string inner = expr.Substring(lastOpen + 1, nextClose - lastOpen - 1);
            double result = EvaluateExpression(inner);

            expr = expr.Substring(0, lastOpen) + result.ToString(CultureInfo.InvariantCulture) + expr.Substring(nextClose + 1);
        }

        // Evaluar funciones especiales ANTES de operadores
        expr = EvaluateFunctions(expr);

        // Tokenizar la expresión
        List<string> tokens = Tokenize(expr);

        // Evaluar multiplicación, división y módulo (precedencia alta)
        tokens = EvaluateOperators(tokens, new[] { "*", "/", "%" });

        // Evaluar suma y resta (precedencia baja)
        tokens = EvaluateOperators(tokens, new[] { "+", "-" });

        return double.Parse(tokens[0], CultureInfo.InvariantCulture);
    }

    private string EvaluateFunctions(string expr)
    {
        // π (pi) - reemplazar primero
        expr = expr.Replace("π", Math.PI.ToString(CultureInfo.InvariantCulture));

        // √ (raíz cuadrada)
        expr = Regex.Replace(expr, @"√([\d.]+)", m => Math.Sqrt(double.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture));

        // X² (cuadrado)
        expr = Regex.Replace(expr, @"([\d.]+)²", m => Math.Pow(double.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture), 2).ToString(CultureInfo.InvariantCulture));

        // X! (factorial)
        expr = Regex.Replace(expr, @"([\d]+)!", m => Factorial(int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture));

        return expr;
    }

    private List<string> Tokenize(string expr)
    {
        List<string> tokens = new();
        string current = "";

        for (int i = 0; i < expr.Length; i++)
        {
            char c = expr[i];

            if (c == '+' || c == '-' || c == '*' || c == '/' || c == '%')
            {
                if (current.Length > 0)
                {
                    tokens.Add(current);
                    current = "";
                }
                tokens.Add(c.ToString());
            }
            else
            {
                current += c;
            }
        }

        if (current.Length > 0)
            tokens.Add(current);

        return tokens;
    }

    private List<string> EvaluateOperators(List<string> tokens, string[] operators)
    {
        foreach (string op in operators)
        {
            for (int i = 1; i < tokens.Count; i += 2)
            {
                if (i < tokens.Count && tokens[i] == op)
                {
                    double left = double.Parse(tokens[i - 1], CultureInfo.InvariantCulture);
                    double right = double.Parse(tokens[i + 1], CultureInfo.InvariantCulture);

                    double result = op switch
                    {
                        "+" => left + right,
                        "-" => left - right,
                        "*" => left * right,
                        "/" => right != 0 ? left / right : throw new DivideByZeroException("División entre cero"),
                        "%" => left % right,
                        _ => 0
                    };

                    tokens[i - 1] = result.ToString(CultureInfo.InvariantCulture);
                    tokens.RemoveRange(i, 2);
                    i -= 2;
                }
            }
        }

        return tokens;
    }

    private long Factorial(int n)
    {
        if (n < 0) throw new ArgumentException("El factorial no es válido para números negativos");
        if (n == 0 || n == 1) return 1;
        return n * Factorial(n - 1);
    }
}