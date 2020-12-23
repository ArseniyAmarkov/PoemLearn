using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Poem_Learn
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    // Класс для работы с информацией по стихотворению
    public class PoemInf
    {
        public string writer;
        public string name;
        public string poem;
        //public string year;
    }

    public partial class SecondPage : ContentPage
    {
        // Глобальные переменные для работы приложения
        public static string rememberWord;
        public static int poemIndex;
        public PoemInf[] poemArray = new PoemInf[25];
        public Dictionary<string, string> dictionary = new Dictionary<string, string>();
        public string[] simWords;
        public PoemAndDictionary pp = new PoemAndDictionary();

        public SecondPage()
        {
            Device.SetFlags(new string[] { "RadioButton_Experimental" });
            InitializeComponent();

            poemIndex = 0;


            // Получаем путь к файлу со стихотворениями (Если его нет, создаём через File.WriteAllText(filename, text))
            //string poemsfile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "poems.txt");
            //File.WriteAllText(poemsfile, poems);
            //string[] poemlines = File.ReadAllLines(poemsfile);
            //File.Delete(poemsfile);

            pp.score = 0;
            string[] poemlines = pp.poems.Split('\n');



            // Получаем путь к файлу словаря...
            //string dictfile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "reverse_dict.txt");
            //File.WriteAllText(dictfile, dicttext);
            //string[] textDict = File.ReadAllLines(dictfile);
            //File.Delete(dictfile);

            string[] textDict = pp.dict.Split('\n');
    
            // ...и заполняем словарь ключ - значение (слово - часть речи)
            //var dictionary = new Dictionary<string, string>();
            for (int i = 0; i < textDict.Length - 1; i++)
            {
                // Делим строку на две части (граница - пробел между словами)
                var partsLine = textDict[i].Split(' ');
                // Само слово как ключ, часть речи как значение
                dictionary[partsLine[0]] = partsLine[1];
            }


            // Массив стихотворений, которые будут выводиться по порядку
            for (int i = 0; i < poemlines.Length; i += 7)
            {
                int id = int.Parse(poemlines[i]) - 1;
                poemArray[id] = new PoemInf();
                poemArray[id].name = poemlines[i + 1];
                poemArray[id].writer = poemlines[i + 2];
                poemArray[id].poem = "";
                for (int j = i + 3; j < i + 7; j++)
                    poemArray[id].poem += poemlines[j] + "\n";
            }

            // Перемешиваем массив стихотворений для вывода их в случайном порядке
            var rand = new Random();
            for (int i = poemArray.Length - 1; i >= 1; i--)
            {
                int j = rand.Next(i + 1);

                PoemInf tmp = poemArray[j];
                poemArray[j] = poemArray[i];
                poemArray[i] = tmp;
            }

            // Функция вывода стихотворения с автором и названием
            PrintPoem(); // poemArray, dictionary
        }

        // Кнопка возвращения на главный экран
        private async void BackButton_Click(object sender, System.EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        // Функция вывода стихотворения с автором и названием
        public void PrintPoem() // PoemInf[] poemArray, Dictionary<string, string> dictionary
        {
            // Из класса вытаскиваем стихотворения для вывода на экран
            author.Text = poemArray[poemIndex].name;
            name.Text = poemArray[poemIndex].writer;
            //poema.Text = poemArray[poemIndex].poem;



            //
            //
            // Включаем функцию обработки стиха и запоминания слова
            //rememberWord = MainProcess(poema.Text);
            rememberWord = MainProcess(poemArray[poemIndex].poem);

            // Массив слов, похожих на rememberWord (Включая само это слово) (Туда же закидываем и словарь)
            simWords = SimilarWords(dictionary);


            var rand = new Random();
            Span2.Text = simWords[rand.Next(simWords.Length)];
        }


        // Функция обработки стихотворения, заменяет слово на нижнее подчёркивание и запоминает это слово в rememberWord
        private string MainProcess(string poem)
        {
            // Массив слов, стоящих в конце каждого абзаца (называю так конец строки стиха, чтобы не путать)
            string[] endWords;
            endWords = new string[4];

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
                    while ((poem[j] != ' ') && (poem[j] != '-'))
                    {
                        // и исключаем спец. символы
                        if ((poem[j] != '.') && (poem[j] != ',') && (poem[j] != ';') && (poem[j] != ':') && (poem[j] != '-') && (poem[j] != '!') && (poem[j] != '?'))
                            // Записываем слова в массив слов
                            endWords[k] = poem[j] + endWords[k]; ;
                        j--;
                    }

                    k++;

                }
            }

            // заводим переменную, которая будет получать рандомный индекс из списка конечных слов
            var rand = new Random();
            string remWord;
            while (true)
            {
                int index = rand.Next(endWords.Length - 1);
                // и присваиваем новой переменной рандомное слово из списка
                remWord = endWords[index];
                if (dictionary.ContainsKey(remWord))
                    break;
            }
            // Убираем это слово из стихотворения
            int id = poem.IndexOf(remWord);
            if (id >= 0)
            {
                Span1.Text = poem.Substring(0, id);
                Span2.Text = remWord;
                Span3.Text = poem.Substring(id + remWord.Length);

                //poem = poem.Substring(0, id) + "_____" + poem.Substring(id + remWord.Length);
            }
            //poema.Text = poem;

            return remWord;
        }

        // Функция получения массива слов, рифмованных с пропущенным
        private string[] SimilarWords(Dictionary<string, string> dictionary)
        {
            // Массив слов, похожих на пропущенное
            string[] simWords = new string[3];
            // В первую позицию сразу закидываем само пропущенное слово
            simWords[0] = rememberWord;
            int k = 0;


            // Пробегаем по всему словарю и ищем слова, рифмованные с пропущенным
            foreach (var e in dictionary)
            {
                // Считываем слова побуквенно и считаем количество гласных букв
                var array = new[]{ 'a', 'e','i','o','u','y' };
                var word1 = new StringBuilder();
                word1.Append(rememberWord);
                int glas1 = 0;
                for (int i = 0; i < word1.Length; i++)
                    foreach (var let in array)
                        if (word1[i] == let)
                            glas1++;

                var word2 = new StringBuilder();
                word2.Append(e.Key);
                int glas2 = 0;
                for (int i = 0; i < word2.Length; i++)
                    foreach (var let in array)
                        if (word2[i] == let)
                            glas2++;

                // Проверяем, совпадают ли последние 2 буквы пропущенного слова и слова из словаря
                // Плюс проверяем, что части речи слов равны и что это не одно и то же слово
                if ((word1[word1.Length - 1] == word2[word2.Length - 1]) &&
                    (word1[word1.Length - 2] == word2[word2.Length - 2]) &&
                    (dictionary[rememberWord] == e.Value) && (rememberWord != e.Key)
                    && (glas1 - glas2 > -2) && (glas1 - glas2 < 2))
                {
                    // Если да, то добавляем слово в массив
                    k++;
                    simWords[k] = e.Key;
                }
                // Если массив похожих слов полностью заполнен, прерываем цикл
                if (k + 1 == simWords.Length)
                    break;
                
            }

            // Второй раз пробегаем по словарю, если нашлись не все рифмы (в этот раз без проверки на часть речи)
            if (k + 1 != simWords.Length)
            {
                foreach (var e in dictionary)
                {
                    //var word1 = new StringBuilder();
                    //word1.Append(rememberWord);

                    //var word2 = new StringBuilder();
                    //word2.Append(e.Key);

                    // Считываем слова побуквенно и считаем количество гласных букв
                    var array = new[] { 'a', 'e', 'i', 'o', 'u', 'y' };
                    var word1 = new StringBuilder();
                    word1.Append(rememberWord);
                    int glas1 = 0;
                    for (int i = 0; i < word1.Length; i++)
                        foreach (var let in array)
                            if (word1[i] == let)
                                glas1++;

                    var word2 = new StringBuilder();
                    word2.Append(e.Key);
                    int glas2 = 0;
                    for (int i = 0; i < word2.Length; i++)
                        foreach (var let in array)
                            if (word2[i] == let)
                                glas2++;

                    // Проверяем, совпадают ли последние 2 буквы пропущенного слова и слова из словаря
                    // Плюс проверяем, что части речи слов равны и что это не одно и то же слово
                    if ((word1[word1.Length - 1] == word2[word2.Length - 1]) &&
                        (word1[word1.Length - 2] == word2[word2.Length - 2])
                        && (glas1 - glas2 > -2) && (glas1 - glas2 < 2))
                    {
                        // Если да, то добавляем слово в массив
                        k++;
                        simWords[k] = e.Key;
                    }
                    // Если массив похожих слов полностью заполнен, прерываем цикл
                    if (k + 1 == simWords.Length)
                        break;
                }
                
            }
            return simWords;
        }
        

        // Кнопка возвращения на главный экран
        private async void ImageButton_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
        

        // Кнопка смены слова к стихотворении
        private void ChangeButton_Clicked(object sender, EventArgs e)
        {
            if (Span2.Text == simWords[0])
                Span2.Text = simWords[1];
            else if (Span2.Text == simWords[1])
                Span2.Text = simWords[2];
            else if (Span2.Text == simWords[2])
                Span2.Text = simWords[0];
        }


        // Кнопка проверки ответа
        private void CheckButton_Clicked(object sender, EventArgs e)
        {
            if (Span2.Text == rememberWord)
            {
                CheckButton.BackgroundColor = Color.Green;
                Span2.ForegroundColor = Color.Green;
                pp.score++;
            }
            else
            {
                CheckButton.BackgroundColor = Color.Red;
                Span2.ForegroundColor = Color.Red;
                Span2.Text += "(" + rememberWord + ")";
            }

            ChangeButton.IsEnabled = false;
            CheckButton.IsEnabled = false;
        }


        // Кнопка переключения на следующее стихотворение
        private async void NextPoemButton_Clicked(object sender, EventArgs e)
        {
            poemIndex++;
            PrintPoem(); // poemArray, dictionary
            ChangeButton.IsEnabled = true;
            CheckButton.IsEnabled = true;
            if (poemIndex == 24)
            {
                NextPoemButton.IsEnabled = false;
                await Navigation.PushModalAsync(new LastPage(pp));
            }
            CheckButton.BackgroundColor = Color.Default;
        }


    }
}
