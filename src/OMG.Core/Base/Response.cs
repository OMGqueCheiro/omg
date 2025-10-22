using System.Text.Json.Serialization;

namespace OMG.Core.Base;

public class Response<TData> : Response
{
    public Response(TData? data = default, int code = 200, string message = "") : base(code, message)
    {
        Data = data;
        Code = code;
    }

    public TData? Data { get; set; }

}

public class Response(int code = 200, string message = "")
{
    public string Message { get; set; } = message;
    public int Code { get; set; } = code;

    [JsonIgnore]
    public bool IsSuccess
        => Code is >= 200 and <= 299;
}
