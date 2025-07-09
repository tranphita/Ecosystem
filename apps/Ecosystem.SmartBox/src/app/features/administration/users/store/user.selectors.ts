import { createFeatureSelector, createSelector } from '@ngrx/store';
import { UserState } from './user.state';
import { SmartBoxUserDto } from '../../../../shared/models';

/**
 * User Selectors cho NgRx Store
 */

// Feature selector
export const selectUserFeature = createFeatureSelector<UserState>('users');

// === Basic Selectors ===
export const selectUsers = createSelector(
  selectUserFeature,
  (state: UserState) => state.users
);

export const selectSelectedUser = createSelector(
  selectUserFeature,
  (state: UserState) => state.selectedUser
);

export const selectUserError = createSelector(
  selectUserFeature,
  (state: UserState) => state.error
);

export const selectIsLoading = createSelector(
  selectUserFeature,
  (state: UserState) => state.isLoading
);

// === Pagination Selectors ===
export const selectCurrentPage = createSelector(
  selectUserFeature,
  (state: UserState) => state.currentPage
);

export const selectPageSize = createSelector(
  selectUserFeature,
  (state: UserState) => state.pageSize
);

export const selectTotalCount = createSelector(
  selectUserFeature,
  (state: UserState) => state.totalCount
);

export const selectTotalPages = createSelector(
  selectUserFeature,
  (state: UserState) => state.totalPages
);

export const selectPaginationInfo = createSelector(
  selectCurrentPage,
  selectPageSize,
  selectTotalCount,
  selectTotalPages,
  (currentPage, pageSize, totalCount, totalPages) => ({
    currentPage,
    pageSize,
    totalCount,
    totalPages,
    hasNextPage: currentPage < totalPages,
    hasPreviousPage: currentPage > 1
  })
);

// === Filter Selectors ===
export const selectFilter = createSelector(
  selectUserFeature,
  (state: UserState) => state.filter
);

export const selectSearchTerm = createSelector(
  selectFilter,
  (filter) => filter.searchTerm
);

export const selectIsActiveFilter = createSelector(
  selectFilter,
  (filter) => filter.isActive
);

export const selectCompanyFilter = createSelector(
  selectFilter,
  (filter) => filter.companyId
);

export const selectRoleFilter = createSelector(
  selectFilter,
  (filter) => filter.roleId
);

// === UI State Selectors ===
export const selectIsDialogOpen = createSelector(
  selectUserFeature,
  (state: UserState) => state.isDialogOpen
);

export const selectDialogMode = createSelector(
  selectUserFeature,
  (state: UserState) => state.dialogMode
);

export const selectIsRoleDialogOpen = createSelector(
  selectUserFeature,
  (state: UserState) => state.isRoleDialogOpen
);

export const selectSelectedUserForRoles = createSelector(
  selectUserFeature,
  (state: UserState) => state.selectedUserForRoles
);

// === Validation Selectors ===
export const selectIsValidatingUsername = createSelector(
  selectUserFeature,
  (state: UserState) => state.isValidatingUsername
);

export const selectIsValidatingEmail = createSelector(
  selectUserFeature,
  (state: UserState) => state.isValidatingEmail
);

export const selectIsValidatingEmployeeCode = createSelector(
  selectUserFeature,
  (state: UserState) => state.isValidatingEmployeeCode
);

export const selectValidationStates = createSelector(
  selectIsValidatingUsername,
  selectIsValidatingEmail,
  selectIsValidatingEmployeeCode,
  (isValidatingUsername, isValidatingEmail, isValidatingEmployeeCode) => ({
    isValidatingUsername,
    isValidatingEmail,
    isValidatingEmployeeCode,
    isValidating: isValidatingUsername || isValidatingEmail || isValidatingEmployeeCode
  })
);

// === Computed Selectors ===
export const selectActiveUsers = createSelector(
  selectUsers,
  (users: SmartBoxUserDto[]) => users.filter((user: SmartBoxUserDto) => user.isActive)
);

export const selectInactiveUsers = createSelector(
  selectUsers,
  (users: SmartBoxUserDto[]) => users.filter((user: SmartBoxUserDto) => !user.isActive)
);

export const selectUsersByCompany = createSelector(
  selectUsers,
  (users: SmartBoxUserDto[], props: { companyId: string }) => 
    users.filter((user: SmartBoxUserDto) => user.companyId === props.companyId)
);

export const selectUserById = createSelector(
  selectUsers,
  (users: SmartBoxUserDto[], props: { userId: string }) => 
    users.find((user: SmartBoxUserDto) => user.id === props.userId)
);

export const selectLastSyncTime = createSelector(
  selectUserFeature,
  (state: UserState) => state.lastSyncTime
);

// === Dashboard Selectors ===
export const selectUserStats = createSelector(
  selectUsers,
  (users: SmartBoxUserDto[]) => {
    const totalUsers = users.length;
    const activeUsers = users.filter((u: SmartBoxUserDto) => u.isActive).length;
    const inactiveUsers = totalUsers - activeUsers;
    
    const usersByDepartment = users.reduce((acc: Record<string, number>, user: SmartBoxUserDto) => {
      const department = user.department || 'Không xác định';
      acc[department] = (acc[department] || 0) + 1;
      return acc;
    }, {} as Record<string, number>);

    const usersByPosition = users.reduce((acc: Record<string, number>, user: SmartBoxUserDto) => {
      const position = user.position || 'Không xác định';
      acc[position] = (acc[position] || 0) + 1;
      return acc;
    }, {} as Record<string, number>);

    return {
      totalUsers,
      activeUsers,
      inactiveUsers,
      usersByDepartment,
      usersByPosition,
      activePercentage: totalUsers > 0 ? Math.round((activeUsers / totalUsers) * 100) : 0
    };
  }
);

// === Complex Query Selectors ===
export const selectFilteredUsers = createSelector(
  selectUsers,
  selectFilter,
  (users: SmartBoxUserDto[], filter) => {
    let filteredUsers = [...users];

    // Apply search filter
    if (filter.searchTerm) {
      const searchTerm = filter.searchTerm.toLowerCase();
      filteredUsers = filteredUsers.filter((user: SmartBoxUserDto) =>
        user.fullName?.toLowerCase().includes(searchTerm) ||
        user.userName.toLowerCase().includes(searchTerm) ||
        user.email.toLowerCase().includes(searchTerm) ||
        user.employeeCode?.toLowerCase().includes(searchTerm)
      );
    }

    // Apply active filter
    if (filter.isActive !== undefined) {
      filteredUsers = filteredUsers.filter((user: SmartBoxUserDto) => user.isActive === filter.isActive);
    }

    // Apply company filter
    if (filter.companyId) {
      filteredUsers = filteredUsers.filter((user: SmartBoxUserDto) => user.companyId === filter.companyId);
    }

    // Apply role filter
    if (filter.roleId) {
      filteredUsers = filteredUsers.filter((user: SmartBoxUserDto) => 
        user.roles.some((role) => role.id === filter.roleId)
      );
    }

    // Apply department filter
    if (filter.department) {
      filteredUsers = filteredUsers.filter((user: SmartBoxUserDto) => user.department === filter.department);
    }

    // Apply position filter
    if (filter.position) {
      filteredUsers = filteredUsers.filter((user: SmartBoxUserDto) => user.position === filter.position);
    }

    return filteredUsers;
  }
);

export const selectIsLoadingOrValidating = createSelector(
  selectIsLoading,
  selectValidationStates,
  (isLoading, validationStates) => isLoading || validationStates.isValidating
); 