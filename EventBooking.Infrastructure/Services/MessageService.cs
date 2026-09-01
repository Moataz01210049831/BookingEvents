using System.Text.Json;
using EventBooking.Application.Common.Interfaces;

namespace EventBooking.Infrastructure.Services
{
    public class MessageService : IMessageService
    {
        private readonly Dictionary<string, string> _messages;

        public MessageService()
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "Resources", "messages.json");
            var json = File.ReadAllText(filePath);
            _messages = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                ?? new Dictionary<string, string>();
        }

        public string Get(string key)
        {
            return _messages.TryGetValue(key, out var message)
                ? message
                : $"[رسالة غير موجودة: {key}]";
        }
    }
}