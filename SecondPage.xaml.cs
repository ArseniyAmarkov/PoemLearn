using System;
using System.Collections.Generic;
using System.IO.Pipes;
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
            Device.SetFlags(new string[] { "RadioButton_Experimental" });
            InitializeComponent();
            author.Text = "Пушкин А. С.";
            name.Text = "Керн";
            poema.Text = "Я помню чудное мгновенье:\n" +
                         "Передо мной явилась ты,\n" +
                         "Как мимолетное виденье,\n" +
                         "Как гений чистой красоты.";

            // Включаем функцию обработки стиха и запоминания слова
            rememberWord = MainProcess(poema.Text);

            // Массив слов, похожих на rememberWord (Включая само это слово)
            string[] simWords = SimilarWords();

            var rand = new Random();
            

            // Заводим индексы для распледеления ответов
            int p1, p2, p3;
            p1 = rand.Next(simWords.Length);
            p2 = rand.Next(simWords.Length);
            p3 = rand.Next(simWords.Length);
            while ((p1 == p2) || (p1 == p3) || (p2 == p3))
            {
                p2 = rand.Next(simWords.Length);
                p3 = rand.Next(simWords.Length);
            }

            // Раскидываем ответы по кнопкам выбора
            But1.Text = simWords[p1];
            But2.Text = simWords[p2];
            But3.Text = simWords[p3];


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
            Button button = (Button)sender;
            // Если Пользователь выбрал 1-й вариант ответа
            if (But1.IsChecked == true)
            {
                if (But1.Text == rememberWord)
                    CheckLabel.Text = "ТАК ДЕРЖАТЬ!!!";
                else CheckLabel.Text = "Неверно. Правильный ответ: " + rememberWord;
            }
            // Если Пользователь выбрал 2-й вариант ответа
            if (But2.IsChecked == true)
            {
                if (But2.Text == rememberWord)
                    CheckLabel.Text = "ТАК ДЕРЖАТЬ!!!";
                else CheckLabel.Text = "Неверно. Правильный ответ: " + rememberWord;
            }
            // Если Пользователь выбрал 3-й вариант ответа
            if (But3.IsChecked == true)
            {
                if (But3.Text == rememberWord)
                    CheckLabel.Text = "ТАК ДЕРЖАТЬ!!!";
                else CheckLabel.Text = "Неверно. Правильный ответ: " + rememberWord;
            }
            // Блокируем кнопку от повторного нажатия
            button.IsEnabled = false;
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
            int index = rand.Next(endWords.Length - 1);

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

        private string[] SimilarWords()
        {
            // Массив всевозможных похожих слов
            string[] allWords = { "ателье", "оливье", "забытье", "куранты", "ласты", "остроты" };

            // Массив слов, похожих на пропущенное
            string[] simWords = new string[3];
            // В первую позицию сразу закидываем само пропущенное слово
            simWords[0] = rememberWord;
            int k = 0;

            // Циклом пробегаем по массиву всех слов и ищем слова, похожие на пропущенное 
            for (int i = 0; i < allWords.Length; i++)
            {
                var word1 = new StringBuilder();
                word1.Append(rememberWord);

                var word2 = new StringBuilder();
                word2.Append(allWords[i]);

                // Проверяем, совпадают ли последние 2 буквы пропущенного слова и слова из массива
                if ((word1[word1.Length - 1] == word2[word2.Length - 1]) &&
                    (word1[word1.Length - 2] == word2[word2.Length - 2]))
                {
                    // Если да, то добавляем слово в массив
                    k++;
                    simWords[k] = allWords[i];   
                }
                // Если массив похожих слов полностью заполнен, прерываем цикл
                if (k + 1 == simWords.Length) break;
            }
            return simWords;
        }
       
    }
}
