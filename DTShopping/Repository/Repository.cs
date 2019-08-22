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
        private int RoleId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["RoleId"]);

        private int CompanyId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CompanyId"]);

        private string ApiUrl = System.Configuration.ConfigurationManager.AppSettings["ApiUrl"];

        private string GetGetShoppingPortalAllFrontPageProductsListAction = "GetShoppingPortalAllFrontPageProductsList/";

        private string ManageListWithFilterAction = "ManageListWithFilter";

        private string ManageProductsAction = "ManageProducts/";

        private string GetUserPointsByUserIdAction = "GetUserPointsByUserId/";

        private string ManageProductImagesAction = "ManageProductImages/";

        private string ManageCartAction = "ManageCart/";

        private string ManageShoppingProductsListWithFilter = "ManageShoppingProductsListWithFilter/";

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

        public async Task<ShoppingPortalFrontPageProdList> GetGetShoppingPortalAllFrontPageProductsList()
        {
            var result = await CallPostFunction(string.Empty, GetGetShoppingPortalAllFrontPageProductsListAction + CompanyId);
            if (result == null)
            {
                return null;
            }
            else
            {
                var productList = JsonConvert.DeserializeObject<ShoppingPortalFrontPageProdList>(result.ResponseValue);
                return productList;
            }
        }

        public async Task<Response> Register(UserDetails user)
        {
            user.role_id = RoleId;
            user.company_id = CompanyId;
            var detail = JsonConvert.SerializeObject(user);
            var result = await CallPostFunction(detail, "ManageVendor/Add");
            if (result == null || !result.Status)
            {
                return null;
            }
            else
            {
                //var result = JsonConvert.DeserializeObject<Response>(result.ResponseValue);
                return result;
            }
        }

        public async Task<List<R_StateMaster>> GetStateList()
        {
            var result = await CallPostFunction(string.Empty, "StateList");
            if (result == null || !result.Status)
            {
                return null;
            }
            else
            {
                var states = JsonConvert.DeserializeObject<List<R_StateMaster>>(result.ResponseValue);
                return states;
            }
        }

        public async Task<List<R_CityMaster>> GetCityListById(string Id)
        {
            var result = await CallPostFunction(string.Empty, "CityList/" + Id);
            if (result == null || !result.Status)
            {
                return null;
            }
            else
            {
                var cities = JsonConvert.DeserializeObject<List<R_CityMaster>>(result.ResponseValue);
                return cities;
            }
        }

        public async Task<Response> ManageCart(CartFilter filter, string action)
        {
            var filterData = JsonConvert.SerializeObject(filter);
            var result = await CallPostFunction(filterData, ManageCartAction + action);
            if (result == null)
            {
                return null;
            }
            else
            {
                return result;
            }
        }

        public async Task<Response> GetProductImage(int prodId)
        {
            var data = new product_images();
            data.product_id = prodId;
            var d = JsonConvert.SerializeObject(data);
            var result = await CallPostFunction(d, ManageProductImagesAction + "List");
            if (result == null)
            {
                return null;
            }
            else
            {
                return result;
            }
        }

        public async Task<Product> GetProductDetailById(List<Product> products)
        {
            var productData = JsonConvert.SerializeObject(products);
            var result = await CallPostFunction(productData, ManageProductsAction + "ById");
            if (result == null || !result.Status)
            {
                return null;
            }
            else
            {
                var prod = JsonConvert.DeserializeObject<Product>(result.ResponseValue);
                return prod;
            }
        }

        public async Task<Response> GetCategoryProducts(Filters FilterDetails)
        {
            var productData = JsonConvert.SerializeObject(FilterDetails);
            var result = await CallPostFunction(productData, ManageShoppingProductsListWithFilter);
            if (result == null)
            {
                return null;
            }
            else
            {
                return result;
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

        //send ProductByCategory in pageName for parod list with page no detail.
        public async Task<Response> ManageListWithFilter(Filters filter)
        {
            var detail = JsonConvert.SerializeObject(filter);
            var result = await CallPostFunction(detail, ManageListWithFilterAction);
            return result;
        }
    }
}