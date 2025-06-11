# 🧪 Project: Application.IntegrationTests

## 1. Mục Đích

Kiểm thử sự phối hợp hoạt động của nhiều thành phần trong hệ thống với nhau, từ điểm vào (`WebApi`) đến các lớp hạ tầng (`Persistence`, `Infrastructure`) và cơ sở dữ liệu. Mục đích là để đảm bảo toàn bộ luồng xử lý hoạt động đúng như mong đợi trong một môi trường gần giống production.

## 2. Thành Phần Chính

* Các lớp test cho các API endpoints.
    * *Ví dụ:* `OrdersControllerTests.cs`.
* Sử dụng `WebApplicationFactory<T>` để khởi động ứng dụng trong bộ nhớ.
* Sử dụng các công cụ hỗ trợ như **Testcontainers** để khởi tạo các dependency (PostgreSQL, Redis) dưới dạng Docker container cho mỗi lần chạy test.

## 3. Nguyên Tắc Cốt Lõi

* **Kiểm thử từ đầu đến cuối:** Mô phỏng một yêu cầu HTTP thật và xác minh phản hồi cũng như trạng thái của cơ sở dữ liệu.
* **Giảm thiểu Mock:** Chỉ mock các dịch vụ của bên thứ ba thực sự nằm ngoài tầm kiểm soát (ví dụ: cổng thanh toán, dịch vụ gửi SMS). Sử dụng database và các dependency khác ở phiên bản thật (nhưng dành cho test).
* **Quản lý dữ liệu test:** Mỗi bài test phải có dữ liệu khởi tạo riêng và phải dọn dẹp sau khi chạy xong để đảm bảo tính độc lập.

## 4. Các Phụ Thuộc

* **Tham chiếu đến:** `WebApi`.