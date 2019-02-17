using BeaconManagerApp;
using Plugin.BeaconManager;
using Plugin.BeaconManager.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Linq;
using BeaconManagerLib.Entities;
using System.Net.Http;
using Newtonsoft.Json;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace BeaconManagerLib
{
    public partial class App : Application
    {
        Label logLabel;
        string monitorLog;
        Button monitorButton;


        Entry nameEntry, emailEntry, passwordEntry, bioEntry;
        Button signInButton, signUpButton;
        Label questionLabel;

        string url = "http://event-net.somee.com";

        MyEvent footballMatch = new MyEvent(), newYearShow = new MyEvent(), testEvent, currentEvent;
        User alexUser, andrewUser, johnUser, currentUser;

        List<User> users;
        List<MyEvent> events;

        private readonly string SignUpUrl = "http://beacon.somee.com/Account/SignUp";
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());
        }

        public async void AddToEvent()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                //var response = client.GetAsync(client.BaseAddress);
                string jsonString = JsonConvert.SerializeObject(currentUser);

                Dictionary<string, string> data = new Dictionary<string, string>
                {
                    { "newEventUser", jsonString }
                };
                FormUrlEncodedContent content = new FormUrlEncodedContent(data);
                HttpResponseMessage response = await client.PostAsync($"{client.BaseAddress}/Home/AddToEvent/", content);
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            if (Device.OS == TargetPlatform.Android && CrossBeaconManager.Current.IsInitialized)
            {
                CrossBeaconManager.Current.SetBackgroundMode(true);
            }
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            if (Device.OS == TargetPlatform.Android && CrossBeaconManager.Current.IsInitialized)
            {
                CrossBeaconManager.Current.SetBackgroundMode(false);
            }
        }

        //UI
        private async void UpdateDisplay()
        {
            string processDots = "";
            while (true)
            {
                logLabel.Text = processDots;

                if (CrossBeaconManager.Current.IsInitialized)
                {

                    logLabel.Text = "";

                    logLabel.Text += "Ranging:" + CrossBeaconManager.Current.IsRanging;
                    logLabel.Text += "  Monitoring:" + CrossBeaconManager.Current.IsMonitoring;
                    logLabel.Text += "\nAuthorized:" + CrossBeaconManager.Current.IsAuthorized;
                    logLabel.Text += "  BluetoothEnabled:" + CrossBeaconManager.Current.IsBluetoothEnabled;

                    if (CrossBeaconManager.Current.IsMonitoring)
                    {
                        logLabel.Text += "\n\nMonitoring " + CrossBeaconManager.Current.GetMonitoredRegions().Count + " regions" + processDots + "\n\n";
                        logLabel.Text += monitorLog;
                    }

                    if (CrossBeaconManager.Current.IsRanging)
                    {
                        logLabel.Text += "\n\nRanging " + CrossBeaconManager.Current.GetRangedRegions().Count + " regions" + processDots + "\n\n";
                        //logLabel.Text += rangeLog;
                    }
                }


                processDots = processDots.Length < 5 ? processDots + "." : "";
                await Task.Delay(1000);

            }

        }
    }
}
