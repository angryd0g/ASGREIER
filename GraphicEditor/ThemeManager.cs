using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using GraphicEditor;

public static class ThemeManager
{

    public static bool IsDarkTheme { get; private set; }
    private static readonly SolidColorBrush LightToolBarFiForegroundBrush = new SolidColorBrush(Colors.Black); // Черный текст для ToolBarFi
    private static readonly SolidColorBrush FileMenuBackgroundBrush = new SolidColorBrush(Color.FromRgb(21, 208, 14)); // Зеленый фон
    // Ресурсы для светлой темы
    private static readonly SolidColorBrush LightBackgroundBrush = new SolidColorBrush(Colors.White);
    private static readonly SolidColorBrush LightMenuBackgroundBrush = new SolidColorBrush(Color.FromRgb(210, 210, 210)); // #FFD2D2D2
    private static readonly SolidColorBrush LightMenuForegroundBrush = new SolidColorBrush(Colors.Black);
    private static readonly SolidColorBrush LightToolBarBackgroundBrush = new SolidColorBrush(Color.FromRgb(230, 230, 230)); // #FFE6E6E6
    private static readonly SolidColorBrush LightStatusBarBackgroundBrush = new SolidColorBrush(Color.FromRgb(230, 230, 230)); // #FFE6E6E6
    private static readonly SolidColorBrush LightViewToolBarBackgroundBrush = new SolidColorBrush(Color.FromRgb(230, 230, 230)); // #FFE6E6E6
    private static readonly SolidColorBrush LightToolBarFiBackgroundBrush = new SolidColorBrush(Color.FromRgb(230, 230, 230)); // Светлый фон для ToolBarFi
    private static readonly SolidColorBrush LightToolBarInsBackgroundBrush = new SolidColorBrush(Color.FromRgb(230, 230, 230)); // Светлый фон для ToolBarIns
    private static readonly SolidColorBrush LightToolBarInsForegroundBrush = new SolidColorBrush(Colors.Black); // Черный текст для ToolBarIns


    // Ресурсы для темной темы
    private static readonly SolidColorBrush DarkBackgroundBrush = new SolidColorBrush(Color.FromRgb(45, 45, 45)); // #FF2D2D2D
    private static readonly SolidColorBrush DarkMenuBackgroundBrush = new SolidColorBrush(Color.FromRgb(60, 60, 60)); // #FF3C3C3C
    private static readonly SolidColorBrush DarkMenuForegroundBrush = new SolidColorBrush(Colors.White);
    private static readonly SolidColorBrush DarkToolBarBackgroundBrush = new SolidColorBrush(Color.FromRgb(80, 80, 80)); // #FF505050
    private static readonly SolidColorBrush DarkStatusBarBackgroundBrush = new SolidColorBrush(Color.FromRgb(45, 45, 45)); // #FF505050
    private static readonly SolidColorBrush DarkViewToolBarBackgroundBrush = new SolidColorBrush(Color.FromRgb(80, 80, 80)); // #FF505050
    private static readonly SolidColorBrush DarkToolBarFiBackgroundBrush = new SolidColorBrush(Color.FromRgb(180, 180, 180)); // Светлый фон для ToolBarFi в темной теме
    private static readonly SolidColorBrush DarkToolBarFiForegroundBrush = new SolidColorBrush(Colors.White); // Белый текст для ToolBarFi в темной теме
    private static readonly SolidColorBrush DarkToolBarInsBackgroundBrush = new SolidColorBrush(Color.FromRgb(180, 180, 180)); // Светлый фон для ToolBarIns в темной теме
    private static readonly SolidColorBrush DarkToolBarInsForegroundBrush = new SolidColorBrush(Colors.White); // Белый текст для ToolBarIns в темной теме

    private const string ThemeConfigFilePath = "theme.config";

