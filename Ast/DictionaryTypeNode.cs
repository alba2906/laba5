using System.Text;

namespace Laba1.Ast
{
    public class DictionaryTypeNode : AstNode
    {
        public string Name { get; set; } = "Dictionary";
        public string KeyType { get; set; }
        public string ValueType { get; set; }

        public override string Print(string indent = "", bool isLast = true)
        {
            var branch = isLast ? "└── " : "├── ";
            var childIndent = indent + (isLast ? "    " : "│   ");

            var sb = new StringBuilder();
            sb.AppendLine($"{indent}{branch}DictionaryTypeNode");
            sb.AppendLine($"{childIndent}├── Name: {Name}");
            sb.AppendLine($"{childIndent}├── KeyType: {KeyType}");
            sb.AppendLine($"{childIndent}└── ValueType: {ValueType}");

            return sb.ToString();
        }
    }
}