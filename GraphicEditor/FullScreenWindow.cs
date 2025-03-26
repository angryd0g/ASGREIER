using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace GraphicEditor
{
    public class FullScreenWindow : Window
    {
        public FullScreenWindow(Canvas canvas)
        {
            Title = "Полноэкранный режим";
            WindowState = WindowState.Maximized; // Разворачиваем окно на весь экран
            WindowStyle = WindowStyle.None; // Убираем рамку окна
            ResizeMode = ResizeMode.NoResize; // Запрещаем изменение размера окна

            // Создаем контейнер для содержимого
            Grid grid = new Grid();
            grid.Background = Brushes.White;

            // Копируем содержимое DrawingCanvas
            Canvas fullScreenCanvas = new Canvas();
            foreach (var child in canvas.Children)
            {
                UIElement element = child as UIElement;
                if (element != null)
                {
                    UIElement clone = CloneElement(element);
                    fullScreenCanvas.Children.Add(clone);
                }
            }

            // Растягиваем Canvas на весь экран
            fullScreenCanvas.Width = SystemParameters.PrimaryScreenWidth;
            fullScreenCanvas.Height = SystemParameters.PrimaryScreenHeight;

            grid.Children.Add(fullScreenCanvas);
            Content = grid;

            // Добавляем кнопку для выхода из полноэкранного режима
            Button closeButton = new Button
            {
                Content = "Выйти из полноэкранного режима",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(10),
                Padding = new Thickness(5),
                Background = Brushes.Green,
                Foreground = Brushes.Black
            };
            closeButton.Click += (s, ev) => Close();
            grid.Children.Add(closeButton);
        }

        // Метод для клонирования элементов Canvas
        private UIElement CloneElement(UIElement element)
        {
            if (element is FrameworkElement frameworkElement)
            {
                FrameworkElement clone = (FrameworkElement)Activator.CreateInstance(frameworkElement.GetType());
                foreach (var property in frameworkElement.GetType().GetProperties())
                {
                    if (property.CanWrite && property.CanRead)
                    {
                        property.SetValue(clone, property.GetValue(frameworkElement));
                    }
                }
                return clone;
            }
            return element;
        }
    }
}
