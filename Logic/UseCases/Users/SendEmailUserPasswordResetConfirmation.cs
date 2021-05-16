using MediatR;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using WebLicense.Core.Models.Identity;
using WebLicense.Logic.Auxiliary;

namespace WebLicense.Logic.UseCases.Users
{
    public sealed class SendEmailUserPasswordResetConfirmation : IRequest<CaseResult>
    {
        internal readonly User User;
        internal readonly string CallbackUrl;

        public SendEmailUserPasswordResetConfirmation(User user, string callbackUrl)
        {
            User = user;
            CallbackUrl = callbackUrl;
        }
    }

    internal sealed class SendEmailUserPasswordResetHandler : IRequestHandler<SendEmailUserPasswordResetConfirmation, CaseResult>
    {
        private readonly IMailManager mailManager;
        private readonly IWebHostEnvironment webHostEnvironment;

        public SendEmailUserPasswordResetHandler(IMailManager mailManager, IWebHostEnvironment webHostEnvironment)
        {
            this.mailManager = mailManager ?? throw new ArgumentNullException(nameof(mailManager));
            this.webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        }

        public async Task<CaseResult> Handle(SendEmailUserPasswordResetConfirmation request, CancellationToken cancellationToken)
        {
            try
            {
                var to = mailManager.GetMailAddress(request.User.Email, request.User.UserName);
                var template = Path.Combine(webHostEnvironment.WebRootPath, "server", "email-templates", "password-reset-confirmation.html");
                var message = mailManager.GetMailMessageFromTemplate(null, to, template, ("{button-link}", request.CallbackUrl));

                LinkResources(mailManager, message);

                await mailManager.SendEmailAsync(message, cancellationToken);

                return new();
            }
            catch (Exception e)
            {
                return new(e);
            }
        }

        #region Methods

        private void LinkResources(IMailManager manager, MailMessage mail)
        {
            var view = mail.AlternateViews.FirstOrDefault(q => MediaTypeNames.Text.Html.Equals(q.ContentType.MediaType, StringComparison.OrdinalIgnoreCase));

            var logoFile = Path.Combine(webHostEnvironment.WebRootPath, "server", "email-templates", "logo-main.png");

            if (File.Exists(logoFile)) manager.AddLinkedResource(view, "Logo_0", logoFile, "image/png");
        }

        #endregion
    }
}