# ⚡ Project: Persistence.ReadDb

## 1. Mục Đích

Đây là một project `Infrastructure` chuyên biệt, chịu trách nhiệm cho việc **truy vấn dữ liệu** (Read Model) một cách hiệu quả nhất theo mô hình CQRS. Nó được tối ưu hóa cho tốc độ đọc và thường sử dụng các công cụ truy vấn nhẹ như Dapper để thực thi các câu lệnh SQL gốc.

Mục tiêu chính của project này là cung cấp dữ liệu đã được định hình sẵn cho tầng trình bày (`WebApi`) một cách nhanh chóng, bỏ qua các overhead của việc tracking thay đổi trong EF Core.

## 2. Thành Phần Chính

* **Repository Implementations (Read):** Triển khai cụ thể cho các interface repository (đọc) từ tầng `Application`.
    * *Ví dụ:* `OrderReadRepository`. Các repository này làm việc trực tiếp với `IDbConnection` và Dapper.
* **Raw SQL Queries:** Chứa các câu lệnh SQL được tối ưu hóa cao, có thể sử dụng các tính năng nâng cao của cơ sở dữ liệu (ví dụ: `json_build_object` trong PostgreSQL) để định hình dữ liệu.
* **Query DTOs:** Các đối tượng truyền dữ liệu (DTO) được thiết kế riêng cho các mục đích hiển thị.
    * *Ví dụ:* `OrderDetailDto`, `OrderListDto`, `OrderAnalyticsDto`. Các DTO này thường là các class "phẳng" (flat).

## 3. Nguyên Tắc Cốt Lõi

* **Tối ưu cho tốc độ đọc:** Ưu tiên hiệu năng trên hết. Các truy vấn có thể được denormalize để giảm số lần join và tăng tốc độ.
* **KHÔNG trả về Domain Entities:** Đây là quy tắc **quan trọng nhất**. Project này **KHÔNG BAO GIỜ** được trả về các đối tượng từ tầng `Domain` (như `Order`, `Customer`). Việc này ép buộc người dùng phải tuân thủ mô hình CQRS và ngăn chặn các thay đổi dữ liệu không mong muốn từ các luồng truy vấn.
* **Độc lập về Schema:** Cơ sở dữ liệu cho Read Model có thể có cấu trúc (schema) khác với Write Model để tối ưu cho việc đọc.

## 4. Các Phụ Thuộc

* **Tham chiếu đến:** `Application`, `Domain` (chỉ để sử dụng các định danh như `OrderId` hoặc các `enum`, không phải để trả về entity), `Shared`.