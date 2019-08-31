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

        public async Task<ActionResult> ProductList(string cat,string root,int? page,string SortBy, string Order,string FilterFromPoint ,string FilterToPoint)
        {
            Filters c = new Filters();
            c.CategoryId = Convert.ToInt16(cat);
            c.pageNo = page;
            c.NoOfRecord = 10;
            if (!string.IsNullOrEmpty(SortBy))
            {
                if (SortBy.ToLower() == "price")
                {
                    c.SortByPrice = true;
                    if (Order.ToLower() == "asc")
                    {
                        c.IsPriceLowToHigh = true;
                    }
                    else
                    {
                        c.IsPriceLowToHigh = false;
                    }
                }
                else if (SortBy.ToLower() == "points")
                {
                    c.SortByPoints = true;
                    if (Order.ToLower() == "asc")
                    {
                        c.IsPointLowToHigh = true;
                    }
                    else
                    {
                        c.IsPointLowToHigh = false;
                    }
                }
            }
            if (!string.IsNullOrEmpty(FilterFromPoint) && !string.IsNullOrEmpty(FilterToPoint))
            {
                c.FilterFromPoint = Convert.ToInt32(FilterFromPoint);
                c.FilterToPoint = Convert.ToInt32(FilterToPoint);
            }

            var result = await objRepository.GetCategoryProducts(c);
            List<Product> listProducts = JsonConvert.DeserializeObject<List<Product>>(result.ResponseValue);
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            var Filteredlist = listProducts.ToPagedList(pageIndex, pageSize);
            ViewBag.category = cat;
            ViewBag.Page = page;
            ViewBag.ParentId = root;            
            return View(listProducts);           
        }

        [HttpGet]
        public async Task<ActionResult> GetCategories()
        {
            List<Category> list = new List<Category>();
            try
            {
                //if (Session["MenuList"] != null)
                //{

                //}
                //else
                //{
                    var MenuItems = await objRepository.GetMenuList();
                if (MenuItems != null)
                {
                    MenuItems = MenuItems.Where(r => r.status == true).OrderBy(r => r.ordar).ToList();
                    list = getNestedChildren(MenuItems.Where(r => r.status == true && r.parent_id == 1 && r.parent_id != r.id).ToList(), MenuItems.Where(r => r.status == true).ToList());
                    Session["MenuList"] = list;
                }
                //}                                                      
            }
            catch (Exception ex)
            {

            }
            return PartialView("Category", list);
        }

        public ActionResult getCatHeirarchy(string Cat,string subCat)
        {
            var CategoryList = new List<Category>();
            int categor = 1;
            if (!string.IsNullOrEmpty(Cat))
            {
                categor = Convert.ToInt16(Cat);
            }
            if (Session["MenuList"] != null)
            {
                var MenuItems = Session["MenuList"] as List<Category>;
                CategoryList = getNestedChildren(MenuItems.Where(r => r.status == true && r.id == categor).ToList(), MenuItems);
            }
            ViewBag.ParentId = Cat;
            ViewBag.category = subCat;
            ViewBag.Page = 1;
            
            return PartialView("_filterSideBar", CategoryList);
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

        public ActionResult Claims()
        {
            return View();
        }
    }
}