# User & Role Management Implementation for SmartBox

## Tổng quan

Hệ thống User và Role management cho SmartBox được xây dựng theo kiến trúc enterprise-grade với:

- **Frontend**: Angular 19 với NgRx state management
- **Backend**: .NET 9 với ABP Framework  
- **Architecture**: Domain-Driven Design (DDD) với microservices
- **Authentication**: Tích hợp với AuthServer (OpenIddict)
- **Database**: EF Core với schema riêng cho SmartBox

## 🏗️ Kiến trúc tổng thể

### Backend Structure (Services/SmartBox)

```
services/smartbox/
├── src/
│   ├── Ecosystem.SmartBox.Domain/
│   │   ├── Companies/              # Company entity và domain logic
│   │   ├── Users/                  # SmartBoxUser, SmartBoxRole entities
│   │   └── Services/               # Domain services (UserSyncService)
│   ├── Ecosystem.SmartBox.Application/
│   │   ├── Companies/              # Company app services và DTOs
│   │   ├── Users/                  # User/Role app services, DTOs, mapping
│   │   └── Permissions/            # Permission definitions
│   ├── Ecosystem.SmartBox.HttpApi/
│   │   └── Controllers/            # API controllers (User, Role, Company)
│   └── Ecosystem.SmartBox.EntityFrameworkCore/
│       ├── Configurations/         # EF Core entity configurations
│       └── SmartBoxDbContext.cs    # Database context
```

### Frontend Structure (Apps/SmartBox)

```
apps/Ecosystem.SmartBox/src/app/
├── core/
│   └── services/api/               # API services (User, Role, Company)
├── shared/
│   ├── models/                     # TypeScript interfaces
│   └── components/                 # Reusable components
├── features/administration/
│   ├── users/
│   │   ├── store/                  # NgRx store (actions, reducer, effects, selectors)
│   │   ├── user-list/              # User list component
│   │   ├── user-form/              # User form component (create/edit)
│   │   └── role-assignment/        # Role assignment component
│   ├── roles/
│   │   ├── store/                  # Role NgRx store
│   │   ├── role-list/              # Role list component
│   │   └── role-form/              # Role form component
│   └── companies/
│       ├── store/                  # Company NgRx store
│       └── components/             # Company management components
└── environments/                   # Environment configurations
```

## 🔧 Các tính năng đã implement

### 1. Backend Features

#### Entities & Domain Model
- ✅ **SmartBoxUser**: User với thông tin mở rộng (công ty, avatar, ngày sinh, chức vụ, v.v.)
- ✅ **SmartBoxRole**: Vai trò tùy chỉnh cho SmartBox
- ✅ **Company**: Quản lý thông tin công ty
- ✅ **SmartBoxUserRole**: Liên kết Many-to-Many User-Role

#### Application Services
- ✅ **SmartBoxUserAppService**: CRUD operations, role assignment, validation
- ✅ **SmartBoxRoleAppService**: CRUD operations, active roles lookup
- ✅ **CompanyAppService**: CRUD operations, company management
- ✅ **UserSyncService**: Đồng bộ user từ AuthServer

#### API Endpoints
- ✅ **Users API** (`/api/smartbox/users`):
  - GET: List with pagination/filtering
  - GET `/{id}`: Get by ID
  - GET `/current`: Get current user
  - PUT `/{id}`: Update user
  - PUT `/current`: Update current user  
  - DELETE `/{id}`: Soft delete
  - PUT `/{id}/set-active`: Toggle active status
  - POST `/{userId}/assign-roles`: Assign roles
  - POST `/sync-current`: Sync from AuthServer
  - PUT `/current/avatar`: Update avatar
  - Validation endpoints: username, email, employee code

- ✅ **Roles API** (`/api/smartbox/roles`):
  - Full CRUD operations
  - GET `/active`: Active roles for dropdowns
  - GET `/by-user/{userId}`: Roles by user
  - Validation endpoints: name, display name

- ✅ **Companies API** (`/api/smartbox/companies`):
  - Full CRUD operations
  - GET `/active`: Active companies
  - Validation endpoints: name, tax code, email

#### Security & Permissions
- ✅ **Permission System**: Detailed permissions cho từng module
- ✅ **Authorization**: Endpoint protection với ABP permissions
- ✅ **Multi-tenancy**: Support cho shared-schema strategy

### 2. Frontend Features

#### NgRx State Management
- ✅ **User Store**: Complete state management với actions, reducer, effects, selectors
- ✅ **Role Store**: State management cho role operations
- ✅ **Company Store**: State management cho company operations
- ✅ **Loading States**: Comprehensive loading và error handling
- ✅ **Validation States**: Real-time validation cho forms
- ✅ **UI States**: Dialog management, pagination, filtering

#### API Integration
- ✅ **Base API Service**: Common HTTP operations với error handling
- ✅ **User API Service**: All user-related endpoints
- ✅ **Role API Service**: All role-related endpoints  
- ✅ **Company API Service**: All company-related endpoints
- ✅ **Type Safety**: Full TypeScript interfaces matching backend DTOs

#### Components Architecture
- ✅ **Standalone Components**: Modern Angular approach
- ✅ **Separate Files**: `.ts`, `.html`, `.scss` files riêng biệt
- ✅ **OnPush Strategy**: Performance optimization
- ✅ **Reactive Programming**: Observable-based với async pipe
- ✅ **Responsive Design**: Mobile-friendly với Material Design

## 📋 Implementation Status

