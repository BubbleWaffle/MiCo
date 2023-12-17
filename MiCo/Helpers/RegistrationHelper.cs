namespace MiCo.Helpers
{
    public class RegistrationHelper
    {
        public RegistrationHelper(bool success, string message)
        {
            RHsuccess = success;
            RHmessage = message;
        }

        public bool RHsuccess {  get; set; }

        public string RHmessage { get; set; }
    }
}
