namespace _005_Chat
{
    [Serializable]
    public class MyMessage
    {
        public string ComputerName { get; set; } = SystemInformation.ComputerName;
        public string Message { get; set; } 

        public bool IsOnline { get; set; }

        public override string ToString()
        {
            return ComputerName;
        }
    }
}
