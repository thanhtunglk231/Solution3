
    using System;
    using System.Text.Json.Serialization;

    namespace CoreLib.Models
    {
        public class Employee
        {
            public string HO_TEN { get; set; }
            public string MANV { get; set; }
            public DateTime? NGSINH { get; set; }         // Sửa thành property và nullable
            public string DCHI { get; set; }
            public string PHAI { get; set; }
            public float? LUONG { get; set; }             // Sửa nullable nếu có thể null
            public string MA_NQL { get; set; }
            public int? MAPHG { get; set; }               // Nullable nếu có thể null
            public DateTime? NGAY_VAO { get; set; }       // Sửa thành property và nullable
           
            public float? HOAHONG { get; set; }           // Nullable nếu có thể null
            public string? MAJOB { get; set; }
        }
    }
