using Microsoft.AspNetCore.SignalR.Client;
using System;

namespace wpf.Models
{
    public class RetryPolicy : IRetryPolicy
    {
        private readonly Random _random = new Random();
        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {
            return TimeSpan.FromSeconds(_random.Next(5, 10));
        }
    }
}
