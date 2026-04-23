using System.Text;

namespace Laba1.Ast
{
    public class DictionaryDeclarationNode : AstNode
    {
        public DictionaryTypeNode Type { get; set; }
        public IdentifierNode Identifier { get; set; }
        public DictionaryInitializerNode Initializer { get; set; }

        public override string Print(string indent = "", bool isLast = true)
        {
            var branch = isLast ? "└── " : "├── ";
            var childIndent = indent + (isLast ? "    " : "│   ");

            var sb = new StringBuilder();
            sb.AppendLine($"{indent}{branch}DictionaryDeclarationNode");

            if (Type != null)
                sb.Append(Type.Print(childIndent, false));

            if (Identifier != null)
                sb.Append(Identifier.Print(childIndent, false));

            if (Initializer != null)
                sb.Append(Initializer.Print(childIndent, true));

            return sb.ToString();
        }
    }
}