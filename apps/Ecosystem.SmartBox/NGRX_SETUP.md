# NgRx Setup Guide for SmartBox

## 1. Cài đặt NgRx Dependencies

Chạy lệnh sau để cài đặt các package NgRx cần thiết:

```bash
npm install @ngrx/store @ngrx/effects @ngrx/store-devtools @ngrx/entity @ngrx/component-store
```

Hoặc với pnpm:

```bash
pnpm add @ngrx/store @ngrx/effects @ngrx/store-devtools @ngrx/entity @ngrx/component-store
```

## 2. Cập nhật package.json

Thêm các dependencies sau vào `package.json`:

```json
{
  "dependencies": {
    "@ngrx/store": "^18.0.0",
    "@ngrx/effects": "^18.0.0", 
    "@ngrx/store-devtools": "^18.0.0",
    "@ngrx/entity": "^18.0.0",
    "@ngrx/component-store": "^18.0.0"
  }
}
```

## 3. Cấu hình Root Store

Tạo file `src/app/store/app.config.ts`:

```typescript
import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideStore } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { provideStoreDevtools } from '@ngrx/store-devtools';
import { environment } from '../environments/environment';

// Import reducers và effects
import { userReducer } from '../features/administration/users/store/user.reducer';
import { UserEffects } from '../features/administration/users/store/user.effects';

export const appStoreConfig: ApplicationConfig['providers'] = [
  // NgRx Store
  provideStore({
    users: userReducer,
    // roles: roleReducer,
    // companies: companyReducer
  }),
  
  // NgRx Effects
  provideEffects([
    UserEffects,
    // RoleEffects,
    // CompanyEffects
  ]),
  
  // DevTools (chỉ trong development)
  provideStoreDevtools({
    maxAge: 25,
    logOnly: environment.production
  })
];
```

## 4. Cập nhật App Config

Trong file `src/app/app.config.ts`, thêm store providers:

```typescript
import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { appStoreConfig } from './store/app.config';

import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(),
    
    // NgRx Store Configuration
    ...appStoreConfig,
    
    // Other providers...
  ]
};
```

## 5. Sử dụng Store trong Components

### Import Store Service

```typescript
import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';

// Import actions và selectors
import * as UserActions from '../store/user.actions';
import * as UserSelectors from '../store/user.selectors';
import { SmartBoxUserDto } from '../../../shared/models';
```

### Inject Store và Dispatch Actions

```typescript
@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html'
})
export class UserListComponent implements OnInit {
  // Observables từ store
  users$ = this.store.select(UserSelectors.selectUsers);
  isLoading$ = this.store.select(UserSelectors.selectIsLoading);
  error$ = this.store.select(UserSelectors.selectUserError);
  
  constructor(private store: Store) {}
  
  ngOnInit(): void {
    // Dispatch action để load users
    this.store.dispatch(UserActions.loadUsers({ 
      input: { skipCount: 0, maxResultCount: 10 } 
    }));
  }
  
  onEditUser(user: SmartBoxUserDto): void {
    // Dispatch action để mở dialog edit
    this.store.dispatch(UserActions.openUserDialog({ 
      mode: 'edit', 
      user 
    }));
  }
}
```

## 6. Environment Configuration

Cập nhật `src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7003/api',
  authUrl: 'https://localhost:7001',
  appName: 'SmartBox',
  version: '1.0.0'
};
```

## 7. HTTP Client Setup

Đảm bảo HTTP client được cấu hình đúng trong `src/app/app.config.ts`:

```typescript
import { provideHttpClient, withInterceptors } from '@angular/common/http';

export const appConfig: ApplicationConfig = {
  providers: [
    // HTTP Client với interceptors
    provideHttpClient(
      withInterceptors([
        // authInterceptor,
        // errorInterceptor
      ])
    ),
    
    // NgRx Configuration
    ...appStoreConfig
  ]
};
```

## 8. Testing Store

### Unit Test cho Reducer

```typescript
import { userReducer, initialUserState } from './user.reducer';
import * as UserActions from './user.actions';

describe('User Reducer', () => {
  it('should return the initial state', () => {
    const action = {} as any;
    const result = userReducer(undefined, action);
    expect(result).toBe(initialUserState);
  });

  it('should handle loadUsers action', () => {
    const input = { skipCount: 0, maxResultCount: 10 };
    const action = UserActions.loadUsers({ input });
    const result = userReducer(initialUserState, action);
    
    expect(result.isLoading).toBe(true);
    expect(result.error).toBe(null);
  });
});
```

### Unit Test cho Effects

```typescript
import { TestBed } from '@angular/core/testing';
import { provideMockActions } from '@ngrx/effects/testing';
import { Observable } from 'rxjs';
import { UserEffects } from './user.effects';

describe('UserEffects', () => {
  let actions$: Observable<any>;
  let effects: UserEffects;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        UserEffects,
        provideMockActions(() => actions$)
      ]
    });

    effects = TestBed.inject(UserEffects);
  });

  it('should be created', () => {
    expect(effects).toBeTruthy();
  });
});
```

## 9. Redux DevTools

Cài đặt Redux DevTools Extension cho browser để debug store:

1. Chrome: [Redux DevTools](https://chrome.google.com/webstore/detail/redux-devtools/lmhkpmbekcpmknklioeibfkpmmfibljd)
2. Firefox: [Redux DevTools](https://addons.mozilla.org/en-US/firefox/addon/reduxdevtools/)

## 10. Troubleshooting

### Lỗi thường gặp:

1. **Module không tìm thấy '@ngrx/store'**
   ```bash
   npm install @ngrx/store @ngrx/effects
   ```

2. **Circular dependency warning**
   - Tránh import trực tiếp giữa các module store
   - Sử dụng barrel exports

3. **Type errors với selectors**
   - Đảm bảo state interface được định nghĩa đúng
   - Sử dụng generic types cho selectors

### Performance Tips:

1. Sử dụng `OnPush` change detection strategy
2. Unsubscribe observables với `takeUntil` hoặc `async` pipe
3. Sử dụng `trackBy` functions trong `*ngFor`
4. Memoize expensive selectors

## 11. Next Steps

Sau khi setup xong NgRx:

1. Tạo Role store tương tự User store
2. Tạo Company store 
3. Implement shared components với NgRx
4. Add internationalization support
5. Setup permission-based access control

## References

- [NgRx Official Documentation](https://ngrx.io/)
- [Angular Architecture Patterns](https://angular.io/guide/architecture)
- [RxJS Documentation](https://rxjs.dev/) 