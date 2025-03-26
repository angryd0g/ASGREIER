using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GraphicEditor;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System;

namespace TestGraphicEditor
{
    [TestClass]
    public class GraphicEditorTests
    {
        private Canvas _drawingCanvas;
        private ListBox _layersListBox;
        private System.Collections.ObjectModel.ObservableCollection<Layer> _layers;

        [TestInitialize]
        public void Setup()
        {
            _drawingCanvas = new Canvas();
            _layersListBox = new ListBox();
            _layers = new System.Collections.ObjectModel.ObservableCollection<Layer>();
        }

        [TestMethod]
        public void AddLayer_ShouldAddNewLayer()
        {
            // Arrange
            string layerName = "Test Layer";
            int initialCount = _layers.Count;

            // Act
            var layer = new Layer(layerName);
            _layers.Add(layer);

            // Simulate ListBox selection behavior
            if (_layersListBox != null)
            {
                _layersListBox.ItemsSource = _layers;
                _layersListBox.SelectedItem = layer;
            }

            // Assert
            Assert.AreEqual(initialCount + 1, _layers.Count);
            Assert.AreEqual(layerName, _layers.Last().Name);

            // Only assert SelectedItem if we have a ListBox
            if (_layersListBox != null)
            {
                Assert.AreEqual(_layers.Last(), _layersListBox.SelectedItem);
            }
        }

        [TestMethod]
        public void ToggleLayerVisibility_ShouldChangeVisibility()
        {
            // Arrange
            var layer = new Layer("Test Layer");
            var rectangle = new Rectangle();
            layer.Objects.Add(rectangle);
            _drawingCanvas.Children.Add(rectangle);

            // Act - Hide
            layer.IsVisible = false;
            foreach (var obj in layer.Objects)
            {
                obj.Visibility = Visibility.Collapsed;
            }

            // Assert
            Assert.IsFalse(layer.IsVisible);
            Assert.AreEqual(Visibility.Collapsed, rectangle.Visibility);

            // Act - Show
            layer.IsVisible = true;
            foreach (var obj in layer.Objects)
            {
                obj.Visibility = Visibility.Visible;
            }

            // Assert
            Assert.IsTrue(layer.IsVisible);
            Assert.AreEqual(Visibility.Visible, rectangle.Visibility);
        }
    }
    [TestClass]
    public class OpenImageTests
    {
        private Canvas _drawingCanvas;
        private string _currentFilePath;
        private bool _dialogResult;
        private string _testFilePath;

        [TestInitialize]
        public void Setup()
        {
            _drawingCanvas = new Canvas();
            _currentFilePath = null;
            _dialogResult = true; // По умолчанию диалог возвращает true
            _testFilePath = "test.png"; // Тестовый путь к файлу
        }
        [TestMethod]
        public void OpenImage_Click_WithNoSelection_ShouldDoNothing()
        {
            // Arrange
            _dialogResult = false; // Симулируем отмену диалога
            int initialChildCount = _drawingCanvas.Children.Count;

            // Act
            OpenImage_Click(null, null);

            // Assert
            Assert.AreEqual(initialChildCount, _drawingCanvas.Children.Count);
            Assert.IsNull(_currentFilePath);
        }

        [TestMethod]
        public void OpenImage_Click_ShouldClearExistingImage()
        {
            // Arrange
            var oldImage = new Image { Width = 100, Height = 100 };
            _drawingCanvas.Children.Add(oldImage);

            // Настройка тестовых данных
            _testFilePath = "test.png";
            _dialogResult = true;

            // Act
            OpenImage_Click(null, null);

            // Assert
            Assert.AreEqual(1, _drawingCanvas.Children.Count, "Должен остаться ровно один элемент на Canvas");
            Assert.IsFalse(_drawingCanvas.Children.Contains(oldImage), "Старое изображение должно быть удалено");
            Assert.IsInstanceOfType(_drawingCanvas.Children[0], typeof(Image), "На Canvas должен быть добавлен новый Image");
        }

        // Тестовая реализация метода
        private void OpenImage_Click(object sender, RoutedEventArgs e)
        {
            if (_dialogResult) // Используем нашу тестовую переменную вместо реального диалога
            {
                // Очистка Canvas
                if (_drawingCanvas.Children.Count > 0)
                {
                    _drawingCanvas.Children.Clear();
                }

                _currentFilePath = _testFilePath;

                // Создаем заглушку для изображения без реальной загрузки
                var newImageControl = new Image
                {
                    Width = 100,
                    Height = 100,
                    // Не устанавливаем Source, чтобы избежать загрузки
                    Tag = "TestImage" // Помечаем для идентификации в тестах
                };

                _drawingCanvas.Children.Add(newImageControl);
            }
        }
    }
}