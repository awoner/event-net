using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BeaconManagerLib.Entities;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace BeaconManagerLib
{
    public partial class MainPage : ContentPage
    {
        string url = "";
        public MainPage()
        {
            url = "http://event-net.somee.com";
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e) { }

        private async void SignInButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignIn(url));
        }

        private async void SignUpButton_Clicked(object sender, EventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);

                User user = new User()
                {
                    Name = nameEntry.Text,
                    Email = emailEntry.Text,
                    Password = passwordEntry.Text,
                    Bio = bioEntry.Text
                };

                string jsonStr = JsonConvert.SerializeObject(user);

                Dictionary<string, string> dict = new Dictionary<string, string>()
                {
                    { "str", jsonStr}
                };

                FormUrlEncodedContent form = new FormUrlEncodedContent(dict);
                HttpResponseMessage response = await client.PostAsync($"{client.BaseAddress}Home/SignUp", form);
                string result = await response.Content.ReadAsStringAsync();

                string subString = "<!--";
                int indexOfSubstring = result.IndexOf(subString);
                string[] words = result.Split(new char[] { '<', '!', '-' });

                if (words[0] != String.Empty)
                {
                    if (!words[0].Contains("ERROR"))
                    {
                        User currentUser = JsonConvert.DeserializeObject<User>(words[0]);

                        await DisplayAlert("Result", "Registered was successful.", "OK");

                        HomePage hp = new HomePage(url, currentUser);
                        hp.AuthorizedUser(currentUser);

                        await Navigation.PushAsync(hp);
                    }
                    else
                    {
                        await DisplayAlert("Error", words[0], "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Invalid data", "Please, enter all fields right.", "OK");
                }
            }
        }

        internal void AuthorizedUser(User user1)
        {
            throw new NotImplementedException();
        }
    }
}
