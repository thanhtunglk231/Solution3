# 📌Triển khai xác thực đa yếu tố (MFA) trong ứng dụng web – OTP qua email/SMS, Google Authenticator. 

👤 Phạm Thanh Tùng
MSV: 22810310257
Nội dung làm:
Triển khai xác thực đa yếu tố bằng Google Authenticator (TOTP) theo chuẩn RFC 6238.
Xây dựng logic sinh, xác minh và quản lý mã TOTP.
Thiết kế và triển khai Stored Procedures (SP) phục vụ xác thực và ghi log bảo mật.
Tích hợp Redis Cache để lưu trạng thái xác thực và tối ưu hiệu năng hệ thống.
Phối hợp hoàn thiện kiến trúc backend xác thực đa yếu tố (MFA).

👤 Cấn Anh Quân
MSV: 22810310260
Nội dung làm:
Triển khai xác thực OTP qua Email.
Xây dựng chức năng gửi OTP bằng Email (SMTP).
Xử lý kiểm tra OTP, thời gian hiệu lực và chống gửi lặp.
Sử dụng Redis Cache để lưu OTP tạm thời, giảm truy vấn cơ sở dữ liệu.
Kết hợp Stored Procedures trong các nghiệp vụ xác thực Email OTP.

👤 Nguyễn Văn Quốc Khánh
MSV: 22810310261
Nội dung làm:
Triển khai xác thực OTP qua SMS.
Tích hợp dịch vụ gửi SMS OTP.
Xử lý xác minh OTP SMS và thời gian hết hạn.
Áp dụng Redis Cache để quản lý OTP SMS và kiểm soát số lần gửi.
Tham gia viết Stored Procedures liên quan đến OTP SMS và ghi nhận log hệ thống.

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
git clone https://github.com/thanhtunglk231/Solution3.git

cd Solution3

dotnet restore

dotnet run

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
| Đăng nhập              | <img width="546" height="424" alt="image" src="https://github.com/user-attachments/assets/583e90f5-69c9-4af4-884f-db59d6ead46a" />|
| Gửi email               |<img width="436" height="395" alt="image" src="https://github.com/user-attachments/assets/e12492b3-af58-4f0b-ab41-b59dd93a9e9f" />|
| Email nhận về          | <img width="999" height="108" alt="image" src="https://github.com/user-attachments/assets/a06760ea-4d58-421e-ab11-f3b714435af2" />|
| Đăng nhập bằng authen  | <img width="572" height="533" alt="image" src="https://github.com/user-attachments/assets/52821122-0e06-42f6-90fb-849b0cf231e5" />|
| Ảnh Gửi về             |<img width="278" height="412" alt="image" src="https://github.com/user-attachments/assets/0b281239-bc9b-4926-a4dc-1c121716a597" /> |
| SMS                    |<img width="427" height="429" alt="image" src="https://github.com/user-attachments/assets/ca848857-629d-435c-b2bc-24cdbbc2f4ac" /> |
| Đăng ký sms FireBase   |<img width="1909" height="960" alt="image" src="https://github.com/user-attachments/assets/72218e36-13e4-4b48-84b8-ca11e7a147f2" />|


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


