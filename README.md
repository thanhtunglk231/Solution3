# 📌 HỆ THỐNG QUẢN LÝ NHÂN VIÊN = + XÁC THỰC ĐA YẾU TỐ (MFA)

## 📝 1. Giới thiệu

Hệ thống quản lý nhân sự nội bộ tích hợp:

- ✅ Đăng nhập, phân quyền người dùng (Admin / User)
- ✅ Quản lý nhân viên, phòng ban, chức vụ, lịch sử công việc
=
- ✅ Xác thực đa yếu tố (MFA):
  - OTP qua Email / SMS  
  - Google Authenticator (TOTP)  
  - Backup Codes – dùng khi mất thiết bị  
  - Trusted Devices – “Nhớ thiết bị” không hỏi lại MFA  
- ✅ Giao diện ASP.NET MVC / Razor + Oracle Database

---

## ⚙️ 2. Công nghệ sử dụng

| Công nghệ         | Mô tả |
|-------------------|-------------------------------------------|
| **Ngôn ngữ**      | C#, ASP.NET Core MVC (.NET 8)            |
| **CSDL**          | Oracle Database                          |
| **ORM**           | Entity Framework Core / Dapper           |
| **MFA**           | QRCodeGenerator, Otp.NET, Email/SMS OTP |
| **Realtime Chat** | SignalR                                  |
| **Frontend**      | Razor View, Bootstrap 4/5, jQuery        |
| **Công cụ**       | Visual Studio 2022, Oracle SQL Developer, Postman, Git |

---

## 📂 3. Cấu trúc thư mục

```
Solution/
├── Controllers/
│   ├── AccountController.cs         # Đăng nhập, đăng ký, MFA
│   ├── MfaController.cs             # OTP, Google Authenticator
│   ├── NhanVienController.cs        # Quản lý nhân viên
│   ├── ChatController.cs            # Chat nội bộ
│
├── Models/
│   ├── Users.cs, NhanVien.cs, PhongBan.cs
│   ├── MfaBackupCode.cs, TrustedDevice.cs
│   ├── ChatGroup.cs, ChatMessage.cs
│
├── Views/
│   ├── Account/ (Login, Register, VerifyOTP)
│   ├── Mfa/ (EnableMFA, QRCode, BackupCodes)
│   ├── NhanVien/
│   └── Chat/
│
├── Data/
│   ├── ApplicationDbContext.cs
│   ├── SeedData.cs
│
├── Services/
│   ├── EmailService.cs            # Gửi OTP qua Email
│   ├── SmsService.cs              # Gửi OTP qua SMS
│   ├── TotpService.cs             # Google Authenticator
│   ├── TrustedDeviceService.cs
│
├── wwwroot/                        # CSS, JS, hình ảnh
├── appsettings.json                # Kết nối Oracle, cấu hình JWT, SMTP
└── README.md
```

---

## 🛠️ 4. Hướng dẫn cài đặt & chạy

### ✅ 4.1 Yêu cầu hệ thống

| Phần mềm        | Phiên bản |
|------------------|-----------|
| .NET SDK         | 8.0+     |
| Oracle Database  | 11g/12c/19c |
| Visual Studio    | 2022     |

### ✅ 4.2 Tạo và khôi phục database Oracle

#### **Tạo user và phân quyền**
```sql
CREATE USER C##USER02 IDENTIFIED BY toto;
GRANT CONNECT, RESOURCE TO C##USER02;
ALTER USER C##USER02 QUOTA UNLIMITED ON USERS;
```

#### **Import dữ liệu từ file .dmp**
```bash
impdp C##USER02/toto@localhost:1521/xe   DIRECTORY=BACKUP_DIR   DUMPFILE=your_backup.dmp   LOGFILE=import.log   REMAP_SCHEMA=OLD_SCHEMA:C##USER02
```

---

### ✅ 4.3 Cấu hình `appsettings.json`

```json
"ConnectionStrings": {
  "OracleDb": "User Id=C##user02;Password=toto;Data Source=localhost:1521/xe;"
}
```

---

### ✅ 4.4 Chạy hệ thống

```bash
cd Solution
dotnet restore
dotnet run
```

---

## 👤 5. Tài khoản Demo

| Role  | Username  | Password  |
|-------|-----------|-----------|
| User  | 123123    | 123       |
| Admin | string231 | string    |

---

## 📷 6. Giao diện (demo)

| Màn hình               | Ảnh |
|------------------------|-----|
| Đăng nhập              | ![image](https://github.com/user-attachments/assets/77fb9133-b948-484c-ab83-d5607b79a968) |
| Quét QR Google Auth    | ![image](https://github.com/user-attachments/assets/230c9a04-03e2-4d5f-bc8d-fafde3d0f748) |
| OTP gửi về             | ![image](https://github.com/user-attachments/assets/03ae59f7-2422-4719-a40c-70851e72c7a6) |

---

## ✅ 7. Các bảng CSDL chính

| Bảng                  | Chức năng |
|-----------------------|-----------|
| USERS                 | Tài khoản đăng nhập |
| MFA_TOTP_AUDIT        | Lịch sử xác thực MFA |
| MFA_BACKUP_CODES      | Mã dự phòng |
| TRUSTED_DEVICES       | Thiết bị đáng tin |
| NHANVIEN, PHONGBAN    | Quản lý nhân viên |
| CHAT_GROUPS, CHAT_MESSAGES | Chat nhóm & tin nhắn |
| PERMISSIONS           | Phân quyền người dùng |

---

## 🎯 8. Tính năng nổi bật

- ✅ Google Authenticator (QR + TOTP)
- ✅ OTP Email / SMS
- ✅ “Ghi nhớ thiết bị” (Trusted Device Cookie)
- ✅ Recovery Codes khi mất điện thoại
- ✅ Chat nhóm + Chat trực tiếp
- ✅ Quản lý nhân viên & phân quyền

## ✅ 9. Clone dự án về máy
```bash
git clone https://github.com/thanhtunglk231/Solution3.git
cd Solution3
dotnet restore
dotnet run
```