    public static void SaveTheme(bool isDarkTheme)
    {
        File.WriteAllText(ThemeConfigFilePath, isDarkTheme ? "Dark" : "Light", Encoding.UTF8);
    }
    public static bool LoadTheme()
    {
        if (File.Exists(ThemeConfigFilePath))
        {
            string theme = File.ReadAllText(ThemeConfigFilePath, Encoding.UTF8);
            return theme == "Dark";
        }
        return false; // По умолчанию светлая тема
    }
    /// <summary>
    /// Применяет тему к окну.
    /// </summary>
    /// <param name="window">Окно, к которому применяется тема.</param>
    /// <param name="isDarkTheme">Если true, применяется темная тема, иначе — светлая.</param>
    public static void ApplyTheme(Window window, bool isDarkTheme)
    {
        IsDarkTheme = isDarkTheme;
        var backgroundBrush = isDarkTheme ? DarkBackgroundBrush : LightBackgroundBrush;
        var menuBackgroundBrush = isDarkTheme ? DarkMenuBackgroundBrush : LightMenuBackgroundBrush;
        var menuForegroundBrush = isDarkTheme ? DarkMenuForegroundBrush : LightMenuForegroundBrush;
        var toolBarBackgroundBrush = isDarkTheme ? DarkToolBarBackgroundBrush : LightToolBarBackgroundBrush;
        var statusBarBackgroundBrush = isDarkTheme ? DarkStatusBarBackgroundBrush : LightStatusBarBackgroundBrush;
        var viewToolBarBackgroundBrush = isDarkTheme ? DarkViewToolBarBackgroundBrush : LightViewToolBarBackgroundBrush;

        // Применение темы к фону окна
        window.Background = backgroundBrush;

        // Применение темы к меню
        var menu = window.FindName("Menu") as Menu;
        if (menu != null)
        {
            menu.Background = menuBackgroundBrush;
            menu.Foreground = menuForegroundBrush;
            SetMenuItemsForeground(menu.Items, menuForegroundBrush, isDarkTheme);
        }

        // Применение темы к ToolBarTray
        var toolBarTray = window.FindName("MainToolBarTray") as ToolBarTray;
        if (toolBarTray != null)
        {
            toolBarTray.Background = toolBarBackgroundBrush;
            foreach (var toolBar in toolBarTray.ToolBars)
            {
                toolBar.Foreground = menuForegroundBrush;
                SetToolBarItemsForeground(toolBar.Items, menuForegroundBrush);
            }
        }

        // Применение темы к ViewToolBarTray
        var viewToolBarTray = window.FindName("ViewToolBarTray") as ToolBarTray;
        if (viewToolBarTray != null)
        {
            viewToolBarTray.Background = viewToolBarBackgroundBrush;
            foreach (var toolBar in viewToolBarTray.ToolBars)
            {
                toolBar.Foreground = menuForegroundBrush;
                SetToolBarItemsForeground(toolBar.Items, menuForegroundBrush);
            }
        }

        // Применение темы к StatusBar
        var statusBar = window.FindName("StatusBar") as StatusBar;
        if (statusBar != null)
        {
            statusBar.Background = statusBarBackgroundBrush;
            foreach (var item in statusBar.Items)
            {
                if (item is Control control)
                {
                    control.Foreground = menuForegroundBrush;
                }
            }
        }

        // Применение темы к ToolBarFi1, ToolBarFi2, ToolBarFi3, ToolBarFi4, ToolBarFi5
        ApplyThemeToToolBar(window.FindName("ToolBarFi1") as ToolBar, isDarkTheme);
        ApplyThemeToToolBar(window.FindName("ToolBarFi2") as ToolBar, isDarkTheme);
        ApplyThemeToToolBar(window.FindName("ToolBarFi3") as ToolBar, isDarkTheme);
        ApplyThemeToToolBar(window.FindName("ToolBarFi4") as ToolBar, isDarkTheme);
        ApplyThemeToToolBar(window.FindName("ToolBarFi5") as ToolBar, isDarkTheme);

        // Применение темы к ToolBarIns1
        ApplyThemeToToolBar(window.FindName("ToolBarIns1") as ToolBar, isDarkTheme);
        ApplyThemeToToolBar(window.FindName("ToolBarIns2") as ToolBar, isDarkTheme);
    }
    public static void LoadAndApplyTheme(Window window)
    {
        bool isDarkTheme = ThemeSettings.LoadTheme();
        ApplyTheme(window, isDarkTheme);
    }

    /// <summary>
    /// Сохраняет выбранную тему и применяет её к окну.
    /// </summary>
    /// <param name="window">Окно, к которому применяется тема.</param>
    /// <param name="isDarkTheme">Если true, применяется темная тема, иначе — светлая.</param>
    public static void SaveAndApplyTheme(Window window, bool isDarkTheme)
    {
        ThemeSettings.SaveTheme(isDarkTheme);
        ApplyTheme(window, isDarkTheme);
    }

    /// <summary>
    /// Рекурсивно устанавливает цвет текста для всех элементов меню.
    /// </summary>
    private static void SetMenuItemsForeground(ItemCollection items, Brush foregroundBrush, bool isDarkTheme)
    {
        foreach (var item in items)
        {
            if (item is MenuItem menuItem)
            {
                // Если это вкладка "Файл", применяем зеленый цвет
                if (menuItem.Header?.ToString() == "Файл" || menuItem.Header?.ToString() == "File")
                {
                    menuItem.Background = FileMenuBackgroundBrush; // Зеленый фон
                }
                else
                {
                    // Для остальных вкладок применяем тему полностью
                    menuItem.Background = isDarkTheme ? DarkMenuBackgroundBrush : LightMenuBackgroundBrush;
                    menuItem.Foreground = foregroundBrush;
                }

                // Рекурсивно применяем тему к вложенным элементам
                SetMenuItemsForeground(menuItem.Items, isDarkTheme ? DarkMenuForegroundBrush : LightMenuForegroundBrush, isDarkTheme);
            }
        }
    }

