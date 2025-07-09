# Hướng dẫn Setup SmartBox User & Role Management

## 🚀 Bước 1: Cài đặt NgRx packages

```bash
cd apps/Ecosystem.SmartBox
npm install @ngrx/store @ngrx/effects @ngrx/store-devtools @ngrx/entity
```

## 🔧 Bước 2: Cấu hình NgRx trong app.config.ts

```typescript
import { provideStore } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { provideStoreDevtools } from '@ngrx/store-devtools';
import { isDevMode } from '@angular/core';

// Import reducers và effects
import { userReducer } from './features/administration/users/store/user.reducer';
import { roleReducer } from './features/administration/roles/store/role.reducer';
import { UserEffects } from './features/administration/users/store/user.effects';
import { RoleEffects } from './features/administration/roles/store/role.effects';

export const appConfig: ApplicationConfig = {
  providers: [
    // ... existing providers
    
    // NgRx Store
    provideStore({
      users: userReducer,
      roles: roleReducer
    }),
    
    // NgRx Effects
    provideEffects([
      UserEffects,
      RoleEffects
    ]),
    
    // NgRx DevTools (chỉ trong development)
    provideStoreDevtools({
      maxAge: 25,
      logOnly: !isDevMode(),
      autoPause: true,
      trace: false,
      traceLimit: 75
    })
  ]
};
```

## 🌐 Bước 3: Cập nhật environment.ts

```typescript
// apps/Ecosystem.SmartBox/src/environments/environment.ts
export const environment = {
  production: false,
  apiUrl: 'https://localhost:44321' // URL của SmartBox API
};

// apps/Ecosystem.SmartBox/src/environments/environment.prod.ts
export const environment = {
  production: true,
  apiUrl: 'https://your-production-api.com'
};
```

## 📊 Bước 4: Cấu hình Translation (ngx-translate)

```bash
npm install @ngx-translate/core @ngx-translate/http-loader
```

Thêm vào app.config.ts:

```typescript
import { provideHttpClient } from '@angular/common/http';
import { importProvidersFrom } from '@angular/core';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

export const appConfig: ApplicationConfig = {
  providers: [
    // ... existing providers
    provideHttpClient(),
    importProvidersFrom(
      TranslateModule.forRoot({
        loader: {
          provide: TranslateLoader,
          useFactory: HttpLoaderFactory,
          deps: [HttpClient]
        },
        defaultLanguage: 'vi'
      })
    )
  ]
};
```

## 🛠️ Bước 5: Sử dụng Components

### User List Component
```typescript
import { UserListComponent } from './features/administration/users/user-list/user-list.component';

// Trong template
<app-user-list></app-user-list>
```

### Role List Component
```typescript
import { RoleListComponent } from './features/administration/roles/role-list/role-list.component';

// Trong template
<app-role-list></app-role-list>
```

### Shared Data Table Component
```typescript
import { DataTableComponent, TableColumn, TableAction } from './shared/components/data-table/data-table.component';

// Example usage
export class MyComponent {
  columns: TableColumn[] = [
    { key: 'name', header: 'Name', type: 'text' },
    { key: 'email', header: 'Email', type: 'email' },
    { key: 'isActive', header: 'Status', type: 'boolean' }
  ];

  actions: TableAction[] = [
    { key: 'view', icon: 'visibility', label: 'View' },
    { key: 'edit', icon: 'edit', label: 'Edit' },
    { key: 'delete', icon: 'delete', label: 'Delete', color: 'warn' }
  ];

  onActionClick(event: { action: string; item: any }) {
    console.log('Action:', event.action, 'Item:', event.item);
  }
}
```

