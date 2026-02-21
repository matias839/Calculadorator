using System;
using Calculadora.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Calculadora.ViewModels;

/// <summary>
/// ViewModel de la ventana principal. Gestiona la lógica de la calculadora.
/// Se comunica con la UI a través de comandos (RelayCommand) y propiedades observables.
/// Utiliza el patrón MVVM con CommunityToolkit.Mvvm.
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    // Motor de cálculo que evalúa las expresiones matemáticas
    private readonly CalculatorEngine _calculator = new();

    /// <summary>
    /// Texto que se muestra en la pantalla de la calculadora.
    /// Se actualiza automáticamente en la UI al cambiar (propiedad observable).
    /// </summary>
    [ObservableProperty]
    private string display = "0";

    // Almacena la expresión que el usuario está escribiendo
    private string _input = "";
    
    // Indica si el último carácter insertado fue un operador
    // Se usa para evitar múltiples operadores consecutivos (ej: "5++3")
    private bool _lastWasOperator;
    
    // Indica si el usuario acaba de presionar "=" (calcular)
    // Si es true y se ingresa un número, se comienza una nueva operación
    private bool _lastWasEquals;

    /// <summary>
    /// Comando que se ejecuta cuando el usuario presiona un botón numérico (0-9) o decimal (,).
    /// Agrega el número a la entrada actual.
    /// </summary>
    [RelayCommand]
    public void NumberInput(string number)
    {
        // Si el último fue un "=", comenzar una nueva expresión
        if (_lastWasEquals)
        {
            _input = "";
            _lastWasEquals = false;
        }

        // Agregar el número a la entrada
        _input += number;
        // Actualizar la pantalla (mostrar lo que escribe el usuario)
        Display = _input.Length > 0 ? _input : "0";
        // Marcar que el último carácter no fue operador
        _lastWasOperator = false;
    }

    /// <summary>
    /// Comando que se ejecuta cuando el usuario presiona un operador (+, -, *, /, %).
    /// Agrega el operador a la entrada, pero evita múltiples operadores consecutivos.
    /// </summary>
    [RelayCommand]
    public void OperatorInput(string op)
    {
        // No permitir operadores si no hay entrada previa o si ya hay un operador
        if (string.IsNullOrEmpty(_input) || _lastWasOperator)
            return;

        // Agregar el operador a la entrada
        _input += op;
        // Actualizar la pantalla
        Display = _input;
        // Marcar que el último carácter fue un operador
        _lastWasOperator = true;
        _lastWasEquals = false;
    }

    /// <summary>
    /// Comando que se ejecuta cuando el usuario presiona un botón de función especial
    /// (√ raíz, ² cuadrado, ! factorial, π pi, MOD módulo).
    /// Agrega el símbolo especial a la entrada.
    /// </summary>
    [RelayCommand]
    public void SpecialFunction(string func)
    {
        // Si el último fue un "=", comenzar una nueva expresión
        if (_lastWasEquals)
        {
            _input = "";
            _lastWasEquals = false;
        }

        // Agregar el símbolo correspondiente según la función especial
        switch (func.ToLower())
        {
            case "sqrt":
                _input += "√";
                break;
            case "square":
                _input += "²";
                break;
            case "factorial":
                _input += "!";
                break;
            case "pi":
                _input += "π";
                break;
            case "mod":
                _input += "%";
                break;
        }

        // Actualizar la pantalla
        Display = _input;
        _lastWasOperator = false;
    }

    /// <summary>
    /// Comando que se ejecuta cuando el usuario presiona paréntesis ( o ).
    /// Agrega el paréntesis a la entrada para agrupar operaciones.
    /// </summary>
    [RelayCommand]
    public void Parenthesis(string paren)
    {
        // Agregar el paréntesis a la entrada
        _input += paren;
        // Actualizar la pantalla
        Display = _input;
        _lastWasOperator = false;
        _lastWasEquals = false;
    }

    /// <summary>
    /// Comando que se ejecuta cuando el usuario presiona el botón DEL (rojo).
    /// Borra completamente toda la entrada y reinicia la calculadora.
    /// </summary>
    [RelayCommand]
    public void Delete()
    {
        // Limpiar la entrada completamente
        _input = "";
        // Mostrar "0" en la pantalla
        Display = "0";
        // Reiniciar estados
        _lastWasOperator = false;
        _lastWasEquals = false;
    }

    /// <summary>
    /// Comando que se ejecuta cuando el usuario presiona el botón = (verde).
    /// Evalúa la expresión completa y muestra el resultado.
    /// Si hay error en la expresión, muestra "Error".
    /// </summary>
    [RelayCommand]
    public void Calculate()
    {
        try
        {
            // No calcular si la entrada está vacía
            if (string.IsNullOrWhiteSpace(_input))
            {
                Display = "0";
                return;
            }

            // Usar el motor de cálculo para evaluar la expresión
            double result = _calculator.Evaluate(_input);
            
            // Mostrar el resultado, eliminando ceros innecesarios al final
            // Ejemplo: 5.0000000000 → 5
            Display = result.ToString("F10").TrimEnd('0').TrimEnd('.');
            
            // Guardar el resultado como nueva entrada (por si el usuario quiere operar sobre él)
            _input = result.ToString("F10");
            
            // Marcar que se ejecutó "=" (el siguiente número comienza nueva expresión)
            _lastWasEquals = true;
            _lastWasOperator = false;
        }
        catch
        {
            // Si hay cualquier error (expresión inválida, división por cero, etc.)
            Display = "Error";
            _input = "";
            _lastWasEquals = false;
            _lastWasOperator = false;
        }
    }
}