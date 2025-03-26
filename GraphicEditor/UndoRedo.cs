using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows;

namespace GraphicEditor
{
    class UndoRedo : Canvas
    {
        public Stack<UIElement> _undoStack = new Stack<UIElement>();
        public Stack<UIElement> _redoStack = new Stack<UIElement>();

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        public void Undo()
        {
            if (CanUndo)
            {
                // Извлекаем последний элемент из стека отмены
                var element = _undoStack.Pop();

                // Удаляем элемент с холста
                Children.Remove(element);

                // Добавляем элемент в стек возврата
                _redoStack.Push(element);
            }
        }

        public void Redo()
        {
            if (CanRedo)
            {
                var element = _redoStack.Pop();
                _undoStack.Push(element);
                Children.Add(element);
            }
        }
        public void AddElement(UIElement element)
        {
            _undoStack.Push(element);
            _redoStack.Clear();
            Children.Add(element);
        }
    }
}
