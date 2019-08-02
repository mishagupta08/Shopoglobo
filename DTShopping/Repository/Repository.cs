using DTShopping.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DTShopping.Repository
{
    public class APIRepository
    {

        private string ApiUrl = System.Configuration.ConfigurationManager.AppSettings["ApiUrl"];

        public async Task<List<Category>> GetMenuList()
        {            
            var result = await CallPostFunction(string.Empty, "ManageCategories/List");
            if (result == null || !result.Status)
            {
                return null;
            }
            else
            {
                try
                {
                    var CategoryList = JsonConvert.DeserializeObject<List<Category>>(result.ResponseValue);
                    return CategoryList;
                }
                catch (Exception ex)
                {

                }
                return null;
            }
        }

        public async Task<List<Banners>> GetBannerImageList(string Id)
        {
            var result = await CallPostFunction(string.Empty, "GetBannerImageList/" + Id);
            if (result == null || !result.Status)
            {
                return null;
            }
            else
            {
                var BannersList = JsonConvert.DeserializeObject<List<Banners>>(result.ResponseValue);
                return BannersList;
            }
        }

        public async Task<UserDetails> Login(UserDetails user)
        {
            var result = await CallPostFunction(string.Empty, "LoginShoppigPortalUser");
            if (result == null || !result.Status)
            {
                return null;
            }
            else
            {
                var UserDetails = JsonConvert.DeserializeObject<UserDetails>(result.ResponseValue);
                return UserDetails;
            }
        }

        public async Task<UserDetails> Register(UserDetails user)
        {
            var result = await CallPostFunction(string.Empty, "ManageVendor/Add");
            if (result == null || !result.Status)
            {
                return null;
            }
            else
            {
                var UserDetails = JsonConvert.DeserializeObject<UserDetails>(result.ResponseValue);
                return UserDetails;
            }
        }

        private async Task<Response> CallPostFunction(string detail, string action)
        {
            using (var httpClient = new HttpClient())
            {
                // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                var httpContent = new StringContent(detail, Encoding.UTF8, "application/json");

                // Do the actual request and await the response
                var httpResponse = await httpClient.PostAsync(ApiUrl + action, httpContent);

                // If the response contains content we want to read it!
                if (httpResponse.Content != null)
                {
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<Response>(responseContent);

                    return result;
                }
            }

            return null;
        }

        private async Task<Response> CallGetFunction(string action)
        {
            using (var httpClient = new HttpClient())
            {
                // Do the actual request and await the response
                var httpResponse = await httpClient.GetAsync(ApiUrl + action);

                // If the response contains content we want to read it!
                if (httpResponse.Content != null)
                {
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<Response>(responseContent);

                    return result;
                }
            }

            return null;
        }



    }
}