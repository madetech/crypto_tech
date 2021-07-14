using System;
using System.Linq;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Test.TestDouble;
using CryptoTechReminderSystem.UseCase;
using FluentAssertions;
using NUnit.Framework;

namespace CryptoTechReminderSystem.Test.UseCase
{
    public class GetBillablePeopleTests
    {
        private static SlackGatewaySpy _slackGatewaySpy;
        private static SlackGatewayStub _slackGatewayStub;

        [SetUp]
        public void SetUp()
        {
            _slackGatewaySpy = new SlackGatewaySpy();
        }
        
        [TestFixture]
        public class CanGetBillablePeople
        {
            private ClockStub _clock;
            private GetBillablePeople _getBillablePeople;

            private GetBillablePeople _getBillablePeopleWithExclusions;
            
            [SetUp]
            public void SetUp()
            {
                _slackGatewaySpy = new SlackGatewaySpy();
                _clock = new ClockStub(
                    new DateTimeOffset(
                        new DateTime(2019, 03, 01, 10, 30, 0)
                    )
                );

                _getBillablePeople = new GetBillablePeople(_slackGatewaySpy, _clock);

                _slackGatewayStub = new SlackGatewayStub
                {
                    BillablePeople = new[]
                    {
                        new SlackBillablePerson
                        {
                            Email = "fred@fred.com",
                            Id = "U8723"
                        }, 
                        new SlackBillablePerson
                        {
                            Email = "Joe@Bloggs.com",
                            Id = "U9999"
                        },
                        new SlackBillablePerson
                        {
                            Email = "Jim@Bloggs.com",
                            Id = "U9998"
                        }
                    }
                };

                _getBillablePeopleWithExclusions = new GetBillablePeople(_slackGatewayStub, _clock);

            }

            [Test]
            public void CanGetSlackBillablePeople()
            {

                _getBillablePeople.Execute();

                _slackGatewaySpy.IsRetrieveBillablePeopleCalled.Should().BeTrue();
            }

            [Test]
            public void CanGetSlackBillablePeopleAndExlcudeNonBillable()
            {
                Environment.SetEnvironmentVariable("NON_BILLABLE_PEOPLE","Joe@Bloggs.com,Jim@Bloggs.com");
                
                var response = _getBillablePeopleWithExclusions.Execute();

                response.BillablePeople.Should().HaveCount(1);
            }
        } 
    }
}
