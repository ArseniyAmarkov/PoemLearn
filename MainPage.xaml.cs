using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PoemLearn
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            author.Text = "Пушкин А. С.";
            name.Text = "Керн";
            poema.Text = "Я помню чудное мгновенье:\n" +
                         "Передо мной явилась ты,\n" +
                         "Как мимолетное виденье,\n" +
                         "Как гений чистой красоты.";


            // Прокрутка экрана на случай, если весь текст не помещается полностью в границы экрана
            // Или в XAML через <ScrollView>в начале   </ScrollView>в конце
            ScrollView scrollView = new ScrollView();
            scrollView.Content = stackLabels;
            this.Content = scrollView;
        }
    }
}
