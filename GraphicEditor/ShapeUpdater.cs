using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace GraphicEditor
{
    public class ShapeUpdater
    {
        public static void UpdateShape(Shape shape, string tool, Point startPoint, Point endPoint)
        {
            switch (tool)
            {
                case "Line":
                    if (shape is Line line)
                    {
                        line.X2 = endPoint.X;
                        line.Y2 = endPoint.Y;
                    }
                    break;

                case "Rectangle":
                    if (shape is Rectangle rectangle)
                    {
                        Canvas.SetLeft(rectangle, Math.Min(startPoint.X, endPoint.X));
                        Canvas.SetTop(rectangle, Math.Min(startPoint.Y, endPoint.Y));
                        rectangle.Width = Math.Abs(endPoint.X - startPoint.X);
                        rectangle.Height = Math.Abs(endPoint.Y - startPoint.Y);
                    }
                    break;

                case "Ellipse":
                    if (shape is Ellipse ellipse)
                    {
                        Canvas.SetLeft(ellipse, Math.Min(startPoint.X, endPoint.X));
                        Canvas.SetTop(ellipse, Math.Min(startPoint.Y, endPoint.Y));
                        ellipse.Width = Math.Abs(endPoint.X - startPoint.X);
                        ellipse.Height = Math.Abs(endPoint.Y - startPoint.Y);
                    }
                    break;

                case "Triangle":
                    if (shape is Polygon triangle)
                    {
                        triangle.Points = new PointCollection
                    {
                        new Point(startPoint.X, startPoint.Y), // Верхняя вершина
                        new Point(endPoint.X, endPoint.Y), // Правая нижняя вершина
                        new Point(startPoint.X - (endPoint.X - startPoint.X), endPoint.Y) // Левая нижняя вершина
                    };
                    }
                    break;

                case "RightTriangle":
                    if (shape is Polygon rightTriangle)
                    {
                        rightTriangle.Points = new PointCollection
                    {
                        new Point(startPoint.X, startPoint.Y), // Верхняя вершина
                        new Point(endPoint.X, endPoint.Y), // Правая нижняя вершина
                        new Point(startPoint.X, endPoint.Y) // Левая нижняя вершина
                    };
                    }
                    break;

                case "Diamond":
                    if (shape is Polygon diamond)
                    {
                        double centerX = (startPoint.X + endPoint.X) / 2;
                        double centerY = (startPoint.Y + endPoint.Y) / 2;
                        double width = Math.Abs(endPoint.X - startPoint.X);
                        double height = Math.Abs(endPoint.Y - startPoint.Y);

                        diamond.Points = new PointCollection
                    {
                        new Point(centerX, centerY - height / 2), // Верхняя вершина
                        new Point(centerX + width / 2, centerY), // Правая вершина
                        new Point(centerX, centerY + height / 2), // Нижняя вершина
                        new Point(centerX - width / 2, centerY) // Левая вершина
                    };
                    }
                    break;

                case "Pentagon":
                    if (shape is Polygon pentagon)
                    {
                        double width = Math.Abs(endPoint.X - startPoint.X);
                        double height = Math.Abs(endPoint.Y - startPoint.Y);
                        double centerX = (startPoint.X + endPoint.X) / 2;
                        double centerY = (startPoint.Y + endPoint.Y) / 2;

                        pentagon.Points = new PointCollection
                    {
                        new Point(centerX, centerY - height / 2), // Верхняя вершина
                        new Point(centerX + width / 2, centerY - height / 4), // Правая верхняя вершина
                        new Point(centerX + width / 3, centerY + height / 2), // Правая нижняя вершина
                        new Point(centerX - width / 3, centerY + height / 2), // Левая нижняя вершина
                        new Point(centerX - width / 2, centerY - height / 4) // Левая верхняя вершина
                    };
                    }
                    break;

                case "Hexagon":
                    if (shape is Polygon hexagon)
                    {
                        double width = Math.Abs(endPoint.X - startPoint.X);
                        double height = Math.Abs(endPoint.Y - startPoint.Y);
                        double centerX = (startPoint.X + endPoint.X) / 2;
                        double centerY = (startPoint.Y + endPoint.Y) / 2;

                        hexagon.Points = new PointCollection
                    {
                        new Point(centerX, centerY - height / 2), // Верхняя вершина
                        new Point(centerX + width / 2, centerY - height / 4), // Правая верхняя вершина
                        new Point(centerX + width / 2, centerY + height / 4), // Правая нижняя вершина
                        new Point(centerX, centerY + height / 2), // Нижняя вершина
                        new Point(centerX - width / 2, centerY + height / 4), // Левая нижняя вершина
                        new Point(centerX - width / 2, centerY - height / 4) // Левая верхняя вершина
                    };
                    }
                    break;

                case "ArrowRight":
                    if (shape is Polygon arrowRight)
                    {
                        double width = Math.Abs(endPoint.X - startPoint.X);
                        double height = Math.Abs(endPoint.Y - startPoint.Y);

                        arrowRight.Points = new PointCollection
                    {
                        new Point(startPoint.X, startPoint.Y), // Левая вершина
                        new Point(endPoint.X - width / 3, startPoint.Y), // Правая точка перед стрелкой
                        new Point(endPoint.X - width / 3, startPoint.Y - height / 2), // Верхний кончик стрелки
                        new Point(endPoint.X, startPoint.Y), // Центральный кончик стрелки
                        new Point(endPoint.X - width / 3, startPoint.Y + height / 2), // Нижний кончик стрелки
                        new Point(endPoint.X - width / 3, startPoint.Y) // Правая точка перед стрелкой (замыкание)
                    };
                    }
                    break;

                case "ArrowLeft":
                    if (shape is Polygon arrowLeft)
                    {
                        double width = Math.Abs(endPoint.X - startPoint.X);
                        double height = Math.Abs(endPoint.Y - startPoint.Y);

                        arrowLeft.Points = new PointCollection
                    {
                        new Point(endPoint.X, startPoint.Y), // Правая вершина
                        new Point(startPoint.X + width / 3, startPoint.Y), // Левая точка перед стрелкой
                        new Point(startPoint.X + width / 3, startPoint.Y - height / 2), // Верхний кончик стрелки
                        new Point(startPoint.X, startPoint.Y), // Центральный кончик стрелки
                        new Point(startPoint.X + width / 3, startPoint.Y + height / 2), // Нижний кончик стрелки
                        new Point(startPoint.X + width / 3, startPoint.Y) // Левая точка перед стрелкой (замыкание)
                    };
                    }
                    break;

                case "ArrowUp":
                    if (shape is Polygon arrowUp)
                    {
                        double width = Math.Abs(endPoint.X - startPoint.X);
                        double height = Math.Abs(endPoint.Y - startPoint.Y);

                        arrowUp.Points = new PointCollection
                    {
                        new Point(startPoint.X, endPoint.Y), // Нижняя вершина
                        new Point(startPoint.X, startPoint.Y + height / 3), // Верхняя точка перед стрелкой
                        new Point(startPoint.X - width / 2, startPoint.Y + height / 3), // Левый кончик стрелки
                        new Point(startPoint.X, startPoint.Y), // Центральный кончик стрелки
                        new Point(startPoint.X + width / 2, startPoint.Y + height / 3), // Правый кончик стрелки
                        new Point(startPoint.X, startPoint.Y + height / 3) // Верхняя точка перед стрелкой (замыкание)
                    };
                    }
                    break;

                case "ArrowDown":
                    if (shape is Polygon arrowDown)
                    {
                        double width = Math.Abs(endPoint.X - startPoint.X);
                        double height = Math.Abs(endPoint.Y - startPoint.Y);

                        arrowDown.Points = new PointCollection
                    {
                        new Point(startPoint.X, startPoint.Y), // Верхняя вершина
                        new Point(startPoint.X, endPoint.Y - height / 3), // Нижняя точка перед стрелкой
                        new Point(startPoint.X - width / 2, endPoint.Y - height / 3), // Левый кончик стрелки
                        new Point(startPoint.X, endPoint.Y), // Центральный кончик стрелки
                        new Point(startPoint.X + width / 2, endPoint.Y - height / 3), // Правый кончик стрелки
                        new Point(startPoint.X, endPoint.Y - height / 3) // Нижняя точка перед стрелкой (замыкание)
                    };
                    }
                    break;

                case "Star4":
                case "Star5":
                case "Star6":
                    if (shape is Polygon star)
                    {
                        UpdateStarPoints starUpdater = new UpdateStarPoints();
                        int points = tool == "Star4" ? 4 : tool == "Star5" ? 5 : 6;
                        starUpdater.UpdateStarPoints1(star, startPoint, endPoint, points);
                    }
                    break;

                default:
                    throw new ArgumentException("Неизвестный инструмент: " + tool);
            }
        }
    }
}
