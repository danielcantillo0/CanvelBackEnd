using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace CanvelBackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SendEmailController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public SendEmailController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public class EmailModel
        {
            [Required]
            public string Name { get; set; }
            [Required]
            public string Email { get; set; }
            [Required]
            public string Message { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EmailModel email)
        {
            string secretEmail = _configuration["email"];
            string secretPassword = _configuration["password"];
            string secretSmtpServer = _configuration["smtp-server"];    

            try
            {
                // Create a new MimeMessage
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Nuevo Contacto por la Pagina Web", $"{secretEmail}"));
                message.To.Add(new MailboxAddress("", "info@canvel.co"));
                message.Subject = "Nuevo contacto a traves de la Pagina Web";

                // Set the message body (plain text)
                message.Body = new TextPart("plain")
                {
                    Text = $"Nombre: {email.Name}\n\nCorreo: {email.Email}\n\nMensaje: {email.Message}"
                };


                using (var client = new SmtpClient())
                {
                    // For demo purposes, accept all SSL certificates (not recommended for production)
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    // Connect to the SMTP server
                    await client.ConnectAsync($"{secretSmtpServer}", 465, SecureSocketOptions.SslOnConnect);

                    // Authenticate using your SMTP credentials
                    await client.AuthenticateAsync($"{secretEmail}", $"{secretPassword}");

                    // Send the email
                    await client.SendAsync(message);

                    // Disconnect cleanly
                    await client.DisconnectAsync(true);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {

            // Return a simple JSON object for testing purposes.
            await Task.Delay(500);
            return Ok(new { message = "Hello from the API!", timestamp = DateTime.UtcNow });
        }
    }
}

