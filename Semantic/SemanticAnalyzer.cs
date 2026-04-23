using System.Collections.Generic;
using Laba1.Ast;

namespace Laba1.Semantic
{
    public class SemanticAnalyzer
    {
        public List<SemanticErrorInfo> Errors { get; } = new();

        public void Analyze(ProgramNode program)
        {
            HashSet<string> declaredNames = new();

            foreach (var decl in program.Declarations)
            {
                if (!declaredNames.Add(decl.Identifier.Name))
                {
                    Errors.Add(new SemanticErrorInfo
                    {
                        Message = $"Идентификатор \"{decl.Identifier.Name}\" уже объявлен ранее",
                        Line = decl.Identifier.Line,
                        Column = decl.Identifier.Column
                    });
                }

                HashSet<int> keys = new();

                foreach (var element in decl.Initializer.Elements)
                {
                    if (element.Key.Value < 0)
                    {
                        Errors.Add(new SemanticErrorInfo
                        {
                            Message = $"Ключ словаря {element.Key.Value} вне допустимого диапазона",
                            Line = element.Key.Line,
                            Column = element.Key.Column
                        });
                    }

                    if (!keys.Add(element.Key.Value))
                    {
                        Errors.Add(new SemanticErrorInfo
                        {
                            Message = $"Ключ \"{element.Key.Value}\" уже существует в словаре",
                            Line = element.Key.Line,
                            Column = element.Key.Column
                        });
                    }
                }
            }
        }
    }

    public class SemanticErrorInfo
    {
        public string Message { get; set; } = string.Empty;
        public int Line { get; set; }
        public int Column { get; set; }
    }
}