using System;
using System.Configuration;
using System.Linq;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.Test.TestDouble;
using CryptoTechReminderSystem.UseCase;
using FluentAssertions;
using NUnit.Framework;

namespace CryptoTechReminderSystem.Test
{
    public class ShameLateDevelopersTests
    {
        private RemindDeveloperSpy _remindDeveloperSpy;
        private GetLateDevelopersSpy _getLateDevelopersSpy;
        private ClockStub _clock;

        [SetUp]
        public void SetUp()
        {
            _remindDeveloperSpy = new RemindDeveloperSpy();
            _getLateDevelopersSpy = new GetLateDevelopersSpy();
            _clock = new ClockStub(
                new DateTimeOffset(
                    new DateTime(2019, 03, 01, 13, 30, 0)
                )
            );
        }
        [Test]
        public void CanGetLateDevelopers()
        {
            var shameLateDevelopers = new ShameLateDevelopers
                (_getLateDevelopersSpy, _remindDeveloperSpy, _clock);

            shameLateDevelopers.Execute(
                new ShameLateDevelopersRequest()
            );

            _getLateDevelopersSpy.Called.Should().BeTrue();
        }

        [Test]
        public void CanRemindDevelopers()
        {
            var shameLateDevelopers = new ShameLateDevelopers(_getLateDevelopersSpy, _remindDeveloperSpy, _clock);

            shameLateDevelopers.Execute(
                new ShameLateDevelopersRequest()
                {
                    Message = "TIMESHEETS ARE GOOD YO!"
                }
            );

            _remindDeveloperSpy.Called.Should().BeTrue();
        }

        [Test]
        [TestCase( "W0123CHAN", "W123AMON", "W789ROSS")]
        [TestCase( "W0123CHAN", "W123AMON")]
        public void CanCheckMessageHasAllUsers(params string[] userId)
        {
            var getLateDevelopersStub = new GetLateDevelopersStub(userId.ToList());
            var shameLateDevelopers = new ShameLateDevelopers(getLateDevelopersStub,_remindDeveloperSpy, _clock);
            var expectedMessage = "TIMESHEETS ARE GOOD YO!";

            shameLateDevelopers.Execute(
                new ShameLateDevelopersRequest
                {
                    Message = expectedMessage
                }
            );

            foreach (var user in userId)
            {
                expectedMessage += $"\n• <@{user}>";
            }
            
            _remindDeveloperSpy.Text.Should().Be(expectedMessage);
        }

        [Test]
        public void CanCheckMessageHadChannel()
        {
            var shameLateDevelopers = new ShameLateDevelopers(_getLateDevelopersSpy,_remindDeveloperSpy, _clock);

            const string expectedChannel = "CH123456";
            
            shameLateDevelopers.Execute(
                new ShameLateDevelopersRequest
                {
                    Channel = expectedChannel
                }
            );
            
            _remindDeveloperSpy.Channels.First().Should().Be(expectedChannel);
        }
        
        [Test]
        [TestCase(01, 13, 30, true)]
        [TestCase(01, 13, 00, false)]
        [TestCase(01, 14, 00, false)]
        [TestCase(02, 13, 30, false)]
        [TestCase(03, 13, 30, false)]
        public void CanOnlyShameLateDevelopersAtLearnTech(int day, int hour, int minute, bool expectedOutcome)
        {
            var clock = new ClockStub(
                new DateTimeOffset(
                    new DateTime(2019, 03, day, hour, minute, 0)
                )
            );
            var shameLateDevelopers = new ShameLateDevelopers(_getLateDevelopersSpy, _remindDeveloperSpy, clock);

            shameLateDevelopers.Execute(
                new ShameLateDevelopersRequest
                {
                    Message = "TIMESHEETS ARE GOOD YO!"
                }
            );

            _remindDeveloperSpy.Called.Should().Be(expectedOutcome);
        }
    }
}