### Filter Panel Component
```typescript
import { FilterPanelComponent, FilterField } from './shared/components/filter-panel/filter-panel.component';

export class MyComponent {
  filterFields: FilterField[] = [
    { 
      key: 'search', 
      label: 'Search', 
      type: 'text', 
      placeholder: 'Enter search term...' 
    },
    { 
      key: 'status', 
      label: 'Status', 
      type: 'select',
      options: [
        { value: null, label: 'All' },
        { value: true, label: 'Active' },
        { value: false, label: 'Inactive' }
      ]
    }
  ];

  onFilterChange(filters: { [key: string]: any }) {
    console.log('Filters:', filters);
  }
}
```

## 🔐 Bước 6: Sử dụng Permission System

### Trong Template
```html
<!-- Ẩn/hiện element dựa trên permission -->
<button *hasPermission="'SmartBox.Users.Create'">Create User</button>

<!-- Cần tất cả permissions -->
<div *hasPermission="['SmartBox.Users.View', 'SmartBox.Users.Edit']" [requireAll]="true">
  Advanced Actions
</div>
```

### Trong Component
```typescript
import { PermissionService } from './core/services/permission.service';

export class MyComponent {
  constructor(private permissionService: PermissionService) {}

  checkPermission() {
    this.permissionService.hasPermission('SmartBox.Users.View').subscribe(
      hasPermission => {
        if (hasPermission) {
          // User có quyền
        }
      }
    );
  }

  // Sync check (cho template)
  get canCreateUser(): boolean {
    return this.permissionService.hasPermissionSync('SmartBox.Users.Create');
  }
}
```

### Route Guards
```typescript
// app.routes.ts
const routes: Routes = [
  {
    path: 'users',
    component: UserListComponent,
    canActivate: [PermissionGuard],
    data: { permissions: ['SmartBox.Users.View'] }
  }
];
```

## 📱 Bước 7: Responsive Design

Tất cả components đã được thiết kế responsive:
- Desktop: Full table với tất cả columns
- Tablet: Ẩn một số columns ít quan trọng
- Mobile: Compact layout với columns stack

## 🎨 Bước 8: Theming

Components sử dụng Angular Material theming:
- Light theme (default)
- Dark theme (auto-detect từ system preference)
- Print styles cho tất cả components

## 🧪 Bước 9: Testing

```bash
# Run unit tests
npm run test

# Run e2e tests
npm run e2e
```

## 🚀 Bước 10: Development Server

```bash
# Start development server
npm start

# Hoặc
ng serve
```

Truy cập: http://localhost:4200

## 📝 Ghi chú quan trọng

### API Integration
- Các API services đã sẵn sàng, chỉ cần backend endpoints
- Mock data đang được sử dụng cho development
- Uncomment actual API calls trong services khi backend ready

### Permission System
- Mock permissions trong `PermissionService`
- Thay thế bằng actual API calls khi backend ready
- Constants cho permission names trong `PermissionService.PERMISSIONS`

### State Management
- NgRx store đã được setup đầy đủ
- Actions, Reducers, Effects, Selectors đã implement
- DevTools integration cho debugging

### Translation
- Vietnamese và English đã được setup
- Thêm ngôn ngữ mới bằng cách tạo file .json trong assets/i18n/
- Translation keys đã được organize theo modules

## 🐛 Troubleshooting

### NgRx Store not working
- Kiểm tra app.config.ts có provideStore() không
- Verify reducer imports
- Check NgRx DevTools trong browser

### API calls failing
- Kiểm tra environment.ts có đúng API URL không
- Verify CORS settings trên backend
- Check network tab trong DevTools

### Permissions not working
- Kiểm tra PermissionService có load permissions không
- Verify permission names match backend
- Check browser console for errors

### Translation not working
- Kiểm tra translation files trong assets/i18n/
- Verify TranslateModule configuration
- Check browser network tab for loading errors

## 📚 Tài liệu tham khảo

- [NgRx Documentation](https://ngrx.io/)
- [Angular Material](https://material.angular.io/)
- [ngx-translate](https://github.com/ngx-translate/core)
- [ABP Framework](https://abp.io/)

## 👥 Support

Nếu gặp vấn đề, vui lòng:
1. Check console errors
2. Verify configuration
3. Review this documentation
4. Contact development team 