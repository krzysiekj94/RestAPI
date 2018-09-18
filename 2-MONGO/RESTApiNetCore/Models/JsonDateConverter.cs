using Newtonsoft.Json.Converters;

namespace RESTApiNetCore.Models
{
    class JsonDateConverter : IsoDateTimeConverter
    {
        public JsonDateConverter()
        {
            DateTimeFormat = "yyyy-MM-dd";
        }
    }
}