namespace MiCo.Helpers
{
    public class ResultHelper
    {
        public ResultHelper(bool success, string message)
        {
            RHsuccess = success;
            RHmessage = message;
        }

        public bool RHsuccess {  get; set; }

        public string RHmessage { get; set; }
    }
}
