import { createReducer, on } from '@ngrx/store';
import { UserState, initialUserState } from './user.state';
import * as UserActions from './user.actions';

/**
 * User Reducer cho NgRx Store
 */
export const userReducer = createReducer(
  initialUserState,

  // === Load Users Actions ===
  on(UserActions.loadUsers, (state, { input }) => ({
    ...state,
    isLoading: true,
    error: null,
    filter: {
      ...state.filter,
      ...input,
      searchTerm: input.filter || state.filter.searchTerm
    }
  })),

  on(UserActions.loadUsersSuccess, (state, { result }) => ({
    ...state,
    users: result.items,
    totalCount: result.totalCount,
    totalPages: Math.ceil(result.totalCount / state.pageSize),
    isLoading: false,
    error: null
  })),

  on(UserActions.loadUsersFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // === Load Single User Actions ===
  on(UserActions.loadUser, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(UserActions.loadUserSuccess, (state, { user }) => ({
    ...state,
    selectedUser: user,
    isLoading: false,
    error: null
  })),

  on(UserActions.loadUserFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // === Current User Actions ===
  on(UserActions.loadCurrentUser, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(UserActions.loadCurrentUserSuccess, (state, { user }) => ({
    ...state,
    selectedUser: user,
    isLoading: false,
    error: null,
    lastSyncTime: new Date()
  })),

  on(UserActions.loadCurrentUserFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // === Update User Actions ===
  on(UserActions.updateUser, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(UserActions.updateUserSuccess, (state, { user }) => {
    const updatedUsers = state.users.map(u => u.id === user.id ? user : u);
    return {
      ...state,
      users: updatedUsers,
      selectedUser: state.selectedUser?.id === user.id ? user : state.selectedUser,
      isLoading: false,
      error: null
    };
  }),

  on(UserActions.updateUserFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // === Update Current User Actions ===
  on(UserActions.updateCurrentUser, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(UserActions.updateCurrentUserSuccess, (state, { user }) => ({
    ...state,
    selectedUser: user,
    isLoading: false,
    error: null,
    lastSyncTime: new Date()
  })),

  on(UserActions.updateCurrentUserFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // === Delete User Actions ===
  on(UserActions.deleteUser, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(UserActions.deleteUserSuccess, (state, { id }) => ({
    ...state,
    users: state.users.filter(u => u.id !== id),
    selectedUser: state.selectedUser?.id === id ? null : state.selectedUser,
    isLoading: false,
    error: null
  })),

  on(UserActions.deleteUserFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // === Toggle Active Actions ===
  on(UserActions.toggleUserActive, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(UserActions.toggleUserActiveSuccess, (state, { user }) => {
    const updatedUsers = state.users.map(u => u.id === user.id ? user : u);
    return {
      ...state,
      users: updatedUsers,
      selectedUser: state.selectedUser?.id === user.id ? user : state.selectedUser,
      isLoading: false,
      error: null
    };
  }),

  on(UserActions.toggleUserActiveFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // === Assign Roles Actions ===
  on(UserActions.assignRolesToUser, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(UserActions.assignRolesToUserSuccess, (state) => ({
    ...state,
    isLoading: false,
    error: null,
    isRoleDialogOpen: false,
    selectedUserForRoles: null
  })),

  on(UserActions.assignRolesToUserFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // === Sync User Actions ===
  on(UserActions.syncCurrentUser, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(UserActions.syncCurrentUserSuccess, (state, { user }) => ({
    ...state,
    selectedUser: user,
    isLoading: false,
    error: null,
    lastSyncTime: new Date()
  })),

  on(UserActions.syncCurrentUserFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // === Avatar Actions ===
  on(UserActions.updateAvatar, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(UserActions.updateAvatarSuccess, (state, { user }) => ({
    ...state,
    selectedUser: user,
    isLoading: false,
    error: null
  })),

  on(UserActions.updateAvatarFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // === Validation Actions ===
  on(UserActions.validateUsername, (state) => ({
    ...state,
    isValidatingUsername: true
  })),

  on(UserActions.validateUsernameSuccess, UserActions.validateUsernameFailure, (state) => ({
    ...state,
    isValidatingUsername: false
  })),

  on(UserActions.validateEmail, (state) => ({
    ...state,
    isValidatingEmail: true
  })),

  on(UserActions.validateEmailSuccess, UserActions.validateEmailFailure, (state) => ({
    ...state,
    isValidatingEmail: false
  })),

  on(UserActions.validateEmployeeCode, (state) => ({
    ...state,
    isValidatingEmployeeCode: true
  })),

  on(UserActions.validateEmployeeCodeSuccess, UserActions.validateEmployeeCodeFailure, (state) => ({
    ...state,
    isValidatingEmployeeCode: false
  })),

  // === UI State Actions ===
  on(UserActions.openUserDialog, (state, { mode, user }) => ({
    ...state,
    isDialogOpen: true,
    dialogMode: mode,
    selectedUser: user || null
  })),

  on(UserActions.closeUserDialog, (state) => ({
    ...state,
    isDialogOpen: false,
    dialogMode: null
  })),

  on(UserActions.openRoleDialog, (state, { user }) => ({
    ...state,
    isRoleDialogOpen: true,
    selectedUserForRoles: user
  })),

  on(UserActions.closeRoleDialog, (state) => ({
    ...state,
    isRoleDialogOpen: false,
    selectedUserForRoles: null
  })),

  // === Filter Actions ===
  on(UserActions.updateFilter, (state, { filter }) => ({
    ...state,
    filter: {
      ...state.filter,
      ...filter
    },
    currentPage: 1 // Reset về trang đầu khi filter thay đổi
  })),

  on(UserActions.clearFilter, (state) => ({
    ...state,
    filter: {
      searchTerm: '',
      sortField: 'fullName',
      sortDirection: 'asc',
      isActive: undefined,
      companyId: undefined,
      roleId: undefined,
      department: undefined,
      position: undefined
    },
    currentPage: 1
  })),

  // === Pagination Actions ===
  on(UserActions.setPage, (state, { page }) => ({
    ...state,
    currentPage: page
  })),

  on(UserActions.setPageSize, (state, { pageSize }) => ({
    ...state,
    pageSize,
    currentPage: 1,
    totalPages: Math.ceil(state.totalCount / pageSize)
  })),

  // === Clear State Actions ===
  on(UserActions.clearUserState, () => initialUserState),

  on(UserActions.clearSelectedUser, (state) => ({
    ...state,
    selectedUser: null
  })),

  on(UserActions.clearError, (state) => ({
    ...state,
    error: null
  }))
); 