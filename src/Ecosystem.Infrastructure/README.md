# 🏗️ Project: Infrastructure

## 1. Mục Đích

Project `Infrastructure` chứa các triển khai cụ thể cho các abstractions (interfaces) đã được định nghĩa ở tầng `Application`. Nó xử lý tất cả các vấn đề liên quan đến các công nghệ và dịch vụ bên ngoài.

Đây là nơi "cách làm" (how) gặp "cái gì" (what). Tầng `Application` định nghĩa "cần gửi email", tầng `Infrastructure` sẽ triển khai "gửi email bằng SendGrid".

## 2. Thành Phần Chính

* **Triển khai các Services:**
    * *Ví dụ:* `RedisCacheService`, `HangfireBackgroundJobService`, `JwtService`, `LaunchDarklyFeatureFlagService`.
* **Cấu hình các thư viện bên ngoài:**
    * *Ví dụ:* `MassTransitConfiguration`, `OpenTelemetryConfiguration`.
* **Consumers/Subscribers:** Các lớp lắng nghe sự kiện từ message broker.
    * *Ví dụ:* `ErrorMessageConsumer`.
* **gRPC Services:** Triển khai các dịch vụ gRPC.
* **Các Clients:** Code để giao tiếp với các API của bên thứ ba.

## 3. Nguyên Tắc Cốt Lõi

* **Triển khai Interfaces:** Các class trong này thường triển khai các interface từ tầng `Application`.
* **Tính "cắm-rút" (Pluggable):** Về lý thuyết, bạn có thể thay thế project này bằng một project `Infrastructure` khác (ví dụ: chuyển từ RabbitMQ sang Kafka) mà không cần thay đổi `Application` và `Domain`.
* **Nơi chứa các gói NuGet của bên thứ ba:** Hầu hết các gói NuGet liên quan đến hạ tầng (ví dụ: `MassTransit.RabbitMQ`, `AWSSDK.S3`, `Polly`) được cài đặt tại đây.

## 4. Các Phụ Thuộc

* **Tham chiếu đến:** `Application`, `Domain`, `Shared`.