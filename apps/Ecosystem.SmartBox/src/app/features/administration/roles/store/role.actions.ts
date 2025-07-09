import { createAction, props } from '@ngrx/store';
import { SmartBoxRoleDto, CreateUpdateSmartBoxRoleDto, GetSmartBoxRolesInput, PagedResultDto, ListResultDto } from '../../../../shared/models';

/**
 * Role Actions cho NgRx Store
 */

// === Load Roles Actions ===
export const loadRoles = createAction(
  '[Role] Load Roles',
  props<{ input: GetSmartBoxRolesInput }>()
);

export const loadRolesSuccess = createAction(
  '[Role] Load Roles Success',
  props<{ result: PagedResultDto<SmartBoxRoleDto> }>()
);

export const loadRolesFailure = createAction(
  '[Role] Load Roles Failure',
  props<{ error: string }>()
);

// === Load Active Roles Actions ===
export const loadActiveRoles = createAction(
  '[Role] Load Active Roles'
);

export const loadActiveRolesSuccess = createAction(
  '[Role] Load Active Roles Success',
  props<{ roles: SmartBoxRoleDto[] }>()
);

export const loadActiveRolesFailure = createAction(
  '[Role] Load Active Roles Failure',
  props<{ error: string }>()
);

// === Load Single Role Actions ===
export const loadRole = createAction(
  '[Role] Load Role',
  props<{ id: string }>()
);

export const loadRoleSuccess = createAction(
  '[Role] Load Role Success',
  props<{ role: SmartBoxRoleDto }>()
);

export const loadRoleFailure = createAction(
  '[Role] Load Role Failure',
  props<{ error: string }>()
);

// === Create Role Actions ===
export const createRole = createAction(
  '[Role] Create Role',
  props<{ input: CreateUpdateSmartBoxRoleDto }>()
);

export const createRoleSuccess = createAction(
  '[Role] Create Role Success',
  props<{ role: SmartBoxRoleDto }>()
);

export const createRoleFailure = createAction(
  '[Role] Create Role Failure',
  props<{ error: string }>()
);

// === Update Role Actions ===
export const updateRole = createAction(
  '[Role] Update Role',
  props<{ id: string; input: CreateUpdateSmartBoxRoleDto }>()
);

export const updateRoleSuccess = createAction(
  '[Role] Update Role Success',
  props<{ role: SmartBoxRoleDto }>()
);

export const updateRoleFailure = createAction(
  '[Role] Update Role Failure',
  props<{ error: string }>()
);

// === Delete Role Actions ===
export const deleteRole = createAction(
  '[Role] Delete Role',
  props<{ id: string }>()
);

export const deleteRoleSuccess = createAction(
  '[Role] Delete Role Success',
  props<{ id: string }>()
);

export const deleteRoleFailure = createAction(
  '[Role] Delete Role Failure',
  props<{ error: string }>()
);

// === Load Roles by User Actions ===
export const loadRolesByUser = createAction(
  '[Role] Load Roles by User',
  props<{ userId: string }>()
);

export const loadRolesByUserSuccess = createAction(
  '[Role] Load Roles by User Success',
  props<{ userId: string; roles: SmartBoxRoleDto[] }>()
);

export const loadRolesByUserFailure = createAction(
  '[Role] Load Roles by User Failure',
  props<{ error: string }>()
);

// === Validation Actions ===
export const validateRoleName = createAction(
  '[Role] Validate Role Name',
  props<{ name: string; excludeId?: string }>()
);

export const validateRoleNameSuccess = createAction(
  '[Role] Validate Role Name Success',
  props<{ exists: boolean }>()
);

export const validateRoleNameFailure = createAction(
  '[Role] Validate Role Name Failure',
  props<{ error: string }>()
);

export const validateDisplayName = createAction(
  '[Role] Validate Display Name',
  props<{ displayName: string; excludeId?: string }>()
);

export const validateDisplayNameSuccess = createAction(
  '[Role] Validate Display Name Success',
  props<{ exists: boolean }>()
);

export const validateDisplayNameFailure = createAction(
  '[Role] Validate Display Name Failure',
  props<{ error: string }>()
);

// === UI State Actions ===
export const openRoleDialog = createAction(
  '[Role] Open Role Dialog',
  props<{ mode: 'create' | 'edit' | 'view'; role?: SmartBoxRoleDto }>()
);

export const closeRoleDialog = createAction(
  '[Role] Close Role Dialog'
);

// === Filter Actions ===
export const updateRoleFilter = createAction(
  '[Role] Update Role Filter',
  props<{ filter: Partial<GetSmartBoxRolesInput> }>()
);

export const clearRoleFilter = createAction(
  '[Role] Clear Role Filter'
);

// === Pagination Actions ===
export const setRolePage = createAction(
  '[Role] Set Role Page',
  props<{ page: number }>()
);

export const setRolePageSize = createAction(
  '[Role] Set Role Page Size',
  props<{ pageSize: number }>()
);

// === Clear State Actions ===
export const clearRoleState = createAction(
  '[Role] Clear Role State'
);

export const clearSelectedRole = createAction(
  '[Role] Clear Selected Role'
);

export const clearRoleError = createAction(
  '[Role] Clear Role Error'
); 