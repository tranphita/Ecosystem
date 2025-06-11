# 🚀 Project: AppHost

## 1. Mục Đích

Đây là project điều phối (orchestration) chính của .NET Aspire. Nó không chứa logic nghiệp vụ. Vai trò của nó là định nghĩa, cấu hình và khởi chạy tất cả các tài nguyên (databases, caches, message queues) và các dịch vụ (.NET projects, containers) tạo nên ứng dụng phân tán của bạn.

Việc chạy project này sẽ khởi động toàn bộ hệ thống cục bộ (local) và mở ra **Aspire Dashboard** để theo dõi.

## 2. Thành Phần Chính

* **Program.cs:** Chứa logic xây dựng ứng dụng phân tán bằng `IDistributedApplicationBuilder`.
* **Khai báo tài nguyên:** Các lệnh gọi như `builder.AddPostgreSQL(...)`, `builder.AddRedis(...)`.
* **Khai báo các dịch vụ:** Các lệnh gọi `builder.AddProject<Projects.WebApi>(...)` để thêm các project .NET vào ứng dụng.
* **Kết nối các thành phần:** Sử dụng `.WithReference(...)` để cung cấp connection string và cấu hình kết nối giữa các dịch vụ.

## 3. Nguyên Tắc Cốt Lõi

* **Chỉ dành cho Development & Deployment:** Project này dùng để đơn giản hóa việc phát triển cục bộ và cung cấp một "bản thiết kế" cho việc triển khai. Nó không được deploy như một service thông thường.
* **Quản lý cấu hình tập trung:** Giúp quản lý connection strings và các biến môi trường một cách tiện lợi.

## 4. Các Phụ Thuộc

* **Tham chiếu đến:** Các project thực thi mà nó cần khởi chạy, ví dụ: `WebApi`.