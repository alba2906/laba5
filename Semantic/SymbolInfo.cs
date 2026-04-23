namespace Laba1.Semantic
{
    public class SymbolInfo
    {
        public string Name { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public int DeclLine { get; set; }
        public int DeclColumn { get; set; }
    }
}