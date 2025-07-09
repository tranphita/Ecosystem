import { createReducer, on } from '@ngrx/store';
import { RoleState, initialRoleState } from './role.state';
import * as RoleActions from './role.actions';

/**
 * Role Reducer cho NgRx Store
 */
export const roleReducer = createReducer(
  initialRoleState,

  // === Load Roles Actions ===
  on(RoleActions.loadRoles, (state, { input }) => ({
    ...state,
    isLoading: true,
    error: null,
    filter: {
      ...state.filter,
      ...input,
      searchTerm: input.filter || state.filter.searchTerm
    }
  })),

  on(RoleActions.loadRolesSuccess, (state, { result }) => ({
    ...state,
    roles: result.items,
    totalCount: result.totalCount,
    totalPages: Math.ceil(result.totalCount / state.pageSize),
    isLoading: false,
    error: null
  })),

  on(RoleActions.loadRolesFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // === Load Active Roles Actions ===
  on(RoleActions.loadActiveRoles, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(RoleActions.loadActiveRolesSuccess, (state, { roles }) => ({
    ...state,
    activeRoles: roles,
    isLoading: false,
    error: null
  })),

  on(RoleActions.loadActiveRolesFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // === Load Single Role Actions ===
  on(RoleActions.loadRole, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(RoleActions.loadRoleSuccess, (state, { role }) => ({
    ...state,
    selectedRole: role,
    isLoading: false,
    error: null
  })),

  on(RoleActions.loadRoleFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // === Create Role Actions ===
  on(RoleActions.createRole, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(RoleActions.createRoleSuccess, (state, { role }) => ({
    ...state,
    roles: [...state.roles, role],
    activeRoles: role.isActive ? [...state.activeRoles, role] : state.activeRoles,
    totalCount: state.totalCount + 1,
    totalPages: Math.ceil((state.totalCount + 1) / state.pageSize),
    isLoading: false,
    error: null
  })),

  on(RoleActions.createRoleFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // === Update Role Actions ===
  on(RoleActions.updateRole, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(RoleActions.updateRoleSuccess, (state, { role }) => {
    const updatedRoles = state.roles.map(r => r.id === role.id ? role : r);
    const updatedActiveRoles = role.isActive 
      ? state.activeRoles.map(r => r.id === role.id ? role : r)
      : state.activeRoles.filter(r => r.id !== role.id);
    
    return {
      ...state,
      roles: updatedRoles,
      activeRoles: updatedActiveRoles,
      selectedRole: state.selectedRole?.id === role.id ? role : state.selectedRole,
      isLoading: false,
      error: null
    };
  }),

  on(RoleActions.updateRoleFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // === Delete Role Actions ===
  on(RoleActions.deleteRole, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(RoleActions.deleteRoleSuccess, (state, { id }) => ({
    ...state,
    roles: state.roles.filter(r => r.id !== id),
    activeRoles: state.activeRoles.filter(r => r.id !== id),
    selectedRole: state.selectedRole?.id === id ? null : state.selectedRole,
    totalCount: Math.max(0, state.totalCount - 1),
    totalPages: Math.ceil(Math.max(0, state.totalCount - 1) / state.pageSize),
    isLoading: false,
    error: null
  })),

  on(RoleActions.deleteRoleFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // === Load Roles by User Actions ===
  on(RoleActions.loadRolesByUser, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(RoleActions.loadRolesByUserSuccess, (state, { userId, roles }) => ({
    ...state,
    isLoading: false,
    error: null
    // Note: roles by user không được store trong state, 
    // chỉ được return qua effects cho component sử dụng
  })),

  on(RoleActions.loadRolesByUserFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // === Validation Actions ===
  on(RoleActions.validateRoleName, (state) => ({
    ...state,
    isValidatingName: true
  })),

  on(RoleActions.validateRoleNameSuccess, RoleActions.validateRoleNameFailure, (state) => ({
    ...state,
    isValidatingName: false
  })),

  on(RoleActions.validateDisplayName, (state) => ({
    ...state,
    isValidatingDisplayName: true
  })),

  on(RoleActions.validateDisplayNameSuccess, RoleActions.validateDisplayNameFailure, (state) => ({
    ...state,
    isValidatingDisplayName: false
  })),

  // === UI State Actions ===
  on(RoleActions.openRoleDialog, (state, { mode, role }) => ({
    ...state,
    isDialogOpen: true,
    dialogMode: mode,
    selectedRole: role || null
  })),

  on(RoleActions.closeRoleDialog, (state) => ({
    ...state,
    isDialogOpen: false,
    dialogMode: null
  })),

  // === Filter Actions ===
  on(RoleActions.updateRoleFilter, (state, { filter }) => ({
    ...state,
    filter: {
      ...state.filter,
      ...filter
    },
    currentPage: 1 // Reset về trang đầu khi filter thay đổi
  })),

  on(RoleActions.clearRoleFilter, (state) => ({
    ...state,
    filter: {
      searchTerm: '',
      sortField: 'displayOrder',
      sortDirection: 'asc' as 'asc' | 'desc',
      isActive: undefined,
      isSystem: undefined
    },
    currentPage: 1
  })),

  // === Pagination Actions ===
  on(RoleActions.setRolePage, (state, { page }) => ({
    ...state,
    currentPage: page
  })),

  on(RoleActions.setRolePageSize, (state, { pageSize }) => ({
    ...state,
    pageSize,
    currentPage: 1,
    totalPages: Math.ceil(state.totalCount / pageSize)
  })),

  // === Clear State Actions ===
  on(RoleActions.clearRoleState, () => initialRoleState),

  on(RoleActions.clearSelectedRole, (state) => ({
    ...state,
    selectedRole: null
  })),

  on(RoleActions.clearRoleError, (state) => ({
    ...state,
    error: null
  }))
); 