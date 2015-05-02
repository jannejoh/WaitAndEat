using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace WaitAndEat
{
    /// <summary>
    /// Wait list page.
    /// </summary>
    public sealed partial class WaitListPage : Page
    {
        // Logged in restaurant user (= restaurant host)
        private Model.RestaurantUser user = null;
        // View model for the restaurant user's waiting list
        private ViewModel.WaitingListViewModel waitingListViewModel = null;

        public WaitListPage()
        {
            this.InitializeComponent();
            Loaded += WaitingListPage_Loaded;
        }

        void WaitingListPage_Loaded(object sender, RoutedEventArgs e)
        {
            waitingListViewModel = new ViewModel.WaitingListViewModel();
            DataContext = waitingListViewModel;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("Navigated to wait list page");

            SetDefaultPartyValues();
            if (e.Parameter is Model.RestaurantUser)
            {
                user = e.Parameter as Model.RestaurantUser;
                Windows.Storage.ApplicationDataContainer localSettings
                        = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["User"] = Utils.JsonHelper.Serialize(user);
                UpdateModel();
            }
        }

        private async void UpdateModel()
        {
            Debug.WriteLine("Updating waiting list");

            Dictionary<string, Model.Party> partyDictionary = await Utils.Firebase.getParties(user);

            // Update ids to fetched parties
            if (partyDictionary != null)
            {
                foreach (KeyValuePair<string, Model.Party> entry in partyDictionary)
                {
                    entry.Value.id = entry.Key;
                }
            }

            // Remove parties that no loger exist or have changed
            for (int index = waitingListViewModel.WaitingList.Count - 1; index >= 0; index--)
            {
                Model.Party party = waitingListViewModel.WaitingList[index];
                if (partyDictionary == null || !partyDictionary.Values.Contains(party))
                {
                    waitingListViewModel.WaitingList.RemoveAt(index);
                }
            }

            // Add new and changed parties
            if (partyDictionary != null)
            {
                List<Model.Party> parties = partyDictionary.Values.ToList();
                for (int index = 0; index < parties.Count; index++)
                {
                    Model.Party party = parties[index];
                    if (!waitingListViewModel.WaitingList.Contains(party))
                    {
                        waitingListViewModel.WaitingList.Insert(index, party);
                    }
                }
            }

            Debug.WriteLine("Fetched " + waitingListViewModel.WaitingList.Count + " parties to waiting list");
        }

        private void ShowPartyFlyoutMenu(object sender, HoldingRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);
        }

        private void Add_Party_Click(object sender, RoutedEventArgs e)
        {
            Model.Party party = new Model.Party();
            party.name = PartyNameField.Text;
            party.phone = PartyPhoneField.Text;
            party.size = PartySizeField.SelectedValue as string;
            Utils.Firebase.saveParty(user, party);
            waitingListViewModel.WaitingList.Add(party);
            SetDefaultPartyValues();
        }

        private void Send_Sms_Click(object sender, RoutedEventArgs e)
        {
            Button item = sender as Button;
            if (item != null)
            {
                Model.Party party = item.DataContext as Model.Party;
                if (party != null)
                {
                    Debug.WriteLine("Sending text message to " + party.phone);
                    Utils.Firebase.sendTextMessage(user, party);
                    party.notified = Model.Party.NotifiedYes;
                    Utils.Firebase.saveParty(user, party);
                }
            }
        }

        private void Remove_Party_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            if (item != null)
            {
                Model.Party party = item.DataContext as Model.Party;
                if(party != null)
                {
                    Debug.WriteLine("Deleting party " + party.id);
                    Utils.Firebase.deleteParty(user, party);
                    waitingListViewModel.WaitingList.Remove(party);
                }
            }
        }

        private void Toggle_Party_Status(object sender, RoutedEventArgs e)
        {
            CheckBox item = sender as CheckBox;
            if (item != null)
            {
                Model.Party party = item.DataContext as Model.Party;
                if (party != null)
                {
                    Debug.WriteLine("Changing party " + party.id + " status to " + (party.done ? "not done" : "done"));
                    party.done = !party.done;
                    Utils.Firebase.saveParty(user, party);
                }
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            UpdateModel();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            // "logout" string as a parameter to Main page indicates logout action
            Frame.Navigate(typeof(MainPage), "logout");
        }

        private async void About_Click(object sender, RoutedEventArgs e)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            MessageDialog dialog = new MessageDialog(loader.GetString("AboutText"));
            await dialog.ShowAsync();
        }

        private void SetDefaultPartyValues()
        {
            PartyNameField.Text = "";
            PartyPhoneField.Text = "";
            PartySizeField.SelectedIndex = 1;
        }
    }
}
