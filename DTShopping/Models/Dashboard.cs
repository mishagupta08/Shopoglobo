using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTShopping.Models
{
    public class Dashboard
    {
        public List<Category> MenuItems { get; set; }
        public List<Banners> Banners { get; set; }
        public List<R_StateMaster> States { get; set; }
        public List<R_CityMaster> Cities { get; set; }
        public List<Product> Products { get; set; }
        public UserDetails User { get; set; }
    }
}