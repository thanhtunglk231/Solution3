using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Helpers
{
    public class PhoneNumberHelper
    {

        public static string NormalizePhone(string rawPhone)
        {
            if (string.IsNullOrWhiteSpace(rawPhone))
                return null;

            rawPhone = rawPhone.Trim();

            // Trường hợp Firebase: +84 -> 0
            if (rawPhone.StartsWith("+84"))
            {
                return "0" + rawPhone.Substring(3); // Bỏ +84, thêm số 0
            }

            // Trường hợp người dùng nhập 84xxxx (không dấu +)
            if (rawPhone.StartsWith("84") && rawPhone.Length > 9)
            {
                return "0" + rawPhone.Substring(2);
            }

            // Nếu đã là dạng 0xxxxxxxx thì trả nguyên
            if (rawPhone.StartsWith("0"))
            {
                return rawPhone;
            }

            // Nếu vẫn sai format thì trả null
            return null;
        }

    }
}
