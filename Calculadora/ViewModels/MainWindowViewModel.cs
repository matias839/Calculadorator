using System;
using Calculadora.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Calculadora.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly CalculatorEngine _calculator = new();

    [ObservableProperty]
    private string display = "0";

    private string _input = "";
    private bool _lastWasOperator;
    private bool _lastWasEquals;

    [RelayCommand]
    public void NumberInput(string number)
    {
        if (_lastWasEquals)
        {
            _input = "";
            _lastWasEquals = false;
        }

        _input += number;
        Display = _input.Length > 0 ? _input : "0";
        _lastWasOperator = false;
    }

    [RelayCommand]
    public void OperatorInput(string op)
    {
        if (string.IsNullOrEmpty(_input) || _lastWasOperator)
            return;

        _input += op;
        Display = _input;
        _lastWasOperator = true;
        _lastWasEquals = false;
    }

    [RelayCommand]
    public void SpecialFunction(string func)
    {
        if (_lastWasEquals)
        {
            _input = "";
            _lastWasEquals = false;
        }

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

        Display = _input;
        _lastWasOperator = false;
    }

    [RelayCommand]
    public void Parenthesis(string paren)
    {
        _input += paren;
        Display = _input;
        _lastWasOperator = false;
        _lastWasEquals = false;
    }

    [RelayCommand]
    public void Delete()
    {
        _input = "";
        Display = "0";
        _lastWasOperator = false;
        _lastWasEquals = false;
    }

    [RelayCommand]
    public void Calculate()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_input))
            {
                Display = "0";
                return;
            }

            double result = _calculator.Evaluate(_input);
            Display = result.ToString("F10").TrimEnd('0').TrimEnd('.');
            _input = result.ToString("F10");
            _lastWasEquals = true;
            _lastWasOperator = false;
        }
        catch
        {
            Display = "Error";
            _input = "";
            _lastWasEquals = false;
            _lastWasOperator = false;
        }
    }
}