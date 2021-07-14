using System.Collections.Generic;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.Test.TestDouble;
using CryptoTechReminderSystem.UseCase;
using FluentAssertions;
using NUnit.Framework;

namespace CryptoTechReminderSystem.Test.UseCase
{
    public class RemindBillablePeopleTests
    {
        private SendReminderSpy _sendReminderSpy;
        private GetBillablePeopleSpy _getBillablePeopleSpy;
        private GetBillablePeopleStub _getBillablePeopleStub;

        [SetUp]
        public void SetUp()
        {
            _sendReminderSpy = new SendReminderSpy();
            _getBillablePeopleSpy = new GetBillablePeopleSpy();
            _getBillablePeopleStub = new GetBillablePeopleStub();  
        }
        
        private void HandleSetUp(IGetBillablePeople GetBillablePeople)
        {
            var remindBillablePeople = new RemindBillablePeople(GetBillablePeople, _sendReminderSpy);
            
            remindBillablePeople.Execute(
                new RemindBillablePeopleRequest
                {
                    Message = "TIMESHEETS ARE GOOD YO!"
                }
            );
        }
        
        [Test]
        public void CanGetBillablePeople()
        {
            HandleSetUp(_getBillablePeopleSpy);
               
            _getBillablePeopleSpy.Called.Should().BeTrue(); 
        }
        
        [Test]
        public void CanRemindBillablePeople()
        {
            HandleSetUp(_getBillablePeopleStub);

            _sendReminderSpy.Called.Should().BeTrue(); 
        }
        
        [Test]
        public void CanRemindAllBillablePeople()
        {
           HandleSetUp(_getBillablePeopleStub);

            _sendReminderSpy.CountCalled.Should().Be(3);
        }
        
        [Test]
        public void CanRemindBillablePeopleDirectly()
        {
            HandleSetUp(_getBillablePeopleStub);
            
            _sendReminderSpy.Channels.Should().BeEquivalentTo(
                new List<string> {
                    "W0123CHAN",
                    "W123AMON",
                    "W789ROSS"
                }
            );
        }
        
        [Test]
        public void CanRemindBillablePeopleDirectlyWithAMessage()
        {
            HandleSetUp(_getBillablePeopleStub);

            _sendReminderSpy.Text.Should().Be("TIMESHEETS ARE GOOD YO!");
        }   
    }    
}
