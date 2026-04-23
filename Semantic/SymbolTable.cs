using System.Collections.Generic;

namespace Laba1.Semantic
{
    public class SymbolTable
    {
        private readonly Dictionary<string, SymbolInfo> _symbols = new();

        public bool Declare(SymbolInfo symbol)
        {
            if (_symbols.ContainsKey(symbol.Name))
                return false;

            _symbols[symbol.Name] = symbol;
            return true;
        }

        public SymbolInfo Lookup(string name)
        {
            _symbols.TryGetValue(name, out var symbol);
            return symbol;
        }
    }
}