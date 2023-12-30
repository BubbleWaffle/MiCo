namespace MiCo.Helpers
{
    public class ResultHelper
    {
        public ResultHelper(bool success, string message)
        {
            RHsuccess = success;
            RHmessage = message;
        }

        public ResultHelper(bool success, string message, int no)
        {
            RHsuccess = success;
            RHmessage = message;
            RHno = no;
        }

        public bool RHsuccess {  get; set; }

        public string RHmessage { get; set; }

        public int RHno { get; set; }
    }
}
