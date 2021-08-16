using MAUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace MAUI.Services
{
    public class DataAccessService : IAuthorizationService
    {
        private const string BASE_URL = "https://localhost:44389/";
        private const string API_VERSION = "1.0";

        public async Task<Token> GetTokenAsync(string login, string password)
        {
            Token token = default;
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage httpResponseMessage = 
                    await httpClient.GetAsync($"{BASE_URL}users/token?api-version=${API_VERSION}&username=${login}&password=${password}");


                token = JsonSerializer.Deserialize<Token>(httpResponseMessage.Content.ToString());
                //Console.WriteLine(anotherObject.Message); // "Hello Again."
            }



            return token;
        }
    }
}
