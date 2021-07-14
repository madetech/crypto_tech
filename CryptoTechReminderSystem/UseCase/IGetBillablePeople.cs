using CryptoTechReminderSystem.Boundary;

namespace CryptoTechReminderSystem.UseCase
{
    public interface IGetBillablePeople
    {
        GetBillablePeopleResponse Execute();
    }
}
