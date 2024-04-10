using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRS
{
    public class CategoryList
    {
        private int _categoryid;
        public int Categoryid
        {
            get => _categoryid; set => _categoryid = value;
        }
        private string _categoryname;
        public string Categoryname
        {
            get=> _categoryname; set => _categoryname = value;
        }
        public CategoryList(int paracategoryid, string paracategoryName)
        {
            this.Categoryid=paracategoryid;
            this.Categoryname=paracategoryName;
        }

        public CategoryList() { }
        public string FullDetails => $"Category ID:{Categoryid}, Category Name:{Categoryname}";
    }
}
