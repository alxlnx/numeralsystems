using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Main
{
    /// <summary>
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();
            
            this.Closing += Window_Closing;
            this.Loaded += Window_Loaded;
        }

        // ======== КОГДА ОКНО ПОМОЩИ ЗАКРЫВАЕТСЯ ==========
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ((App)Application.Current).isHelpOpened = false;
        }

        // ======== КОГДА ОКНО ПОМОЩИ ОТКРЫВАЕТСЯ ==========
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(Input);
            // Включить ввод букв
        }

        // ======== ОТОБРАЖЕНИЕ НОМЕРА БУКВЫ ПРИ ВВОДЕ ==========
        private void NewInput(object sender, TextChangedEventArgs e)
        {
            bool isMatch = false;

            // Искать букву в строке всех букв
            for (int i = 0; i < symbols.Length; i++)
            {
                // Если найдено
                if (symbols[i].ToString() == Input.Text)
                {
                    i += 10;
                    // Номер символа на 10 больше его позиции

                    InputAnswer.Content = "Буква в 10 СИ: " + i;
                    isMatch = true;
                }
            }

            if (!isMatch) InputAnswer.Content = "Буква в 10 СИ: ";
            // Если ничего не было найдено, то стереть прошлый результат
        }

        private string symbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    }
}
