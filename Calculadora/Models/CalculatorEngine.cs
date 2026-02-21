using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Calculadora.Models;

/// <summary>
/// Motor de cálculo que evalúa expresiones matemáticas complejas.
/// Soporta: operadores básicos (+, -, *, /), módulo (%), paréntesis,
/// y funciones especiales (√, ², !, π).
/// </summary>
public class CalculatorEngine
{
    /// <summary>
    /// Punto de entrada para evaluar una expresión matemática.
    /// Limpia la expresión y delega la evaluación al método principal.
    /// </summary>
    /// <param name="expression">Expresión a evaluar (ej: "5+3*2")</param>
    /// <returns>Resultado de la evaluación</returns>
    public double Evaluate(string expression)
    {
        // Eliminar espacios en blanco y convertir comas a puntos (para decimales)
        expression = expression.Replace(" ", "").Replace(",", ".");
        return EvaluateExpression(expression);
    }

    /// <summary>
    /// Método principal que evalúa la expresión respetando el orden de operaciones.
    /// Orden: 1) Paréntesis, 2) Funciones especiales, 3) *, /, % y 4) +, -
    /// </summary>
    private double EvaluateExpression(string expr)
    {
        // PASO 1: Resolver paréntesis de adentro hacia afuera
        // Busca el paréntesis de apertura más lejano y evalúa recursivamente su contenido
        while (expr.Contains("("))
        {
            int lastOpen = expr.LastIndexOf("(", StringComparison.Ordinal);
            int nextClose = expr.IndexOf(")", lastOpen, StringComparison.Ordinal);

            // Extrae la expresión dentro de los paréntesis
            string inner = expr.Substring(lastOpen + 1, nextClose - lastOpen - 1);
            // Evalúa recursivamente (para paréntesis anidados)
            double result = EvaluateExpression(inner);

            // Reemplaza los paréntesis y su contenido por el resultado
            expr = expr.Substring(0, lastOpen) + result.ToString(CultureInfo.InvariantCulture) + expr.Substring(nextClose + 1);
        }

        // PASO 2: Evaluar funciones especiales (√, ², !, π)
        expr = EvaluateFunctions(expr);

        // PASO 3: Convertir la expresión a tokens (números y operadores)
        // Ejemplo: "5+3*2" → ["5", "+", "3", "*", "2"]
        List<string> tokens = Tokenize(expr);

        // PASO 4: Evaluar operadores de alta precedencia (* / %)
        // Se procesan primero porque tienen mayor prioridad que + y -
        tokens = EvaluateOperators(tokens, new[] { "*", "/", "%" });

        // PASO 5: Evaluar operadores de baja precedencia (+ -)
        // Se procesan al final porque tienen menor prioridad
        tokens = EvaluateOperators(tokens, new[] { "+", "-" });

        // Al final, solo debe quedar un token: el resultado
        return double.Parse(tokens[0], CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Evalúa y reemplaza funciones especiales por sus valores numéricos.
    /// Reemplaza: π → valor de pi, √x → raíz cuadrada, x² → potencia 2, x! → factorial
    /// </summary>
    private string EvaluateFunctions(string expr)
    {
        // Reemplazar π por su valor numérico (3.14159...)
        expr = expr.Replace("π", Math.PI.ToString(CultureInfo.InvariantCulture));

        // Buscar patrones de raíz cuadrada: √(número) y calcular su valor
        // Ejemplo: √16 → 4
        expr = Regex.Replace(expr, @"√([\d.]+)", m => Math.Sqrt(double.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture));

        // Buscar patrones de cuadrado: número² y calcular su valor
        // Ejemplo: 5² → 25
        expr = Regex.Replace(expr, @"([\d.]+)²", m => Math.Pow(double.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture), 2).ToString(CultureInfo.InvariantCulture));

        // Buscar patrones de factorial: número! y calcular su valor
        // Ejemplo: 5! → 120
        expr = Regex.Replace(expr, @"([\d]+)!", m => Factorial(int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture));

        return expr;
    }

    /// <summary>
    /// Convierte una expresión en tokens (números y operadores separados).
    /// Esto facilita el procesamiento de operaciones.
    /// Ejemplo: "5+3*2" → ["5", "+", "3", "*", "2"]
    /// </summary>
    private List<string> Tokenize(string expr)
    {
        List<string> tokens = new();
        string current = "";

        // Iterar por cada carácter de la expresión
        for (int i = 0; i < expr.Length; i++)
        {
            char c = expr[i];

            // Si es un operador, guardar el número actual y el operador como tokens separados
            if (c == '+' || c == '-' || c == '*' || c == '/' || c == '%')
            {
                if (current.Length > 0)
                {
                    tokens.Add(current);  // Guardar el número
                    current = "";
                }
                tokens.Add(c.ToString());  // Guardar el operador
            }
            else
            {
                // Si no es operador, acumular el carácter al número actual
                current += c;
            }
        }

        // Guardar el último número (no terminó en operador)
        if (current.Length > 0)
            tokens.Add(current);

        return tokens;
    }

    /// <summary>
    /// Evalúa los operadores especificados en la lista de tokens.
    /// Recorre de izquierda a derecha aplicando la operación y reduciendo tokens.
    /// Ejemplo: ["5", "+", "3", "*", "2"] con ops ["*"] → ["5", "+", "6"]
    /// </summary>
    private List<string> EvaluateOperators(List<string> tokens, string[] operators)
    {
        // Para cada operador en la lista
        foreach (string op in operators)
        {
            // Iterar por los tokens (saltando de 2 en 2: número, operador, número)
            for (int i = 1; i < tokens.Count; i += 2)
            {
                // Si encontramos el operador que buscamos
                if (i < tokens.Count && tokens[i] == op)
                {
                    // Obtener los números a izquierda y derecha del operador
                    double left = double.Parse(tokens[i - 1], CultureInfo.InvariantCulture);
                    double right = double.Parse(tokens[i + 1], CultureInfo.InvariantCulture);

                    // Realizar la operación
                    double result = op switch
                    {
                        "+" => left + right,
                        "-" => left - right,
                        "*" => left * right,
                        "/" => right != 0 ? left / right : throw new DivideByZeroException("División entre cero"),
                        "%" => left % right,
                        _ => 0
                    };

                    // Reemplazar los 3 tokens (número, operador, número) por el resultado
                    tokens[i - 1] = result.ToString(CultureInfo.InvariantCulture);
                    tokens.RemoveRange(i, 2);  // Eliminar operador y número derecho
                    i -= 2;  // Retroceder para revisar nuevamente desde esta posición
                }
            }
        }

        return tokens;
    }

    /// <summary>
    /// Calcula el factorial de un número (n! = n × (n-1) × ... × 1).
    /// Ejemplo: 5! = 5 × 4 × 3 × 2 × 1 = 120
    /// </summary>
    private long Factorial(int n)
    {
        // Validar que no sea negativo
        if (n < 0) throw new ArgumentException("El factorial no es válido para números negativos");
        // Casos base: 0! = 1 y 1! = 1
        if (n == 0 || n == 1) return 1;
        // Recursión: n! = n × (n-1)!
        return n * Factorial(n - 1);
    }
}