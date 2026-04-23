using System.Text;

namespace Laba1.Ast
{
    public class DictionaryElementNode : AstNode
    {
        public NumberLiteralNode Key { get; set; }
        public StringLiteralNode Value { get; set; }

        public override string Print(string indent = "", bool isLast = true)
        {
            var branch = isLast ? "└── " : "├── ";
            var childIndent = indent + (isLast ? "    " : "│   ");

            var sb = new StringBuilder();
            sb.AppendLine($"{indent}{branch}DictionaryElementNode");

            if (Key != null)
                sb.Append(Key.Print(childIndent, false));

            if (Value != null)
                sb.Append(Value.Print(childIndent, true));

            return sb.ToString();
        }
    }
}