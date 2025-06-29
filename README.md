# Hệ sinh thái Microservice - ABP (.NET 9, Angular)

## 1. Giới thiệu

Đây là dự án mẫu hệ sinh thái Microservice sử dụng ABP Framework, .NET 9, Angular, Docker, PostgreSQL, Redis, RabbitMQ, Seq, hỗ trợ đa dịch vụ, đa tenant, bảo mật hiện đại, CI/CD, logging, caching, orchestration.

## 2. Kiến trúc tổng thể

- **Kiến trúc Microservice**: Mỗi domain là một service độc lập, giao tiếp qua HTTP/API Gateway, chia sẻ một số thư viện chung.
- **Các thành phần chính:**
  - **AppHost**: Orchestration, khởi tạo toàn bộ hệ thống (Aspire/Dotnet Distributed Application).
  - **AuthServer**: Xác thực, cấp phát JWT, OpenIddict, quản lý người dùng.
  - **API Gateway**: Reverse proxy, định tuyến request đến các service.
  - **Các Service**: Administration, Identity, Projects, SaaS (mỗi service là một microservice độc lập, có DB riêng).
  - **AdminUI**: Giao diện quản trị Angular.
  - **DbMigrator**: Tự động migrate database cho toàn bộ hệ thống.
  - **Shared Libraries**: Chứa các module, cấu hình, interface dùng chung.
- **Hạ tầng**: PostgreSQL, Redis, RabbitMQ, Seq (logging), Docker Compose.

## 3. Sơ đồ kiến trúc

```
[AdminUI] <-> [API Gateway] <-> [AuthServer] <-> [Các Service: Administration, Identity, Projects, SaaS]
                                      |           |         |         |
                                 [PostgreSQL]  [Redis]  [RabbitMQ] [Seq]
```

## 4. Chi tiết các thành phần

### 4.1. AppHost
- Orchestration toàn bộ hệ thống, khởi tạo các service, DB, message broker, cache, logging.
- Sử dụng Aspire/Dotnet Distributed Application.

### 4.2. AuthServer
- Xác thực, cấp phát JWT, OpenIddict, quản lý user, đăng nhập SSO.
- Tích hợp với các service qua JWT Bearer.

### 4.3. API Gateway
- Reverse proxy (YARP), định tuyến request đến các service backend.
- Cấu hình route trong `gateway/Ecosystem.Gateway/appsettings.json`.

### 4.4. Các Service (Administration, Identity, Projects, SaaS)
- Mỗi service là một microservice độc lập, có DB riêng, triển khai theo DDD (Domain-Driven Design).
- Cấu trúc: Domain, Application, Infrastructure, Presentation.
- Hỗ trợ multi-tenancy, caching (Redis), logging (Serilog), background jobs, event bus (RabbitMQ).

### 4.5. AdminUI
- Giao diện quản trị Angular, kết nối qua API Gateway.
- Hỗ trợ quản lý user, tenant, project, phân quyền, cấu hình hệ thống.

### 4.6. DbMigrator
- Tự động migrate database cho toàn bộ hệ thống.
- Chạy độc lập hoặc tích hợp trong quá trình khởi động AppHost.

### 4.7. Shared Libraries
- Chứa các module, interface, cấu hình dùng chung giữa các service.

## 5. Hướng dẫn cài đặt & chạy hệ thống

### 5.1. Yêu cầu
- Docker, Docker Compose
- .NET 9 SDK
- Node.js, Yarn (cho AdminUI)

### 5.2. Chạy toàn bộ hệ thống (khuyến nghị)

```bash
cd apps/Ecosystem.AppHost
# Chạy orchestration toàn bộ hệ thống
# (hoặc dùng Visual Studio/Aspire)
dotnet run
```

### 5.3. Chạy từng service bằng Docker Compose

Ví dụ với service Administration:
```bash
cd services/administration
# Chạy DB, identity-server, administration service
# (tương tự cho các service khác: projects, saas)
docker-compose up --build
```

### 5.4. Chạy giao diện AdminUI

```bash
cd apps/Ecosystem.AdminUI
yarn install
yarn start
# hoặc: ng serve
```
Truy cập: http://localhost:4200

### 5.5. Migrate database

```bash
cd shared/Ecosystem.DbMigrator
dotnet run
```
Hoặc chạy cùng AppHost, DbMigrator sẽ tự động migrate trước khi các service khởi động.

## 6. Phát triển & mở rộng

- Mỗi service có thể phát triển, deploy, scale độc lập.
- Thêm service mới: tạo thư mục mới trong `services/`, kế thừa cấu trúc DDD, đăng ký module vào AppHost.
- Giao diện AdminUI có thể mở rộng module Angular theo chuẩn ABP.
- Sử dụng các shared library để chia sẻ logic, interface, DTO giữa các service.

## 7. Test & CI/CD

- Unit test, integration test cho từng service (thư mục `test/` trong mỗi service).
- Có thể tích hợp CI/CD pipeline (GitHub Actions, Azure DevOps, GitLab CI...) để build, test, deploy tự động.
- Đóng gói Docker image cho từng service, push lên registry.

## 8. Best Practices & Design

- Áp dụng DDD, Clean Architecture, ABP Module.
- Multi-tenancy: mỗi tenant dùng chung schema, tách biệt dữ liệu logic.
- Caching: Redis, sử dụng IDistributedCache, tự động invalidate khi có domain event.
- Logging: Serilog, Seq, structured logging, log request/response, exception.
- Security: JWT, OpenIddict, phân quyền ABP, bảo vệ API bằng [Authorize].
- Localization: ABP resource, hỗ trợ đa ngôn ngữ.
- Docker hóa toàn bộ hệ thống, dễ dàng scale, deploy.

## 9. Tài liệu tham khảo

- [ABP Framework](https://abp.io/)
- [ABP Microservice Series](https://blog.antosubash.com/posts/abp-microservice-series)
- [Aspire (.NET Distributed Application)](https://learn.microsoft.com/en-us/dotnet/aspire/overview/)
- [YARP Reverse Proxy](https://microsoft.github.io/reverse-proxy/)
- [Serilog](https://serilog.net/)
- [RabbitMQ](https://www.rabbitmq.com/)
- [PostgreSQL](https://www.postgresql.org/)
- [Angular](https://angular.io/)

---

> Mọi thắc mắc, đóng góp vui lòng tạo issue hoặc liên hệ nhóm phát triển.

 