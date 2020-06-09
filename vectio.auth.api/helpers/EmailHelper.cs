using System;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.States;
using Newtonsoft.Json;
using vectio.tasks.api.DbContexts;
using vectio.tasks.api.Entities;
using vectio.tasks.api.Tasks;

namespace vectio.auth.api.Controllers
{
    public  class EmailHelper
    {
        private TasksDbContext _context;
        public EmailHelper(TasksDbContext context)
        {
            _context = context;
        }
        public async Task SendEmailAsync(string recipient,string subject,string body)
        {
            var taskRequest = new TaskRequest
            {
                InstanceId = Guid.Parse("10D90E60-F96D-427B-9537-57E906D3E69F"),
                DispatcherId = Guid.Parse("AC8E34F4-FF1C-495F-85C3-553F2BF5FB48"),
                TaskType = "send-email-group",
                TaskDescription = "tasks-email",
                TaskData = JsonConvert.SerializeObject(new {
                    recipients = new[] {recipient},
                    subject=subject,
                    body=body,
                    isBodyHtml=true
                    },Formatting.None)
            };
            _context.TaskRequests.Add(taskRequest);
            await _context.SaveChangesAsync();
            var queue = taskRequest.TaskType.Equals("send-email", StringComparison.InvariantCultureIgnoreCase) ? $"asap_emails_{taskRequest.DispatcherId}" : $"emails_{taskRequest.DispatcherId}";

            var client = new BackgroundJobClient();
            var state = new EnqueuedState(queue);
            client.Create<ISendEmailTask>(t => t.Send(taskRequest.Id), state);
            taskRequest.IsProcessed = true;
            await _context.SaveChangesAsync();
        }
    }
}