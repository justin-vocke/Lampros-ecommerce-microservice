﻿using Lampros.Services.EmailAPI.Data;
using Lampros.Services.EmailAPI.Models;
using Lampros.Services.EmailAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Lampros.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<EmailDbContext> _dbOptions;

        public EmailService(DbContextOptions<EmailDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

        public async Task EmailCartAndLog(CartDto cartDto)
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine("<br/>Cart email requested");
            message.AppendLine("<br/>Cart Total: " + cartDto.CartHeader.CartTotal);
            message.AppendLine("<ul>");
            foreach (var item in cartDto.CartDetails)
            {
                message.AppendLine("<li>");
                message.AppendLine(item.Product.Name + " x " + item.Count);
                message.AppendLine("</li>");
            }
            message.AppendLine("</ul>");

            await LogAndEmail(message.ToString(), cartDto.CartHeader.Email);

        }

        private async Task<bool> LogAndEmail(string message, string email)
        {
            try
            {
                EmailLogger emailLogger = new()
                {
                    Email = email,
                    EmailSent = DateTime.Now,
                    Message = message,
                };
                await using var _db = new EmailDbContext(_dbOptions);
                _db.EmailLoggers.Add(emailLogger);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
    }
}
