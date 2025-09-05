using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;

namespace Main
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // В разметке интерфейса написано null вместо 0
            LastRoundCorrectNumLabel.Content = "Заданий: 0";
            LastRoundWrongNumLabel.Content = "Ошибок: 0";
        }

        // ================================ ПЕРЕМЕННЫЕ ====================================
        private Random rand = new Random();
        // Random для создания числа, которое нужно перевести

        private (int from, int to) randBounds;
        // Tuple для определения границ числа, которое нужно переводить

        private int[] bases = new int[61];
        // Массив для доступных СИ

        private (int from, int to) baseBounds;
        // Tuple для определения СИ сгенерированного числа

        private string symbols = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        // Символы для построения результата перевода; Максимальная СИ - 62, минимальная - 2

        private char[] answerReversed = new char[100];
        // Массив с перевернутым правильным ответом

        private string answerToCheck; 
        // Копия правильного ответа для проверки ввода пользователя

        private (int correct, int wrong) answerStats;
        // Tuple для текущей статистики

        private int exerNum;
        // Номер текущего задания

        private bool isSidebarExpanded = false;
        // Флаг для боковой панели

        private bool isLastRoundStatsShown = false;
        // Флаг для статистики предыдущего раунда
        // ==================================================================================

        // ============= ЗАПУСК ИГРЫ ПРИ НАЖАТИИ КНОПКИ НАЧАТЬ ====================
        private void StartPlaying(object sender, RoutedEventArgs e)
        {
            bool isSomethingChosen = false;
            // Флаг для запуска игры

            // Если выбран лёгкий уровень сложности
            if (EasyDiffRadiobtn.IsChecked == true)
            {
                // Границы генерируемого для перевода числа
                randBounds.to = 257;
                randBounds.from = 2;

                // Индексы, между которыми будет случайный выбор
                baseBounds.to = 2;
                baseBounds.from = 0;

                // Заполнение индексов СИ
                bases[0] = 2;
                bases[1] = 10;
                bases[2] = 16;

                isSomethingChosen = true;
            }
            // Средний
            else if (MediocreDiffRadiobtn.IsChecked == true)
            {
                // Границы генерируемого для перевода числа
                randBounds.to = 1025;
                randBounds.from = 256;

                // Индексы, между которыми будет случайный выбор
                baseBounds.to = 3;
                baseBounds.from = 0;

                // Заполнение индексов СИ
                bases[0] = 2;
                bases[1] = 10;
                bases[2] = 16;
                bases[3] = 8;

                isSomethingChosen = true;
            }
            // Сложный
            else if (HardDiffRadiobtn.IsChecked == true)
            {
                // Границы генерируемого для перевода числа
                randBounds.to = 100000;
                randBounds.from = 1024;

                // Индексы, между которыми будет случайный выбор
                baseBounds.to = 60;
                baseBounds.from = 0;

                // Заполнение индексов СИ
                for (int i = 0, j = 2; i <= 60; i++, j++)
                {
                    bases[i] = j;
                }

                isSomethingChosen = true;
            }

            // Если выбран хотя бы какой-то уровень сложности
            if (isSomethingChosen)
            {
                ChooseDiffGrid.Visibility = Visibility.Hidden;
                // Прячем панель выбора сложности

                PlayingGrid.Visibility = Visibility.Visible;
                // Показываем панель тренировки

                ThisRoundCorrectViewbox.Visibility = Visibility.Visible;
                ThisRoundWrongViewbox.Visibility = Visibility.Visible;

                Keyboard.Focus(AnswerTextbox);
                // Включить ввод ответа

                // Сброс статистики за этот раунд
                answerStats.wrong = 0;
                answerStats.correct = 0;

                // Сброс номера задания
                exerNum = 1;

                TooltipsHintForChooseDiffGridTextbox.Visibility = Visibility.Hidden;
                TooltipsHintForPlayingGridTextbox.Visibility = Visibility.Visible;

                // Начало игры
                Play();
            }
        }

        // =================== РАБОТА С ПЕРЕВОДИМЫМ ЧИСЛОМ ========================
        private void Play()
        {
            CurrentTaskNumLabel.Content = "Задание №" + exerNum;
            // Обновление номера задания

            // Обновления счётиков правильных и неправильных ответов
            ThisRoundCorrectNumLabel.Content = "Заданий: " + answerStats.correct;
            ThisRoundWrongNumLabel.Content = "Ошибок: " + answerStats.wrong;

            int num = rand.Next(randBounds.from, randBounds.to);
            // Создание числа, которое будет переведено в исходную СИ

            int num_copy = num;
            // Создание его коппи, которая будет переведена в конечную СИ


            // ============== ПЕРЕВОД ЧИСЛА В ИСХОДНУЮ СИ ===============
            int temp = rand.Next(baseBounds.from, baseBounds.to + 1);

            FromNumBaseLabel.Content = bases[temp];
            // Выбор случайной СИ и её отображение

            int baze = bases[temp];

            int j = 0;
            // Индекс, до которого в answerReversed[] находятся 
            // символы переведенного числа
            while (num > 0)
            {
                answerReversed[j] = Convert.ToChar(symbols[num % baze]);
                // Получить остаток
                num /= baze;
                // Уменьшить число
                j++;
            }

            StringBuilder initial = new StringBuilder("", j);
            // Строка для числа в исходной СИ

            for (int i = j - 1; i >= 0; i--)
            {
                initial.Append(answerReversed[i]);
            }
            // Заполнение результата перевода элементами
            // answerReversed[] В ОБРАТНОМ ПОРЯДКЕ

            FromNumLabel.Content = initial;
            // Отображение исходного числа в исходной СИ


            // ============== ПЕРЕВОД ЧИСЛА В КОНЕЧНУЮ СИ =================
            int temp2 = rand.Next(baseBounds.from, baseBounds.to + 1);
            while (temp == temp2)
            {
                temp2 = rand.Next(baseBounds.from, baseBounds.to + 1);
                // Если начальная и конечная СИ совпадают,
                // то перегенерировать конечную СИ
            }
            
            ToNumBaseLabel.Content = bases[temp2];
            // Выбор случайной СИ и её отображение

            baze = bases[temp2];

            j = 0;
            // Сброс индекса
            while (num_copy > 0)
            {
                answerReversed[j] = Convert.ToChar(symbols[num_copy % baze]);
                // Получить остаток
                num_copy /= baze;
                // Уменьшить копию числа
                j++;
            }
             
            StringBuilder answer = new StringBuilder("", j);
            // Строка для числа в конечной СИ

            for (int i = j - 1; i >= 0; i--)
            {
                answer.Append(answerReversed[i]);
            }
            // Заполнение результата перевода элементами
            // answerReversed[] В ОБРАТНОМ ПОРЯДКЕ

            answerToCheck = answer.ToString();
            // Правильный ответ, с которым будет
            // сравниваться ввод пользователя
        }

        // =================== ПРОВЕРКА ОТВЕТА ПОЛЬЗОВАТЕЛЯ =======================
        private void SubmitAnswer(object sender, KeyEventArgs e)
        {
            // Если нажата именно клавиша Enter
            if (e.Key == Key.Return)
            {
                // Включить сложный уровень
                if (AnswerTextbox.Text == "HaRd")
                {
                    HardDiffRadiobtn.Visibility = Visibility.Visible;
                }

                // Проверка правильности ответа пользователя
                if (AnswerTextbox.Text == answerToCheck)
                {
                    exerNum++;
                    // Увеличение номера задания

                    answerStats.correct++;
                    // Увеличение кол-ва правильно 
                    // выполненных заданий

                    AnswerTextbox.Text = null;
                    // Очистка поля для ввода ответа

                    Play();
                    // Следующий раунд
                } else
                {
                    answerStats.wrong++;
                    // Увеличение кол-ва неправильно 
                    // выполненных заданий

                    ThisRoundWrongNumLabel.Content = "Ошибок: " + answerStats.wrong;
                    // Обновление кол-ва неправильных ответов
                    // тут, так как не происходит переход
                    // в следующий раунд

                    AnswerTextbox.Text = null;
                    // Очистка поля для ввода ответа
                }
            }
        }

        /* Во время анимации:
         *      // Поменять изображения стрелок; теперь они смотрят вверх
                //LeftArrow.Source = new BitmapImage(new Uri("Images/arrowUP.ico", UriKind.Relative));
                //RightArrow.Source = new BitmapImage(new Uri("Images/arrowUP.ico", UriKind.Relative));
         * 
         *      // Поменять изображения стрелок; теперь они смотрят вниз
                //LeftArrow.Source = new BitmapImage(new Uri("Images/arrowDOWN.ico", UriKind.Relative));
                //RightArrow.Source = new BitmapImage(new Uri("Images/arrowDOWN.ico", UriKind.Relative));
         * 
         * В xaml на 167 строчке:
         * 
         *  <!-- Сетка для переворачивающихся стрелок при открывании/закрывании боковой панели -->
                <Grid Grid.Row="0">
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="*"/>
                        <!-- Колонка для image с левой стрелкой -->

                        <ColumnDefinition Width="2.3*"/>

                        <ColumnDefinition Width="*"/>
                        <!-- Колонка для image с правой стрелкой -->
                    </Grid.ColumnDefinitions>

                   <!-- Image с левой стрелкой -->
                    <Image x:Name="LeftArrow" Source="Images/arrowUP.ico"
                           Margin="0 20 10 0" Grid.Column="0" 
                           HorizontalAlignment="Left" VerticalAlignment="Bottom" 
                           RenderTransformOrigin="0.5,0.5">
                        <Image.RenderTransform>
                            <TransformGroup>
                                <RotateTransform Angle="270"/>
                            </TransformGroup>
                        </Image.RenderTransform>
                    </Image>

                    <!-- Image с правой стрелкой -->
                    <Image x:Name="RightArrow" Source="Images/arrowUP.ico" 
                           Margin="10 20 0 0" Grid.Column="2"
                           HorizontalAlignment="Right" VerticalAlignment="Bottom"
                           RenderTransformOrigin="0.5,0.5">
                        <Image.RenderTransform>
                            <TransformGroup>
                                <RotateTransform Angle="270"/>
                            </TransformGroup>
                        </Image.RenderTransform>
                    </Image>
                </Grid>    
             */

        // ========== РАЗВОРАЧИВАНИЕ БОКОВОЙ ПАНЕЛИ ПРИ НАЖАТИИ КНОПКИ ============
        private void OpenCloseSidebar(object sender, RoutedEventArgs e)
        {
            //Если сейчас боковая панель открыта, то закрыть
            if (isSidebarExpanded)
            {
                OpenCloseSidebarBtn.Content = "Открыть панель";
                isSidebarExpanded = false;

                // Анимация сворачивания
                ThicknessAnimation MarginAnim = new ThicknessAnimation();

                // Фиксация строк с элементами боковой панели их текущими высотами, чтобы сделать красивую анимацию
                RowForChooseDiffBtn.Height = new GridLength(RowForChooseDiffBtn.ActualHeight);
                RowForLastRoundStatsGrid.Height = new GridLength(RowForChooseDiffBtn.ActualHeight);
                RowForShowLastRoundStatsBtn.Height = new GridLength(RowForChooseDiffBtn.ActualHeight);

                /* ЕСЛИ ТУТ ВРУЧНУЮ (без анимации) ИЗМЕНИТЬ MARGIN SIDEBARGRID
                 * ТО ОБЯЗАТЕЛЬНО НУЖНО ИСПОЛЬЗОВАТЬ ЭТОТ МЕТОД:
                   SidebarGrid.UpdateLayout();
                */

                MarginAnim.From = new Thickness(0, 0, 0, 0);
                MarginAnim.To = new Thickness(0, 0, 0, 1100);

                MarginAnim.Duration = TimeSpan.FromMilliseconds(300);

                // Вызов метода для восстановления высоты строк на *
                MarginAnim.Completed += MarginAnim_Completed;

                // Анимация margin
                SidebarGrid.BeginAnimation(Grid.MarginProperty, MarginAnim);

                Keyboard.Focus(AnswerTextbox);
            }
            // Если сейчас боковая панель открыта, то закрыть
            else
            {
                OpenCloseSidebarBtn.Content = "Закрыть панель";
                isSidebarExpanded = true;



                // Анимация разворачивания
                ThicknessAnimation MarginAnim = new ThicknessAnimation();

                MarginAnim.From = new Thickness(0, 0, 0, 1100);
                MarginAnim.To = new Thickness(0, 0, 0, 0);

                // Фиксация строк с элементами боковой панели высотами специальных строк (у них высота *, а у наших строк сейчас 0),
                // чтобы сделать красивую анимацию
                RowForChooseDiffBtn.Height = new GridLength(RowToResizeChooseDiffBtn.ActualHeight);
                RowForLastRoundStatsGrid.Height = new GridLength(RowToResizeLastRoundStatsGrid.ActualHeight);
                RowForShowLastRoundStatsBtn.Height = new GridLength(RowToResizeShowLastRoundStatsBtn.ActualHeight);
      
                MarginAnim.Duration = TimeSpan.FromMilliseconds(225);

                // Вызов метода для восстановления высоты строк на *
                MarginAnim.Completed += MarginAnim_Completed;

                // Анимация margin
                SidebarGrid.BeginAnimation(Grid.MarginProperty, MarginAnim);
            }
        }

        private void MarginAnim_Completed(object sender, EventArgs e)
        {
            RowForChooseDiffBtn.Height = new GridLength(1.0, GridUnitType.Star);
            RowForLastRoundStatsGrid.Height = new GridLength(1.0, GridUnitType.Star);
            RowForShowLastRoundStatsBtn.Height = new GridLength(1.0, GridUnitType.Star);
        } 



        // ========= ВОЗВРАЩЕНИЕ К ВЫБОРУ СЛОЖНОСТИ ПРИ НАЖАТИИ КНОПКИ ============
        private void ChooseDifficulty (object sender, RoutedEventArgs e)
        {
            // Обновление статистики предыдущего раунда
            LastRoundCorrectNumLabel.Content = "Заданий: " + answerStats.correct;
            LastRoundWrongNumLabel.Content = "Ошибок: " + answerStats.wrong;

            // Спрятать игру, показать выбор сложности
            PlayingGrid.Visibility = Visibility.Hidden;
            ChooseDiffGrid.Visibility = Visibility.Visible;

            // Спрятать статистику для этого раунда ("этот раунд" только что окончился)
            ThisRoundCorrectViewbox.Visibility = Visibility.Hidden;
            ThisRoundWrongViewbox.Visibility = Visibility.Hidden;

            // Поменять текст о подсказках при наведении курсора на элементы ui в левой колоке
            TooltipsHintForPlayingGridTextbox.Visibility = Visibility.Hidden;
            TooltipsHintForChooseDiffGridTextbox.Visibility = Visibility.Visible;
        }
        
        // ===== ОТОБРАЖЕНИЕ СТАТИСТИКИ ПРЕДЫДУЩЕГО РАУНДА ПРИ НАЖАТИИ КНОПКИ ======
        private void ShowLastRoundStats (object sender, RoutedEventArgs e)
        {
            // Данные для статистики обновились в ChooseDifficulty

            // Если статистика сейчас показана, то спрятать
            if (isLastRoundStatsShown)
            {
                ShowLastRoundStatsBtn.Content = "Показать статистику";
                isLastRoundStatsShown = false;

                // Анимация исчезновения
                DoubleAnimation ScoreAnim = new DoubleAnimation();
                ScoreAnim.From = 1;
                ScoreAnim.To = 0;
                ScoreAnim.Duration = TimeSpan.FromMilliseconds(200);
                // Изменяется непрозрачность
                LastRoundStatsGrid.BeginAnimation(Grid.OpacityProperty, ScoreAnim);
            }
            // Если статистика сейчас спрятана, то показать
            else 
            {
                ShowLastRoundStatsBtn.Content = "Скрыть статистику";
                isLastRoundStatsShown = true;

                // Анимация проявления
                DoubleAnimation ScoreAnim = new DoubleAnimation();
                ScoreAnim.From = 0;
                ScoreAnim.To = 1;
                ScoreAnim.Duration = TimeSpan.FromMilliseconds(200);
                // Изменяется непрозрачность
                LastRoundStatsGrid.BeginAnimation(Grid.OpacityProperty, ScoreAnim);        
            }
            
        }



        // ============ ОТКРЫВАНИЕ ОКНА С ПОМОЩЬЮ ПРИ НАЖАТИИ КНОПКИ ===============
        private void OpenCloseHelpWindow(object sender, RoutedEventArgs e)
        {
            // Если окно НЕ открыто в данный момент
            if (!((App)Application.Current).isHelpOpened)
            {
                ((App)Application.Current).isHelpOpened = true;

                HelpWindow Window = new HelpWindow();

                // Чтобы при закрывании окна включался ввод ответа
                Keyboard.Focus(AnswerTextbox);

                // Открыть окно
                Window.Show();
            }
        }
    }
}
