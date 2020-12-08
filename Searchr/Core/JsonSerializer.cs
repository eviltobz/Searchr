namespace Searchr.Core
{
    using System.Text;
    using System.Text.Json;

    public class JsonSerializer //: ISerializer
    {
        private static readonly JsonSerializerOptions SerialisationOptions = new JsonSerializerOptions()
        {
            IncludeFields = true,
            WriteIndented = true
        };
        public byte[] Serialize<T>(T item)
        {
            var serialised = System.Text.Json.JsonSerializer.Serialize(item, SerialisationOptions);
            return Encoding.UTF8.GetBytes(serialised);
        }

        public T? Deserialize<T>(byte[] data)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(data, SerialisationOptions);
        }
    }
}
