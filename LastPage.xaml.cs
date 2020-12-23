using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Poem_Learn
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LastPage : ContentPage
    {
        public LastPage(PoemAndDictionary pp)
        {
            InitializeComponent();

            Scores.Text = "You scored\n" + pp.score.ToString() + " out of 25";
        }

        private async void ImageButton_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PopModalAsync();
            
        }
    }
}
