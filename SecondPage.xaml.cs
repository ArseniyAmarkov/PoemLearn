using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PoemLearn
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SecondPage : ContentPage
    {
        // Глобальная переменная для запоминания пропущенного слова
        public static string rememberWord;

        public SecondPage()
        { 
            InitializeComponent();
            author.Text = "Пушкин А. С.";
            name.Text = "Керн";
            poema.Text = "Я помню чудное мгновенье:\n" +
                         "Передо мной явилась ты,\n" +
                         "Как мимолетное виденье,\n" +
                         "Как гений чистой красоты.";

            rememberWord = MainProcess(poema.Text);

            // Прокрутка экрана на случай, если весь текст не помещается полностью в границы экрана
            // Или в XAML через <ScrollView>в начале   </ScrollView>в конце
            ScrollView scrollView = new ScrollView();
            scrollView.Content = stackLabels;
            this.Content = scrollView;
        }

        // Кнопка возвращения на главный экран
        private async void BackButton_Click(object sender, System.EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        // Кнопка проверки ответа
        private void CheckButton_Click(object sender, System.EventArgs e)
        {
            // При нажатии на кнопку меняется текст на ней
            Button button = (Button)sender;
            button.Text = "Нажато!";
            // И меняем текст CheckLabel и цвет кнопки в зависиммости от правильности ответа
            if (Answer.Text == null)
                CheckLabel.Text = "Вы забыли ввести ответ!";
            else if (Answer.Text == rememberWord)
            {
                CheckLabel.Text = "Поздравляю, вы неплохо знаете это стихотворение!";
                button.BackgroundColor = Color.Green;
                // Блокируем кнопку от повторного нажатия
                button.IsEnabled = false;
            }
            else
            {
                CheckLabel.Text = "Неверно!!! Правильный ответ: " + rememberWord;
                button.BackgroundColor = Color.Red;
                // Блокируем кнопку от повторного нажатия
                button.IsEnabled = false;
            }
        }



        // Функция обработки стихотворения, заменяет слово на нижнее подчёркивание и запоминает это слово в rememberWord
        private string MainProcess(string poem)
        {
            // Массив слов, стоящих в конце каждого абзаца (называю так конец строки стиха, чтобы не путать)
            string[] endWords;
            endWords = new string[5];

            int k = 0;
            // Пробегаемся по всему стихотворению, чтобы заполнить массив endWords
            for (int i = 0; i < poem.Length; i++)
            {
                // Проверка с помощью переноса строки и конца стихотворения
                if ((poem[i] == '\n') || (i == poem.Length - 1))
                {
                    // Нашли конец абзаца? Тогда считываем последнее слово
                    int j = i - 1;
                    // Считываем до пробела
                    while (poem[j] != ' ')
                    {
                        // и исключаем спец. символы
                        if ((poem[j] != '.') && (poem[j] != ',') && (poem[j] != ';') && (poem[j] != ':') && (poem[j] != '-'))
                            // Записываем слова в массив слов
                            endWords[k] = poem[j] + endWords[k]; ;
                        j--;
                    }

                    k++;

                }
            }

            // заводим переменную, которая будет получать рандомный индекс из списка конечных слов
            var rand = new Random();
            int index = rand.Next(endWords.Length);

            // и присваиваем новой переменной рандомное слово из списка
            string remWord = endWords[index];
            // Убираем это слово из стихотворения
            int id = poem.IndexOf(remWord);
            if (id >= 0)
                poem = poem.Substring(0, id) + "_____" + poem.Substring(id + remWord.Length);
            //poem = poem.Replace(rememberWord, "_____");
            poema.Text = poem;

            return remWord;
        }
    }
}