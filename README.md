# Лабораторная работа №5  
## Построение AST и семантический анализ

---

## Название работы и ФИО автора

**Название:**  
Построение абстрактного синтаксического дерева (AST) и реализация семантического анализатора.

**Студент:** Головина А. С.  
**Группа:** АП-327  

---

## Вариант задания

Тема:  
Анализ конструкции объявления словаря с инициализацией.

**Пример корректной строки:**

```csharp
Dictionary<int, string> dict = new Dictionary<int, string> {
    { 1, "one" },
    { 2, "two" }
};
```

---

## Контекстно-зависимые условия

В программе реализованы следующие семантические проверки:

### 1. Уникальность идентификаторов

Идентификатор не должен объявляться повторно.

**Пример:**
```csharp
Dictionary<int, string> dict = new Dictionary<int, string> { {1, "one"} };
Dictionary<int, string> dict = new Dictionary<int, string> { {2, "two"} };
```

**Сообщение:**
```
Ошибка: идентификатор "dict" уже объявлен ранее
```

---

### 2. Совместимость типов

Ключ должен быть типа `int`, значение — `string`.

(В данной реализации обеспечивается синтаксическим анализом.)

---

### 3. Допустимые значения

Ключ должен быть неотрицательным.

**Пример:**
```csharp
Dictionary<int, string> dict = new Dictionary<int, string> { {-1, "one"} };
```

**Сообщение:**
```
Ошибка: ключ словаря -1 вне допустимого диапазона
```

---

### 4. Уникальность ключей

Ключи в словаре не должны повторяться.

**Пример:**
```csharp
Dictionary<int, string> dict = new Dictionary<int, string> {
    {1, "one"},
    {1, "two"}
};
```

**Сообщение:**
```
Ошибка: ключ 1 уже существует в словаре
```

---

## Структура AST

Используемые типы узлов:

- ProgramNode — корень программы  
- DictionaryDeclarationNode — объявление словаря  
- DictionaryTypeNode — тип словаря  
- IdentifierNode — имя переменной  
- DictionaryInitializerNode — инициализация  
- DictionaryElementNode — элемент словаря  
- NumberLiteralNode — числовой литерал  
- StringLiteralNode — строковый литерал  

---

## Пример AST

```text
ProgramNode
└── DictionaryDeclarationNode
    ├── DictionaryTypeNode
    │   ├── Name: Dictionary
    │   ├── KeyType: int
    │   └── ValueType: string
    ├── IdentifierNode
    │   └── name: "dict"
    └── DictionaryInitializerNode
        └── DictionaryElementNode
            ├── NumberLiteralNode
            │   └── Value: 1
            └── StringLiteralNode
                └── Value: "one"
```

---

## Рисунок CST / AST

В отчёте представлены:

- CST — полное синтаксическое дерево
  <img width="998" height="367" alt="image" src="https://github.com/user-attachments/assets/f23a6f2d-7822-4294-bc46-c3be461eeb58" />

- AST — абстрактное дерево 
<img width="930" height="268" alt="image" src="https://github.com/user-attachments/assets/b0d98a4b-a354-432e-b976-76dbbf5755af" />


---

## Формат вывода AST

### Текстовый вывод

- выводится после анализа
- имеет древовидный формат (`├──`, `└──`)
- содержит только корректную часть программы

---

### Графический вывод

Реализован с использованием:

- WPF
- Canvas
- пользовательской отрисовки

Особенности:

- каждый узел — прямоугольник
- используется розовая цветовая схема
- связи — линии между узлами
- отображаются:
  - тип узла
  - ключевые атрибуты

Графическое окно открывается по кнопке **"AST"**.

---

## Тестовые примеры

### 1. Корректный ввод

```csharp
Dictionary<int, string> dict = new Dictionary<int, string> { {1, "one"} };
```

Результат:
- ошибок нет
- AST строится
<img width="402" height="220" alt="image" src="https://github.com/user-attachments/assets/c4936e1f-4ba0-41f5-bd1f-6a00466a3ccf" />
<img width="1782" height="991" alt="image" src="https://github.com/user-attachments/assets/9b3d0eac-7a56-48b4-9aa6-4a5be1ef2556" />

---

### 2. Дублирующийся ключ

```csharp
Dictionary<int, string> dict = new Dictionary<int, string> {
    {1, "one"},
    {1, "two"}
};
```

Результат:
- ошибка семантики
- в AST остаётся только первый элемент
<img width="1106" height="247" alt="image" src="https://github.com/user-attachments/assets/57df9ff6-9eeb-4e2f-90f6-7fddfc8c69df" />
<img width="1741" height="994" alt="image" src="https://github.com/user-attachments/assets/c2965c02-3c10-444c-8204-79cbb3239bb8" />



---

### 3. Повторное объявление

```csharp
Dictionary<int, string> dict = new Dictionary<int, string> { {1, "one"} };
Dictionary<int, string> dict = new Dictionary<int, string> { {2, "two"} };
```

Результат:
- ошибка уникальности идентификатора
- в AST остаётся только первый словарь
<img width="1152" height="264" alt="image" src="https://github.com/user-attachments/assets/fb44aacc-bae2-4872-b63b-b47002d6b9b6" />
<img width="1774" height="987" alt="image" src="https://github.com/user-attachments/assets/9aa67787-17ed-4e09-8d8b-f26e4a52a6f0" />

---



## Инструкция по запуску

1. Открыть проект в Visual Studio  
2. Собрать проект (Build → Build Solution)  
3. Запустить (F5)  
4. Ввести код в редактор  
5. Нажать кнопку **Пуск**  
6. Для графики нажать **AST**

---

## Используемые технологии

- C#
- WPF
- Canvas
- AST
- Семантический анализ

---

## Вывод

В ходе лабораторной работы был реализован:

- построение AST
- семантический анализ

Программа позволяет:

- выявлять ошибки
- строить дерево программы
- визуализировать структуру

