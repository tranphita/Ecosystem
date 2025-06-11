# 🧪 Project: Application.UnitTests

## 1. Mục Đích

Kiểm thử logic của các trường hợp sử dụng (use cases) trong tầng `Application` (chủ yếu là các `CommandHandler` và `QueryHandler`) một cách cô lập khỏi các chi tiết hạ tầng như database hay message broker.

## 2. Thành Phần Chính

* Các lớp test cho các `Handlers` của MediatR.
    * *Ví dụ:* `CreateOrderCommandHandlerTests.cs`.
* Sử dụng các thư viện Mocking (ví dụ: **Moq**, **NSubstitute**) để giả lập các phụ thuộc.

## 3. Nguyên Tắc Cốt Lõi

* **Mock các phụ thuộc:** Tất cả các interface được định nghĩa trong `Application` (như `IOrderRepository`, `IUnitOfWork`, `IPublishEndpoint`) phải được giả lập (mock).
* **Kiểm tra sự tương tác (Interaction Testing):** Mục tiêu không phải là kiểm tra xem dữ liệu có được lưu vào DB thật hay không, mà là để xác minh xem handler có gọi đúng phương thức của các dependency hay không (ví dụ: `_unitOfWork.CommitAsync()` có được gọi 1 lần không?).
* **Tập trung vào logic của handler:** Chỉ kiểm tra logic bên trong handler, không kiểm tra logic của các lớp mà nó phụ thuộc.

## 4. Các Phụ Thuộc

* **Tham chiếu đến:** `Application`.