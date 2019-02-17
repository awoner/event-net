using BeaconManagerLib.Entities;
using Newtonsoft.Json;
using Plugin.BeaconManager;
using Plugin.BeaconManager.Abstractions;
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
	public partial class HomePage : ContentPage
	{
        User currentUser;
        private string url;

        public HomePage()
        {
            InitializeComponent();
        }

        ~HomePage()
        {
            if (CrossBeaconManager.Current.IsMonitoring)
            {
                CrossBeaconManager.Current.StopMonitoring();
            }
        }

        public HomePage (string url, User currentUser)
		{
            this.url = url;
            this.currentUser = currentUser;

            InitializeComponent();

            SetBeacons();

            userInfo.Text = $"Your name: {this.currentUser.Name}\t\nEmail: {this.currentUser.Email}\t\nBio: {this.currentUser.Bio}";
            if (Device.OS == TargetPlatform.Android)
            {
                CrossBeaconManager.Current.Initialize(Android.App.Application.Context.ApplicationContext);

                //Set scan period to once per second for demo purpose (bad for battery)
                CrossBeaconManager.Current.SetBackgroundScanPeriod(1000, null);
            }

            CrossBeaconManager.Current.RequestAuthorization();
            CrossBeaconManager.Current.RegionLeft += RegionLeft;
            CrossBeaconManager.Current.RegionEntered += RegionEntered;


        }

        private async void SetBeacons()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);

                HttpResponseMessage response = await client.GetAsync($"{client.BaseAddress}Home/GetEvents/");

                string result = await response.Content.ReadAsStringAsync();
                string subString = "<!--";
                int indexOfSubstring = result.IndexOf(subString);
                string[] words = result.Split(new char[] { '<', '!', '-' }, StringSplitOptions.RemoveEmptyEntries);

                string events = "";

                foreach (string existEvent in words)
                {
                    events += existEvent;
                }

                if (events != String.Empty)
                {
                    var eventsFromJson = JsonConvert.DeserializeObject<List<MyEvent>>(events);
                    List<XRegion> regions = new List<XRegion>();
                    foreach (MyEvent existEvent in eventsFromJson)
                    {
                        regions.Add(new XRegion(
                            existEvent.Beacon.Type,
                            existEvent.Beacon.UUID,
                            (ushort?)existEvent.Beacon.Minor,
                            (ushort?)existEvent.Beacon.Major));
                    }

                    if (!CrossBeaconManager.Current.IsMonitoring)
                    {
                        CrossBeaconManager.Current.StartMonitoring(regions);
                    }
                }
            }
        }

        private async void RegionEntered(object sender, MonitoredRegionEventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);

                HttpResponseMessage response = await client.GetAsync($"{client.BaseAddress}Home/GetEvents/");

                string result = await response.Content.ReadAsStringAsync();
                string subString = "<!--";
                int indexOfSubstring = result.IndexOf(subString);
                string[] words = result.Split(new char[] { '<', '!', '-' });

                string events = "";

                foreach (string existEvent in words)
                {
                    events += existEvent;
                }

                if (events != String.Empty)
                {
                    var eventsFromJson = JsonConvert.DeserializeObject<List<MyEvent>>(events);

                    foreach (MyEvent existEvent in eventsFromJson)
                    {
                        if (eventsFromJson.Any(x => x.Beacon.UUID.Replace("-", "") == e.Region.UUID.Replace("-", "")) && existEvent.Beacon.UUID.Replace("-", "") == e.Region.UUID.Replace("-", ""))
                        {
                            currentUser.Event = existEvent;

                            Device.BeginInvokeOnMainThread(() => eventInfo.Text = "Event name:\n\t" + currentUser.Event.Name + "\nDescription:\n\t" + currentUser.Event.Description);

                            eventInfo.Text = "Event name:\n\t" + currentUser.Event.Name + "\nDescription:\n\t" + currentUser.Event.Description;

                            string jsonStr = JsonConvert.SerializeObject(currentUser);

                            Dictionary<string, string> dict = new Dictionary<string, string>()
                            {
                                { "str", jsonStr}
                            };

                            FormUrlEncodedContent form = new FormUrlEncodedContent(dict);

                            response = await client.PostAsync($"{client.BaseAddress}Home/AddToEvent/", form);


                        }
                    }

                }
            }
        }

        private async void RegionLeft(object sender, MonitoredRegionEventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                Device.BeginInvokeOnMainThread(() => eventInfo.Text = "");
                client.BaseAddress = new Uri(url);
                //var response = client.GetAsync(client.BaseAddress);

                string jsonString = JsonConvert.SerializeObject(currentUser);

                Dictionary<string, string> data = new Dictionary<string, string>
                {
                    { "str", jsonString }
                };
                FormUrlEncodedContent content = new FormUrlEncodedContent(data);
                HttpResponseMessage response = await client.PostAsync($"{client.BaseAddress}Home/RemoveFromEvent/", content);
            }
        }

        public void AuthorizedUser(User user)
        {
            currentUser = user;
        }
    }
}