import { SmartBoxUserDto, GetSmartBoxUsersInput } from '../../../../shared/models';
import { LoadingState, PaginationState, BaseFilterState } from '../../../../shared/models/app-state.models';

/**
 * Interface cho User filter state
 */
export interface UserFilterState extends BaseFilterState {
  isActive?: boolean;
  companyId?: string;
  roleId?: string;
  department?: string;
  position?: string;
}

/**
 * Interface cho User state
 */
export interface UserState extends LoadingState, PaginationState {
  // Danh sách users
  users: SmartBoxUserDto[];
  
  // User đang được chọn/chỉnh sửa
  selectedUser: SmartBoxUserDto | null;
  
  // Filter state
  filter: UserFilterState;
  
  // UI state
  isDialogOpen: boolean;
  dialogMode: 'create' | 'edit' | 'view' | null;
  
  // Role assignment state
  isRoleDialogOpen: boolean;
  selectedUserForRoles: SmartBoxUserDto | null;
  
  // Validation states
  isValidatingUsername: boolean;
  isValidatingEmail: boolean;
  isValidatingEmployeeCode: boolean;
  
  // Last sync time for current user
  lastSyncTime: Date | null;
}

/**
 * Initial state cho User store
 */
export const initialUserState: UserState = {
  // Data
  users: [],
  selectedUser: null,
  
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
    sortField: 'fullName',
    sortDirection: 'asc',
    isActive: undefined,
    companyId: undefined,
    roleId: undefined,
    department: undefined,
    position: undefined
  },
  
  // UI states
  isDialogOpen: false,
  dialogMode: null,
  isRoleDialogOpen: false,
  selectedUserForRoles: null,
  
  // Validation states
  isValidatingUsername: false,
  isValidatingEmail: false,
  isValidatingEmployeeCode: false,
  
  // Sync state
  lastSyncTime: null
}; 