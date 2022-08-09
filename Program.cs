using System;
using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MailKit.Net.Pop3;
using MailKit;
using MimeKit;

namespace mail
{
    class Program
    {
        private static IConfiguration _appConfiguration;      

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            _appConfiguration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();

            IEmailConfiguration emailConfig = new EmailConfiguration();
            _appConfiguration.GetSection(nameof(EmailConfiguration)).Bind(emailConfig);
            Console.WriteLine($"Hello, { emailConfig.SmtpServer } world!");

            IEmailService _emailService = new EmailService(emailConfig);
            _emailService.ReceiveEmail();
        
        }
        
    }
}
