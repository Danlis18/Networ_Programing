using System.Text;
using System.Text.Json;

namespace _005_Chat
{
    public static class Serializer
    {
        public static byte[] ObjectByteToArray(object objData)
        {
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(objData));
        }

        public static T ByteArrayToObject<T>(byte[] byteArray)
        {

            return JsonSerializer.Deserialize<T>(byteArray);

        }
    }
}
