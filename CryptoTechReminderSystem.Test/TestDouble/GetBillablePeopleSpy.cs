using System.Collections.Generic;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.UseCase;

namespace CryptoTechReminderSystem.Test.TestDouble
{
    public class GetBillablePeopleSpy : IGetBillablePeople
    {
        public bool Called { private set; get; }

        public GetBillablePeopleResponse Execute()
        {
            Called = true;
            
            return new GetBillablePeopleResponse
            {
                BillablePeople = new List<GetBillablePeopleResponse.BillablePerson>
                {
                    new GetBillablePeopleResponse.BillablePerson()
                    {
                        Id = "U9034950",
                        Email = "UncleCraig@aol.com"
                    }
                }
            };
        }
    }
}
