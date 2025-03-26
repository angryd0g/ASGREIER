using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace GraphicEditor
{
    public class ShapeFactory
    {
        public static Shape CreateShape(string tool, Point startPoint, Brush strokeBrush, double strokeThickness)
        {
            switch (tool)
            {
                case "Line":
                    return new Line
                    {
                        Stroke = strokeBrush,
                        StrokeThickness = strokeThickness,
                        X1 = startPoint.X,
                        Y1 = startPoint.Y,
                        X2 = startPoint.X,
                        Y2 = startPoint.Y
                    };

                case "Rectangle":
                    return new Rectangle
                    {
                        Stroke = strokeBrush,
                        StrokeThickness = strokeThickness,
                        Fill = Brushes.Transparent
                    };

                case "Ellipse":
                    return new Ellipse
                    {
                        Stroke = strokeBrush,
                        StrokeThickness = strokeThickness,
                        Fill = Brushes.Transparent
                    };

                case "Triangle":
                    return new Polygon
                    {
                        Stroke = strokeBrush,
                        StrokeThickness = strokeThickness,
                        Fill = Brushes.Transparent,
                        Points = new PointCollection
                    {
                        startPoint,
                        new Point(startPoint.X + 50, startPoint.Y + 100),
                        new Point(startPoint.X - 50, startPoint.Y + 100)
                    }
                    };

                case "RightTriangle":
                    return new Polygon
                    {
                        Stroke = strokeBrush,
                        StrokeThickness = strokeThickness,
                        Fill = Brushes.Transparent,
                        Points = new PointCollection
                    {
                        startPoint,
                        new Point(startPoint.X + 100, startPoint.Y + 100),
                        new Point(startPoint.X, startPoint.Y + 100)
                    }
                    };

                case "Diamond":
                    return new Polygon
                    {
                        Stroke = strokeBrush,
                        StrokeThickness = strokeThickness,
                        Fill = Brushes.Transparent,
                        Points = new PointCollection
                    {
                        startPoint,
                        new Point(startPoint.X, startPoint.Y - 50),
                        new Point(startPoint.X + 50, startPoint.Y),
                        new Point(startPoint.X, startPoint.Y + 50),
                        new Point(startPoint.X - 50, startPoint.Y)
                    }
                    };

                case "Pentagon":
                    return new Polygon
                    {
                        Stroke = strokeBrush,
                        StrokeThickness = strokeThickness,
                        Fill = Brushes.Transparent,
                        Points = new PointCollection
                    {
                        startPoint,
                        new Point(startPoint.X, startPoint.Y - 50),
                        new Point(startPoint.X + 50, startPoint.Y - 20),
                        new Point(startPoint.X + 30, startPoint.Y + 40),
                        new Point(startPoint.X - 30, startPoint.Y + 40),
                        new Point(startPoint.X - 50, startPoint.Y - 20)
                    }
                    };

                case "Hexagon":
                    return new Polygon
                    {
                        Stroke = strokeBrush,
                        StrokeThickness = strokeThickness,
                        Fill = Brushes.Transparent,
                        Points = new PointCollection
                    {
                        startPoint,
                        new Point(startPoint.X, startPoint.Y - 50),
                        new Point(startPoint.X + 50, startPoint.Y - 25),
                        new Point(startPoint.X + 50, startPoint.Y + 25),
                        new Point(startPoint.X, startPoint.Y + 50),
                        new Point(startPoint.X - 50, startPoint.Y + 25),
                        new Point(startPoint.X - 50, startPoint.Y - 25)
                    }
                    };

                case "ArrowRight":
                    return new Polygon
                    {
                        Stroke = strokeBrush,
                        StrokeThickness = strokeThickness,
                        Fill = Brushes.Transparent,
                        Points = new PointCollection
                    {
                        startPoint,
                        new Point(startPoint.X, startPoint.Y),
                        new Point(startPoint.X + 80, startPoint.Y),
                        new Point(startPoint.X + 80, startPoint.Y - 20),
                        new Point(startPoint.X + 100, startPoint.Y),
                        new Point(startPoint.X + 80, startPoint.Y + 20),
                        new Point(startPoint.X + 80, startPoint.Y)
                    }
                    };

                case "ArrowLeft":
                    return new Polygon
                    {
                        Stroke = strokeBrush,
                        StrokeThickness = strokeThickness,
                        Fill = Brushes.Transparent,
                        Points = new PointCollection
                    {
                        startPoint,
                        new Point(startPoint.X, startPoint.Y),
                        new Point(startPoint.X - 50, startPoint.Y + 25),
                        new Point(startPoint.X - 25, startPoint.Y + 25),
                        new Point(startPoint.X - 25, startPoint.Y + 50),
                        new Point(startPoint.X + 25, startPoint.Y),
                        new Point(startPoint.X - 25, startPoint.Y - 50),
                        new Point(startPoint.X - 25, startPoint.Y - 25),
                        new Point(startPoint.X - 50, startPoint.Y - 25)
                    }
                    };

                case "ArrowUp":
                    return new Polygon
                    {
                        Stroke = strokeBrush,
                        StrokeThickness = strokeThickness,
                        Fill = Brushes.Transparent,
                        Points = new PointCollection
                    {
                        startPoint,
                        new Point(startPoint.X + 25, startPoint.Y - 50),
                        new Point(startPoint.X + 25, startPoint.Y - 25),
                        new Point(startPoint.X + 50, startPoint.Y - 25),
                        new Point(startPoint.X, startPoint.Y + 25),
                        new Point(startPoint.X - 50, startPoint.Y - 25),
                        new Point(startPoint.X - 25, startPoint.Y - 25),
                        new Point(startPoint.X - 25, startPoint.Y - 50)
                    }
                    };

                case "ArrowDown":
                    return new Polygon
                    {
                        Stroke = strokeBrush,
                        StrokeThickness = strokeThickness,
                        Fill = Brushes.Transparent,
                        Points = new PointCollection
                    {
                        startPoint,
                        new Point(startPoint.X, startPoint.Y - 50),
                        new Point(startPoint.X + 25, startPoint.Y - 25),
                        new Point(startPoint.X + 25, startPoint.Y),
                        new Point(startPoint.X, startPoint.Y + 25),
                        new Point(startPoint.X - 25, startPoint.Y),
                        new Point(startPoint.X - 25, startPoint.Y - 25)
                    }
                    };

                case "Star4":
                    return new Polygon
                    {
                        Stroke = strokeBrush,
                        StrokeThickness = strokeThickness,
                        Fill = Brushes.Transparent,
                        Points = new PointCollection
                    {
                        startPoint,
                        new Point(startPoint.X + 50, startPoint.Y + 50),
                        new Point(startPoint.X, startPoint.Y + 100),
                        new Point(startPoint.X - 50, startPoint.Y + 50)
                    }
                    };

                case "Star5":
                    return new Polygon
                    {
                        Stroke = strokeBrush,
                        StrokeThickness = strokeThickness,
                        Fill = Brushes.Transparent,
                        Points = new PointCollection
                    {
                        startPoint,
                        new Point(startPoint.X + 30, startPoint.Y + 50),
                        new Point(startPoint.X + 50, startPoint.Y + 100),
                        new Point(startPoint.X, startPoint.Y + 70),
                        new Point(startPoint.X - 50, startPoint.Y + 100),
                        new Point(startPoint.X - 30, startPoint.Y + 50)
                    }
                    };

                case "Star6":
                    return new Polygon
                    {
                        Stroke = strokeBrush,
                        StrokeThickness = strokeThickness,
                        Fill = Brushes.Transparent,
                        Points = new PointCollection
                    {
                        startPoint,
                        new Point(startPoint.X + 30, startPoint.Y + 30),
                        new Point(startPoint.X + 60, startPoint.Y + 30),
                        new Point(startPoint.X + 70, startPoint.Y + 60),
                        new Point(startPoint.X + 30, startPoint.Y + 70),
                        new Point(startPoint.X, startPoint.Y + 100),
                        new Point(startPoint.X - 30, startPoint.Y + 70),
                        new Point(startPoint.X - 70, startPoint.Y + 60),
                        new Point(startPoint.X - 60, startPoint.Y + 30),
                        new Point(startPoint.X - 30, startPoint.Y + 30)
                    }
                    };

                default:
                    throw new ArgumentException("Неизвестный инструмент: " + tool);
            }
        }
    }
}
