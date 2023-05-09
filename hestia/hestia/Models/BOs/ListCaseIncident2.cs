namespace hestia.Models.BOs
{
    public class ListCaseIncident2
    {
        public string DisplayName => familyMembers.Where(mem => mem.keyPlayer.Equals("Y")).FirstOrDefault().lastName;

        public string Address => formatAddress();

        public string caseIncidentNumber { get; set; }
        public string entityType { get; set; }
        public List<FamilyMember> familyMembers { get; set; }
        public string createdDate { get; set; }
        public string caseIncidentType { get; set; }
        public string dateReported { get; set; }
        public string unitNo { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string city { get; set; }
        public string postalCode { get; set; }
        public string provinceState { get; set; }
        public string country { get; set; }
        public string serviceOffice { get; set; }

        private string formatAddress()
        {
            var address = unitNo + addressLine1 + addressLine2
                + city + postalCode + provinceState + country;
            if (address.Length == 0)
            {
                return "NA";
            }
            else
            {
                return (unitNo.Length > 0 ? unitNo : "N/A") + ", " + (addressLine1.Length > 0 ? addressLine1 : "N/A") +
                    ", " + (addressLine2.Length > 0 ? addressLine2 : "N/A") + ", " + (city.Length > 0 ? city : "N/A") +
                    ", " + (postalCode.Length > 0 ? postalCode : "N/A") + ", " + (provinceState.Length > 0 ? provinceState : "N/A") +
                    ", " + (country.Length > 0 ? country : "N/A"); // Refactor this
            }
        }
    }
}

