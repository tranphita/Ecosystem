import { createFeatureSelector, createSelector } from '@ngrx/store';
import { RoleState } from './role.state';
import { SmartBoxRoleDto } from '../../../../shared/models';

/**
 * Role Selectors cho NgRx Store
 */

// Feature selector
export const selectRoleFeature = createFeatureSelector<RoleState>('roles');

// === Basic Selectors ===
export const selectRoles = createSelector(
  selectRoleFeature,
  (state: RoleState) => state.roles
);

export const selectActiveRoles = createSelector(
  selectRoleFeature,
  (state: RoleState) => state.activeRoles
);

export const selectSelectedRole = createSelector(
  selectRoleFeature,
  (state: RoleState) => state.selectedRole
);

export const selectRoleError = createSelector(
  selectRoleFeature,
  (state: RoleState) => state.error
);

export const selectIsRoleLoading = createSelector(
  selectRoleFeature,
  (state: RoleState) => state.isLoading
);

// === Pagination Selectors ===
export const selectRoleCurrentPage = createSelector(
  selectRoleFeature,
  (state: RoleState) => state.currentPage
);

export const selectRolePageSize = createSelector(
  selectRoleFeature,
  (state: RoleState) => state.pageSize
);

export const selectRoleTotalCount = createSelector(
  selectRoleFeature,
  (state: RoleState) => state.totalCount
);

export const selectRoleTotalPages = createSelector(
  selectRoleFeature,
  (state: RoleState) => state.totalPages
);

export const selectRolePaginationInfo = createSelector(
  selectRoleCurrentPage,
  selectRolePageSize,
  selectRoleTotalCount,
  selectRoleTotalPages,
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
export const selectRoleFilter = createSelector(
  selectRoleFeature,
  (state: RoleState) => state.filter
);

export const selectRoleSearchTerm = createSelector(
  selectRoleFilter,
  (filter) => filter.searchTerm
);

export const selectRoleIsActiveFilter = createSelector(
  selectRoleFilter,
  (filter) => filter.isActive
);

export const selectRoleIsSystemFilter = createSelector(
  selectRoleFilter,
  (filter) => filter.isSystem
);

// === UI State Selectors ===
export const selectIsRoleDialogOpen = createSelector(
  selectRoleFeature,
  (state: RoleState) => state.isDialogOpen
);

export const selectRoleDialogMode = createSelector(
  selectRoleFeature,
  (state: RoleState) => state.dialogMode
);

// === Validation Selectors ===
export const selectIsValidatingRoleName = createSelector(
  selectRoleFeature,
  (state: RoleState) => state.isValidatingName
);

export const selectIsValidatingRoleDisplayName = createSelector(
  selectRoleFeature,
  (state: RoleState) => state.isValidatingDisplayName
);

export const selectRoleValidationStates = createSelector(
  selectIsValidatingRoleName,
  selectIsValidatingRoleDisplayName,
  (isValidatingName, isValidatingDisplayName) => ({
    isValidatingName,
    isValidatingDisplayName,
    isValidating: isValidatingName || isValidatingDisplayName
  })
);

// === Computed Selectors ===
export const selectSystemRoles = createSelector(
  selectRoles,
  (roles: SmartBoxRoleDto[]) => roles.filter((role: SmartBoxRoleDto) => role.isSystem)
);

export const selectCustomRoles = createSelector(
  selectRoles,
  (roles: SmartBoxRoleDto[]) => roles.filter((role: SmartBoxRoleDto) => !role.isSystem)
);

export const selectActiveCustomRoles = createSelector(
  selectRoles,
  (roles: SmartBoxRoleDto[]) => roles.filter((role: SmartBoxRoleDto) => !role.isSystem && role.isActive)
);

export const selectRoleById = createSelector(
  selectRoles,
  (roles: SmartBoxRoleDto[], props: { roleId: string }) =>
    roles.find((role: SmartBoxRoleDto) => role.id === props.roleId)
);

export const selectRoleByName = createSelector(
  selectRoles,
  (roles: SmartBoxRoleDto[], props: { name: string }) =>
    roles.find((role: SmartBoxRoleDto) => role.name === props.name)
);

// === Dashboard Selectors ===
export const selectRoleStats = createSelector(
  selectRoles,
  (roles: SmartBoxRoleDto[]) => {
    const totalRoles = roles.length;
    const activeRoles = roles.filter((r: SmartBoxRoleDto) => r.isActive).length;
    const inactiveRoles = totalRoles - activeRoles;
    const systemRoles = roles.filter((r: SmartBoxRoleDto) => r.isSystem).length;
    const customRoles = totalRoles - systemRoles;

    return {
      totalRoles,
      activeRoles,
      inactiveRoles,
      systemRoles,
      customRoles,
      activePercentage: totalRoles > 0 ? Math.round((activeRoles / totalRoles) * 100) : 0
    };
  }
);

// === Complex Query Selectors ===
export const selectFilteredRoles = createSelector(
  selectRoles,
  selectRoleFilter,
  (roles: SmartBoxRoleDto[], filter) => {
    let filteredRoles = [...roles];

    // Apply search filter
    if (filter.searchTerm) {
      const searchTerm = filter.searchTerm.toLowerCase();
      filteredRoles = filteredRoles.filter((role: SmartBoxRoleDto) =>
        role.name.toLowerCase().includes(searchTerm) ||
        role.displayName.toLowerCase().includes(searchTerm) ||
        role.description?.toLowerCase().includes(searchTerm)
      );
    }

    // Apply active filter
    if (filter.isActive !== undefined) {
      filteredRoles = filteredRoles.filter((role: SmartBoxRoleDto) => role.isActive === filter.isActive);
    }

    // Apply system filter
    if (filter.isSystem !== undefined) {
      filteredRoles = filteredRoles.filter((role: SmartBoxRoleDto) => role.isSystem === filter.isSystem);
    }

    // Apply sorting
    filteredRoles.sort((a: SmartBoxRoleDto, b: SmartBoxRoleDto) => {
      let aValue: string | number;
      let bValue: string | number;

      switch (filter.sortField) {
        case 'name':
          aValue = a.name;
          bValue = b.name;
          break;
        case 'displayName':
          aValue = a.displayName;
          bValue = b.displayName;
          break;
        case 'displayOrder':
          aValue = a.displayOrder;
          bValue = b.displayOrder;
          break;
        default:
          aValue = a.displayOrder;
          bValue = b.displayOrder;
      }

      if (aValue < bValue) {
        return filter.sortDirection === 'asc' ? -1 : 1;
      }
      if (aValue > bValue) {
        return filter.sortDirection === 'asc' ? 1 : -1;
      }
      return 0;
    });

    return filteredRoles;
  }
);

export const selectIsRoleLoadingOrValidating = createSelector(
  selectIsRoleLoading,
  selectRoleValidationStates,
  (isLoading, validationStates) => isLoading || validationStates.isValidating
);

// === Role Dropdown Options ===
export const selectRoleDropdownOptions = createSelector(
  selectActiveRoles,
  (activeRoles: SmartBoxRoleDto[]) => activeRoles.map((role: SmartBoxRoleDto) => ({
    value: role.id,
    label: role.displayName,
    isSystem: role.isSystem
  }))
);

// === Permission Helper Selectors ===
export const selectCanDeleteRole = createSelector(
  selectSelectedRole,
  (selectedRole) => selectedRole ? !selectedRole.isSystem : false
);

export const selectCanEditRole = createSelector(
  selectSelectedRole,
  (selectedRole) => selectedRole !== null
); 