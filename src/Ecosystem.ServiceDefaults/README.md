# 🛠️ Project: ServiceDefaults

## 1. Mục Đích

`ServiceDefaults` là một project thư viện dùng chung, cung cấp các cấu hình mặc định, xuyên suốt cho tất cả các dịch vụ trong hệ thống .NET Aspire. Mục đích của nó là tuân thủ nguyên tắc **DRY (Don't Repeat Yourself)** cho các cấu hình chung.

## 2. Thành Phần Chính

* **Extension Methods:** Chứa các phương thức mở rộng, điển hình là `AddServiceDefaults(this IHostApplicationBuilder builder)`.
* **Cấu hình chung:** Bên trong phương thức mở rộng, nó sẽ cấu hình:
    * **Observability (OpenTelemetry):** Thiết lập tracing và metrics một cách nhất quán.
    * **Health Checks:** Thêm các health check mặc định.
    * **Service Discovery:** Cấu hình để các dịch vụ có thể tìm thấy nhau.
    * **Resilience:** Cấu hình các chính sách về độ tin cậy cho `HttpClient`.

## 3. Nguyên Tắc Cốt Lõi

* **Nhất quán:** Đảm bảo tất cả các dịch vụ trong hệ thống đều có một bộ cấu hình nền tảng giống nhau.
* **Tái sử dụng:** Tránh việc phải lặp lại các đoạn code cấu hình giống hệt nhau trong `Program.cs` của mỗi project dịch vụ.

## 4. Các Phụ Thuộc

* **Tham chiếu đến:** Không có (chỉ phụ thuộc vào các gói NuGet của Aspire và OpenTelemetry).
* **Được tham chiếu bởi:** Tất cả các project dịch vụ cần các cấu hình này, ví dụ: `WebApi`.