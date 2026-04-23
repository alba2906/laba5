using Laba1.Ast;
using Laba1.AstVisualization;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Laba1
{
    public static class AstCanvasRenderer
    {
        private const double NodeWidth = 240;
        private const double NodeHeight = 80;
        private const double HorizontalGap = 40;
        private const double VerticalGap = 100;

        private static double _nextLeafX;

        public static void Render(Canvas canvas, AstNode root)
        {
            canvas.Children.Clear();
            _nextLeafX = 20;

            var layoutRoot = BuildLayout(root);
            Measure(layoutRoot, 20);
            Draw(canvas, layoutRoot);

            canvas.Width = GetMaxX(layoutRoot) + 260;
            canvas.Height = GetMaxY(layoutRoot) + 140;
        }

        private static AstLayoutNode CreateVisualNode(string header)
        {
            return new AstLayoutNode
            {
                Header = header,
                Width = NodeWidth,
                Height = NodeHeight
            };
        }

        private static AstLayoutNode BuildLayout(AstNode node)
        {
            var layout = new AstLayoutNode
            {
                AstNode = node,
                Header = GetHeader(node),
                Width = NodeWidth,
                Height = NodeHeight
            };

            switch (node)
            {
                case ProgramNode program:
                    foreach (var decl in program.Declarations)
                        layout.Children.Add(BuildLayout(decl));
                    break;

                case DictionaryDeclarationNode decl:
                    layout.Children.Add(BuildLayout(decl.Type));
                    layout.Children.Add(BuildLayout(decl.Identifier));
                    layout.Children.Add(BuildLayout(decl.Initializer));
                    break;

                case DictionaryTypeNode type:
                    layout.Children.Add(CreateVisualNode($"Name: {type.Name}"));
                    layout.Children.Add(CreateVisualNode($"KeyType: {type.KeyType}"));
                    layout.Children.Add(CreateVisualNode($"ValueType: {type.ValueType}"));
                    break;

                case IdentifierNode id:
                    layout.Children.Add(CreateVisualNode($"Name: {id.Name}"));
                    break;

                case DictionaryInitializerNode init:
                    foreach (var element in init.Elements)
                        layout.Children.Add(BuildLayout(element));
                    break;

                case DictionaryElementNode element:
                    layout.Children.Add(BuildLayout(element.Key));
                    layout.Children.Add(BuildLayout(element.Value));
                    break;

                case NumberLiteralNode number:
                    layout.Children.Add(CreateVisualNode($"Value: {number.Value}"));
                    break;

                case StringLiteralNode str:
                    layout.Children.Add(CreateVisualNode($"Value: \"{str.Value}\""));
                    break;
            }

            return layout;
        }

        private static void Measure(AstLayoutNode node, double y)
        {
            node.Y = y;

            if (node.Children.Count == 0)
            {
                node.X = _nextLeafX;
                _nextLeafX += node.Width + HorizontalGap;
                return;
            }

            foreach (var child in node.Children)
                Measure(child, y + node.Height + VerticalGap);

            var first = node.Children.First();
            var last = node.Children.Last();
            node.X = (first.X + last.X) / 2.0;
        }

        private static void Draw(Canvas canvas, AstLayoutNode node)
        {
            foreach (var child in node.Children)
            {
                DrawLine(
                    canvas,
                    node.X + node.Width / 2,
                    node.Y + node.Height,
                    child.X + child.Width / 2,
                    child.Y);

                Draw(canvas, child);
            }

            DrawNode(canvas, node);
        }

        private static void DrawNode(Canvas canvas, AstLayoutNode node)
        {
            var border = new Border
            {
                Width = node.Width,
                Height = node.Height,
                BorderBrush = Brushes.DeepPink,
                BorderThickness = new Thickness(2),
                Background = new SolidColorBrush(Color.FromRgb(255, 228, 235)),
                CornerRadius = new CornerRadius(12)
            };

            Canvas.SetLeft(border, node.X);
            Canvas.SetTop(border, node.Y);
            canvas.Children.Add(border);

            var text = new TextBlock
            {
                Text = node.Header,
                Width = node.Width - 20,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 13,
                Foreground = Brushes.DarkMagenta
            };

            Canvas.SetLeft(text, node.X + 10);
            Canvas.SetTop(text, node.Y + 12);
            canvas.Children.Add(text);
        }

        private static void DrawLine(Canvas canvas, double x1, double y1, double x2, double y2)
        {
            var line = new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = Brushes.HotPink,
                StrokeThickness = 2
            };

            canvas.Children.Add(line);
        }

        private static string GetHeader(AstNode node)
        {
            return node switch
            {
                ProgramNode => "ProgramNode",
                DictionaryDeclarationNode => "DictionaryDeclarationNode",
                DictionaryTypeNode => "DictionaryTypeNode",
                IdentifierNode => "IdentifierNode",
                DictionaryInitializerNode => "DictionaryInitializerNode",
                DictionaryElementNode => "DictionaryElementNode",
                NumberLiteralNode => "NumberLiteralNode",
                StringLiteralNode => "StringLiteralNode",
                _ => node.GetType().Name
            };
        }

        private static double GetMaxX(AstLayoutNode node)
        {
            double max = node.X + node.Width;
            foreach (var child in node.Children)
                max = Math.Max(max, GetMaxX(child));
            return max;
        }

        private static double GetMaxY(AstLayoutNode node)
        {
            double max = node.Y + node.Height;
            foreach (var child in node.Children)
                max = Math.Max(max, GetMaxY(child));
            return max;
        }
    }
}