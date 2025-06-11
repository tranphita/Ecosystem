# 💾 Project: Persistence.WriteDb

## 1. Mục Đích

Đây là một project `Infrastructure` chuyên biệt, chịu trách nhiệm cho việc **ghi và cập nhật** dữ liệu vào cơ sở dữ liệu chính (Write Model) theo mô hình CQRS. Nó sử dụng Entity Framework Core để đảm bảo tính toàn vẹn giao dịch (transactional consistency).

## 2. Thành Phần Chính

* **DbContext:** Lớp `WriteDbContext` kế thừa từ `DbContext` của EF Core, đại diện cho một session với database.
* **Repository Implementations:** Triển khai cụ thể cho các interface repository (ghi) từ tầng `Application`.
    * *Ví dụ:* `OrderRepository`, `CustomerRepository`. Các repository này làm việc trực tiếp với `DbSet` và `AggregateRoot`.
* **Unit of Work Implementation:** Triển khai `IUnitOfWork` để quản lý các transaction và lưu các thay đổi.
* **Entity Configurations:** Cấu hình ánh xạ từ các `Entities` sang các bảng trong database bằng `IEntityTypeConfiguration<T>`.
* **Database Migrations:** Các file migration được tạo và quản lý bởi FluentMigrator hoặc EF Core Migrations.

## 3. Nguyên Tắc Cốt Lõi

* **Tập trung vào ghi:** Chỉ chứa các logic để thêm, sửa, xóa dữ liệu. Các phương thức trả về thường là `void` hoặc `Task`, không trả về các DTO phức tạp.
* **Sử dụng Aggregate Roots:** Mọi thao tác ghi phải được thực hiện thông qua Aggregate Root để đảm bảo các quy tắc nghiệp vụ trong `Domain` được tuân thủ.
* **Giao dịch (Transactions):** Đảm bảo các hoạt động ghi diễn ra trong một giao dịch duy nhất để duy trì sự nhất quán của dữ liệu.

## 4. Các Phụ Thuộc

* **Tham chiếu đến:** `Application`, `Domain`, `Shared`.