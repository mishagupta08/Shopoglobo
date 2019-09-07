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

                Session["LatestProduct"] = objDashboardDetails.FontpageSections.SpeacialSegment;
            }
            catch (Exception ex)
            {

            }
            return View(objDashboardDetails);
        }

        public ActionResult TermsAndConditions()
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
            return View();
        }

        public  ActionResult PrivacyPolicy()
        {
            try
            {
               
            }
            catch (Exception ex)
            {

            }
            return View();
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

        public async Task<ActionResult> ProductList(string cat,string root,int? page,string SortBy, string Order,string FilterFromPoint ,string FilterToPoint,string searchString)
        {
            Filters c = new Filters();
            if (!string.IsNullOrEmpty(cat))
            {
                c.CategoryId = Convert.ToInt16(cat);
            }            
            c.pageNo = page??1;
            c.NoOfRecord = 10;
            c.SelectedFilterName = "Title";
            c.FilterValue = searchString;
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
            List<Product> listProducts = new List<Product>();
            double totalcount = 0;
            if (result.Status == true && result.ResponseValue != null)
            {
                totalcount = result.TotalRecords;
                listProducts = JsonConvert.DeserializeObject<List<Product>>(result.ResponseValue);
            }

            string catName = string.Empty;            
            var catDetail = await objRepository.GetCategoryDetail(c);
            catName = catDetail.title;

            PagewiseProducts finalprodlist = new PagewiseProducts();            
            finalprodlist.ProductList = listProducts;

            var list = new List<int>();
            for (var i = 1; i <= totalcount; i++)
            {
                list.Add(i);
            }
            finalprodlist.pagerCount = list.ToPagedList(Convert.ToInt32(c.pageNo), 10);           


            ViewBag.category = cat;
            ViewBag.CategoryName = catName;
            ViewBag.Page = page;
            ViewBag.ParentId = root;   
                     
            return View(finalprodlist);           
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
            SideBar objsidebar = new SideBar();
            var CategoryList = new List<Category>();
            int categor = 0;
            if (!string.IsNullOrEmpty(Cat))
            {
                categor = Convert.ToInt16(Cat);
            }
            if (Session["MenuList"] != null && categor!=0)
            {
                var MenuItems = Session["MenuList"] as List<Category>;
                CategoryList = getNestedChildren(MenuItems.Where(r => r.status == true && r.id == categor).ToList(), MenuItems);
            }
            ViewBag.ParentId = Cat;
            ViewBag.category = subCat;
            ViewBag.Page = 1;
            objsidebar.categoryList = CategoryList;
            if (Session["LatestProduct"] != null)
            {
                var product = Session["LatestProduct"] as List<Product>;
                objsidebar.latestProduct = product.FirstOrDefault();
            }
            
            return PartialView("_filterSideBar", objsidebar);
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

        public ActionResult Claims()
        {
            return View();
        }

        public async Task<ActionResult> SearchProductList( int? page, string SortBy, string Order, string searchString)
        {
            Filters c = new Filters();
            
            c.pageNo = page;
            c.NoOfRecord = 10;
            c.SelectedFilterName = "Title";
            c.FilterValue = searchString;
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

            var result = await objRepository.GetCategoryProducts(c);
            List<Product> listProducts = new List<Product>();
            double totalcount = 0;
            if (result.Status == true && result.ResponseValue != null)
            {
                totalcount = result.TotalRecords;
                listProducts = JsonConvert.DeserializeObject<List<Product>>(result.ResponseValue);
            }

            PagewiseProducts finalprodlist = new PagewiseProducts();
            finalprodlist.SearchString = searchString;
            finalprodlist.ProductList = listProducts;

            var list = new List<int>();
            for (var i = 1; i <= totalcount; i++)
            {
                list.Add(i);
            }
            finalprodlist.pagerCount = list.ToPagedList(Convert.ToInt32(page), 10);                                
            ViewBag.Page = page; 
                      
            return View(finalprodlist);
        }

        public async Task<ActionResult> GetAllDealProducts(string Deal, int? page, string SortBy, string Order)
        {
            PagewiseProducts productlist = new PagewiseProducts();
            try {
                Filters c = new Filters();
                string companyId = System.Configuration.ConfigurationManager.AppSettings["CompanyId"];

                c.CompanyId = Convert.ToInt16(companyId);
                c.pageNo = page;
                c.NoOfRecord = 10;
                productlist.SearchString = Deal;
                productlist.sortby = SortBy;
                productlist.order = Order;

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

                var result = await objRepository.GetDealProductsFullList(c,Deal);
                List<Product> listProducts = new List<Product>();
                double totalcount = 0;
                if (result.Status == true && result.ResponseValue != null)
                {
                    totalcount = result.TotalRecords;
                    listProducts = JsonConvert.DeserializeObject<List<Product>>(result.ResponseValue);
                }
               
                productlist.ProductList = listProducts;

                var list = new List<int>();
                for (var i = 1; i <= totalcount; i++)
                {
                    list.Add(i);
                }

                productlist.pagerCount = list.ToPagedList(Convert.ToInt32(page), 10);
                ViewBag.Page = page;
            }
            catch (Exception ex)
            {
            }
            return View("DealProducts",productlist);

        }

        public ActionResult MyAccount()
        {
            return View();
        }

        public ActionResult DiscountCoupons()
        {
            return View();
        }

        public ActionResult Orders()
        {
            return View();
        }

        public ActionResult UpdateAccount()
        {
            return View();
        }


    }
}