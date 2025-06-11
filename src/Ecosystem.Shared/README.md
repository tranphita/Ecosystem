# 🌐 Project: Shared

## 1. Mục Đích

Project `Shared` chứa các code, tiện ích và các định nghĩa chung được sử dụng bởi nhiều project khác trong solution. Mục đích của nó là để tránh lặp code (DRY - Don't Repeat Yourself) cho các thành phần không thuộc về một domain cụ thể nào.

## 2. Thành Phần Chính

* **Lớp `Result`:** Một lớp dùng chung để xử lý kết quả thành công/thất bại một cách nhất quán.
* **Các lớp Error chung:** Các định nghĩa lỗi có thể được tái sử dụng.
* **Các Extension Methods:** Các phương thức mở rộng chung (ví dụ: cho `string`, `IEnumerable`).
* **Các hằng số hoặc Enums chung:** Các giá trị không thay đổi được sử dụng ở nhiều nơi.
* **Các base class dùng chung:** (nếu có).

## 3. Nguyên Tắc Cốt Lõi

* **Không có phụ thuộc:** Project này **KHÔNG** được tham chiếu đến bất kỳ project nào khác trong solution.
* **Tính ổn định cao:** Code trong này nên ít khi thay đổi.
* **Tránh lạm dụng:** Không biến project này thành một "bãi rác" chứa mọi thứ. Chỉ nên đặt vào đây những gì thực sự được dùng chung và không có phụ thuộc.

## 4. Các Phụ Thuộc

* **Tham chiếu đến:** Không có.