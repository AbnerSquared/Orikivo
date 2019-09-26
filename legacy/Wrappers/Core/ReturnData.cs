namespace Orikivo.Systems.Wrappers.Core
{
    public class ReturnData
    {
        public ReturnData(bool isSuccess = false, string jsonData = "")
        {
            IsSuccess = isSuccess;
            JsonData = jsonData;
        }

        public bool IsSuccess { get; set; }
        public string JsonData { get; set; }
        public string Exception { get; set; }
    }
}