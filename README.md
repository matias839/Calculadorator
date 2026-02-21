# 🧮 Calculadora - Avalonia UI

Una calculadora moderna y funcional desarrollada con **C#** y **Avalonia UI**, implementando el patrón **MVVM** con la librería **CommunityToolkit.Mvvm**.

## 🎯 Características

### Operaciones Básicas
- ➕ Suma (`+`)
- ➖ Resta (`-`)
- ✖️ Multiplicación (`*`)
- ➗ División (`/`)
- 📊 Módulo (`%`)

### Funciones Especiales
- 🔲 Raíz cuadrada (`√`)
- ⬜ Elevar al cuadrado (`²`)
- ❗ Factorial (`!`)
- 🥧 Pi (`π`)

### Características Avanzadas
- 🔢 Números decimales (`,` como separador)
- 🔗 Paréntesis para agrupar operaciones `(` `)`
- 🗑️ Botón DEL para borrar completamente
- ✅ Botón = para calcular resultados
- 📱 Interfaz intuitiva con tema oscuro

## 🏗️ Arquitectura

### Estructura del Proyecto

```
Calculadora/
├── Models/
│   └── CalculatorEngine.cs       # Motor de cálculo
├── ViewModels/
│   ├── MainWindowViewModel.cs    # Lógica de la UI
│   └── ViewModelBase.cs          # Clase base
├── Views/
│   ├── MainWindow.axaml          # Interfaz XAML
│   └── MainWindow.axaml.cs       # Code-behind
└── App.axaml                     # Configuración de la app
```

### Patrón MVVM

El proyecto sigue el patrón **Model-View-ViewModel**:

- **Model**: `CalculatorEngine` - Contiene la lógica matemática
- **View**: `MainWindow.axaml` - Interfaz gráfica
- **ViewModel**: `MainWindowViewModel` - Intermediario entre View y Model

## 📚 Descripción de Componentes

### `CalculatorEngine.cs`

Motor de cálculo que evalúa expresiones matemáticas complejas.

**Métodos principales:**

1. **`Evaluate(string expression)`**
   - Punto de entrada público
   - Limpia la expresión (espacios, conversión de decimales)
   - Retorna el resultado numérico

2. **`EvaluateExpression(string expr)`** (privado)
   - Método recursivo principal
   - Respeta el orden de operaciones:
     1. Paréntesis (de adentro hacia afuera)
     2. Funciones especiales (√, ², !, π)
     3. Operadores de alta precedencia (*, /, %)
     4. Operadores de baja precedencia (+, -)

3. **`EvaluateFunctions(string expr)`** (privado)
   - Reemplaza símbolos especiales por sus valores numéricos
   - Usa expresiones regulares (Regex)
   - Ejemplos:
     - `π` → 3.14159265...
     - `√16` → 4
     - `5²` → 25
     - `5!` → 120

4. **`Tokenize(string expr)`** (privado)
   - Convierte una expresión en tokens
   - Separa números de operadores
   - Ejemplo: `"5+3*2"` → `["5", "+", "3", "*", "2"]`

5. **`EvaluateOperators(List<string> tokens, string[] operators)`** (privado)
   - Evalúa operadores en orden de precedencia
   - Reduce progresivamente los tokens
   - Reemplaza triplas (número, operador, número) por el resultado

6. **`Factorial(int n)`** (privado)
   - Calcula el factorial recursivamente
   - Validaciones para números negativos
   - Casos base: 0! = 1, 1! = 1

### `MainWindowViewModel.cs`

ViewModel que gestiona toda la lógica de interacción con la UI.

**Propiedades:**

- `Display` - Texto mostrado en pantalla (observable)
- `_input` - Expresión que escribe el usuario
- `_lastWasOperator` - Flag para evitar operadores consecutivos
- `_lastWasEquals` - Flag para detectar si se acababa de calcular

**Comandos (RelayCommand):**

1. **`NumberInput(string number)`**
   - Se ejecuta al presionar botones numéricos (0-9) o coma
   - Agrega el número a la entrada
   - Si es la primera entrada después de `=`, reinicia la expresión

2. **`OperatorInput(string op)`**
   - Se ejecuta al presionar `+`, `-`, `*`, `/`, `%`
   - Valida que haya entrada previa
   - Evita múltiples operadores consecutivos
   - Actualiza el flag `_lastWasOperator`

3. **`SpecialFunction(string func)`**
   - Maneja funciones especiales: `sqrt`, `square`, `factorial`, `pi`, `mod`
   - Convierte parámetros a minúsculas para flexibilidad
   - Agrega el símbolo correspondiente a la entrada

