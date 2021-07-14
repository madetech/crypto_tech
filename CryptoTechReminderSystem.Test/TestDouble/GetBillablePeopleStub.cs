using System.Collections.Generic;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.UseCase;

namespace CryptoTechReminderSystem.Test.TestDouble
{
    public class GetBillablePeopleStub : IGetBillablePeople
    {
        private readonly List<GetBillablePeopleResponse.BillablePerson> _billablePeople;

        private static List<GetBillablePeopleResponse.BillablePerson> DefaultList => new List<GetBillablePeopleResponse.BillablePerson>
        {
            new GetBillablePeopleResponse.BillablePerson
                {
                    Id = "W0123CHAN"
                },
            new GetBillablePeopleResponse.BillablePerson
                {
                    Id = "W123AMON"
                },
            new GetBillablePeopleResponse.BillablePerson
                {
                    Id = "W789ROSS"
                }
        };
        
        public GetBillablePeopleStub(List<string> billablePeople = null)
        {
            var templateBillablePerson = new List<GetBillablePeopleResponse.BillablePerson>();

            if (billablePeople != null)
                foreach (var billablePerson in billablePeople)
                {
                    templateBillablePerson.Add(new GetBillablePeopleResponse.BillablePerson
                    {
                        Id = billablePerson
                    });
                }

            _billablePeople = templateBillablePerson.Count == 0 ? DefaultList : templateBillablePerson;
        }
        
        public GetBillablePeopleResponse Execute()
        {
            return new GetBillablePeopleResponse
            {
                BillablePeople = _billablePeople
            };
        }
    }
}
