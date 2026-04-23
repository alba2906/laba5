namespace Laba1.Ast
{
    public abstract class AstNode
    {
        public int Line { get; set; }
        public int Column { get; set; }

        public abstract string Print(string indent = "", bool isLast = true);
    }
}