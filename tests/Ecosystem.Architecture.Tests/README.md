# 🏛️ Project: Architecture.Tests

## 1. Mục Đích

Project này không dùng để kiểm thử logic nghiệp vụ mà để **kiểm thử và bảo vệ chính kiến trúc của solution**. Mục đích của nó là tự động hóa việc kiểm tra các quy tắc và ràng buộc về phụ thuộc giữa các project, đảm bảo toàn bộ đội ngũ tuân thủ theo thiết kế Clean Architecture đã đề ra.

Nó hoạt động như một "người bảo vệ kiến trúc", ngăn chặn các vi phạm tiềm ẩn có thể làm xói mòn cấu trúc của hệ thống theo thời gian.

## 2. Thành Phần Chính

* **Architectural Tests:** Các lớp test sử dụng các thư viện phân tích code (reflection) như **NetArchTest.Rules** để định nghĩa và xác minh các quy tắc kiến trúc.
* **Các bộ quy tắc (Rule Sets):** Các bài test thường được nhóm theo từng tầng hoặc theo loại quy tắc. Ví dụ:
    * `DomainLayerTests.cs`: Kiểm tra các quy tắc của tầng Domain (ví dụ: không được tham chiếu đến tầng khác).
    * `ApplicationLayerTests.cs`: Kiểm tra các quy tắc của tầng Application.
    * `NamingConventionTests.cs`: Kiểm tra quy tắc đặt tên (ví dụ: các handler phải có hậu tố là "Handler").
    * `DependencyTests.cs`: Kiểm tra các quy tắc phụ thuộc chéo giữa các tầng.

## 3. Nguyên Tắc Cốt Lõi

* **Kiến trúc là Code (Architecture as Code):** Biến các quy tắc kiến trúc trừu tượng thành các bài test cụ thể, có thể chạy và xác minh được.
* **Phát hiện sớm, sửa chữa nhanh:** Các bài test này thường được chạy trong pipeline CI/CD. Nếu một developer vô tình thêm một tham chiếu sai, pipeline sẽ báo lỗi ngay lập tức, giúp sửa chữa vấn đề trước khi nó trở nên phức tạp.
* **Là tài liệu sống:** Bản thân các bài test này chính là tài liệu rõ ràng nhất về các quy tắc kiến trúc của dự án.

## 4. Các Phụ Thuộc

* **Tham chiếu đến:** Project này cần tham chiếu đến **tất cả các project** trong solution mà nó cần kiểm tra (`Domain`, `Application`, `Infrastructure`, `Persistence.WriteDb`, `Persistence.ReadDb`, `WebApi`, v.v.).