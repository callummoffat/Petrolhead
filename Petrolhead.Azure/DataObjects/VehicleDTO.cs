using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Petrolhead.Azure.DataObjects
{
    public class VehicleDTO
        : ModelBase
    {
        public string Manufacturer { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string ModelIdentifier { get; set; }
        public string ChassisType { get; set; }
        public string EngineType { get; set; }
        public string NumberPlate { get; set; }
        public int NumberOfSeats { get; set; }
        public int NumberOfWheels { get; set; }
        public long Mileage { get; set; }


        public DateTimeOffset? YearManufactured { get; set; }
        public DateTimeOffset? YearPurchased { get; set; }
        public DateTimeOffset? DateOfNextWarrant { get; set; }
        public DateTimeOffset? DateOfNextRegistration { get; set; }

        public virtual ICollection<ExpenseDTO> Expenses { get; set; }
        public virtual ICollection<RefuelDTO> Refuels { get; set; }
        public virtual ICollection<RepairDTO> Repairs { get; set; }
    }
}