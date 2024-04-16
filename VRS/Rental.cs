using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRS
{
    public class Rental
    {
        private int _equipmentid;
        public int equipmentid
        {
            get => _equipmentid;
            set => _equipmentid = value;
        }
        private DateTime _rentaldate;
        public DateTime rentaldate
        {
            get => _rentaldate;
            set => _rentaldate = value;

        }
        private DateTime _returndate;
        public DateTime returndate
        {
            get=> _returndate;
            set => _returndate = value;
        }
        public Rental() { }
        public string FullDetails => $"Rental:{_equipmentid} has been rented from {rentaldate} to {returndate}";
    }
}
