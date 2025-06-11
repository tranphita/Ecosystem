# 🖥️ Project: WebApi

## 1. Mục Đích

Project `WebApi` là điểm vào (entry point) của ứng dụng, chịu trách nhiệm tiếp nhận các yêu cầu HTTP, xác thực, ủy quyền và trả về các phản hồi. Nó đóng vai trò là tầng **Trình Bày (Presentation)** và cũng là **Composition Root** của hệ thống.

## 2. Thành Phần Chính

* **Controllers / Minimal API Endpoints:** Định nghĩa các API endpoints.
* **Program.cs:** Điểm khởi đầu của ứng dụng, nơi cấu hình:
    * **Dependency Injection (DI):** "Nối" các interfaces từ `Application` với các implementations từ `Infrastructure` và `Persistence`.
    * **Middleware Pipeline:** Cấu hình các middleware (xử lý ngoại lệ, xác thực, logging, v.v.).
    * **Services:** Đăng ký các dịch vụ cần thiết (Authentication, Authorization, Health Checks, OpenTelemetry).
* **DTOs / ViewModels:** Các đối tượng dùng riêng cho việc nhận request và trả về response.
* **Authorization Handlers/Policies:** Triển khai các logic ủy quyền tùy chỉnh.
* **Cấu hình API Documentation:** Cài đặt và cấu hình Scalar hoặc Swagger.

## 3. Nguyên Tắc Cốt Lõi

* **Controller "Mỏng" (Thin Controller):** Controller chỉ nên làm nhiệm vụ nhận request, xác thực đầu vào cơ bản, đóng gói dữ liệu thành Command/Query và gửi cho MediatR. Tuyệt đối không chứa logic nghiệp vụ.
* **Composition Root:** Đây là nơi duy nhất trong ứng dụng mà tất cả các tầng được "biết" đến nhau để cấu hình DI.
* **Xử lý các vấn đề của HTTP:** Chịu trách nhiệm về mã trạng thái HTTP, định dạng response (JSON), CORS, rate limiting.

## 4. Các Phụ Thuộc

* **Tham chiếu đến:** `Application`, `Infrastructure`, `Persistence.WriteDb`, `Persistence.ReadDb`, `Shared`.