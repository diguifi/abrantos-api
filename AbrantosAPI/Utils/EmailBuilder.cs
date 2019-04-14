namespace AbrantosAPI.Utils
{
    public class EmailBuilder
    {
        public const string SubjectConfirmEmail = "Confirm e-mail";
        public const string SubjectRecoverPassword = "Password Recovery";
        public string Scheme { get; set; }
        public string BaseUrl { get; set; }
        public string UserId { get; set; }
        public string EmailToken { get; set; }
        public string PasswordToken { get; set; }

        public EmailBuilder(string scheme, string baseUrl, string userId, string emailToken, string passwordToken)
        {
            Scheme = scheme;
            BaseUrl = baseUrl;
            UserId = userId;
            EmailToken = emailToken;
            PasswordToken = passwordToken;
        }

        public string GetEmailConfirmationMessage()
        {
            return  $"<title>{SubjectConfirmEmail}</title><br>" +
                    $"Post this data for abrantos.azurewebsites.net/api/authentication/confirmEmail<br>" +
                    $"userId={UserId}<br>&token={EmailToken}";
        }

        public string GetPasswrodResetMessage()
        {
            return  $"<title>{SubjectRecoverPassword}</title><br>" +
                    $"Post this data for abrantos.azurewebsites.net/api/authentication/recoverPassword<br>" +
                    $"userId={UserId}<br>&token={PasswordToken}<br>&newPassword='yournewpassword'&newPasswordConfirmation='confirmation'";
        }
    }
}