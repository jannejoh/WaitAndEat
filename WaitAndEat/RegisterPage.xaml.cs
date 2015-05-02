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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace WaitAndEat
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RegisterPage : Page
    {
        private Common.NavigationHelper navigationHelper = null;

        public RegisterPage()
        {
            this.InitializeComponent();
            navigationHelper = new Common.NavigationHelper(this);
        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            Model.FirebaseApiResponse registerResponse = await Utils.Firebase.register(UsernameField.Text, PasswordField.Password);
            if (registerResponse.isError())
            {
                RegisterError.Text = ResourceLoader.GetForCurrentView().GetString("ErrorRegister"); ;
                System.Diagnostics.Debug.WriteLine("Register error: " + registerResponse.error.message);
            }
            else
            {
                RegisterError.Text = "";
                Model.RestaurantUser user = new Model.RestaurantUser(registerResponse.user.id, UsernameField.Text, PasswordField.Password, registerResponse.token);
                UsernameField.Text = "";
                PasswordField.Password = "";
                Model.FirebaseApiResponse loginResponse = await Utils.Firebase.login(user.email, user.password);
                if (loginResponse.isError())
                {
                    // This error should not happen since registration was a success
                    RegisterError.Text = ResourceLoader.GetForCurrentView().GetString("ErrorRegister"); ;
                    System.Diagnostics.Debug.WriteLine("Login error: " + registerResponse.error.message);
                }
                else
                {
                    Frame.Navigate(typeof(WaitListPage), user);
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Register page - OnNavigatedFrom");
            navigationHelper.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Register page - OnNavigatedTo");
            navigationHelper.OnNavigatedTo(e);
        }
    }
}
