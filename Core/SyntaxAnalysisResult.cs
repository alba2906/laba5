using System.Collections.Generic;

namespace Laba1.Core
{
    public sealed class SyntaxAnalysisResult
    {
        public List<SyntaxError> Errors { get; } = new();
        public bool HasErrors => Errors.Count > 0;

        public string? UserMessage { get; set; }
        public bool HasMessage => !string.IsNullOrWhiteSpace(UserMessage);
    }
}