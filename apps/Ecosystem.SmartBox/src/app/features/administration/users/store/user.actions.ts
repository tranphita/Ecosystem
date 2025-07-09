import { createAction, props } from '@ngrx/store';
import { SmartBoxUserDto, CreateUpdateSmartBoxUserDto, GetSmartBoxUsersInput, PagedResultDto } from '../../../../shared/models';

/**
 * User Actions cho NgRx Store
 */

// === Load Users Actions ===
export const loadUsers = createAction(
  '[User] Load Users',
  props<{ input: GetSmartBoxUsersInput }>()
);

export const loadUsersSuccess = createAction(
  '[User] Load Users Success',
  props<{ result: PagedResultDto<SmartBoxUserDto> }>()
);

export const loadUsersFailure = createAction(
  '[User] Load Users Failure',
  props<{ error: string }>()
);

// === Load Single User Actions ===
export const loadUser = createAction(
  '[User] Load User',
  props<{ id: string }>()
);

export const loadUserSuccess = createAction(
  '[User] Load User Success',
  props<{ user: SmartBoxUserDto }>()
);

export const loadUserFailure = createAction(
  '[User] Load User Failure',
  props<{ error: string }>()
);

// === Current User Actions ===
export const loadCurrentUser = createAction(
  '[User] Load Current User'
);

export const loadCurrentUserSuccess = createAction(
  '[User] Load Current User Success',
  props<{ user: SmartBoxUserDto }>()
);

export const loadCurrentUserFailure = createAction(
  '[User] Load Current User Failure',
  props<{ error: string }>()
);

// === Update User Actions ===
export const updateUser = createAction(
  '[User] Update User',
  props<{ id: string; input: CreateUpdateSmartBoxUserDto }>()
);

export const updateUserSuccess = createAction(
  '[User] Update User Success',
  props<{ user: SmartBoxUserDto }>()
);

export const updateUserFailure = createAction(
  '[User] Update User Failure',
  props<{ error: string }>()
);

// === Update Current User Actions ===
export const updateCurrentUser = createAction(
  '[User] Update Current User',
  props<{ input: CreateUpdateSmartBoxUserDto }>()
);

export const updateCurrentUserSuccess = createAction(
  '[User] Update Current User Success',
  props<{ user: SmartBoxUserDto }>()
);

export const updateCurrentUserFailure = createAction(
  '[User] Update Current User Failure',
  props<{ error: string }>()
);

// === Delete User Actions ===
export const deleteUser = createAction(
  '[User] Delete User',
  props<{ id: string }>()
);

export const deleteUserSuccess = createAction(
  '[User] Delete User Success',
  props<{ id: string }>()
);

export const deleteUserFailure = createAction(
  '[User] Delete User Failure',
  props<{ error: string }>()
);

// === Toggle Active Actions ===
export const toggleUserActive = createAction(
  '[User] Toggle User Active',
  props<{ id: string; isActive: boolean }>()
);

export const toggleUserActiveSuccess = createAction(
  '[User] Toggle User Active Success',
  props<{ user: SmartBoxUserDto }>()
);

export const toggleUserActiveFailure = createAction(
  '[User] Toggle User Active Failure',
  props<{ error: string }>()
);

// === Assign Roles Actions ===
export const assignRolesToUser = createAction(
  '[User] Assign Roles To User',
  props<{ userId: string; roleIds: string[] }>()
);

export const assignRolesToUserSuccess = createAction(
  '[User] Assign Roles To User Success',
  props<{ userId: string; roleIds: string[] }>()
);

export const assignRolesToUserFailure = createAction(
  '[User] Assign Roles To User Failure',
  props<{ error: string }>()
);

// === Sync User Actions ===
export const syncCurrentUser = createAction(
  '[User] Sync Current User'
);

export const syncCurrentUserSuccess = createAction(
  '[User] Sync Current User Success',
  props<{ user: SmartBoxUserDto }>()
);

export const syncCurrentUserFailure = createAction(
  '[User] Sync Current User Failure',
  props<{ error: string }>()
);

// === Avatar Actions ===
export const updateAvatar = createAction(
  '[User] Update Avatar',
  props<{ avatar: string }>()
);

export const updateAvatarSuccess = createAction(
  '[User] Update Avatar Success',
  props<{ user: SmartBoxUserDto }>()
);

export const updateAvatarFailure = createAction(
  '[User] Update Avatar Failure',
  props<{ error: string }>()
);

// === Validation Actions ===
export const validateUsername = createAction(
  '[User] Validate Username',
  props<{ userName: string; excludeId?: string }>()
);

export const validateUsernameSuccess = createAction(
  '[User] Validate Username Success',
  props<{ exists: boolean }>()
);

export const validateUsernameFailure = createAction(
  '[User] Validate Username Failure',
  props<{ error: string }>()
);

export const validateEmail = createAction(
  '[User] Validate Email',
  props<{ email: string; excludeId?: string }>()
);

export const validateEmailSuccess = createAction(
  '[User] Validate Email Success',
  props<{ exists: boolean }>()
);

export const validateEmailFailure = createAction(
  '[User] Validate Email Failure',
  props<{ error: string }>()
);

export const validateEmployeeCode = createAction(
  '[User] Validate Employee Code',
  props<{ employeeCode: string; excludeId?: string }>()
);

export const validateEmployeeCodeSuccess = createAction(
  '[User] Validate Employee Code Success',
  props<{ exists: boolean }>()
);

export const validateEmployeeCodeFailure = createAction(
  '[User] Validate Employee Code Failure',
  props<{ error: string }>()
);

// === UI State Actions ===
export const openUserDialog = createAction(
  '[User] Open User Dialog',
  props<{ mode: 'create' | 'edit' | 'view'; user?: SmartBoxUserDto }>()
);

export const closeUserDialog = createAction(
  '[User] Close User Dialog'
);

export const openRoleDialog = createAction(
  '[User] Open Role Dialog',
  props<{ user: SmartBoxUserDto }>()
);

export const closeRoleDialog = createAction(
  '[User] Close Role Dialog'
);

// === Filter Actions ===
export const updateFilter = createAction(
  '[User] Update Filter',
  props<{ filter: Partial<GetSmartBoxUsersInput> }>()
);

export const clearFilter = createAction(
  '[User] Clear Filter'
);

// === Pagination Actions ===
export const setPage = createAction(
  '[User] Set Page',
  props<{ page: number }>()
);

export const setPageSize = createAction(
  '[User] Set Page Size',
  props<{ pageSize: number }>()
);

// === Clear State Actions ===
export const clearUserState = createAction(
  '[User] Clear User State'
);

export const clearSelectedUser = createAction(
  '[User] Clear Selected User'
);

export const clearError = createAction(
  '[User] Clear Error'
); 