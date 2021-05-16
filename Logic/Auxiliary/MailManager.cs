using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using WebLicense.Core.Auxiliary;

namespace WebLicense.Logic.Auxiliary
{
    public interface IMailManager
    {
        MailAddress GetMailAddress(string email, string displayName);

        MailMessage GetMailMessage(MailAddress from, MailAddress to, string subject, string body);

        MailMessage GetMailMessageFromTemplate(MailAddress from, MailAddress to, string template, params (string template, string replacement)[] replacements);

        void AddAttachment(MailMessage mail, string name, string content, string mediaType);

        void AddLinkedResource(AlternateView view, string cidName, string filePath, string mediaType);

        void SendEmail(MailMessage mailMessage);

        Task SendEmailAsync(MailMessage mailMessage, CancellationToken cancellationToken = default);
    }

    public sealed class MailManager : IMailManager
    {
        #region C-tor | Fields

        private readonly SmtpSettings _settings;

        public MailManager(IOptionsSnapshot<SmtpSettings> settings)
        {
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        }

        #endregion

        #region Mail methods

        public MailAddress GetMailAddress(string email, string displayName) => new(email, displayName ?? email, Encoding.UTF8);

        public MailMessage GetMailMessage(MailAddress from, MailAddress to, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(to?.Address)) throw new ArgumentNullException(nameof(to));
            if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentNullException(nameof(subject));
            if (string.IsNullOrWhiteSpace(body)) throw new ArgumentNullException(nameof(body));

            from ??= GetFromAddress();

            var mail = new MailMessage(from, to) {Subject = subject, SubjectEncoding = Encoding.UTF8, IsBodyHtml = true};
            mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, Encoding.UTF8, MediaTypeNames.Text.Html));

            return mail;
        }

        public MailMessage GetMailMessageFromTemplate(MailAddress from, MailAddress to, string template, params (string template, string replacement)[] replacements)
        {
            var (subject, body) = ParseTemplate(template, replacements);

            return GetMailMessage(from, to, subject, body);
        }

        public void AddAttachment(MailMessage mail, string name, string content, string mediaType)
        {
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);

            writer.Write(content?.Trim() ?? string.Empty);
            writer.Flush();

            stream.Seek(0, SeekOrigin.Begin);

            mail.Attachments.Add(new (stream, name, mediaType));
        }

        public void AddLinkedResource(AlternateView view, string cidName, string filePath, string mediaType)
        {
            var resource = new LinkedResource(filePath, mediaType)
            {
                ContentId = cidName,
                TransferEncoding = TransferEncoding.Base64,
                ContentLink = new($"cid:{cidName}"),
                ContentType = {MediaType = mediaType, Name = cidName}
            };
            view.LinkedResources.Add(resource);
        }

        public void SendEmail(MailMessage mailMessage)
        {
            GetClient().Send(mailMessage);
        }

        public async Task SendEmailAsync(MailMessage mailMessage, CancellationToken cancellationToken = default)
        {
            await GetClient().SendMailAsync(mailMessage, cancellationToken);
        }

        #endregion

        #region Methods

        private SmtpClient GetClient() => new(_settings.Server.Name, _settings.Server.Port)
        {
            DeliveryFormat = SmtpDeliveryFormat.International,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            EnableSsl = _settings.Server.UseSSL,
            Timeout = 30000,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_settings.Server.Login, _settings.Server.Password)
        };

        private MailAddress GetFromAddress() => new(_settings.From.Email, _settings.From.Name, Encoding.UTF8);

        private (string Subject, string Body) ParseTemplate(string template, (string template, string replacement)[] replacements)
        {
            var templateFile = Regex.Replace(template, @"\.html$", $".{Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower()}.html");
            if (!File.Exists(templateFile)) templateFile = template;
            if (!File.Exists(templateFile)) return (null, null);

            var body = File.ReadAllText(templateFile);

            if (replacements != null)
            {
                foreach (var (t, r) in replacements)
                {
                    body = body.Replace(t, r, StringComparison.OrdinalIgnoreCase);
                }
            }

            var subject = ParseSubject(body, null);

            return (subject, body);
        }

        private string ParseSubject(string html, string fallback)
        {
            var title = Regex.Match(html, "<title>(.*?)</title>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            
            return title.Success && title.Groups.Count > 1 ? title.Groups[1].Value : fallback;
        }

        #endregion
    }
}