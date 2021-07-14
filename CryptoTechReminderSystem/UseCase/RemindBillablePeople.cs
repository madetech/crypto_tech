using System;
using CryptoTechReminderSystem.Boundary;

namespace CryptoTechReminderSystem.UseCase
{
    public class RemindBillablePeople
    {
        private readonly IGetBillablePeople _getBillablePeople;
        private readonly ISendReminder _sendReminder;

        public RemindBillablePeople(IGetBillablePeople getBillablePeople, ISendReminder sendReminder)
        {
            _getBillablePeople = getBillablePeople;
            _sendReminder = sendReminder;
        }
        
        public void Execute(RemindBillablePeopleRequest remindBillablePeopleRequest)
        {
            var billablePeople = _getBillablePeople.Execute();
            
            foreach (var billablePerson in billablePeople.BillablePeople)
            {
                _sendReminder.Execute(new SendReminderRequest
                {
                    Channel = billablePerson.Id,
                    Text = remindBillablePeopleRequest.Message,
                    Email = billablePerson.Email
                });
            }
        }
    }
}
