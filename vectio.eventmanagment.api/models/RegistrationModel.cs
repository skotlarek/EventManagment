using System;

namespace vectio.eventmanagement.api.models
{
    public class RegistrationModel
    {
        public Guid EventId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

    }
}
