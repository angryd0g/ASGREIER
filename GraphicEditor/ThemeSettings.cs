using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows;
using System.Threading.Tasks;

namespace GraphicEditor
{
    public static class ThemeSettings
    {
        private const string ThemeConfigFilePath = "theme.config";

        /// <summary>
        /// Сохраняет выбранную тему в файл.
        /// </summary>
        /// <param name="isDarkTheme">True, если выбрана темная тема, иначе False.</param>
        public static void SaveTheme(bool isDarkTheme)
        {
            try
            {
                File.WriteAllText(ThemeConfigFilePath, isDarkTheme ? "Dark" : "Light", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении темы: " + ex.Message);
            }
        }

        /// <summary>
        /// Загружает сохраненную тему из файла.
        /// </summary>
        /// <returns>True, если выбрана темная тема, иначе False.</returns>
        public static bool LoadTheme()
        {
            try
            {
                if (File.Exists(ThemeConfigFilePath))
                {
                    string theme = File.ReadAllText(ThemeConfigFilePath, Encoding.UTF8);
                    return theme == "Dark";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке темы: " + ex.Message);
            }
            return false; // По умолчанию светлая тема
        }
    }
}
