using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

//CloudMailService is a production service to use instead of LocalMailService. It doesn't actually send an email.
//The code is just here as an example.
namespace CityInfo.API.Services
{
    public class CloudMailService : IMailService
    {
        private string _mailTo = Startup.Configuration["mailSettings:mailToAddress"];
        private string _mailFrom = Startup.Configuration["mailSettings:mailFromAddress"];

        public void Send(string subject, string message)
        {
            //Send a "live" email.
            Debug.WriteLine($"Mail from {_mailFrom} to {_mailTo}, with LocalMailService.");
            Debug.WriteLine($"Subject: {subject}");
            Debug.WriteLine($"Message: {message}");
        }
    }
}
