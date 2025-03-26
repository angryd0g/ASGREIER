using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphicEditor
{
    class UpdateStarPoints
    {
        public void UpdateStarPoints1(Polygon star, Point endPoint, Point startPoint, int starType)
        {
            double width = Math.Abs(endPoint.X - startPoint.X);
            double height = Math.Abs(endPoint.Y - startPoint.Y);
            double centerX = (startPoint.X + endPoint.X) / 2;
            double centerY = (startPoint.Y + endPoint.Y) / 2;

            double outerRadius = Math.Sqrt(width * width + height * height) / 2; // Внешний радиус
            double innerRadius = outerRadius * 0.4; // Внутренний радиус (40% от внешнего)

            // Логика обновления точек в зависимости от типа звезды
            switch (starType)
            {
                case 4: // Четырёхконечная звезда
                    star.Points = CalculateFourPointedStar(new Point(centerX, centerY), outerRadius, innerRadius);
                    break;

                case 5: // Пятиконечная звезда
                    star.Points = CalculateFivePointedStar(new Point(centerX, centerY), outerRadius, innerRadius);
                    break;

                case 6: // Шестиконечная звезда
                    star.Points = CalculateSixPointedStar(new Point(centerX, centerY), outerRadius, innerRadius);
                    break;

                default:
                    throw new ArgumentException("Неизвестный тип звезды");
            }
        }

        // Метод для четырёхконечной звезды
        private PointCollection CalculateFourPointedStar(Point center, double outerRadius, double innerRadius)
        {
            PointCollection points = new PointCollection();
            double angle = 0; // Начинаем с угла 0 градусов (горизонтальная ось)

            for (int i = 0; i < 4; i++)
            {
                // Внешняя вершина
                points.Add(new Point(
                    center.X + Math.Cos(angle) * outerRadius,
                    center.Y - Math.Sin(angle) * outerRadius
                ));
                angle += Math.PI / 4; // 90 градусов

                // Внутренняя вершина
                points.Add(new Point(
                    center.X + Math.Cos(angle) * innerRadius,
                    center.Y - Math.Sin(angle) * innerRadius
                ));
                angle += Math.PI / 4; // 90 градусов
            }

            return points;
        }

        // Метод для пятиконечной звезды
        private PointCollection CalculateFivePointedStar(Point center, double outerRadius, double innerRadius)
        {
            PointCollection points = new PointCollection();
            double angle = Math.PI / 2; // Начинаем с верхней вершины
            double angleIncrement = Math.PI / 5; // Угол между вершинами (72 градуса)

            for (int i = 0; i < 5; i++)
            {
                // Внешняя вершина
                points.Add(new Point(
                    center.X + Math.Cos(angle) * outerRadius,
                    center.Y - Math.Sin(angle) * outerRadius
                ));
                angle += angleIncrement;

                // Внутренняя вершина
                points.Add(new Point(
                    center.X + Math.Cos(angle) * innerRadius,
                    center.Y - Math.Sin(angle) * innerRadius
                ));
                angle += angleIncrement;
            }

            return points;
        }

        // Метод для шестиконечной звезды
        private PointCollection CalculateSixPointedStar(Point center, double outerRadius, double innerRadius)
        {
            PointCollection points = new PointCollection();
            double angle = Math.PI / 2; // Начинаем с верхней вершины
            double angleIncrement = Math.PI / 6; // Угол между вершинами (60 градусов)

            for (int i = 0; i < 6; i++)
            {
                // Внешняя вершина (верхний треугольник)
                points.Add(new Point(
                    center.X + Math.Cos(angle) * outerRadius,
                    center.Y - Math.Sin(angle) * outerRadius
                ));
                angle += angleIncrement;

                // Внутренняя вершина (нижний треугольник)
                points.Add(new Point(
                    center.X + Math.Cos(angle) * innerRadius,
                    center.Y - Math.Sin(angle) * innerRadius
                ));
                angle += angleIncrement;
            }

            return points;
        }
    }
}
