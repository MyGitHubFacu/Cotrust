﻿namespace Cotrust.Intefaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string body);
    }
}
