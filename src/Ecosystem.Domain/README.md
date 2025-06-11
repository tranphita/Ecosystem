# 🚀 Project: Domain

## 1. Mục Đích

Đây là **trái tim** của toàn bộ ứng dụng. Project `Domain` chứa tất cả các quy tắc nghiệp vụ cốt lõi của doanh nghiệp (Enterprise Business Rules). Nó đại diện cho các khái niệm, quy trình và dữ liệu nền tảng của hệ thống, hoàn toàn độc lập với các chi tiết kỹ thuật như cơ sở dữ liệu hay giao diện người dùng.

Mọi thay đổi trong project này đều phải phản ánh một sự thay đổi trong yêu cầu nghiệp vụ thực tế.

## 2. Thành Phần Chính

* **Entities & Aggregate Roots:** Các đối tượng có định danh và vòng đời, đóng vai trò là gốc của một cụm nghiệp vụ.
    * *Ví dụ:* `Order`, `Customer`, `Product`.
* **Value Objects:** Các đối tượng không có định danh, được định nghĩa bởi các thuộc tính của chúng và thường là bất biến (immutable).
    * *Ví dụ:* `Money`, `Email`, `Address`.
* **Domain Events:** Các sự kiện quan trọng xảy ra trong domain, dùng để giao tiếp một cách lỏng lẻo giữa các phần của hệ thống.
    * *Ví dụ:* `OrderCreatedDomainEvent`, `OrderShippedDomainEvent`.
* **Domain Services:** Chứa các logic nghiệp vụ không thuộc về bất kỳ Entity nào.
* **Specifications:** Định nghĩa các tiêu chí truy vấn nghiệp vụ một cách rõ ràng.
* **Custom Exceptions:** Các ngoại lệ đặc thù của nghiệp vụ.
    * *Ví dụ:* `InvalidOrderStatusException`.

## 3. Nguyên Tắc Cốt Lõi

* **Không có phụ thuộc bên ngoài:** Project này **TUYỆT ĐỐI KHÔNG** được tham chiếu đến bất kỳ project nào khác như `Application`, `Infrastructure`, hay `WebApi`. Nó không được chứa code liên quan đến Entity Framework, ASP.NET, MassTransit, v.v.
* **Logic nghiệp vụ phong phú (Rich Domain Model):** Logic xử lý phải nằm bên trong các Entities và Value Objects, thay vì nằm trong các service class.
* **Bất biến (Immutability):** Ưu tiên thiết kế các Value Objects là bất biến để đảm bảo tính toàn vẹn và an toàn.

## 4. Các Phụ Thuộc

* **Tham chiếu đến:** `Shared` (nếu cần thiết cho các tiện ích chung, không phụ thuộc).