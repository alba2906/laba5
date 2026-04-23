using System.Collections.Generic;
using System.Linq;

namespace Laba1.Core
{
    public sealed class SyntaxAnalyzer
    {
        private List<Token> _tokens = new();
        private int _position;
        private SyntaxAnalysisResult _result = new();

        private readonly HashSet<string> _errorKeys = new();

        public SyntaxAnalysisResult Analyze(IEnumerable<Token> tokens)
        {
            _tokens = tokens
                .Where(t => t.Type != TokenType.Whitespace)
                .ToList();

            _position = 0;
            _result = new SyntaxAnalysisResult();
            _errorKeys.Clear();

            if (_tokens.Count == 0)
            {
                _result.UserMessage = "Введите объявление словаря.";
                return _result;
            }

            ParseDictionaryDeclaration();

            if (!IsAtEnd())
            {
                bool duplicateTrailingError = false;

                if (_result.Errors.Count > 0)
                {
                    SyntaxError lastError = _result.Errors[^1];
                    Token current = Current();

                    duplicateTrailingError =
                        lastError.Line == current.Line &&
                        lastError.Column == current.StartColumn;
                }

                if (!duplicateTrailingError)
                {
                    AddError(Current(), $"Лишний фрагмент после конца конструкции: '{Current().Lexeme}'.");
                }
            }

            return _result;
        }

        private void ParseDictionaryDeclaration()
        {
            if (!ExpectDictionaryWordInsertable()) return;

            // Dictionary<int, string>
            ExpectInsertable(TokenType.LessThan, "Ожидался символ '<'.");
            ExpectInsertable(TokenType.KeywordInt, "Ожидалось ключевое слово 'int'.");
            ExpectInsertable(TokenType.Comma, "Ожидался символ ','.");
            ExpectInsertable(TokenType.KeywordString, "Ожидалось ключевое слово 'string'.");
            ExpectInsertable(TokenType.GreaterThan, "Ожидался символ '>'.");

            // имя переменной
            ExpectIdentifierInsertable();

            // =
            ExpectMissingOnly(TokenType.Assign, "Ожидался символ '='.");

            // new
            ExpectMissingKeywordNew();

            // Dictionary<int, string>
            if (!ExpectDictionaryWordInsertable()) return;

            ExpectInsertable(TokenType.LessThan, "Ожидался символ '<'.");
            ExpectInsertable(TokenType.KeywordInt, "Ожидалось ключевое слово 'int'.");
            ExpectInsertable(TokenType.Comma, "Ожидался символ ','.");
            ExpectInsertable(TokenType.KeywordString, "Ожидалось ключевое слово 'string'.");
            ExpectInsertable(TokenType.GreaterThan, "Ожидался символ '>'.");

            // начало инициализации
            bool missingInitializerBrace =
                !IsAtEnd() &&
                Current().Type == TokenType.OpenBrace &&
                LookAhead(TokenType.UnsignedInteger, 1);

            if (missingInitializerBrace)
            {
                AddError(Current(), "Ожидался символ '{' в начале инициализации.");
            }
            else
            {
                if (!ExpectInsertable(TokenType.OpenBrace, "Ожидался символ '{' в начале инициализации."))
                    return;
            }

            if (Check(TokenType.CloseBrace))
            {
                AddError(Current(), "Словарь должен содержать хотя бы один элемент.");
                _position++;
                ExpectMissingOnly(TokenType.Semicolon, "Ожидался символ ';' в конце объявления.");
                return;
            }

            ParseDictionaryElementRecovering();

            while (!IsAtEnd())
            {
                if (Check(TokenType.CloseBrace))
                    break;

                if (Match(TokenType.Comma))
                {
                    if (Check(TokenType.CloseBrace))
                    {
                        AddError(Current(), "После запятой ожидался элемент словаря.");
                        break;
                    }

                    ParseDictionaryElementRecovering();
                    continue;
                }

                if (Check(TokenType.OpenBrace))
                {
                    AddError(Current(), "Ожидалась запятая между элементами словаря.");
                    ParseDictionaryElementRecovering();
                    continue;
                }

                break;
            }

            ExpectMissingOnly(TokenType.CloseBrace, "Ожидался символ '}' в конце инициализации.");
            ExpectMissingOnly(TokenType.Semicolon, "Ожидался символ ';' в конце объявления.");
        }

