namespace Forum.Domain.Models
{
    public class Result
    {
        public static Result Success(string message = "", object data = null)
        {
            return new Result { IsSuccess = true, Message = message, Data = data };
        }

        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }


        public static Result Failure(string message)
        {
            return new Result { IsSuccess = false, Message = message };
        }

    }

}
