using System.Collections.Generic;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.UseCase;

namespace CryptoTechReminderSystem.Test.TestDouble
{
    public class GetBillablePeopleEmptyStub : IGetBillablePeople
    {
        public GetBillablePeopleResponse Execute()
        {
            return new GetBillablePeopleResponse
            {
                BillablePeople = new List<GetBillablePeopleResponse.BillablePerson>()
            };
        }
    }
}
