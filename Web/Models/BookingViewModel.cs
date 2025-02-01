using Core.Entities;
using System;
using System.Collections.Generic;

namespace Web.Models
{
    public class BookingViewModel
    {
        public List<Booking> AllBookings { get; set; }
        public List<Status> AvailableStatuses { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

}
