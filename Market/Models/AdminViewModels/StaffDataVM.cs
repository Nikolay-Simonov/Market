namespace Market.Models.AdminViewModels
{
    public class StaffDataVM
    {
        public string Id { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string FirtsName { get; set; }

        public string MiddelName { get; set; }

        public string LastName { get; set; }

        public string FullName => string.IsNullOrWhiteSpace(MiddelName)
            ? FirtsName + " " + LastName
            : FirtsName + " " + MiddelName + " " + LastName;
    }
}