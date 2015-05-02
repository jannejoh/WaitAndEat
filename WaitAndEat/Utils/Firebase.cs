using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using System.Diagnostics;

namespace WaitAndEat.Utils
{
    class Firebase
    {
        private const string LoginUrl = "https://auth.firebase.com/auth/firebase?email={0}&password={1}&firebase=waitandeat-janne&v=1.4.1";
        private const string RegisterUrl = "https://auth.firebase.com/auth/firebase/create?email={0}&password={1}&firebase=waitandeat-janne";
        private const string WaitAndEatUrl = "https://waitandeat-janne.firebaseio.com/";
        private const string PartiesPath = "users/{0}/parties.json?auth={1}";
        private const string PartyPath = "users/{0}/parties/{1}.json?auth={2}";
        private const string TextMessagesPath = "textMessages.json?auth={0}";

        public static async Task<Model.FirebaseApiResponse> login(string email, string password)
        {
            string response = await firebaseGetRequest(string.Format(LoginUrl, email, password));
            return response != null ? Utils.JsonHelper.Deserialize<Model.FirebaseApiResponse>(response) : null;
        }

        public static async Task<Model.FirebaseApiResponse> register(string email, string password)
        {
            string response = await firebaseGetRequest(string.Format(RegisterUrl, email, password));
            return response != null ? Utils.JsonHelper.Deserialize<Model.FirebaseApiResponse>(response) : null;
        }

        public static async Task<Dictionary<string, Model.Party>> getParties(Model.RestaurantUser user)
        {
            string response = await firebaseGetRequest(string.Format(WaitAndEatUrl + PartiesPath, user.id, user.token));
            if (isError(response))
            {
                Debug.WriteLine("Error fetching parties: " + response);
                return null;
            }
            else
            {
                return Utils.JsonHelper.Deserialize<Dictionary<string, Model.Party>>(response);
            }
        }

        public static async void saveParty(Model.RestaurantUser user, Model.Party party)
        {
            if (party.id == null)
            {
                string response = await firebasePostRequest(string.Format(WaitAndEatUrl + PartiesPath, user.id, user.token), Utils.JsonHelper.Serialize(party));
                if (isError(response))
                {
                    Debug.WriteLine("Error creating party: " + response);
                }
                else
                {
                    Model.CreateResponse createResponse = Utils.JsonHelper.Deserialize<Model.CreateResponse>(response);
                    party.id = createResponse.name;
                }
            }
            else
            {
                string response = await firebasePatchRequest(string.Format(WaitAndEatUrl + PartyPath, user.id, party.id, user.token), Utils.JsonHelper.Serialize(party));
                if (isError(response))
                {
                    Debug.WriteLine("Error updating party: " + response);
                }
            }
        }

        public static async void deleteParty(Model.RestaurantUser user, Model.Party party)
        {
            string response = await firebaseDeleteRequest(string.Format(WaitAndEatUrl + PartyPath, user.id, party.id, user.token));
            if (isError(response))
            {
                Debug.WriteLine("Error deleting party: " + response);
            }
        }

        public static async void sendTextMessage(Model.RestaurantUser user, Model.Party party)
        {
            Model.TextMessage textMessage = new Model.TextMessage();
            textMessage.name = party.name;
            textMessage.phoneNumber = party.phone;
            textMessage.size = party.size;

            string response = await firebasePostRequest(string.Format(WaitAndEatUrl + TextMessagesPath, user.token), Utils.JsonHelper.Serialize(textMessage));
            if (isError(response))
            {
                Debug.WriteLine("Error sending text message: " + response);
            }
        }

        private static async Task<string> firebaseGetRequest(string url)
        {
            return await firebaseRequest(new HttpRequestMessage(HttpMethod.Get, new Uri(url)));
        }

        private static async Task<string> firebasePostRequest(string url, string data)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(url));
            request.Content = new HttpStringContent(data);
            return await firebaseRequest(request);
        }

        private static async Task<string> firebasePatchRequest(string url, string data)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Patch, new Uri(url));
            request.Content = new HttpStringContent(data);
            return await firebaseRequest(request);
        }

        private static async Task<string> firebaseDeleteRequest(string url)
        {
            return await firebaseRequest(new HttpRequestMessage(HttpMethod.Delete, new Uri(url)));
        }

        private static async Task<string> firebaseRequest(HttpRequestMessage request)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    using (var response = await client.SendRequestAsync(request))
                    {
                        // status code 200
                        if (response.StatusCode == HttpStatusCode.Ok)
                        {
                            return await response.Content.ReadAsStringAsync();
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("http response error: " + e.Message);
                }
            }
            return null;
        }

        private static bool isError(string response)
        {
            return response == null || response.Contains("\"error\":");
        }

    }
}
