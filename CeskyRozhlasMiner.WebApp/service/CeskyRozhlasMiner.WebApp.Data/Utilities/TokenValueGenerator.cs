using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeskyRozhlasMiner.WebApp.Data.Utilities
{
    public class TokenValueGenerator
    {
        public static string GetNewValue()
        {
            // Answered by @Guffa at https://stackoverflow.com/a/14644367
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            return Convert.ToBase64String(time.Concat(key).ToArray());
        }
    }
}
