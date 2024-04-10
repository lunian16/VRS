using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRS
{
    public class Equipment
    {
        private int _equipmentid;
        public int Equipmentid
        {
            get => _equipmentid;
            set => _equipmentid = value;
        }
        private string _equipmentname;
        public string Equipmentname
        {
            get => _equipmentname;
            set => _equipmentname = value;
        }
        private double _equipmentprice;
        public double Equipmentprice
        {
            get => _equipmentprice; set => _equipmentprice = value;
        }
        public Equipment(int paraequipmentid, string equipmentname, double equipmentprice)
        {
            this.Equipmentid = paraequipmentid;
            this.Equipmentname = equipmentname;
            this.Equipmentprice = equipmentprice;
        }
        public Equipment() { }

        public string FullDetails => $"Equipment ID:{_equipmentid}, Equipment Name: {_equipmentname}, Equipment Daily Rate:{_equipmentprice}";


    }
}
