namespace Laba1.Ast
{
    public class StringLiteralNode : AstNode
    {
        public string Value { get; set; } = string.Empty;

        public override string Print(string indent = "", bool isLast = true)
        {
            var branch = isLast ? "└── " : "├── ";
            return $"{indent}{branch}StringLiteralNode: \"{Value}\"{System.Environment.NewLine}";
        }
    }
}