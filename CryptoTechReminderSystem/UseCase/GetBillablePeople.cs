using System;
using System.Collections.Generic;
using System.Linq;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.Gateway;

namespace CryptoTechReminderSystem.UseCase
{
    public class GetBillablePeople : IGetBillablePeople
    {
        private readonly ISlackBillablePersonRetriever _slackBillablePersonRetriever;
        private readonly IClock _clock;

        public GetBillablePeople(ISlackBillablePersonRetriever slackBillablePersonRetriever, IClock clock)
        {
            _slackBillablePersonRetriever = slackBillablePersonRetriever;
            _clock = clock;
        }
        
        public GetBillablePeopleResponse Execute()
        {
            var getBillablePeopleResponse = new GetBillablePeopleResponse
            {
                BillablePeople = new List<GetBillablePeopleResponse.BillablePerson>()
            };
            
            var nonBillablePeople = GetNonBillablePeople();

            if (IsWeekend(_clock.Now())) return getBillablePeopleResponse;
            
            var slackGetBillablePeopleResponse = _slackBillablePersonRetriever.RetrieveBillablePeople();

            var dateFrom = GetStartingDate(_clock.Now());
            var dateTo = GetEndingDate(_clock.Now());
            
            foreach (var slackBillablePerson in slackGetBillablePeopleResponse)
            {
              if(!(Array.Exists(nonBillablePeople, element => element == slackBillablePerson.Email))){
                getBillablePeopleResponse.BillablePeople.Add(new GetBillablePeopleResponse.BillablePerson
                {
                  Id = slackBillablePerson.Id,
                  Email = slackBillablePerson.Email
                });
              }
            }
            return getBillablePeopleResponse;
        }
        private static DateTimeOffset GetStartingDate(DateTimeOffset currentDateTime)
        {
            var daysFromMonday = (7 + (currentDateTime.DayOfWeek - DayOfWeek.Monday)) % 7;
            
            return currentDateTime.AddDays(-daysFromMonday);
        }

        private static DateTimeOffset GetEndingDate(DateTimeOffset currentDateTime)
        {
            if (IsEndOfTheMonth(currentDateTime)) return currentDateTime;
            
            var daysToFriday = (7 + (DayOfWeek.Friday - currentDateTime.DayOfWeek)) % 7;
            
            return currentDateTime.AddDays(daysToFriday);
        }

        private static string RemoveTopLevelDomain(string email)
        {
            return email.Replace(".co.uk", "").Replace(".com","");
        }

        private static bool IsEndOfTheMonth(DateTimeOffset currentDateTime)
        {
            return DateTime.DaysInMonth(currentDateTime.Year, currentDateTime.Month) == currentDateTime.Day;
        }
        
        private static bool IsWeekend(DateTimeOffset currentDateTime)
        {
            var weekendDays = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday };
            return weekendDays.Contains(currentDateTime.DayOfWeek);
        }
        
        private static int ExpectedHoursByDate(int weeklyHours, DateTimeOffset currentDateTime)
        {
            var nonWorkingWeekDaysForPartTimeContract = 5 - weeklyHours / 7;
            var workingDaysByToday = currentDateTime.DayOfWeek - DayOfWeek.Monday + 1;

            return (workingDaysByToday - nonWorkingWeekDaysForPartTimeContract) * 7;
        }

        private static string[] GetNonBillablePeople()
        {
          var nonBillablePeople = Environment.GetEnvironmentVariable("NON_BILLABLE_PEOPLE");
          if(nonBillablePeople != null){
            return nonBillablePeople.Split(",");
          }
          return new string[0];
        }
    }
}
