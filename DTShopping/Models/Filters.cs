using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTShopping.Models
{
    public class Filters
    {
        public Nullable<int> pageNo { get; set; }

        public int NoOfRecord { get; set; }

        public Nullable<int> VendorId { get; set; }

        public Nullable<int> CompanyId { get; set; }

        public Nullable<int> CategoryId { get; set; }

        public byte Status { get; set; }

        public string pageName { get; set; }

        public Nullable<int> Id { get; set; }
    }
}