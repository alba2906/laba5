using System.Windows;
using Laba1.Ast;

namespace Laba1.Windows
{
    public partial class AstCanvasWindow : Window
    {
        public AstCanvasWindow(AstNode root)
        {
            InitializeComponent();
            AstCanvasRenderer.Render(AstCanvas, root);
        }
    }
}