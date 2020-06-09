using Microsoft.Extensions.Configuration;
using System;
using vectio.eventmanagement.api.db;
using vectio.tasks.client;

namespace vectio.eventmanagement.api.helpers
{
    public class EmailHelper
    {
        private EventManagementDBContext _context;

        private readonly IConfiguration _config;

        public EmailHelper(EventManagementDBContext context, IConfiguration config)
        {
            _context = context;
            client = new TasksHttpClient(config.GetSection("TasksHttpClient").GetSection("url").Value, config.GetSection("TasksHttpClient").GetSection("instance").Value, config.GetSection("TasksHttpClient").GetSection("dispatcher").Value);

        }
        private TasksHttpClient client;

        public bool SendEmail(string recipient, string subject, string body)
        {
            try
            {

                var emailDetails = new EmailDetails
                {
                    Recipients = new[] { recipient },
                    Subject = subject,
                    IsBodyHtml = true,
                    Body = FillEmailTemplate(body, subject),

                };
                var request = new EmailRequest
                {
                    TaskDescription = "Alumni",
                    TaskType = "send-email",
                    TaskData = emailDetails
                };

                var result = client.SendEmail(request).Result;
                if (!result)
                    throw new Exception("Sending request failed");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        private string FillEmailTemplate(string body, string subject)
        {
            return string.Format(@"<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>
<html xmlns='http://www.w3.org/1999/xhtml'>
<head>
  <meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />
  <title>{2}</title>
  <meta name='viewport' content='width=device-width, initial-scale=1.0'/>
</head>
<body>
<table align='center' border='0' cellpadding='0' cellspacing='0' width='600' style='border-collapse: collapse;'>
 <tr>
  <td>
{0}  
  </td>
 </tr>
 <tr>
 <td>
 <hr/>
 <p>Ten e-mail został wygenerowany automatycznie z portalu {1}. Prosimy na niego nie odpowiadać.</p>
 </td>
 </tr>
 <tr>
 <td>
 <table border='0' cellpadding='0' cellspacing='0' width='100%' style='border-collapse: collapse;'><tr>
 <td width='60'><img src='https://tasks.vectio.pl/logo/vectio/EMAILGUID.png' /></td>
 <td> Powered by Vectio Business Platform</td>
 </tr></table>
 </td>
 </tr>
</table>
</body>
</html>", body, "[VBP EMBA]", subject);


        }
    }
}