    /// <summary>
    /// Устанавливает цвет текста для всех элементов ToolBar.
    /// </summary>
    private static void SetToolBarItemsForeground(ItemCollection items, Brush foregroundBrush)
    {
        foreach (var item in items)
        {
            if (item is Control control)
            {
                control.Foreground = foregroundBrush;
            }
        }
    }
    public static void ApplyThemeToToolBar(ToolBar toolBar, bool isDarkTheme)
    {
        if (toolBar != null)
        {
            // Устанавливаем фон для ToolBar
            if (toolBar.Name.StartsWith("ToolBarFi")) // Проверяем, что это ToolBarFi
            {
                toolBar.Background = isDarkTheme ? DarkToolBarFiBackgroundBrush : LightToolBarFiBackgroundBrush;
            }
            else if (toolBar.Name.StartsWith("ToolBarIns")) // Проверяем, что это ToolBarIns
            {
                toolBar.Background = isDarkTheme ? DarkToolBarInsBackgroundBrush : LightToolBarInsBackgroundBrush;
            }
            else
            {
                toolBar.Background = isDarkTheme ? DarkToolBarBackgroundBrush : LightToolBarBackgroundBrush;
            }

            // Применяем тему ко всем кнопкам внутри ToolBar
            foreach (var item in toolBar.Items)
            {
                if (item is Button button)
                {
                    if (toolBar.Name.StartsWith("ToolBarFi")) // Проверяем, что это ToolBarFi
                    {
                        button.Foreground = isDarkTheme ? DarkToolBarFiForegroundBrush : LightToolBarFiForegroundBrush;
                        button.Background = isDarkTheme ? DarkToolBarFiBackgroundBrush : LightToolBarFiBackgroundBrush;
                    }
                    else if (toolBar.Name.StartsWith("ToolBarIns")) // Проверяем, что это ToolBarIns
                    {
                        button.Foreground = isDarkTheme ? DarkToolBarInsForegroundBrush : LightToolBarInsForegroundBrush;
                        button.Background = isDarkTheme ? DarkToolBarInsBackgroundBrush : LightToolBarInsBackgroundBrush;
                    }
                    else
                    {
                        button.Foreground = isDarkTheme ? DarkMenuForegroundBrush : LightMenuForegroundBrush;
                        button.Background = isDarkTheme ? DarkMenuBackgroundBrush : LightMenuBackgroundBrush;
                    }
                }
            }
        }
    }
    public static void ApplyThemeToDockPanel(DockPanel dockPanel, bool isDarkTheme)
    {
        if (dockPanel != null)
        {
            dockPanel.Background = isDarkTheme ? DarkBackgroundBrush : LightBackgroundBrush;

            // Применяем тему ко всем дочерним элементам DockPanel
            foreach (var child in dockPanel.Children)
            {
                if (child is Border border)
                {
                    border.Background = isDarkTheme ? DarkBackgroundBrush : LightBackgroundBrush;
                    if (border.Child is StackPanel stackPanel)
                    {
                        ApplyThemeToStackPanel(stackPanel, isDarkTheme);
                    }
                }
                else if (child is Button button)
                {
                    // Устанавливаем цвет текста кнопок
                    button.Foreground = isDarkTheme ? Brushes.White : LightMenuForegroundBrush; // Белый текст в темной теме
                    button.Background = isDarkTheme ? DarkMenuBackgroundBrush : LightMenuBackgroundBrush;
                }
            }
        }
    }
    private static void ApplyThemeToStackPanel(StackPanel stackPanel, bool isDarkTheme)
    {
        if (stackPanel != null)
        {
            foreach (var child in stackPanel.Children)
            {
                if (child is TextBlock textBlock)
                {
                    textBlock.Foreground = isDarkTheme ? DarkMenuForegroundBrush : LightMenuForegroundBrush;
                }
                else if (child is Button button)
                {
                    // Устанавливаем цвет текста кнопок
                    button.Foreground = isDarkTheme ? Brushes.White : LightMenuForegroundBrush; // Белый текст в темной теме
                    button.Background = isDarkTheme ? DarkMenuBackgroundBrush : LightMenuBackgroundBrush;
                }
                else if (child is ListBox listBox)
                {
                    listBox.Foreground = isDarkTheme ? DarkMenuForegroundBrush : LightMenuForegroundBrush;
                    listBox.Background = isDarkTheme ? DarkBackgroundBrush : LightBackgroundBrush;
                }
            }
        }
    }
    public static void ApplyThemeToLabel(Label label, bool isDarkTheme)
    {
        if (label != null)
        {
            label.Foreground = isDarkTheme ? DarkMenuForegroundBrush : LightMenuForegroundBrush;
        }
    }
    public static void ApplyThemeToShapes(Canvas canvas, bool isDarkTheme)
    {
        if (canvas != null)
        {
            foreach (var child in canvas.Children)
            {
                if (child is Shape shape)
                {
                    shape.Fill = isDarkTheme ? Brushes.LightGray : Brushes.White;
                }
            }
        }
    }
}