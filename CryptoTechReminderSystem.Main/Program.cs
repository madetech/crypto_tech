using System;
using System.Collections.Generic;
using System.Threading;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.Gateway;
using CryptoTechReminderSystem.UseCase;
using dotenv.net;
using FluentScheduler;

namespace CryptoTechReminderSystem.Main
{
    class Clock : IClock
    {
        public DateTimeOffset Now()
        {
            return DateTimeOffset.Now;
        }
    }

    public class Program : Registry
    {
        private static void Main()
        {
            try
            {
                DotEnv.Config(false);
                JobManager.Initialize(new ReminderRegistry());
                SpinWait.SpinUntil(() => false);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}.");
            }
        }
    }

    public class ReminderRegistry : Registry
    {
        private readonly GetBillablePeople _getBillablePeople;
        private readonly SendReminder _sendReminder;

        public ReminderRegistry()
        {
            var slackGateway = new SlackGateway(
                "https://slack.com/",
                Environment.GetEnvironmentVariable("SLACK_TOKEN")
            );

            var clock = new Clock();
            _getBillablePeople = new GetBillablePeople(slackGateway, clock);
            _sendReminder = new SendReminder(slackGateway);

            CreateSchedule();
        }

        private void ResetSchedule()
        {
            JobManager.RemoveAllJobs();
            CreateSchedule();
        }

        private void CreateSchedule()
        {
            if (!IsLastDayOfTheMonthFridayOrWeekend())
            {
                JobManager.AddJob(ScheduleJobs, s => s.ToRunEvery(0).Months().OnTheLastDay().At(9, 55));
                JobManager.AddJob(ResetSchedule, s => s.ToRunEvery(0).Months().OnTheLastDay().At(12, 45));
            }
            JobManager.AddJob(ScheduleJobs, s => s.ToRunEvery(0).Weeks().On(DayOfWeek.Friday).At(9, 55));
            JobManager.AddJob(ResetSchedule, s => s.ToRunEvery(0).Weeks().On(DayOfWeek.Friday).At(12, 45));
        }

        private void ScheduleJobs()
        {
            // TODO: Comments to explain reasoning behind times chosen, e.g. what are the Ops Team's deadlines for approval?
            JobManager.AddJob(RemindBillablePeopleJob, s => s.ToRunOnceAt(10, 0));
        }

        private void RemindBillablePeopleJob()
        {
            var remindBillablePeople = new RemindBillablePeople(_getBillablePeople, _sendReminder);
            remindBillablePeople.Execute(
                new RemindBillablePeopleRequest
                {
                    Message = Environment.GetEnvironmentVariable("SLACK_REMINDER_MESSAGE")
                }
            );
        }

        private bool IsLastDayOfTheMonthFridayOrWeekend()
        {
            var lastDayOfTheMonth = DateTime.DaysInMonth(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month);

            var lastDayOfTheMonthDayOfWeek = new DateTimeOffset(
                new DateTime(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month, lastDayOfTheMonth)
            ).DayOfWeek;

            return new List<DayOfWeek>
            {
                DayOfWeek.Friday,
                DayOfWeek.Saturday,
                DayOfWeek.Sunday
            }.Contains(lastDayOfTheMonthDayOfWeek);
        }
    }
}
