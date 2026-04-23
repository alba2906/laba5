using Laba1.Ast;
using Laba1.Core;
using Laba1.Semantic;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Laba1.Windows
{
    public partial class Compiler : Window
    {
        private readonly LexicalAnalyzer _lexicalAnalyzer = new();
        private readonly SyntaxAnalyzer _syntaxAnalyzer = new();

        private ProgramNode? _lastAst;

        private string? _currentFilePath;
        private bool _isModified;
        private bool _suppressTextChanged;

        public Compiler()
        {
            InitializeComponent();
            UpdateTitle();
            ResetCounters();
        }

        private static string GetTokenTypeName(TokenType type)
        {
            return type switch
            {
                TokenType.KeywordNew => "ключевое слово new",
                TokenType.KeywordInt => "ключевое слово int",
                TokenType.KeywordString => "ключевое слово string",
                TokenType.Identifier => "идентификатор",
                TokenType.Whitespace => "пробел",
                TokenType.Assign => "оператор присваивания",
                TokenType.StringLiteral => "строковый литерал",
                TokenType.UnsignedInteger => "целое число",
                TokenType.LessThan => "символ <",
                TokenType.GreaterThan => "символ >",
                TokenType.Comma => "символ ,",
                TokenType.OpenBrace => "символ {",
                TokenType.CloseBrace => "символ }",
                TokenType.Semicolon => "символ ;",
                TokenType.Error => "ошибка",
                TokenType.UnterminatedStringLiteral => "незакрытый строковый литерал",
                _ => type.ToString()
            };
        }

        private void ResetCounters()
        {
            LexicalCountText.Text = "Лексем: 0";
            SyntaxCountText.Text = "Ошибок: 0";
            TotalCountText.Text = "Общее количество ошибок: 0";
        }

        private void UpdateTitle()
        {
            string fileName = string.IsNullOrWhiteSpace(_currentFilePath)
                ? "Безымянный"
                : Path.GetFileName(_currentFilePath);

            Title = _isModified
                ? $"Компилятор - {fileName}*"
                : $"Компилятор - {fileName}";
        }

        private string GetEditorText()
        {
            TextRange textRange = new(
                FileContentViewer.Document.ContentStart,
                FileContentViewer.Document.ContentEnd);

            return textRange.Text;
        }

        private void SetEditorText(string text)
        {
            _suppressTextChanged = true;
            try
            {
                FileContentViewer.Document.Blocks.Clear();
                FileContentViewer.Document.Blocks.Add(new Paragraph(new Run(text)));
            }
            finally
            {
                _suppressTextChanged = false;
            }
        }

        private void ClearOutputs()
        {
            TokensDataGrid.ItemsSource = null;
            SyntaxErrorsDataGrid.ItemsSource = null;
            _lastAst = null;
            ResetCounters();
        }

        private bool ConfirmSaveIfNeeded()
        {
            if (!_isModified)
                return true;

            MessageBoxResult result = MessageBox.Show(
                "Сохранить изменения?",
                "Подтверждение",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Cancel)
                return false;

            if (result == MessageBoxResult.Yes)
                return SaveCurrentFile();

            return true;
        }

        private bool SaveCurrentFile()
        {
            if (string.IsNullOrWhiteSpace(_currentFilePath))
                return SaveCurrentFileAs();

            File.WriteAllText(_currentFilePath, GetEditorText());
            _isModified = false;
            UpdateTitle();
            return true;
        }

        private bool SaveCurrentFileAs()
        {
            SaveFileDialog dialog = new()
            {
                Filter = "Text files (*.txt)|*.txt|POO files (*.poo)|*.poo|All files (*.*)|*.*",
                DefaultExt = ".txt"
            };

            if (dialog.ShowDialog() != true)
                return false;

            _currentFilePath = dialog.FileName;
            File.WriteAllText(_currentFilePath, GetEditorText());
            _isModified = false;
            UpdateTitle();
            return true;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (!ConfirmSaveIfNeeded())
                return;

            _suppressTextChanged = true;
            try
            {
                FileContentViewer.Document.Blocks.Clear();
            }
            finally
            {
                _suppressTextChanged = false;
            }

            _currentFilePath = null;
            _isModified = false;
            _lastAst = null;
            ClearOutputs();
            UpdateTitle();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (!ConfirmSaveIfNeeded())
                return;

            OpenFileDialog dialog = new()
            {
                Filter = "Text files (*.txt)|*.txt|POO files (*.poo)|*.poo|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() != true)
                return;

            _currentFilePath = dialog.FileName;
            SetEditorText(File.ReadAllText(_currentFilePath));
            _isModified = false;
            _lastAst = null;
            ClearOutputs();
            UpdateTitle();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentFile();
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentFileAs();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (!ConfirmSaveIfNeeded())
                return;

            Close();
        }

        private void Reference_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "ЛР5: построение AST, семантический анализ и графическая визуализация для конструкции Dictionary<int, string>.",
                "Справка",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new()
            {
                Owner = this
            };
            aboutWindow.ShowDialog();
        }

        private void RunAnalysis_Click(object sender, RoutedEventArgs e)
        {
            string sourceText = GetEditorText().Trim();

            TokensDataGrid.ItemsSource = null;
            SyntaxErrorsDataGrid.ItemsSource = null;
            _lastAst = null;

            if (string.IsNullOrWhiteSpace(sourceText))
            {
                ClearOutputs();

                MessageBox.Show(
                    "Введите объявление словаря.",
                    "Результат анализа",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            List<TokenRow> lexicalRows = new();
            List<SyntaxError> syntaxRows = new();

            AnalysisResult lexicalResult = _lexicalAnalyzer.Analyze(sourceText);

            if (lexicalResult.HasErrors)
            {
                int number = 1;
                foreach (LexicalError error in lexicalResult.Errors)
                {
                    lexicalRows.Add(new TokenRow
                    {
                        Number = number++,
                        Lexeme = error.LexemeRepresentation,
                        TokenType = "лексическая ошибка",
                        Description = error.Message,
                        Line = error.Line,
                        Column = error.Column
                    });
                }

                TokensDataGrid.ItemsSource = lexicalRows;
                LexicalCountText.Text = $"Ошибок: {lexicalResult.Errors.Count}";
            }
            else
            {
                int tokenNumber = 1;
                foreach (Token token in lexicalResult.Tokens)
                {
                    if (token.Type == TokenType.Whitespace)
                        continue;

                    lexicalRows.Add(new TokenRow
                    {
                        Number = tokenNumber++,
                        Lexeme = token.Lexeme,
                        TokenType = GetTokenTypeName(token.Type),
                        Description = string.Empty,
                        Line = token.Line,
                        Column = token.StartColumn
                    });
                }

                TokensDataGrid.ItemsSource = lexicalRows;
                LexicalCountText.Text = $"Лексем: {lexicalRows.Count}";
            }

            SyntaxAnalysisResult syntaxResult = _syntaxAnalyzer.Analyze(lexicalResult.Tokens);

            if (syntaxResult.HasMessage)
            {
                SyntaxErrorsDataGrid.ItemsSource = null;
                SyntaxCountText.Text = "Ошибок: 0";
                TotalCountText.Text = $"Общее количество ошибок: {lexicalResult.Errors.Count}";
                return;
            }

            foreach (SyntaxError error in syntaxResult.Errors)
            {
                syntaxRows.Add(error);
            }

            SyntaxErrorsDataGrid.ItemsSource = syntaxRows;
            SyntaxCountText.Text = $"Ошибок: {syntaxRows.Count}";
            TotalCountText.Text = $"Общее количество ошибок: {lexicalResult.Errors.Count + syntaxRows.Count}";
            if (!lexicalResult.HasErrors && syntaxRows.Count == 0)
            {
                if (TryBuildAstFromTokens(lexicalResult.Tokens, out ProgramNode? programNode))
                {
                    SemanticAnalyzer semanticAnalyzer = new();
                    semanticAnalyzer.Analyze(programNode);

                    if (semanticAnalyzer.Errors.Count > 0)
                    {
                        RemoveDuplicateDeclarationsFromAst(programNode);
                        RemoveDuplicateKeysFromAst(programNode);

                        List<SemanticErrorRow> rows = new();

                        foreach (var err in semanticAnalyzer.Errors)
                        {
                            rows.Add(new SemanticErrorRow
                            {
                                Message = err.Message,
                                Line = err.Line,
                                Column = err.Column
                            });
                        }

                        SemanticErrorsWindow semanticWindow = new(rows)
                        {
                            Owner = this
                        };

                        semanticWindow.ShowDialog();
                    }

                    _lastAst = programNode;

                    MessageBox.Show(
                        _lastAst.Print(),
                        "AST (текстовое представление)",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(
                        "Синтаксический анализ прошёл успешно, но AST построить не удалось.",
                        "AST",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
        }

        private void ShowAst_Click(object sender, RoutedEventArgs e)
        {
            if (_lastAst == null)
            {
                MessageBox.Show(
                    "Сначала выполните корректный анализ без лексических и синтаксических ошибок.",
                    "AST",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            AstCanvasWindow astWindow = new(_lastAst)
            {
                Owner = this
            };
            astWindow.ShowDialog();
        }

        private bool TryBuildAstFromTokens(IReadOnlyList<Token> tokens, out ProgramNode? programNode)
        {
            programNode = null;

            List<Token> source = tokens
                .Where(t => t.Type != TokenType.Whitespace)
                .ToList();

            int index = 0;
            ProgramNode program = new();

            while (index < source.Count)
            {
                DictionaryDeclarationNode? declaration = ParseDictionaryDeclaration(source, ref index);
                if (declaration == null)
                    return false;

                program.Declarations.Add(declaration);
            }

            programNode = program;
            return true;
        }

        private DictionaryDeclarationNode? ParseDictionaryDeclaration(List<Token> tokens, ref int index)
        {
            DictionaryTypeNode? declaredType = ParseDictionaryType(tokens, ref index);
            if (declaredType == null)
                return null;

            if (!TryConsume(tokens, ref index, TokenType.Identifier, out Token identifierToken))
                return null;

            if (!TryConsume(tokens, ref index, TokenType.Assign, out _))
                return null;

            if (!TryConsume(tokens, ref index, TokenType.KeywordNew, out _))
                return null;

            DictionaryTypeNode? createdType = ParseDictionaryType(tokens, ref index);
            if (createdType == null)
                return null;

            DictionaryInitializerNode? initializer = ParseDictionaryInitializer(tokens, ref index);
            if (initializer == null)
                return null;

            if (!TryConsume(tokens, ref index, TokenType.Semicolon, out _))
                return null;

            return new DictionaryDeclarationNode
            {
                Type = declaredType,
                Identifier = new IdentifierNode
                {
                    Name = identifierToken.Lexeme,
                    Line = identifierToken.Line,
                    Column = identifierToken.StartColumn
                },
                Initializer = initializer
            };
        }

        private DictionaryTypeNode? ParseDictionaryType(List<Token> tokens, ref int index)
        {
            if (!TryConsumeDictionaryWord(tokens, ref index))
                return null;

            if (!TryConsume(tokens, ref index, TokenType.LessThan, out _))
                return null;

            if (!TryConsume(tokens, ref index, TokenType.KeywordInt, out _))
                return null;

            if (!TryConsume(tokens, ref index, TokenType.Comma, out _))
                return null;

            if (!TryConsume(tokens, ref index, TokenType.KeywordString, out _))
                return null;

            if (!TryConsume(tokens, ref index, TokenType.GreaterThan, out _))
                return null;

            return new DictionaryTypeNode
            {
                Name = "Dictionary",
                KeyType = "int",
                ValueType = "string"
            };
        }

        private DictionaryInitializerNode? ParseDictionaryInitializer(List<Token> tokens, ref int index)
        {
            if (!TryConsume(tokens, ref index, TokenType.OpenBrace, out _))
                return null;

            DictionaryInitializerNode initializer = new();

            DictionaryElementNode? first = ParseDictionaryElement(tokens, ref index);
            if (first == null)
                return null;

            initializer.Elements.Add(first);

            while (index < tokens.Count && tokens[index].Type == TokenType.Comma)
            {
                index++;

                DictionaryElementNode? next = ParseDictionaryElement(tokens, ref index);
                if (next == null)
                    return null;

                initializer.Elements.Add(next);
            }

            if (!TryConsume(tokens, ref index, TokenType.CloseBrace, out _))
                return null;

            return initializer;
        }

        private DictionaryElementNode? ParseDictionaryElement(List<Token> tokens, ref int index)
        {
            if (!TryConsume(tokens, ref index, TokenType.OpenBrace, out _))
                return null;

            if (!TryConsume(tokens, ref index, TokenType.UnsignedInteger, out Token numberToken))
                return null;

            if (!TryConsume(tokens, ref index, TokenType.Comma, out _))
                return null;

            if (!TryConsume(tokens, ref index, TokenType.StringLiteral, out Token stringToken))
                return null;

            if (!TryConsume(tokens, ref index, TokenType.CloseBrace, out _))
                return null;

            if (!int.TryParse(numberToken.Lexeme, out int numberValue))
                return null;

            return new DictionaryElementNode
            {
                Key = new NumberLiteralNode
                {
                    Value = numberValue,
                    Line = numberToken.Line,
                    Column = numberToken.StartColumn
                },
                Value = new StringLiteralNode
                {
                    Value = TrimQuotes(stringToken.Lexeme),
                    Line = stringToken.Line,
                    Column = stringToken.StartColumn
                }
            };
        }

        private bool TryConsume(List<Token> tokens, ref int index, TokenType expectedType, out Token token)
        {
            token = default!;

            if (index >= tokens.Count)
                return false;

            if (tokens[index].Type != expectedType)
                return false;

            token = tokens[index];
            index++;
            return true;
        }

        private bool TryConsumeDictionaryWord(List<Token> tokens, ref int index)
        {
            if (index >= tokens.Count)
                return false;

            Token token = tokens[index];

            if (token.Type == TokenType.Identifier && token.Lexeme == "Dictionary")
            {
                index++;
                return true;
            }

            return false;
        }

        private void RemoveDuplicateDeclarationsFromAst(ProgramNode program)
        {
            HashSet<string> seenNames = new();
            List<DictionaryDeclarationNode> uniqueDeclarations = new();

            foreach (var decl in program.Declarations)
            {
                if (seenNames.Add(decl.Identifier.Name))
                {
                    uniqueDeclarations.Add(decl);
                }
            }

            program.Declarations.Clear();

            foreach (var decl in uniqueDeclarations)
            {
                program.Declarations.Add(decl);
            }
        }

        private void RemoveDuplicateKeysFromAst(ProgramNode program)
        {
            foreach (var decl in program.Declarations)
            {
                HashSet<int> seenKeys = new();
                List<DictionaryElementNode> uniqueElements = new();

                foreach (var element in decl.Initializer.Elements)
                {
                    if (seenKeys.Add(element.Key.Value))
                    {
                        uniqueElements.Add(element);
                    }
                }

                decl.Initializer.Elements.Clear();

                foreach (var element in uniqueElements)
                {
                    decl.Initializer.Elements.Add(element);
                }
            }
        }

        private static string TrimQuotes(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            if (value.Length >= 2 && value[0] == '"' && value[^1] == '"')
                return value.Substring(1, value.Length - 2);

            return value;
        }

        private void SyntaxErrorsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SyntaxErrorsDataGrid.SelectedItem is not SyntaxError error)
                return;

            TextPointer? pointer = TextPositionHelper.GetTextPointerAt(
                FileContentViewer,
                error.Line,
                error.Column);

            if (pointer == null)
                return;

            FileContentViewer.Focus();
            FileContentViewer.CaretPosition = pointer;
            FileContentViewer.Selection.Select(pointer, pointer);
        }

        private void FileContentViewer_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_suppressTextChanged)
                return;

            _isModified = true;
            UpdateTitle();
        }
    }
}