import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';

import { UserApiService } from '../../../../core/services/api/user-api.service';
import * as UserActions from './user.actions';

/**
 * User Effects cho NgRx Store
 */
@Injectable()
export class UserEffects {
  private actions$ = inject(Actions);
  private userApiService = inject(UserApiService);

  // === Load Users Effect ===
  loadUsers$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UserActions.loadUsers),
      switchMap(({ input }) =>
        this.userApiService.getUsers(input).pipe(
          map(result => UserActions.loadUsersSuccess({ result })),
          catchError(error => of(UserActions.loadUsersFailure({ 
            error: error.message || 'Lỗi khi tải danh sách người dùng' 
          })))
        )
      )
    )
  );

  // === Load Single User Effect ===
  loadUser$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UserActions.loadUser),
      switchMap(({ id }) =>
        this.userApiService.getUser(id).pipe(
          map(user => UserActions.loadUserSuccess({ user })),
          catchError(error => of(UserActions.loadUserFailure({ 
            error: error.message || 'Lỗi khi tải thông tin người dùng' 
          })))
        )
      )
    )
  );

  // === Create User Effect ===
  // Note: UserApiService không có createUser method
  // Component hiện tại không sử dụng create action nên có thể bỏ qua

  // === Update User Effect ===
  updateUser$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UserActions.updateUser),
      switchMap(({ id, input }) =>
        this.userApiService.updateUser(id, input).pipe(
          map(user => UserActions.updateUserSuccess({ user })),
          catchError(error => of(UserActions.updateUserFailure({ 
            error: error.message || 'Lỗi khi cập nhật người dùng' 
          })))
        )
      )
    )
  );

  // === Delete User Effect ===
  deleteUser$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UserActions.deleteUser),
      switchMap(({ id }) =>
        this.userApiService.deleteUser(id).pipe(
          map(() => UserActions.deleteUserSuccess({ id })),
          catchError(error => of(UserActions.deleteUserFailure({ 
            error: error.message || 'Lỗi khi xóa người dùng' 
          })))
        )
      )
    )
  );

  // === Assign Roles Effect ===
  assignRoles$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UserActions.assignRolesToUser),
      switchMap(({ userId, roleIds }) =>
        this.userApiService.assignRolesToUser(userId, roleIds).pipe(
          map(() => UserActions.assignRolesToUserSuccess({ userId, roleIds })),
          catchError(error => of(UserActions.assignRolesToUserFailure({ 
            error: error.message || 'Lỗi khi gán vai trò' 
          })))
        )
      )
    )
  );

  // === Toggle Active Effect ===
  toggleActive$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UserActions.toggleUserActive),
      switchMap(({ id, isActive }) =>
        this.userApiService.setActive(id, isActive).pipe(
          map((user) => UserActions.toggleUserActiveSuccess({ user })),
          catchError(error => of(UserActions.toggleUserActiveFailure({ 
            error: error.message || 'Lỗi khi thay đổi trạng thái người dùng' 
          })))
        )
      )
    )
  );

  // === Validation Effects ===
  validateUsername$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UserActions.validateUsername),
      switchMap(({ userName, excludeId }) =>
        this.userApiService.isUserNameExist(userName, excludeId).pipe(
          map(exists => UserActions.validateUsernameSuccess({ exists })),
          catchError(error => of(UserActions.validateUsernameFailure({ 
            error: error.message || 'Lỗi khi kiểm tra tên đăng nhập' 
          })))
        )
      )
    )
  );

  validateEmail$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UserActions.validateEmail),
      switchMap(({ email, excludeId }) =>
        this.userApiService.isEmailExist(email, excludeId).pipe(
          map(exists => UserActions.validateEmailSuccess({ exists })),
          catchError(error => of(UserActions.validateEmailFailure({ 
            error: error.message || 'Lỗi khi kiểm tra email' 
          })))
        )
      )
    )
  );

  validateEmployeeCode$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UserActions.validateEmployeeCode),
      switchMap(({ employeeCode, excludeId }) =>
        this.userApiService.isEmployeeCodeExist(employeeCode, excludeId).pipe(
          map(exists => UserActions.validateEmployeeCodeSuccess({ exists })),
          catchError(error => of(UserActions.validateEmployeeCodeFailure({ 
            error: error.message || 'Lỗi khi kiểm tra mã nhân viên' 
          })))
        )
      )
    )
  );
} 