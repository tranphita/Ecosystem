# ⚙️ Project: Application

## 1. Mục Đích

Project `Application` chứa các quy tắc nghiệp vụ của ứng dụng (Application Business Rules). Nó đóng vai trò điều phối, nhận yêu cầu, sử dụng các đối tượng từ tầng `Domain` để thực hiện các trường hợp sử dụng (use cases) cụ thể của hệ thống.

Đây là nơi định nghĩa "ứng dụng của bạn có thể làm gì".

## 2. Thành Phần Chính

* **Commands & Queries (CQRS):** Tách biệt các hành động thay đổi dữ liệu (Commands) và truy vấn dữ liệu (Queries).
    * *Ví dụ:* `CreateOrderCommand`, `GetOrderByIdQuery`.
* **Handlers (MediatR):** Các lớp xử lý logic cho từng Command và Query.
    * *Ví dụ:* `CreateOrderCommandHandler`, `GetOrderByIdQueryHandler`.
* **Abstractions (Interfaces):** Định nghĩa các "hợp đồng" mà tầng `Infrastructure` phải tuân theo. Đây là chìa khóa của Dependency Inversion.
    * *Ví dụ:* `IOrderRepository`, `IUnitOfWork`, `ICacheService`, `IJwtService`.
* **Validators (FluentValidation):** Các lớp kiểm tra tính hợp lệ của dữ liệu đầu vào cho các Command.
* **DTOs (Data Transfer Objects):** Các đối tượng dùng để truyền dữ liệu giữa các tầng, đặc biệt là giữa `Application` và `Presentation` (`WebApi`).
* **Mappers:** Cấu hình để ánh xạ giữa các `Entities` của `Domain` và `DTOs`.

## 3. Nguyên Tắc Cốt Lõi

* **Phụ thuộc vào trong:** Chỉ phụ thuộc vào `Domain`. **KHÔNG** được phụ thuộc vào `Infrastructure` hay `WebApi`.
* **Không chứa chi tiết hạ tầng:** Logic trong tầng này không quan tâm đến việc dữ liệu được lưu ở đâu (SQL Server, PostgreSQL) hay message được gửi bằng công nghệ gì (RabbitMQ, Kafka). Nó chỉ làm việc với các interfaces.
* **Vertical Slice Architecture:** Code được tổ chức theo tính năng (`Features/{FeatureName}`), giúp tăng tính gắn kết (cohesion) và dễ dàng tìm kiếm, bảo trì.

## 4. Các Phụ Thuộc

* **Tham chiếu đến:** `Domain`, `Shared`.