using System.Collections.Generic;
using System.Windows;

namespace Laba1.Windows
{
    public partial class SemanticErrorsWindow : Window
    {
        public SemanticErrorsWindow(List<SemanticErrorRow> errors)
        {
            InitializeComponent();
            ErrorsGrid.ItemsSource = errors;
        }
    }

    public class SemanticErrorRow
    {
        public string Message { get; set; } = string.Empty;
        public int Line { get; set; }
        public int Column { get; set; }
    }
}