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

        // public Program(IConfiguration configuration)
        // {
        //     //AppSettings settings = new AppSettings();

        //     _appConfiguration = configuration;
        //     //configuration.Bind(settings);            
        // }

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
        //var builder = Host.CreateDefaultBuilder().Build();

        //IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true);
        //IConfigurationRoot root = builder.Build();

        
        //IEmailConfiguration _emailConfiguration =  _appConfiguration.GetSection("EmailConfiguration").Get<EmailConfiguration>();

       
        //Host.CreateDefaultBuilder(args);

            // var client = new POPClient();
            // client.Connect("pop.gmail.com", 995, true);
            // client.Authenticate("admin@bendytree.com", "YourPasswordHere");

            // var count = client.GetMessageCount();
            // Message message = client.GetMessage(count);
            // Console.WriteLine(message.Headers.Subject);

            //var config = new ConfigurationBuilder().SetBasePath(".").AddJsonFile("appsettings.json").Build();

           //  _emailConfiguration = configuration.GetSection("EmailConfiguration");//.GetSection<EmailConfiguration>();

             //ar AppName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["APP_Name"];

            // message.Headers.From.Address; 
            // from address
            // messagePart.BodyEncoding.GetString(messagePart.Body);
            // message body
        }


        public static List<EmailMessage> ReceiveEmail(int maxCount = 10)
        {
            IEmailConfiguration _emailConfiguration =  _appConfiguration.GetSection("EmailConfiguration").Get<IEmailConfiguration>();
            using (var emailClient = new Pop3Client())
            {
                emailClient.Connect(_emailConfiguration.PopServer, _emailConfiguration.PopPort, true);

                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                emailClient.Authenticate(_emailConfiguration.PopUsername, _emailConfiguration.PopPassword);

                List<EmailMessage> emails = new List<EmailMessage>();
                for(int i=0; i < emailClient.Count && i < maxCount; i++)
                {
                    var message = emailClient.GetMessage(i);
                    var emailMessage = new EmailMessage
                    {
                        Content = !string.IsNullOrEmpty(message.HtmlBody) ? message.HtmlBody : message.TextBody,
                        Subject = message.Subject
                    };
                    //emailMessage.ToAddresses.AddRange(message.To.Select(x => (MailboxAddress)x).Select(x => new EmailAddress { Address = x.Address, Name = x.Name }));
                    //emailMessage.FromAddresses.AddRange(message.From.Select(x => (MailboxAddress)x).Select(x => new EmailAddress { Address = x.Address, Name = x.Name }));
                    emails.Add(emailMessage);
                }

                return emails;
            }
        }
    }
}
