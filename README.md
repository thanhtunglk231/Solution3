📌 HỆ THỐNG QUẢN LÝ NHÂN VIÊN + CHAT + XÁC THỰC ĐA YẾU TỐ (MFA)
📝 1. Giới thiệu

Dự án là hệ thống quản lý nhân sự nội bộ, tích hợp các chức năng:

✅ Đăng nhập, phân quyền người dùng (Admin / User)

✅ Quản lý nhân viên, phòng ban, chức vụ, lịch sử công việc

✅ Chat nội bộ theo nhóm (Chat Group / Private Chat)

✅ Xác thực đa yếu tố (MFA):

OTP qua Email / SMS

Google Authenticator (TOTP)

Backup Codes – dùng khi mất thiết bị

Trusted Devices – “Nhớ thiết bị” không hỏi lại MFA trong X ngày

✅ Giao diện Web ASP.NET MVC / Razor, kết nối Oracle DB

⚙️ 2. Công nghệ sử dụng
Công nghệ	Mô tả
Ngôn ngữ	C#, ASP.NET Core MVC (.NET 8)
CSDL	Oracle Database
ORM	Entity Framework Core (hoặc Dapper + Oracle.ManagedDataAccess)
Thư viện	QRCodeGenerator (TOTP), Otp.NET (Google Authenticator), SMTP / Twilio (Email/SMS OTP)
Frontend	Razor View, Bootstrap 4/5, jQuery, SignalR (Realtime chat)
Công cụ	Visual Studio 2022, Oracle SQL Developer, Postman, Git
📂 3. Cấu trúc thư mục dự án
Solution/
├── Controllers/
│   ├── AccountController.cs        # Đăng nhập, đăng ký, MFA
│   ├── MfaController.cs            # OTP, Google Authenticator, Backup Codes
│   ├── NhanVienController.cs       # Quản lý nhân viên
│   ├── ChatController.cs           # Giao diện chat nội bộ
│
├── Models/
│   ├── Users.cs, NhanVien.cs, PhongBan.cs
│   ├── MfaBackupCode.cs, TrustedDevice.cs
│   ├── ChatGroup.cs, ChatMessage.cs
│
├── Views/
│   ├── Account/ (Login, Register, VerifyOTP)
│   ├── Mfa/     (EnableMFA, QRCode, BackupCodes)
│   ├── NhanVien/
│   └── Chat/
│
├── Data/
│   ├── ApplicationDbContext.cs      # EF DbContext
│   ├── SeedData.cs                  # Tạo dữ liệu mẫu
│
├── Services/
│   ├── EmailService.cs              # Gửi Email OTP
│   ├── SmsService.cs                # Gửi OTP qua SMS
│   ├── TotpService.cs               # Google Authenticator
│   ├── TrustedDeviceService.cs
│
├── wwwroot/                         # CSS, JS, hình ảnh
├── appsettings.json                 # ConnectionString Oracle, SMTP
└── README.md

🛠️ 4. Hướng dẫn cài đặt & chạy
✅ 4.1 Yêu cầu hệ thống
Phần mềm	Phiên bản
.NET SDK	8.0+
Oracle Database	11g / 12c / 19c
Visual Studio	2022

✅ 4.2 Import database

✔ Mở Oracle SQL Developer → New Connection → Run file Database/script.sql
-- Tạo user để chứa dữ liệu
CREATE USER C##USER02 IDENTIFIED BY toto;
GRANT CONNECT, RESOURCE TO C##USER02;
ALTER USER C##USER02 QUOTA UNLIMITED ON USERS;

-- Import dữ liệu từ .dmp
impdp C##USER02/toto@localhost:1521/xe \
  DIRECTORY=BACKUP_DIR \
  DUMPFILE=your_backup.dmp \
  LOGFILE=import.log \
  REMAP_SCHEMA=OLD_SCHEMA:C##USER02


✅ 4.3 Cấu hình Oracle & Email / SMS OTP

📌 Trong file appsettings.json:

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Serilog": {
    "Using": [ "Serilog.Sinks.File" ],
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "Logs\\log-font.txt",
          "rollingInterval": "Day",
          "rollOnFileSizeLimit": true,
          "fileSizeLimitBytes": 1024000,
          "retainedFileCountLimit": 100,
          "shared": true,
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] ({ThreadId}) {Message}{NewLine}{Exception}"
        }
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ]
  },
  "PathStrings": {
    "Url": "https://localhost:7053/api/"
  },
  "ConnectionStrings": {
    "Redis": "localhost:6379",
    "OracleDb": "User Id=C##user02;Password=toto;Data Source=localhost:1521/xe;"
  },
  "JwtSettings": {
    "Issuer": "your-api",
    "Audience": "your-client",
    "SecretKey": "this_is_a_super_secret_key_12321321321321321!"
  },
  "AllowedHosts": "*"
}


✅ 4.4 Chạy hệ thống
cd Solution
dotnet restore
dotnet run

👤 5. Tài khoản Demo
Role	Username	Password
user	123123	123
admin	string231	string

🔐 Sau khi đăng nhập → hệ thống yêu cầu thiết lập MFA (Google Authenticator / OTP Email).

📷 6. Ảnh giao diện (gợi ý)
Mô tả	Ảnh
Màn hình đăng nhập	<img width="960" height="616" alt="image" src="https://github.com/user-attachments/assets/77fb9133-b948-484c-ab83-d5607b79a968" />

Kích hoạt Google Authenticator
(QR)	<img width="971" height="915" alt="image" src="https://github.com/user-attachments/assets/230c9a04-03e2-4d5f-bc8d-fafde3d0f748" />

Nhập mã OTP Email	images/otp_email.png

<img width="795" height="649" alt="image" src="https://github.com/user-attachments/assets/6725a642-3009-4d11-8296-11f05eb26bf6" />

<img width="1537" height="728" alt="image" src="https://github.com/user-attachments/assets/03ae59f7-2422-4719-a40c-70851e72c7a6" />




✅ 7. Các bảng CSDL chính
Bảng	Chức năng
USERS	Tài khoản đăng nhập
MFA_TOTP_AUDIT	Lưu lịch sử đăng nhập, xác thực MFA
MFA_BACKUP_CODES	Code dự phòng nếu mất Google Authenticator
TRUSTED_DEVICES	“Nhớ thiết bị đăng nhập” – không hỏi lại mã
NHANVIEN, PHONGBAN	Quản lý nhân viên – phòng ban
CHAT_GROUPS, CHAT_MESSAGES, GROUP_MEMBERS	Chat nội bộ realtime
PERMISSIONS, USER_PERMISSIONS	Phân quyền người dùng
🎯 8. Tính năng nổi bật

✔ Google Authenticator (quét QR + nhập mã)
✔ OTP qua Email/SMS
✔ Ghi nhớ thiết bị (Trusted Device Cookie)
✔ Recovery Codes (dùng khi mất điện thoại)
✔ Chat nhóm + Chat trực tiếp
✔ Quản lý nhân viên / phân quyền / lịch sử công việc
