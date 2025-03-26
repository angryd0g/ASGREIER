using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Media.Animation;

namespace GraphicEditor
{
    public partial class MainWindow : Window
    {
        private Stack<List<UIElement>> undoStack = new Stack<List<UIElement>>();
        private Stack<List<UIElement>> redoStack = new Stack<List<UIElement>>();
        internal ObservableCollection<Layer> Layers { get; set; } = new ObservableCollection<Layer>();
        internal Layer CurrentLayer => LayersListBox.SelectedItem as Layer;

        private bool isColorPickerActive = false;
        public Point startPoint;
        private Shape currentShape;
        private string currentTool = "Pencil";
        private Brush currentColor = Brushes.Black;
        private double currentThickness = 2;
        private bool isDrawing = false;
        private Polyline freehandLine;
        private bool isSelecting = false;
        private Rectangle selectionRect;
        private Point selectionStart;
        private Polyline eraserLine;
        private Rect selectionBounds;
        private bool isSelectionActive = false;
        private string currentFilePath = null;
        private bool isTextModeActive = false;
        private bool isSelectionModeActive = false;
        private string currentBrush = "Кисть 1";

        private Dictionary<string, Brush> brushes = new Dictionary<string, Brush>
        {
            { "Кисть 1", Brushes.Black }, // Обычная кисть
            { "Кисть 2", new SolidColorBrush(Color.FromArgb(128, 0, 0, 0)) }, // Полупрозрачная кисть
            { "Кисть 3", new LinearGradientBrush(Colors.Red, Colors.Yellow, 45) }, // Градиентная кисть
            { "Кисть 4", new RadialGradientBrush(Colors.Blue, Colors.Transparent) } // Радиальная кисть
        };
        public MainWindow()
        {
            InitializeComponent();

            ThemeManager.LoadAndApplyTheme(this);

            this.KeyDown += MainWindow_KeyDown;

            RenderOptions.SetBitmapScalingMode(DrawingCanvas, BitmapScalingMode.HighQuality);

            DrawingCanvas.Focus();

            UpdateUndoStack();

            LayersListBox.ItemsSource = Layers;

            AddLayer("Слой 1");
        }
        private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control) 
            {
                if (e.Key == Key.Z) 
                {
                    Undo();
                    e.Handled = true; 
                }
                else if (e.Key == Key.Y) 
                {
                    Redo();
                    e.Handled = true;
                }
            }
        }
        public void AddLayer(string name)
        {
            var layer = new Layer(name);
            Layers.Add(layer);
            LayersListBox.SelectedItem = layer;
        }
        private void AddLayer_Click(object sender, RoutedEventArgs e)
        {
            AddLayer($"Слой {Layers.Count + 1}");
        }
        public void RemoveLayer_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentLayer != null)
            {
                foreach (var obj in CurrentLayer.Objects)
                {
                    DrawingCanvas.Children.Remove(obj);
                }
                Layers.Remove(CurrentLayer);
            }
        }
        internal void ToggleLayerVisibility(Layer layer, bool isVisible)
        {
            layer.IsVisible = isVisible;
            foreach (var obj in layer.Objects)
            {
                obj.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        private void LayersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var layer in Layers)
            {
                ToggleLayerVisibility(layer, layer.IsVisible);
            }
        }
        public void MoveLayerUp_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentLayer != null)
            {
                int index = Layers.IndexOf(CurrentLayer);
                if (index > 0)
                {
                    Layers.Move(index, index - 1);
                    UpdateLayersZIndex();
                }
            }
        }
        public void MoveLayerDown_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentLayer != null)
            {
                int index = Layers.IndexOf(CurrentLayer);
                if (index < Layers.Count - 1)
                {
                    Layers.Move(index, index + 1);
                    UpdateLayersZIndex();
                }
            }
        }
        private void UpdateLayersZIndex()
        {
            for (int i = 0; i < Layers.Count; i++)
            {
                foreach (var obj in Layers[i].Objects)
                {
                    Canvas.SetZIndex(obj, i);
                }
            }
        }
        public void AddObjectToCurrentLayer(UIElement obj)
        {
            if (CurrentLayer != null)
            {
                CurrentLayer.Objects.Add(obj);
                DrawingCanvas.Children.Add(obj);
                Canvas.SetZIndex(obj, Layers.IndexOf(CurrentLayer));
            }
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            bool hasDrawnElements = DrawingCanvas.Children
                .OfType<UIElement>()
                .Any(child =>
                {
                    if (child == GridLinesCanvas)
                        return false;
                    if (child is System.Windows.Controls.Panel panel)
                        return panel.Children.Count > 0;
                    return true;
                });
            if (!hasDrawnElements)
            {
                return;
            }
            MessageBoxResult result = System.Windows.MessageBox.Show(
                "Данные могут быть не сохранены! \nВы уверены, что хотите выйти из приложения?",
                "Подтверждение выхода",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
                MessageBoxResult saveResult = System.Windows.MessageBox.Show(
                    "Хотите сохранить изменения?",
                    "Сохранить изменения",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question
                );
                if (saveResult == MessageBoxResult.Yes)
                {
                    SaveAs_Click(this, null);
                }
                else if (saveResult == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
        }
        private void ToggleToolBarTrayVisibility(object sender, RoutedEventArgs e)
        {
            if (MainToolBarTray.Visibility == Visibility.Collapsed)
            {
                MainToolBarTray.Visibility = Visibility.Visible;
                label1.Visibility = Visibility.Visible;
                label2.Visibility = Visibility.Visible;
                label3.Visibility = Visibility.Visible;
                label4.Visibility = Visibility.Visible;
                sp1.Visibility = Visibility.Visible;
                sp2.Visibility = Visibility.Visible;
                sp3.Visibility = Visibility.Visible;

                MainMenuItem1.Background = ThemeManager.IsDarkTheme ? new SolidColorBrush(Color.FromRgb(80, 80, 80)) : new SolidColorBrush(Color.FromRgb(230, 230, 230));

                ViewToolBarTray.Visibility = Visibility.Collapsed;
                sp4.Visibility = Visibility.Collapsed;
                label5.Visibility = Visibility.Collapsed;
                sp4_Копировать.Visibility = Visibility.Collapsed;
                label6.Visibility = Visibility.Collapsed;

                MainMenuItem3.Background = ThemeManager.IsDarkTheme ? new SolidColorBrush(Color.FromRgb(60, 60, 60)) : new SolidColorBrush(Color.FromRgb(210, 210, 210));
            }
            else
            {
                MainToolBarTray.Visibility = Visibility.Collapsed;
                label1.Visibility = Visibility.Collapsed;
                label2.Visibility = Visibility.Collapsed;
                label3.Visibility = Visibility.Collapsed;
                label4.Visibility = Visibility.Collapsed;
                sp1.Visibility = Visibility.Collapsed;
                sp2.Visibility = Visibility.Collapsed;
                sp3.Visibility = Visibility.Collapsed;

                MainMenuItem1.Background = ThemeManager.IsDarkTheme ? new SolidColorBrush(Color.FromRgb(60, 60, 60)) : new SolidColorBrush(Color.FromRgb(210, 210, 210));
            }
        }
        private void ToggleViewToolBarTrayVisibility1(object sender, RoutedEventArgs e)
        {
            if (ViewToolBarTray.Visibility == Visibility.Collapsed)
            {
                ViewToolBarTray.Visibility = Visibility.Visible;
                sp4.Visibility = Visibility.Visible;
                sp4_Копировать.Visibility = Visibility.Visible;
                label5.Visibility = Visibility.Visible;
                label6.Visibility = Visibility.Visible;

                MainMenuItem3.Background = ThemeManager.IsDarkTheme ? new SolidColorBrush(Color.FromRgb(80, 80, 80)) : new SolidColorBrush(Color.FromRgb(230, 230, 230));

                MainToolBarTray.Visibility = Visibility.Collapsed;
                label1.Visibility = Visibility.Collapsed;
                label2.Visibility = Visibility.Collapsed;
                label3.Visibility = Visibility.Collapsed;
                label4.Visibility = Visibility.Collapsed;
                sp1.Visibility = Visibility.Collapsed;
                sp2.Visibility = Visibility.Collapsed;
                sp3.Visibility = Visibility.Collapsed;

                MainMenuItem1.Background = ThemeManager.IsDarkTheme ? new SolidColorBrush(Color.FromRgb(60, 60, 60)) : new SolidColorBrush(Color.FromRgb(210, 210, 210));
            }
            else
            {
                ViewToolBarTray.Visibility = Visibility.Collapsed;
                sp4.Visibility = Visibility.Collapsed;
                sp4_Копировать.Visibility = Visibility.Collapsed;
                label5.Visibility = Visibility.Collapsed;
                label6.Visibility = Visibility.Collapsed;

                MainMenuItem3.Background = ThemeManager.IsDarkTheme ? new SolidColorBrush(Color.FromRgb(60, 60, 60)) : new SolidColorBrush(Color.FromRgb(210, 210, 210));
            }
        }
        private void Tool_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            if (button != null)
            {
                currentTool = button.Tag.ToString();
                StatusText.Text = $"Выбран инструмент: {button.ToolTip}"; 
                ResetStates();
            }
        }
        private void Color_Click(object sender, RoutedEventArgs e)
        {
            var colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                currentColor = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
            }
        }
        private void Thickness_Click(object sender, RoutedEventArgs e)
        {
            var thicknessDialog = new ThicknessDialog();
            if (thicknessDialog.ShowDialog() == true)
            {
                currentThickness = thicknessDialog.Thickness;
                StatusText.Text = $"Толщина линии: {currentThickness}";
            }
        }
        private void ClearCanvas_Click(object sender, RoutedEventArgs e)
        {
            DrawingCanvas.Children.Clear();
            StatusText.Text = "Холст очищен";
        }
        private void New_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Вы уверены, что хотите создать новый файл? Несохраненные изменения будут потеряны.", "Подтверждение создания нового файла", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                DrawingCanvas.Children.Clear(); 
                StatusText.Text = "Новый файл создан. Начните рисовать!";
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(currentFilePath))
            {
                SaveToFile(currentFilePath);
            }
            else
            {
                SaveAs_Click(sender, e);
            }
        }
        private void SaveToFile(string filePath)
        {
            try
            {
                double scale = CanvasScaleTransform.ScaleX;

                double canvasWidth = DrawingCanvas.ActualWidth * scale;
                double canvasHeight = DrawingCanvas.ActualHeight * scale;

                double dpi = 96d; 

                RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                    (int)canvasWidth,
                    (int)canvasHeight,
                    dpi, dpi, PixelFormats.Pbgra32);

                DrawingVisual drawingVisual = new DrawingVisual();
                using (DrawingContext context = drawingVisual.RenderOpen())
                {
                    VisualBrush visualBrush = new VisualBrush(DrawingCanvas);
                    context.DrawRectangle(visualBrush, null, new Rect(0, 0, canvasWidth, canvasHeight));
                }

                renderBitmap.Render(drawingVisual);

                BitmapEncoder encoder = null;
                string extension = System.IO.Path.GetExtension(filePath).ToLower();
                switch (extension)
                {
                    case ".png":
                        encoder = new PngBitmapEncoder();
                        break;
                    case ".jpg":
                        encoder = new JpegBitmapEncoder();
                        break;
                    case ".bmp":
                        encoder = new BmpBitmapEncoder();
                        break;
                    default:
                        encoder = new PngBitmapEncoder();
                        break;
                }
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    encoder.Save(fs);
                }
                StatusText.Text = $"Изображение сохранено: {filePath}";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Ошибка при сохранении файла: {ex.Message}";
            }
        }
        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PNG Image|*.png|JPEG Image|*.jpg|BMP Image|*.bmp",
                Title = "Сохранить рисунок"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                currentFilePath = saveFileDialog.FileName;
                SaveToFile(currentFilePath);
            }
        }
        private void OpenImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Изображения (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|Все файлы (*.*)|*.*",
                Title = "Открыть изображение"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                if (DrawingCanvas.Children.Count > 0)
                {
                    var existingImageControl = DrawingCanvas.Children[0] as Image; 
                    if (existingImageControl != null)
                    {
                        existingImageControl.Source = null;
                    }
                    DrawingCanvas.Children.Clear(); 
                }
                currentFilePath = openFileDialog.FileName;

                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(currentFilePath);
                image.EndInit();

                var newImageControl = new Image
                {
                    Source = image,
                    Width = image.PixelWidth,
                    Height = image.PixelHeight
                };
                DrawingCanvas.Children.Add(newImageControl);
            }
        }
        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            currentFilePath = null;
            if (DrawingCanvas.Children.Count > 0)
            {
                var existingImageControl = DrawingCanvas.Children[0] as Image; 
                if (existingImageControl != null)
                {
                    existingImageControl.Source = null; 
                }
                DrawingCanvas.Children.Clear(); 
            }
            StatusText.Text = "Создан новый файл.";
        }
        private void FloodFill(Canvas canvas, Point startPoint, SolidColorBrush fillColor)
        {
            // Преобразуем координаты с учетом масштаба
            double scaleX = CanvasScaleTransform.ScaleX;
            double scaleY = CanvasScaleTransform.ScaleY;
            int x = (int)(startPoint.X / scaleX);
            int y = (int)(startPoint.Y / scaleY);

            // Получаем цвет пикселя в точке начала заливки
            Color targetColor = GetColorAtPixel(canvas, new Point(x, y));

            // Если цвет уже совпадает с цветом заливки, ничего не делаем
            if (targetColor == fillColor.Color)
            {
                return;
            }

            // Создаем WriteableBitmap для работы с пикселями
            RenderTargetBitmap renderTarget = new RenderTargetBitmap(
                (int)(canvas.ActualWidth * scaleX),  // Ширина с учетом масштаба
                (int)(canvas.ActualHeight * scaleY), // Высота с учетом масштаба
                96,                                  // DPI по горизонтали
                96,                                  // DPI по вертикали
                PixelFormats.Pbgra32                 // Формат пикселей
            );

            // Рендерим холст в RenderTargetBitmap
            renderTarget.Render(canvas);

            // Создаем WriteableBitmap для редактирования пикселей
            WriteableBitmap writableBitmap = new WriteableBitmap(renderTarget);

            // Получаем размеры холста
            int width = writableBitmap.PixelWidth;
            int height = writableBitmap.PixelHeight;

            // Создаем массив для хранения пикселей
            int[] pixels = new int[width * height];
            writableBitmap.CopyPixels(pixels, width * 4, 0);

            // Создаем очередь для хранения точек, которые нужно обработать
            Queue<Point> queue = new Queue<Point>();
            queue.Enqueue(new Point(x, y));

            // Создаем массив для отслеживания посещенных точек
            bool[,] visited = new bool[width, height];

            // Цвет заливки в формате ARGB
            int fillColorArgb = (fillColor.Color.A << 24) | (fillColor.Color.R << 16) | (fillColor.Color.G << 8) | fillColor.Color.B;

            // Целевой цвет в формате ARGB
            int targetColorArgb = (targetColor.A << 24) | (targetColor.R << 16) | (targetColor.G << 8) | targetColor.B;

            // Пока очередь не пуста, обрабатываем точки
            while (queue.Count > 0)
            {
                Point currentPoint = queue.Dequeue();
                int currentX = (int)currentPoint.X;
                int currentY = (int)currentPoint.Y;

                // Проверяем, находится ли точка в пределах холста
                if (currentX < 0 || currentX >= width || currentY < 0 || currentY >= height)
                {
                    continue;
                }

                // Проверяем, была ли точка уже посещена
                if (visited[currentX, currentY])
                {
                    continue;
                }

                // Получаем цвет текущего пикселя
                int pixelIndex = currentY * width + currentX;
                int currentColorArgb = pixels[pixelIndex];

                // Если цвет текущего пикселя совпадает с целевым цветом, заливаем его
                if (currentColorArgb == targetColorArgb)
                {
                    // Заливаем пиксель
                    pixels[pixelIndex] = fillColorArgb;

                    // Помечаем точку как посещенную
                    visited[currentX, currentY] = true;

                    // Добавляем соседние точки в очередь
                    queue.Enqueue(new Point(currentX + 1, currentY));
                    queue.Enqueue(new Point(currentX - 1, currentY));
                    queue.Enqueue(new Point(currentX, currentY + 1));
                    queue.Enqueue(new Point(currentX, currentY - 1));
                }
            }

            // Обновляем WriteableBitmap с новыми пикселями
            writableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0);

            // Очищаем холст и добавляем обновленное изображение
            canvas.Children.Clear();
            Image image = new Image { Source = writableBitmap };
            canvas.Children.Add(image);
        }

        public Color GetColorAtPixel(Canvas canvas, Point position)
        {
            try
            {
                if (position.X < 0 || position.X >= canvas.ActualWidth ||
                    position.Y < 0 || position.Y >= canvas.ActualHeight)
                {
                    throw new ArgumentOutOfRangeException(nameof(position), "Точка находится за пределами холста.");
                }
                RenderTargetBitmap renderTarget = new RenderTargetBitmap(
                    (int)canvas.ActualWidth,  
                    (int)canvas.ActualHeight, 
                    96,                       
                    96,                      
                    PixelFormats.Pbgra32       
                );
                renderTarget.Render(canvas);
                byte[] pixel = new byte[4];
                renderTarget.CopyPixels(new Int32Rect((int)position.X, (int)position.Y, 1, 1), pixel, 4, 0);
                return Color.FromArgb(pixel[3], pixel[2], pixel[1], pixel[0]);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при получении цвета: {ex.Message}");
                return Colors.Transparent;
            }
        }
        private Point GetUnscaledPoint(Point point)
        {
            double scaleX = CanvasScaleTransform.ScaleX;
            double scaleY = CanvasScaleTransform.ScaleY;
            return new Point(point.X / scaleX, point.Y / scaleY);
        }
        private void Text_Click(object sender, RoutedEventArgs e)
        {
            isTextModeActive = true;
            isSelecting = true;
        }
        private void AddTextToCanvas(Point position, System.Drawing.Font font)
        {
            // Создаем TextBox для ввода текста
            System.Windows.Controls.TextBox textBox = new System.Windows.Controls.TextBox
            {
                Width = selectionBounds.Width, // Ширина выделенной области
                Height = selectionBounds.Height, // Высота выделенной области
                FontSize = (double)font.Size, // Размер шрифта
                FontFamily = new FontFamily(font.Name), // Шрифт
                Foreground = currentColor, // Цвет текста
                Background = Brushes.Transparent, // Прозрачный фон
                BorderThickness = new Thickness(0), // Без рамки
                AcceptsReturn = true, // Разрешаем перенос строк
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left
            };

            // Позиционируем TextBox на холсте
            Canvas.SetLeft(textBox, position.X);
            Canvas.SetTop(textBox, position.Y);

            // Добавляем TextBox на холст
            DrawingCanvas.Children.Add(textBox);

            // Фокусируем TextBox для ввода текста
            textBox.Focus();

            // Обработчик завершения ввода текста
            textBox.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    // Преобразуем TextBox в TextBlock после завершения ввода
                    TextBlock textBlock = new TextBlock
                    {
                        Text = textBox.Text, // Текст, введенный пользователем
                        FontSize = textBox.FontSize,
                        FontFamily = textBox.FontFamily,
                        Foreground = textBox.Foreground,
                        Background = Brushes.Transparent
                    };

                    // Позиционируем TextBlock на холсте
                    Canvas.SetLeft(textBlock, Canvas.GetLeft(textBox));
                    Canvas.SetTop(textBlock, Canvas.GetTop(textBox));

                    // Удаляем TextBox и добавляем TextBlock
                    DrawingCanvas.Children.Remove(textBox);
                    DrawingCanvas.Children.Add(textBlock);

                    // Открываем меню изменения шрифта
                    OpenFontMenu(textBlock);
                }
            };
        }
        private void OpenFontMenu(TextBlock textBlock)
        {
            // Создаем контекстное меню
            System.Windows.Controls.ContextMenu fontMenu = new System.Windows.Controls.ContextMenu()
            {
                StaysOpen = true // Меню не будет закрываться автоматически
            };

            // Добавляем пункты меню для изменения шрифта
            System.Windows.Controls.MenuItem fontFamilyItem = new System.Windows.Controls.MenuItem { Header = "Шрифт" };
            System.Windows.Controls.MenuItem fontSizeItem = new System.Windows.Controls.MenuItem { Header = "Размер шрифта" };

            // Добавляем подменю для выбора шрифта
            foreach (var fontFamily in Fonts.SystemFontFamilies)
            {
                System.Windows.Controls.MenuItem fontItem = new System.Windows.Controls.MenuItem
                {
                    Header = fontFamily.Source,
                    Tag = fontFamily,
                    StaysOpenOnClick = true // Подменю не будет закрываться после выбора
                };
                fontItem.Click += (sender, e) =>
                {
                    textBlock.FontFamily = (FontFamily)((System.Windows.Controls.MenuItem)sender).Tag;
                };
                fontFamilyItem.Items.Add(fontItem);
            }

            // Добавляем подменю для выбора размера шрифта
            for (int i = 8; i <= 72; i += 2)
            {
                System.Windows.Controls.MenuItem sizeItem = new System.Windows.Controls.MenuItem
                {
                    Header = i.ToString(),
                    Tag = (double)i, // Сохраняем значение как double
                    StaysOpenOnClick = true // Подменю не будет закрываться после выбора
                };
                sizeItem.Click += (sender, e) =>
                {
                    // Приводим Tag к double
                    textBlock.FontSize = (double)((System.Windows.Controls.MenuItem)sender).Tag;
                };
                fontSizeItem.Items.Add(sizeItem);
            }

            // Добавляем пункты меню для жирного шрифта, курсива и подчеркивания
            System.Windows.Controls.MenuItem boldItem = new System.Windows.Controls.MenuItem
            {
                Header = "Жирный",
                IsCheckable = true,
                IsChecked = textBlock.FontWeight == FontWeights.Bold,
                StaysOpenOnClick = true
            };
            boldItem.Click += (sender, e) =>
            {
                textBlock.FontWeight = boldItem.IsChecked ? FontWeights.Bold : FontWeights.Normal;
            };

            System.Windows.Controls.MenuItem italicItem = new System.Windows.Controls.MenuItem
            {
                Header = "Курсив",
                IsCheckable = true,
                IsChecked = textBlock.FontStyle == FontStyles.Italic,
                StaysOpenOnClick = true
            };
            italicItem.Click += (sender, e) =>
            {
                textBlock.FontStyle = italicItem.IsChecked ? FontStyles.Italic : FontStyles.Normal;
            };

            System.Windows.Controls.MenuItem underlineItem = new System.Windows.Controls.MenuItem
            {
                Header = "Подчеркивание",
                IsCheckable = true,
                IsChecked = textBlock.TextDecorations != null && textBlock.TextDecorations.Contains(TextDecorations.Underline[0]),
                StaysOpenOnClick = true
            };
            underlineItem.Click += (sender, e) =>
            {
                if (underlineItem.IsChecked)
                {
                    textBlock.TextDecorations = TextDecorations.Underline;
                }
                else
                {
                    textBlock.TextDecorations = null;
                }
            };

            // Добавляем пункты меню в контекстное меню
            fontMenu.Items.Add(fontFamilyItem);
            fontMenu.Items.Add(fontSizeItem);
            fontMenu.Items.Add(new System.Windows.Controls.Separator()); // Разделитель
            fontMenu.Items.Add(boldItem);
            fontMenu.Items.Add(italicItem);
            fontMenu.Items.Add(underlineItem);

            // Открываем контекстное меню
            textBlock.ContextMenu = fontMenu;
            textBlock.ContextMenu.IsOpen = true;
        }
        private void BrushSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BrushSelector.SelectedItem is ComboBoxItem selectedItem)
            {
                currentBrush = selectedItem.Tag.ToString();
            }
        }
        private Brush ApplyBrushEffect()
        {
            SolidColorBrush solidColorBrush = currentColor as SolidColorBrush;

            switch (currentBrush)
            {
                case "Кисть 1":
                    return new SolidColorBrush(solidColorBrush?.Color ?? Colors.Black); // Обычная кисть
                case "Кисть 2":
                    return new SolidColorBrush(Color.FromArgb(128, solidColorBrush?.Color.R ?? 0, solidColorBrush?.Color.G ?? 0, solidColorBrush?.Color.B ?? 0)); // Полупрозрачная кисть
                case "Кисть 3":
                    return new LinearGradientBrush(Colors.Red, Colors.Yellow, 45); // Градиентная кисть
                case "Кисть 4":
                    return new RadialGradientBrush(Colors.Blue, Colors.Transparent); // Радиальная кисть
                default:
                    return new SolidColorBrush(solidColorBrush?.Color ?? Colors.Black); // По умолчанию
            }
        }
        public void UpdateUndoStack()
        {
            List<UIElement> currentElements = new List<UIElement>();
            foreach (UIElement element in DrawingCanvas.Children)
            {
                currentElements.Add(element);
            }
            undoStack.Push(currentElements);
            redoStack.Clear(); // Очищаем стек возврата при новом действии
        }

        public void Undo()
        {
            if (undoStack.Count > 0)
            {
                List<UIElement> lastState = undoStack.Pop();
                redoStack.Push(new List<UIElement>(DrawingCanvas.Children.Cast<UIElement>()));
                DrawingCanvas.Children.Clear();

                foreach (var element in lastState)
                {
                    DrawingCanvas.Children.Add(element);
                }
            }
        }

        private void Redo()
        {
            if (redoStack.Count > 0)
            {
                List<UIElement> redoState = redoStack.Pop();
                undoStack.Push(new List<UIElement>(DrawingCanvas.Children.Cast<UIElement>()));
                DrawingCanvas.Children.Clear();

                foreach (var element in redoState)
                {
                    DrawingCanvas.Children.Add(element);
                }
            }
        }

        ///Canvas_MouseDown
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UpdateUndoStack();

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                startPoint = e.GetPosition(DrawingCanvas);

                // Преобразуем координаты с учетом масштаба
                var unscaledStartPoint = GetUnscaledPoint(startPoint);

                if (DrawingCanvas.ActualWidth <= 0 || DrawingCanvas.ActualHeight <= 0)
                {
                    System.Windows.MessageBox.Show("Холст не инициализирован или имеет нулевые размеры.");
                    return;
                }

                // Ограничиваем координаты внутри DrawingCanvas
                unscaledStartPoint.X = Math.Max(0, Math.Min(unscaledStartPoint.X, DrawingCanvas.ActualWidth));
                unscaledStartPoint.Y = Math.Max(0, Math.Min(unscaledStartPoint.Y, DrawingCanvas.ActualHeight));

                if (isColorPickerActive)
                {
                    // Захват цвета под курсором мыши
                    Color color = GetColorAtPixel(DrawingCanvas, unscaledStartPoint);

                    // Преобразуем Color в SolidColorBrush
                    currentColor = new SolidColorBrush(color);

                    // Применяем цвет к Border
                    CurrentColorDisplay.Fill = currentColor;

                    // Деактивируем режим "пипетки" после выбора цвета
                    isColorPickerActive = false;
                    System.Windows.MessageBox.Show($"Выбран цвет: {color}");
                }
                else if (isTextModeActive)
                {
                    // Начало выделения текста
                    isSelecting = true;
                    selectionStart = unscaledStartPoint;

                    selectionRect = new Rectangle
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 1,
                        Fill = Brushes.Transparent
                    };

                    Canvas.SetLeft(selectionRect, selectionStart.X);
                    Canvas.SetTop(selectionRect, selectionStart.Y);
                    DrawingCanvas.Children.Add(selectionRect);
                }
                else if (currentTool == "Select")
                {
                    // Начало выделения объекта
                    RemoveSelection();
                    isSelecting = true;
                    selectionStart = unscaledStartPoint;

                    selectionRect = new Rectangle
                    {
                        Stroke = Brushes.Blue,
                        StrokeDashArray = new DoubleCollection { 4, 2 },
                        StrokeThickness = 1,
                        Fill = Brushes.Transparent
                    };

                    Canvas.SetLeft(selectionRect, selectionStart.X);
                    Canvas.SetTop(selectionRect, selectionStart.Y);
                    DrawingCanvas.Children.Add(selectionRect);
                }
                else if (currentTool == "Fill")
                {
                    // Преобразуем координаты с учетом масштаба
                    var unscaledFillPoint = GetUnscaledPoint(startPoint);

                    // Проверка на выход за границы холста
                    if (unscaledFillPoint.X < 0 || unscaledFillPoint.X >= DrawingCanvas.ActualWidth ||
                        unscaledFillPoint.Y < 0 || unscaledFillPoint.Y >= DrawingCanvas.ActualHeight)
                    {
                        System.Windows.MessageBox.Show("Точка заливки находится вне границ холста.");
                        return;
                    }

                    // Вызываем заливку
                    if (currentColor is SolidColorBrush solidColorBrush)
                    {
                        FloodFill(DrawingCanvas, unscaledFillPoint, solidColorBrush);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Заливка поддерживает только SolidColorBrush.");
                    }
                }
                else if (currentTool == "Eraser")
                {
                    eraserLine = new Polyline
                    {
                        Stroke = Brushes.White, // Цвет ластика (фон холста)
                        StrokeThickness = currentThickness,
                        StrokeLineJoin = PenLineJoin.Round,
                        StrokeStartLineCap = PenLineCap.Round,
                        StrokeEndLineCap = PenLineCap.Round
                    };
                    eraserLine.Points.Add(unscaledStartPoint);
                    AddObjectToCurrentLayer(eraserLine);
                }
                else
                {
                    // Логика для других инструментов
                    switch (currentTool)
                    {
                        case "Pencil":
                            freehandLine = new Polyline
                            {
                                Stroke = ApplyBrushEffect(),
                                StrokeThickness = currentThickness,
                                StrokeLineJoin = PenLineJoin.Round,
                                StrokeStartLineCap = PenLineCap.Round,
                                StrokeEndLineCap = PenLineCap.Round
                            };
                            freehandLine.Points.Add(unscaledStartPoint);
                            AddObjectToCurrentLayer(freehandLine);
                            break;

                        default:
                            currentShape = ShapeFactory.CreateShape(currentTool, unscaledStartPoint, ApplyBrushEffect(), currentThickness);
                            AddObjectToCurrentLayer(currentShape);
                            break;
                    }
                }
                isDrawing = true;
            }
        }


        ///Canvas_MouseMove 
        public void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isDrawing)
            {
                // Получаем текущие координаты курсора относительно DrawingCanvas
                var endPoint = e.GetPosition(DrawingCanvas);

                // Преобразуем координаты с учетом масштаба
                double scaleX = CanvasScaleTransform.ScaleX;
                double scaleY = CanvasScaleTransform.ScaleY;
                var unscaledEndPoint = GetUnscaledPoint(endPoint);

                // Ограничиваем координаты внутри DrawingCanvas
                unscaledEndPoint.X = Math.Max(0, Math.Min(unscaledEndPoint.X, DrawingCanvas.ActualWidth));
                unscaledEndPoint.Y = Math.Max(0, Math.Min(unscaledEndPoint.Y, DrawingCanvas.ActualHeight));

                // Обновляем положение горизонтального ползунка
                if (HorizontalRuler != null && HorizontalRuler.Visibility == Visibility.Visible)
                {
                    Canvas.SetLeft(HorizontalSlider, unscaledEndPoint.X);
                }

                // Обновляем положение вертикального ползунка
                if (VerticalRuler != null && VerticalRuler.Visibility == Visibility.Visible)
                {
                    Canvas.SetTop(VerticalSlider, unscaledEndPoint.Y);
                }

                // Обновляем текст позиции курсора
                CursorPositionText.Text = $"X: {(int)unscaledEndPoint.X}, Y: {(int)unscaledEndPoint.Y}";

                if (isSelecting && selectionRect != null)
                {
                    // Преобразуем начальную точку выделения с учетом масштаба
                    var unscaledStartPoint = new Point(selectionStart.X / scaleX, selectionStart.Y / scaleY);

                    // Обновляем размеры и позицию прямоугольника выделения
                    double x = Math.Min(unscaledStartPoint.X, unscaledEndPoint.X);
                    double y = Math.Min(unscaledStartPoint.Y, unscaledEndPoint.Y);
                    double width = Math.Abs(unscaledEndPoint.X - unscaledStartPoint.X);
                    double height = Math.Abs(unscaledEndPoint.Y - unscaledStartPoint.Y);

                    Canvas.SetLeft(selectionRect, x);
                    Canvas.SetTop(selectionRect, y);
                    selectionRect.Width = width;
                    selectionRect.Height = height;

                    selectionBounds = new Rect(x, y, width, height);
                }
                else if (currentTool == "Eraser" && eraserLine != null)
                {
                    // Добавляем точку к ластику (без преобразования, так как это относительные координаты)
                    eraserLine.Points.Add(unscaledEndPoint);
                }
                else if (currentTool == "Pencil" && freehandLine != null)
                {
                    // Добавляем точку к линии (без преобразования, так как это относительные координаты)
                    freehandLine.Points.Add(unscaledEndPoint);
                }
                else if (currentShape != null)
                {
                    // Преобразуем начальную точку с учетом масштаба
                    var unscaledStartPoint = new Point(startPoint.X / scaleX, startPoint.Y / scaleY);

                    // Обновляем фигуру с учетом реальных координат
                    ShapeUpdater.UpdateShape(currentShape, currentTool, unscaledStartPoint, unscaledEndPoint);
                }
            }
        }

        ///Canvas_MouseUp      
        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (currentTool == "Select" && isSelecting)
            {
                // Режим выделения области
                isSelecting = false;
                isSelectionActive = true;
                StatusText.Text = "Область выделена";
            }
            else if (isTextModeActive && isSelecting)
            {
                // Режим добавления текста
                isSelecting = false;

                // Удаляем прямоугольник выделения
                DrawingCanvas.Children.Remove(selectionRect);
                selectionRect = null;

                // Добавляем TextBox для ввода текста в выделенную область
                System.Drawing.Font defaultFont = new System.Drawing.Font("Arial", 16); // Шрифт по умолчанию
                AddTextToCanvas(selectionBounds.TopLeft, defaultFont);
            }
            else
            {
                // Сброс состояний
                ResetStates();
                StatusText.Text = "Готово";
            }
        }
        private void Canvas_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete && isSelectionActive) // Проверяем, активно ли выделение
            {
                // Удаляем все объекты, которые пересекаются с выделенной областью
                var elementsToRemove = DrawingCanvas.Children.OfType<UIElement>()
                    .Where(element =>
                    {
                        if (element is Polyline polyline)
                        {
                            // Специальная проверка для Polyline
                            return PolylineIntersectsWith(polyline, selectionBounds);
                        }
                        else if (element is Shape shape)
                        {
                            // Проверка для других фигур
                            var elementBounds = new Rect(
                                Canvas.GetLeft(shape),
                                Canvas.GetTop(shape),
                                shape.Width,
                                shape.Height
                            );
                            return selectionBounds.IntersectsWith(elementBounds);
                        }
                        else
                        {
                            // Стандартная проверка для других объектов
                            var elementBounds = new Rect(
                                Canvas.GetLeft(element),
                                Canvas.GetTop(element),
                                element.RenderSize.Width,
                                element.RenderSize.Height
                            );
                            return selectionBounds.IntersectsWith(elementBounds);
                        }
                    })
                    .ToList();

                foreach (var element in elementsToRemove)
                {
                    DrawingCanvas.Children.Remove(element);
                }

                // Удаляем прямоугольник выделения
                RemoveSelection();

                // Сбрасываем флаг выделения
                isSelectionActive = false;
                StatusText.Text = "Выделенная область и объекты удалены";
            }
        }
        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                double scaleFactor = e.Delta > 0 ? 1.1 : 0.9; 
                ScaleSlider.Value *= scaleFactor;
                e.Handled = true;
            }
        }
        private void RemoveSelection()
        {
            if (selectionRect != null)
            {
                DrawingCanvas.Children.Remove(selectionRect);
                selectionRect = null;
            }
            isSelectionActive = false;
        }
        private void ResetStates()
        {
            isDrawing = false;
            isSelecting = false;
            isSelectionActive = false;
            isSelectionModeActive = false;
            isTextModeActive = false;
            if (selectionRect != null)
            {
                DrawingCanvas.Children.Remove(selectionRect);
                selectionRect = null;
            }
            currentShape = null;
            freehandLine = null;
        }
        private bool PolylineIntersectsWith(Polyline polyline, Rect selectionBounds)
        {
            foreach (var point in polyline.Points)
            {
                if (selectionBounds.Contains(point))
                {
                    return true;
                }
            }
            return false;
        }
        private void SelectTool_Click(object sender, RoutedEventArgs e)
        {
            currentTool = "Select";
            StatusText.Text = "Инструмент: Выделение";
        }
        private void ResizeCanvas_Click(object sender, RoutedEventArgs e)
        {
            var resizeDialog = new ResizeDialog();
            if (resizeDialog.ShowDialog() == true)
            {
                DrawingCanvas.Width = resizeDialog.CanvasWidth;
                DrawingCanvas.Height = resizeDialog.CanvasHeight;
                StatusText.Text = $"Размер холста изменен: {resizeDialog.CanvasWidth}x{resizeDialog.CanvasHeight}";
            }
        }
        private void ScaleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (CanvasScaleTransform != null)
            {
                double newScale = Math.Max(0.1, Math.Min(8.0, ScaleSlider.Value));
                CanvasScaleTransform.ScaleX = newScale;
                CanvasScaleTransform.ScaleY = newScale;
                StatusText.Text = $"{(int)(newScale * 100)}%";
            }
        }
        private void Rotate90Right_Click(object sender, RoutedEventArgs e)
        {
            ApplyTransformToSelectedObjects(new RotateTransform(90));
        }
        private void Rotate90Left_Click(object sender, RoutedEventArgs e)
        {
            ApplyTransformToSelectedObjects(new RotateTransform(-90));
        }
        private void Rotate180_Click(object sender, RoutedEventArgs e)
        {
            ApplyTransformToSelectedObjects(new RotateTransform(180));
        }
        private void FlipHorizontal_Click(object sender, RoutedEventArgs e)
        {
            ApplyTransformToSelectedObjects(new ScaleTransform { ScaleX = -1, ScaleY = 1 });
        }
        private void FlipVertical_Click(object sender, RoutedEventArgs e)
        {
            ApplyTransformToSelectedObjects(new ScaleTransform { ScaleX = 1, ScaleY = -1 });
        }
        private void ApplyTransformToSelectedObjects(Transform transform)
        {
            foreach (var child in DrawingCanvas.Children)
            {
                if (child is FrameworkElement element)
                {
                    if (element.RenderTransform is TransformGroup transformGroup)
                    {
                        transformGroup.Children.Add(transform);
                    }
                    else
                    {
                        var group = new TransformGroup();
                        group.Children.Add(element.RenderTransform);
                        group.Children.Add(transform);
                        element.RenderTransform = group;
                    }
                    EnsureElementWithinCanvasBounds(element);
                }
            }
        }
        private void EnsureElementWithinCanvasBounds(FrameworkElement element)
        {
            double left = Canvas.GetLeft(element);
            double top = Canvas.GetTop(element);
            var transform = element.RenderTransform;
            var bounds = transform.TransformBounds(new Rect(0, 0, element.ActualWidth, element.ActualHeight));
            double offsetX = 0;
            double offsetY = 0;
            if (bounds.Left < 0)
            {
                offsetX = -bounds.Left;
            }
            else if (bounds.Right > DrawingCanvas.ActualWidth)
            {
                offsetX = DrawingCanvas.ActualWidth - bounds.Right;
            }
            if (bounds.Top < 0)
            {
                offsetY = -bounds.Top;
            }
            else if (bounds.Bottom > DrawingCanvas.ActualHeight)
            {
                offsetY = DrawingCanvas.ActualHeight - bounds.Bottom;
            }
            Canvas.SetLeft(element, left + offsetX);
            Canvas.SetTop(element, top + offsetY);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
        private void ApplyThemeToCanvasAndLabels(bool isDarkTheme)
        {
            ThemeManager.ApplyThemeToLabel(label1, isDarkTheme);
            ThemeManager.ApplyThemeToLabel(label2, isDarkTheme);
            ThemeManager.ApplyThemeToLabel(label3, isDarkTheme);
            ThemeManager.ApplyThemeToLabel(label4, isDarkTheme);
            ThemeManager.ApplyThemeToLabel(label5, isDarkTheme);
            ThemeManager.ApplyThemeToLabel(label6, isDarkTheme);
            ThemeManager.ApplyThemeToDockPanel(UprSloi, isDarkTheme);
            ThemeManager.ApplyThemeToShapes(DrawingCanvas, isDarkTheme);
        }
        private void WhiteTheme_Click(object sender, RoutedEventArgs e)
        {
            bool isDarkTheme = !ThemeManager.IsDarkTheme;
            ThemeManager.SaveAndApplyTheme(this, isDarkTheme);
            ThemeManager.ApplyTheme(this, false);
            ApplyThemeToCanvasAndLabels(false);
        }

        private void DarkTheme_Click(object sender, RoutedEventArgs e)
        {
            bool isDarkTheme = !ThemeManager.IsDarkTheme;
            ThemeManager.SaveAndApplyTheme(this, isDarkTheme);
            ThemeManager.ApplyTheme(this, true); 
            ApplyThemeToCanvasAndLabels(true);
        }
        private void PalitraColor_Click(object sender, RoutedEventArgs e)
        {
            isColorPickerActive = true;
        }
        private void MenuCancel_Click(object sender, RoutedEventArgs e)
        {
            Undo();
        }
        private void MenuReturn_Click(object sender, RoutedEventArgs e)
        {
            Redo();
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        private void Reference(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Вы находитесь в приложении..\n" +
                                           "Графический Редактор (Graphic Editor)\r\n\n" +
                                           "Приложение называется, 'ASGREIR'\n" +
                                           "Расшифровка:\n" +
                                           "'Advanced System for Graphic Editing and Image Rendering'\n" +
                                           "\nРазработчик: Астахов Максим Евгеньевич\n" +
                                           "Группа: ИС-22\n" +
                                           "Задание по курсовой работе\n" +
                                           "\nИНСТРУКЦИЯ ИСПОЛЬЗОВАНИЯ", "ASGREIR - Справка", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchText = SearchTextBox.Text;
            if (string.IsNullOrEmpty(searchText))
            {
                System.Windows.MessageBox.Show("Введите текст для поиска.");
                return;
            }
            foreach (var toolBar in MainToolBarTray.ToolBars)
            {
                foreach (var child in toolBar.Items)
                {
                    if (child is FrameworkElement element && element.ToolTip != null)
                    {
                        if (element.ToolTip.ToString().Equals(searchText, StringComparison.OrdinalIgnoreCase))
                        {
                            if (element is System.Windows.Controls.Control control)
                            {
                                control.Background = Brushes.Yellow;
                            }
                        }
                        else
                        {
                            if (element is System.Windows.Controls.Control control)
                            {
                                control.Background = Brushes.Transparent;
                            }
                        }
                    }
                }
            }
        }
        private void StatusBarCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (StatusBar != null)
            {
                StatusBar.Visibility = Visibility.Collapsed;
            }
        }
        private void StatusBarCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (StatusBar != null)
            {
                StatusBar.Visibility = Visibility.Visible;
            }
        }
        private void GridLinesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (GridLinesCanvas != null)
            {
                GridLinesCanvas.Children.Clear();

                double step = 30; 
                double width = DrawingCanvas.ActualWidth;
                double height = DrawingCanvas.ActualHeight;
                for (double x = 0; x <= width; x += step)
                {
                    Line line = new Line
                    {
                        X1 = x,
                        Y1 = 0,
                        X2 = x,
                        Y2 = height,
                        Stroke = Brushes.Green,
                        StrokeThickness = 1
                    };
                    GridLinesCanvas.Children.Add(line);
                }
                for (double y = 0; y <= height; y += step)
                {
                    Line line = new Line
                    {
                        X1 = 0,
                        Y1 = y,
                        X2 = width,
                        Y2 = y,
                        Stroke = Brushes.Green,
                        StrokeThickness = 1
                    };
                    GridLinesCanvas.Children.Add(line);
                }
            }
        }
        private void GridLinesCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (GridLinesCanvas != null)
            {
                GridLinesCanvas.Children.Clear();
            }
        }
        private void RulersCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (HorizontalRuler != null && VerticalRuler != null)
            {
                HorizontalRuler.Visibility = Visibility.Visible;
                VerticalRuler.Visibility = Visibility.Visible;
            }
        }
        private void RulersCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (HorizontalRuler != null && VerticalRuler != null)
            {
                HorizontalRuler.Visibility = Visibility.Collapsed;
                VerticalRuler.Visibility = Visibility.Collapsed;
            }
        }
        private void CheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.CheckBox checkBox && checkBox.DataContext is Layer layer)
            {
                ToggleLayerVisibility(layer, checkBox.IsChecked == true);
            }
        }
        private void ShowLayersCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UprSloi.Visibility = Visibility.Visible;
            DrawingCanvas.Margin = new Thickness(300, 0, 0, 0); 
        }
        private void ShowLayersCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            UprSloi.Visibility = Visibility.Collapsed;
            DrawingCanvas.Margin = new Thickness(0); 
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeInAnimation = new DoubleAnimation
            {
                From = 0, 
                To = 1,  
                Duration = new Duration(TimeSpan.FromSeconds(0.4)) 
            };
            this.BeginAnimation(OpacityProperty, fadeInAnimation);
        }
        private void Print_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.PrintDialog printDialog = new System.Windows.Controls.PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(DrawingCanvas, "Печать холста");
            }
        }
        private void ViewPrint_Click(object sender, RoutedEventArgs e)
        {
            DrawingCanvas.UpdateLayout();
            FlowDocument doc = new FlowDocument();
            doc.PagePadding = new Thickness(50);
            doc.ColumnGap = 0;
            doc.ColumnWidth = double.MaxValue; 
            RenderTargetBitmap rtb = new RenderTargetBitmap(
                (int)DrawingCanvas.ActualWidth,
                (int)DrawingCanvas.ActualHeight,
                96, 96, PixelFormats.Pbgra32);
            rtb.Render(DrawingCanvas);
            Image canvasImage = new Image();
            canvasImage.Source = rtb;
            doc.Blocks.Add(new BlockUIContainer(canvasImage));
            PrintPreviewWindow previewWindow = new PrintPreviewWindow(doc);
            previewWindow.ShowDialog();
        }
        private void FullCanvas_Click(object sender, RoutedEventArgs e)
        {
            FullScreenWindow fullScreenWindow = new FullScreenWindow(DrawingCanvas);
            fullScreenWindow.ShowDialog();
        }
    }
}
    