4. **`Parenthesis(string paren)`**
   - Agrega paréntesis `(` o `)` a la entrada
   - Permite agrupar operaciones

5. **`Delete()`**
   - **Borra toda la entrada completamente**
   - Reinicia pantalla a "0"
   - Resetea todos los flags de estado

6. **`Calculate()`**
   - Evalúa la expresión usando `CalculatorEngine`
   - Muestra el resultado eliminando ceros innecesarios
   - Maneja excepciones (división por cero, sintaxis inválida)
   - Marca que se ejecutó un cálculo para diferenciar el siguiente input

### `MainWindow.axaml`

Interfaz gráfica con estructura de grid.

**Estructura:**

- **Fila 0**: Pantalla (TextBlock vinculado a `Display`)
- **Fila 1**: DEL, (, ), MOD, π
- **Fila 2**: 7, 8, 9, ÷, √
- **Fila 3**: 4, 5, 6, *, ²
- **Fila 4**: 1, 2, 3, -, !
- **Fila 5**: 0, ,, %, +, =

**Vinculación de botones:**

Cada botón está vinculado a un comando del ViewModel mediante `Command` y `CommandParameter`:

```xml
<Button Content="7"
        Command="{Binding NumberInputCommand}" 
        CommandParameter="7"/>
```

## 🚀 Cómo Usar

### Requisitos

- **.NET 10.0** o superior
- **Avalonia UI** (incluido en las dependencias)
- **CommunityToolkit.Mvvm** (incluido en las dependencias)

### Instalación

```bash
# Clonar el repositorio
git clone https://github.com/matias839/Calculadora.git
cd Calculadora

# Restaurar dependencias
dotnet restore

# Compilar el proyecto
dotnet build

# Ejecutar la aplicación
dotnet run --project Calculadora/Calculadora.csproj
```

### Ejemplos de Uso

| Operación | Entrada | Resultado |
|-----------|---------|-----------|
| Suma | `5+3` | `8` |
| Multiplicación | `4*5` | `20` |
| Raíz cuadrada | `√16` | `4` |
| Factorial | `5!` | `120` |
| Cuadrado | `3²` | `9` |
| Paréntesis | `(2+3)*4` | `20` |
| Pi | `π*2` | `6.283...` |
| Módulo | `10%3` | `1` |
| Decimal | `3,5+2,5` | `6` |

## 🎨 Tema Visual

- **Colores**: Tema oscuro profesional
  - Fondo: `#FF2A2A2A` (gris muy oscuro)
  - Botones: `#FF2E2E2E` (gris oscuro)
  - Bordes: `#FF1E1E1E` (negro)
  - Texto: Blanco
  - Botón DEL: Rojo (`#FFFF2F2F`)
  - Botón =: Verde

- **Tipografía**: Arial/Default, FontSize 36 para pantalla

## 📋 Notas Técnicas

### Tokenización

El algoritmo utiliza **tokenización** en lugar de sustitución de strings directa:

1. Separa la expresión en números y operadores
2. Procesa operadores por precedencia
3. Reduce progresivamente hasta obtener un solo resultado

**Ventajas:**
- ✅ Manejo correcto de precedencia
- ✅ Evita errores con números negativos
- ✅ Más eficiente y legible

### Culturas (CultureInfo)

Se utiliza `CultureInfo.InvariantCulture` para:
- Evitar problemas con separadores decimales según locales
- Garantizar coherencia entre entrada y cálculo
- Permitir que funcione en cualquier región

### Manejo de Errores

- Valida expresiones incompletas
- Detecta división por cero
- Detecta factoriales de números negativos
- Muestra "Error" para fallos no esperados

## 🔧 Desarrollo Futuro

Posibles mejoras:

- [ ] Historial de cálculos
- [ ] Temas claros/oscuros intercambiables
- [ ] Soporte para más funciones (logaritmo, exponencial, trigonometría)
- [ ] Teclado numérico físico
- [ ] Copiar/pegar resultados
- [ ] Guardado de variables
- [ ] Conversión de unidades

## 📄 Licencia

Este proyecto está bajo licencia **MIT**. Puedes usarlo libremente en tus proyectos.

## 👤 Autor

Desarrollado por **Matías** como proyecto de aprendizaje en **C#** y **Avalonia UI**.

---

**Última actualización:** Febrero 2026

⭐ Si te ha sido útil, ¡dale una estrella en GitHub!

