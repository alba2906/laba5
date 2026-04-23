namespace Laba1.Ast
{
    public class IdentifierNode : AstNode
    {
        public string Name;

        public override string Print(string indent = "", bool last = true)
        {
            var b = last ? "└── " : "├── ";
            return indent + b + "Identifier: " + Name + "\n";
        }
    }
}