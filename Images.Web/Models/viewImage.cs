using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Images.Data;

namespace Images.Web.Models
{
    public class viewImage
    {
        public Image image { get; set; }
        public int id { get; set; }
        public string message { get; set; }
        public bool submittedCorrectPassword { get; set; }
        public bool alreadyHasAccess { get; set; }
    }
}
