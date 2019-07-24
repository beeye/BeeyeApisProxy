using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WebApi.Proxies.Clients;
using WebApi.Proxies.Models;

namespace HelloBeeye
{
    class Program
    {
        static string apiKey = "";
        //Cookies are used to hold authentification information. 
        static readonly CookieContainer cookies = new CookieContainer();
        static readonly HttpClientHandler handler = new HttpClientHandler() { CookieContainer = cookies };
        static async Task Main(string[] args)
        {
            if (string.IsNullOrEmpty(apiKey) && args.Length < 1)
            {
                Console.WriteLine("No API key provided");
                Console.WriteLine("Apikey can be provided as parameter or in code");
                return;
            }
            if (string.IsNullOrEmpty(apiKey))
            {
                apiKey = args[0];
            }
            using (var beeyeLoginApi = new LoginClient(handler, false))
            using (var rhApi = new EmployesClient(handler, false))
            {
                //Example of Synchronous calls
                DemoSynchronousCalls(beeyeLoginApi, rhApi);

                //Example of Asunc code
                await DemoAsyncCalls(beeyeLoginApi, rhApi);
            }

            //cleaning up
            if (handler != null)
            {
                handler.Dispose();
            }
        }

        static void DemoSynchronousCalls(LoginClient login, EmployesClient rh)
        {
            Console.WriteLine("Synchronous calls demo");
            //Example of Sync conde
            //Call Login api. This call will also set up necessary cookie
            UpdateResultWithId loginResult = login.ApiLogin(apiKey);
            if (!CheckLogin(loginResult))
            {
                return;
            }
            //getting list of actives (not deleted) users
            var employyes = rh.GetAllRessources(true);
            PrintUsers(employyes);
            //logining out. This is for demo purpose only
            _ = login.LogOut();
        }

        static async Task DemoAsyncCalls(LoginClient login, EmployesClient rh)
        {
            Console.WriteLine(Environment.NewLine + "Async calls demo");
            //getting http response
            //Call Login api. This call will also set up necessary cookie
            var loginResponse = await login.ApiLoginAsync(apiKey);
            //Converting response into object.
            var loginResult = await loginResponse.Content.ReadAsAsync<UpdateResultWithId>();
            CheckLogin(loginResult);
            //getting list of actives (not deleted) users
            var employeesResponse = await rh.GetAllRessourcesAsync(true);
            var employyes = await employeesResponse.Content.ReadAsAsync<IEnumerable<EmployeJS__>>();
            PrintUsers(employyes);
            _ = login.LogOut();
        }

        #region Small helper
        private static bool CheckLogin(UpdateResultWithId loginResult)
        {
            if (!loginResult.Result)
            {
                Console.WriteLine("Login error. Please check your credentials");
            }
            else
            {
                Console.WriteLine($"Successfully logged using API. User Id  :{loginResult.NewId}");
            }
            return loginResult.Result;
        }

        private static void PrintUsers(IEnumerable<EmployeJS__> employyes)
        {
            foreach (var e in employyes)
            {
                Console.WriteLine($"Name : {e.FullName}, Id Beeye : {e.Id}, Title : {e.Occupation}, External id : {e.ExternalId}");
            }
        }
        #endregion
    }
}
