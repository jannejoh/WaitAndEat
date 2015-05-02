using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace WaitAndEat
{
    /// <summary>
    /// Wait and Eat main page containg login action.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Logged in restaurant user (= restaurant host)
        private Model.RestaurantUser user = null;
        private Common.NavigationHelper navigationHelper = null;
        private Windows.Storage.ApplicationDataContainer localSettings = null;

        public MainPage()
        {
            this.InitializeComponent();
            navigationHelper = new Common.NavigationHelper(this);
            navigationHelper.SaveState += NavigationHelper_SaveState;
            navigationHelper.LoadState += NavigationHelper_LoadState;
            NavigationCacheMode = NavigationCacheMode.Required;
            localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        }

        private void NavigationHelper_LoadState(object sender, Common.LoadStateEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Main page - load state");

            // Load data from local settings and auto login
            if (localSettings.Values.ContainsKey("User"))
            {
                user = Utils.JsonHelper.Deserialize<Model.RestaurantUser>(localSettings.Values["User"] as string);
                if (user != null) 
                {
                    System.Diagnostics.Debug.WriteLine("User object loaded successfully");
                    // The token expires in 24 hours, user must login again on app restart
                    Login(user.email, user.password);
                }
            }
        }

        private void NavigationHelper_SaveState(object sender, Common.SaveStateEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Main page - save state");

            // Navigation state is not saved in order to login after app restart
            localSettings.Values["User"] = Utils.JsonHelper.Serialize(user);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Main page - OnNavigatedFrom");
            navigationHelper.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Main page - OnNavigatedTo");

            // "logout" string as a parameter indicates logout action
            if ("logout".Equals(e.Parameter as string))
            {
                System.Diagnostics.Debug.WriteLine("Main page - logging out");
                user = null;
                localSettings.Values.Remove("User");
            }
            
            navigationHelper.OnNavigatedTo(e);
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            Login(UsernameField.Text, PasswordField.Password);
        }

        private async void Login(string username, string password)
        {
            System.Diagnostics.Debug.WriteLine("Main page - login user: " + username);
            Model.FirebaseApiResponse loginResponse = await Utils.Firebase.login(username, password);
            if (loginResponse.isError())
            {
                LoginError.Text = ResourceLoader.GetForCurrentView().GetString("ErrorLogin"); ;
                System.Diagnostics.Debug.WriteLine("Login error: " + loginResponse.error.message);
            }
            else
            {
                LoginError.Text = "";
                user = new Model.RestaurantUser(loginResponse.user.id, username, password, loginResponse.token);
                UsernameField.Text = "";
                PasswordField.Password = "";

                localSettings.Values["User"] = Utils.JsonHelper.Serialize(user);
                Frame.Navigate(typeof(WaitListPage), user);
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RegisterPage));
        }
    }
}
