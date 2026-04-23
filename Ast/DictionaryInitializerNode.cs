using System.Collections.Generic;
using System.Text;

namespace Laba1.Ast
{
    public class DictionaryInitializerNode : AstNode
    {
        public List<DictionaryElementNode> Elements = new();

        public override string Print(string indent = "", bool last = true)
        {
            var b = last ? "└── " : "├── ";
            var child = indent + (last ? "    " : "│   ");

            var sb = new StringBuilder();
            sb.AppendLine(indent + b + "Initializer");

            for (int i = 0; i < Elements.Count; i++)
                sb.Append(Elements[i].Print(child, i == Elements.Count - 1));

            return sb.ToString();
        }
    }
}