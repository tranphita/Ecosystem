# 🧪 Project: Domain.UnitTests

## 1. Mục Đích

Kiểm thử các quy tắc nghiệp vụ cốt lõi trong tầng `Domain` một cách hoàn toàn cô lập. Đây là lớp kiểm thử nền tảng và quan trọng nhất, đảm bảo logic nghiệp vụ của doanh nghiệp hoạt động chính xác.

## 2. Thành Phần Chính

* Các lớp test cho **Entities**, **Aggregate Roots**, và **Value Objects**.
    * *Ví dụ:* `OrderTests.cs`, `MoneyTests.cs`, `EmailTests.cs`.
* Các kịch bản test tập trung vào các phương thức khởi tạo và các hành vi của đối tượng domain.
    * *Ví dụ:* Test `Order.Create()` có tạo ra `OrderCreatedDomainEvent` hay không.

## 3. Nguyên Tắc Cốt Lõi

* **Hoàn toàn cô lập:** **Không sử dụng mock.** Không có bất kỳ phụ thuộc nào vào database, network hay file system.
* **Tốc độ:** Các bài test này phải chạy cực kỳ nhanh.
* **Kiểm tra trạng thái và sự kiện:** Xác minh trạng thái của đối tượng sau khi một hành động được thực hiện và kiểm tra xem các Domain Events tương ứng có được tạo ra không.

## 4. Các Phụ Thuộc

* **Tham chiếu đến:** `Domain`.