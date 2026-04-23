namespace Laba1.Ast
{
    public class NumberLiteralNode : AstNode
    {
        public int Value { get; set; }

        public override string Print(string indent = "", bool isLast = true)
        {
            var branch = isLast ? "└── " : "├── ";
            return $"{indent}{branch}NumberLiteralNode: {Value}{System.Environment.NewLine}";
        }
    }
}