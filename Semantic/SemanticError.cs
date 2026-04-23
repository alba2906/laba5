namespace Laba1.Semantic
{
    public class SemanticError
    {
        public string Message { get; set; } = string.Empty;
        public int Line { get; set; }
        public int Column { get; set; }

        public override string ToString()
        {
            return $"Ошибка: {Message} (строка {Line}, символ {Column})";
        }
    }
}