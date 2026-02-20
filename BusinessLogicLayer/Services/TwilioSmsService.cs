using BusinessLogicLayer.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML.Messaging;
using Twilio.Types;

namespace BusinessLogicLayer.Services
{
    public class TwilioSmsService : ISmsService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromPhoneNumber;
        private readonly string? _testRecipient; // For Trial accounts

        public TwilioSmsService(IConfiguration config)
        {
            // Pull directly from config inside the constructor
            _accountSid = config["Twilio:AccountSID"] ?? "";
            _authToken = config["Twilio:AuthToken"] ?? "";
            _fromPhoneNumber = config["Twilio:TwilioPhoneNumber"] ?? "";
            _testRecipient = config["Twilio:TwilioVirtualNumberTo"];

            TwilioClient.Init(_accountSid, _authToken);
        }

        public async Task SendSmsAsync(string toPhoneNumber, string message)
        {
            // If we have a test recipient configured, redirect the message there.
            // Otherwise, send it to the actual intended user.
            var finalRecipient = !string.IsNullOrEmpty(_testRecipient)
                ? _testRecipient
                : toPhoneNumber;

            var body = !string.IsNullOrEmpty(_testRecipient)
                ? $"[To: {toPhoneNumber}] {message}"
                : message;

            await MessageResource.CreateAsync(
                to: new PhoneNumber(finalRecipient),
                from: new PhoneNumber(_fromPhoneNumber),
                body: body
            );
        }
    }
}
