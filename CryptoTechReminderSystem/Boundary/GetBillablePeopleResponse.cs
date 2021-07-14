using System.Collections.Generic;

namespace CryptoTechReminderSystem.Boundary
{
    public class GetBillablePeopleResponse
    {
        public List<BillablePerson> BillablePeople;

        public class BillablePerson
        {
            public string Id { get; set; }
            public string Email { get; set; }
        }
    }
    
}
