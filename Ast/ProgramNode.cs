using System.Collections.Generic;
using System.Text;

namespace Laba1.Ast
{
    public class ProgramNode : AstNode
    {
        public List<DictionaryDeclarationNode> Declarations { get; } = new();

        public override string Print(string indent = "", bool last = true)
        {
            var sb = new StringBuilder();
            sb.AppendLine("ProgramNode");

            for (int i = 0; i < Declarations.Count; i++)
                sb.Append(Declarations[i].Print("", i == Declarations.Count - 1));

            return sb.ToString();
        }
    }
}