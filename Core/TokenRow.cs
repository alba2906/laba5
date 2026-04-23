namespace Laba1.Core
{
    public sealed class TokenRow
    {
        public int Number { get; set; }
        public string Lexeme { get; set; } = string.Empty;
        public string TokenType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Line { get; set; }
        public int Column { get; set; }
    }
}