using BeaconManagerLib.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BeaconManagerLib
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SignIn : ContentPage
	{
        private string url = "";

        public SignIn()
        {
            InitializeComponent();
        }

        public SignIn (string url)
		{
            this.url = url;
            InitializeComponent ();
		}

        private void Button_Clicked(object sender, EventArgs e)
        {

        }

        private async void SignIn_Clicked(object sender, EventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);

                User user = new User()
                {
                    Name = nameEntry.Text,
                    Email = emailEntry.Text,
                    Password = passwordEntry.Text
                };

                string jsonStr = JsonConvert.SerializeObject(user);

                Dictionary<string, string> dict = new Dictionary<string, string>()
                    {
                        { "str", jsonStr}
                    };
                FormUrlEncodedContent form = new FormUrlEncodedContent(dict);
                HttpResponseMessage response = await client.PostAsync($"{client.BaseAddress}Home/SignIn/", form);

                string result = await response.Content.ReadAsStringAsync();
                string subString = "<!--";
                int indexOfSubstring = result.IndexOf(subString);
                string[] words = result.Split(new char[] { '<', '!', '-' });

                if (words[0] != String.Empty)
                {
                    if (!words[0].Contains("ERROR"))
                    {

                        User currentUser = JsonConvert.DeserializeObject<User>(words[0]);
                        await Navigation.PushModalAsync(new HomePage(url, currentUser));
                        
                    }
                    else
                    {
                        await DisplayAlert("Error", words[1], "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Result", "You entered invalid emal or password.", "Ok");
                }
            }
        }
    }
}