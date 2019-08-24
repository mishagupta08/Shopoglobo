using System.Web.Mvc;
using DTShopping.Repository;
using System;
using System.Collections.Generic;
using DTShopping.Models;
using System.Threading.Tasks;
using System.Linq;
using PagedList;
using System.Threading;
using Newtonsoft.Json;

namespace DTShopping.Controllers
{
    
    public class HomeController : Controller
    {
        APIRepository objRepository = new APIRepository();
        public async Task<ActionResult> Index()
        {
            Dashboard objDashboardDetails = new Dashboard();
            string companyId = System.Configuration.ConfigurationManager.AppSettings["CompanyId"];
            try
            {
                objDashboardDetails.Banners = new List<Banners>();           
                objDashboardDetails.Banners = await objRepository.GetBannerImageList(companyId);

                objDashboardDetails.FontpageSections = new ShoppingPortalFrontPageProdList();
                objDashboardDetails.FontpageSections =  await objRepository.GetShoppingPortalFrontPageProdList(companyId);
            }
            catch (Exception ex)
            {

            }
            return View(objDashboardDetails);
        }

        public async Task<ActionResult> GetProductDetail(int prodId)
        {
            var dashboard = new Dashboard();
            this.objRepository = new APIRepository();
            try
            {
                var prodList = new List<Product>();
                prodList.Add(new Product { id = prodId });
                dashboard.ProductDetail = await objRepository.GetProductDetailById(prodList);
                if (dashboard.ProductDetail != null)
                {
                    dashboard.ProductDetail.description_detail = dashboard.ProductDetail.description_detail.Replace("\r\n\r\n", "");
                }
            }
            catch (Exception ex)
            {

            }

            return View("productDetailPage", dashboard);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public async Task<ActionResult> ProductList(string cat,int? page)
        {
            Filters c = new Filters();
            c.CategoryId = Convert.ToInt16(cat);
            c.pageNo = page;
            c.NoOfRecord = 10;
            var result = await objRepository.GetCategoryProducts(c);
            List<Product> listProducts = JsonConvert.DeserializeObject<List<Product>>(result.ResponseValue);
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            var Filteredlist = listProducts.ToPagedList(pageIndex, pageSize);
            return View(Filteredlist);           
        }

        [HttpGet]
        public async Task<ActionResult> GetCategories()
        {
            List<Category> list = new List<Category>();
            try
            {
                if (Session["MenuList"] != null)
                {

                }
                else
                {
                    var MenuItems = await objRepository.GetMenuList();
                    list = getNestedChildren(MenuItems.Where(r => r.parent_id == 1 && r.parent_id != r.id).ToList(), MenuItems);
                    Session["MenuList"] = list;
                }                                                      
            }
            catch (Exception ex)
            {

            }
            return PartialView("Category", list);
        }
       
        public List<Category> getNestedChildren(List<Category> ParentList, List<Category> MenuList)
        {
            var orderedList = new List<Category>();
            if (ParentList.Count > 0)
            {
                foreach (var parent in ParentList)
                {
                    var submenu = MenuList.Where(r => r.parent_id == parent.id).ToList();
                    if (submenu.Count > 0 && !(parent.id == parent.parent_id))
                    {
                        parent.Childern = new List<Category>();
                        parent.Childern = (getNestedChildren(submenu, MenuList));
                    }
                    orderedList.Add(parent);
                }

            }
            return orderedList;
        }

        public async Task<ActionResult> ListProducts(string category, int? page)
        { 
            Filters c = new Filters();            
            var result = await objRepository.GetCategoryProducts(c);
            List<Product> listProducts = JsonConvert.DeserializeObject<List<Product>>(result.ResponseValue);
            int pageSize = 5;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            var Filteredlist = listProducts.ToPagedList(pageIndex, pageSize);
            return View(Filteredlist);
        }        
    }
}