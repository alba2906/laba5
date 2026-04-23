using Laba1.Ast;
using System.Collections.Generic;

namespace Laba1.AstVisualization
{
    public class AstLayoutNode
    {
        public AstNode? AstNode { get; set; }
        public string Header { get; set; } = string.Empty;

        public double X { get; set; }
        public double Y { get; set; }

        public double Width { get; set; } = 240;
        public double Height { get; set; } = 80;

        public List<AstLayoutNode> Children { get; } = new();
    }
}