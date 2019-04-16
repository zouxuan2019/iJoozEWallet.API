using Newtonsoft.Json.Converters;

namespace iJoozEWallet.API.Utils
{
    public class CustomDateTimeConverter: IsoDateTimeConverter
    {
        public CustomDateTimeConverter()
        {
            DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        }
    }
}