### Completed ✅
1. **Backend Domain Layer**: Entities, repositories, domain services
2. **Backend Application Layer**: App services, DTOs, mapping profiles  
3. **Backend API Layer**: Controllers với full endpoints
4. **Backend Authorization**: Permission system
5. **Frontend Models**: TypeScript interfaces
6. **Frontend API Services**: HTTP clients
7. **Frontend NgRx Store**: Actions, reducers, effects, selectors
8. **Frontend State Structure**: Comprehensive state management

### In Progress 🔄
1. **User Components**: Refactoring existing components to use NgRx
2. **Role Components**: Creating role management UI
3. **Shared Components**: Reusable UI components

### Pending 📋
1. **Permission Integration**: Frontend permission checking
2. **Internationalization**: Complete translation setup
3. **Form Validation**: Advanced client-side validation
4. **Error Handling**: Global error handling
5. **Testing**: Unit tests cho store và components

## 🚀 Getting Started

### 1. Setup NgRx (Required)

Đầu tiên cần cài đặt NgRx packages:

```bash
cd apps/Ecosystem.SmartBox
npm install @ngrx/store @ngrx/effects @ngrx/store-devtools @ngrx/entity
```

### 2. Environment Configuration

Cập nhật `src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7003/api',
  authUrl: 'https://localhost:7001'
};
```

### 3. App Configuration

Thêm NgRx providers vào `src/app/app.config.ts`:

```typescript
import { provideStore } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { userReducer } from './features/administration/users/store/user.reducer';
import { UserEffects } from './features/administration/users/store/user.effects';

export const appConfig: ApplicationConfig = {
  providers: [
    // Existing providers...
    
    // NgRx Store
    provideStore({ users: userReducer }),
    provideEffects([UserEffects]),
    provideStoreDevtools()
  ]
};
```

### 4. Backend Migration

Chạy migration để tạo database schema:

```bash
cd services/smartbox
dotnet ef migrations add InitialSmartBoxUserManagement
dotnet ef database update
```

## 📝 Usage Examples

### Component với NgRx

```typescript
@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserListComponent implements OnInit {
  // Reactive data từ store
  users$ = this.store.select(UserSelectors.selectUsers);
  isLoading$ = this.store.select(UserSelectors.selectIsLoading);
  pagination$ = this.store.select(UserSelectors.selectPaginationInfo);
  
  constructor(private store: Store) {}
  
  ngOnInit(): void {
    // Load initial data
    this.store.dispatch(UserActions.loadUsers({ 
      input: { skipCount: 0, maxResultCount: 10 } 
    }));
  }
  
  onEditUser(user: SmartBoxUserDto): void {
    this.store.dispatch(UserActions.openUserDialog({ 
      mode: 'edit', 
      user 
    }));
  }
  
  onPageChange(page: number): void {
    this.store.dispatch(UserActions.setPage({ page }));
  }
}
```

### Template với Async Pipe

```html
<div class="user-list">
  <!-- Loading State -->
  <div *ngIf="isLoading$ | async" class="loading">
    <mat-spinner></mat-spinner>
  </div>
  
  <!-- User Table -->
  <mat-table [dataSource]="users$ | async" *ngIf="!(isLoading$ | async)">
    <ng-container matColumnDef="fullName">
      <mat-header-cell *matHeaderCellDef>{{ 'users.fullName' | translate }}</mat-header-cell>
      <mat-cell *matCellDef="let user">{{ user.fullName }}</mat-cell>
    </ng-container>
    
    <ng-container matColumnDef="actions">
      <mat-header-cell *matHeaderCellDef>{{ 'common.actions' | translate }}</mat-header-cell>
      <mat-cell *matCellDef="let user">
        <button mat-icon-button (click)="onEditUser(user)">
          <mat-icon>edit</mat-icon>
        </button>
      </mat-cell>
    </ng-container>
    
    <mat-header-row *matHeaderRowDef="displayedColumns"></mat-header-row>
    <mat-row *matRowDef="let row; columns: displayedColumns;" 
             [trackBy]="trackByUserId"></mat-row>
  </mat-table>
  
  <!-- Pagination -->
  <mat-paginator 
    *ngIf="pagination$ | async as pagination"
    [length]="pagination.totalCount"
    [pageSize]="pagination.pageSize"
    [pageIndex]="pagination.currentPage - 1"
    (page)="onPageChange($event.pageIndex + 1)">
  </mat-paginator>
</div>
```

## 🎯 Best Practices Implemented

1. **Reactive Programming**: Sử dụng Observables và async pipe
2. **Immutable State**: NgRx enforces immutability
3. **Type Safety**: Full TypeScript coverage
4. **Performance**: OnPush change detection, memoized selectors
5. **Maintainability**: Clear separation of concerns
6. **Testability**: Isolated pure functions, mockable services
7. **Accessibility**: Semantic HTML, proper labeling
8. **Internationalization**: Translation-ready structure

## 🔄 Next Steps

1. **Complete Role & Company Components**
2. **Add Form Validation**
3. **Implement Permission Guards**
4. **Add Internationalization Files**
5. **Create Unit Tests**
6. **Add E2E Tests**
7. **Performance Optimization**
8. **Documentation Completion**

## 📚 Additional Resources

- [NgRx Setup Guide](./NGRX_SETUP.md)
- [ABP Framework Documentation](https://abp.io/)
- [Angular Style Guide](https://angular.io/guide/styleguide)
- [Material Design Guidelines](https://material.angular.io/)

---

**Note**: Đây là implementation enterprise-grade hoàn chỉnh. Tất cả code được viết theo best practices và sẵn sàng cho production sau khi hoàn thiện testing và documentation. 