        private void ParseDictionaryElementRecovering()
        {
            if (!ExpectInsertable(TokenType.OpenBrace, "Ожидался символ '{' в элементе словаря."))
                return;

            if (!ExpectConsumable(TokenType.UnsignedInteger, "Ожидалось целое число."))
                return;

            if (!ExpectInsertable(TokenType.Comma, "Ожидался символ ',' между ключом и значением."))
                return;

            if (Check(TokenType.StringLiteral))
            {
                _position++;
            }
            else if (Check(TokenType.UnterminatedStringLiteral))
            {
                AddError(Current(), "Строковый литерал не закрыт.");
                _position++;

                // После битой строки НЕ генерируем каскад по этому же элементу,
                // а переходим к границе: следующему элементу, }, , или ;
                while (!IsAtEnd() &&
                       !Check(TokenType.OpenBrace) &&
                       !Check(TokenType.CloseBrace) &&
                       !Check(TokenType.Comma) &&
                       !Check(TokenType.Semicolon))
                {
                    _position++;
                }

                return;
            }
            else
            {
                AddError(CurrentOrLast(), "Ожидалась строка в двойных кавычках.");
                return;
            }

            if (!ExpectInsertable(TokenType.CloseBrace, "Ожидался символ '}' в элементе словаря."))
                return;
        }
        private bool ExpectMissingOnly(TokenType expectedType, string message)
        {
            if (!IsAtEnd() && Current().Type == expectedType)
            {
                _position++;
                return true;
            }

            AddError(CurrentOrLast(), message);
            return true;
        }

        private void ExpectMissingKeywordNew()
        {
            if (!IsAtEnd() && Current().Type == TokenType.KeywordNew)
            {
                _position++;
                return;
            }

            AddError(CurrentOrLast(), "Ожидалось ключевое слово 'new'.");
        }

        private bool ExpectDictionaryWordInsertable()
        {
            if (!IsAtEnd() &&
                Current().Type == TokenType.Identifier &&
                Current().Lexeme == "Dictionary")
            {
                _position++;
                return true;
            }

            AddError(CurrentOrLast(), "Ожидалось слово 'Dictionary'.");
            return false;
        }

        private bool ExpectIdentifierInsertable()
        {
            if (!IsAtEnd() &&
                Current().Type == TokenType.Identifier &&
                Current().Lexeme != "Dictionary")
            {
                _position++;
                return true;
            }

            AddError(CurrentOrLast(), "Ожидался идентификатор словаря.");
            return false;
        }

        private bool ExpectInsertable(TokenType expectedType, string message)
        {
            if (!IsAtEnd() && Current().Type == expectedType)
            {
                _position++;
                return true;
            }

            AddError(CurrentOrLast(), message);
            return false;
        }

        private bool ExpectConsumable(TokenType expectedType, string message)
        {
            if (!IsAtEnd() && Current().Type == expectedType)
            {
                _position++;
                return true;
            }

            AddError(CurrentOrLast(), message);

            if (!IsAtEnd())
                _position++;

            return false;
        }

        private bool Match(TokenType type)
        {
            if (Check(type))
            {
                _position++;
                return true;
            }

            return false;
        }

        private bool Check(TokenType type)
        {
            return !IsAtEnd() && Current().Type == type;
        }

        private bool LookAhead(TokenType type, int offset)
        {
            int index = _position + offset;
            return index < _tokens.Count && _tokens[index].Type == type;
        }

        private void AddError(Token token, string message)
        {
            string key = $"{token.Line}:{token.StartColumn}:{message}";
            if (_errorKeys.Contains(key))
                return;

            _errorKeys.Add(key);

            _result.Errors.Add(new SyntaxError
            {
                InvalidFragment = token.Lexeme,
                Line = token.Line,
                Column = token.StartColumn,
                Message = message
            });
        }

        private Token Current()
        {
            return _tokens[_position];
        }

        private Token CurrentOrLast()
        {
            if (_tokens.Count == 0)
            {
                return new Token
                {
                    Lexeme = string.Empty,
                    Line = 1,
                    StartColumn = 1,
                    EndColumn = 1
                };
            }

            if (_position >= _tokens.Count)
                return _tokens[_tokens.Count - 1];

            return _tokens[_position];
        }

        private bool IsAtEnd()
        {
            return _position >= _tokens.Count;
        }
    }
}