using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphicEditor
{
    class Layer
    {
        public string Name { get; set; }
        public bool IsVisible { get; set; } = true;
        public ObservableCollection<UIElement> Objects { get; set; } = new ObservableCollection<UIElement>();

        public Layer(string name)
        {
            Name = name;
        }
    }
}
