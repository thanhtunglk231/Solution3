using System.ComponentModel.DataAnnotations.Schema;

public class Employee
{
    [Column("hO_TEN")]
    public string HO_TEN { get; set; }

    [Column("manv")]
    public string MANV { get; set; }

    [Column("ngsinh")]
    public DateTime? NGSINH { get; set; }

    [Column("dchi")]
    public string DCHI { get; set; }

    [Column("phai")]
    public string PHAI { get; set; }

    [Column("luong")]
    public float? LUONG { get; set; }

    [Column("mA_NQL")]
    public string MA_NQL { get; set; }

    [Column("maphg")]
    public int? MAPHG { get; set; }

    [Column("ngaY_VAO")]
    public DateTime? NGAY_VAO { get; set; }

    [Column("hoahong")]
    public float? HOAHONG { get; set; }

    [Column("majob")]
    public string? MAJOB { get; set; }
}
