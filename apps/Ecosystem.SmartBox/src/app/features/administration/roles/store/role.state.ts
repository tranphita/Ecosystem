import { SmartBoxRoleDto } from '../../../../shared/models';
import { LoadingState, PaginationState, BaseFilterState } from '../../../../shared/models/app-state.models';

/**
 * Interface cho Role filter state
 */
export interface RoleFilterState extends BaseFilterState {
  isActive?: boolean;
  isSystem?: boolean;
}

/**
 * Interface cho Role state
 */
export interface RoleState extends LoadingState, PaginationState {
  // Danh sách roles
  roles: SmartBoxRoleDto[];
  
  // Active roles cho dropdown
  activeRoles: SmartBoxRoleDto[];
  
  // Role đang được chọn/chỉnh sửa
  selectedRole: SmartBoxRoleDto | null;
  
  // Filter state
  filter: RoleFilterState;
  
  // UI state
  isDialogOpen: boolean;
  dialogMode: 'create' | 'edit' | 'view' | null;
  
  // Validation states
  isValidatingName: boolean;
  isValidatingDisplayName: boolean;
}

/**
 * Initial state cho Role store
 */
export const initialRoleState: RoleState = {
  // Data
  roles: [],
  activeRoles: [],
  selectedRole: null,
  
  // Loading & Error
  isLoading: false,
  error: null,
  
  // Pagination
  currentPage: 1,
  pageSize: 10,
  totalCount: 0,
  totalPages: 0,
  
  // Filter
  filter: {
    searchTerm: '',
    sortField: 'displayOrder',
    sortDirection: 'asc',
    isActive: undefined,
    isSystem: undefined
  },
  
  // UI states
  isDialogOpen: false,
  dialogMode: null,
  
  // Validation states
  isValidatingName: false,
  isValidatingDisplayName: false
}; 