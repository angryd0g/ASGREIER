using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GraphicEditor
{
    public class PrintPreviewWindow : Window
    {
        public PrintPreviewWindow(FlowDocument document)
        {
            Title = "Предварительный просмотр печати";
            Width = 720;
            Height = 480;

            // Создаем ScrollViewer для прокрутки содержимого
            ScrollViewer scrollViewer = new ScrollViewer();

            // Создаем Image для отображения содержимого документа
            Image image = new Image();

            // Преобразуем FlowDocument в изображение
            var paginator = ((IDocumentPaginatorSource)document).DocumentPaginator;
            var visual = new DrawingVisual();
            using (var context = visual.RenderOpen())
            {
                context.DrawRectangle(Brushes.White, null, new Rect(paginator.PageSize));
                for (int i = 0; i < paginator.PageCount; i++)
                {
                    var page = paginator.GetPage(i);
                    context.DrawRectangle(new VisualBrush(page.Visual), null, new Rect(page.Size));
                }
            }

            RenderTargetBitmap rtb = new RenderTargetBitmap(
                (int)paginator.PageSize.Width,
                (int)paginator.PageSize.Height,
                96, 96, PixelFormats.Pbgra32);
            rtb.Render(visual);

            image.Source = rtb;
            scrollViewer.Content = image;
            Content = scrollViewer;
        }
    }
}
