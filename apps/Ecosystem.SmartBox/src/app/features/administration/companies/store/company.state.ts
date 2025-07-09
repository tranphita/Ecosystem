import { CompanyDto } from '../../../../shared/models';
import { LoadingState, PaginationState, BaseFilterState } from '../../../../shared/models/app-state.models';

/**
 * Interface cho Company filter state
 */
export interface CompanyFilterState extends BaseFilterState {
  isActive?: boolean;
}

/**
 * Interface cho Company state
 */
export interface CompanyState extends LoadingState, PaginationState {
  // Danh sách companies
  companies: CompanyDto[];
  
  // Active companies cho dropdown
  activeCompanies: CompanyDto[];
  
  // Company đang được chọn/chỉnh sửa
  selectedCompany: CompanyDto | null;
  
  // Filter state
  filter: CompanyFilterState;
  
  // UI state
  isDialogOpen: boolean;
  dialogMode: 'create' | 'edit' | 'view' | null;
  
  // Validation states
  isValidatingName: boolean;
  isValidatingTaxCode: boolean;
  isValidatingEmail: boolean;
}

/**
 * Initial state cho Company store
 */
export const initialCompanyState: CompanyState = {
  // Data
  companies: [],
  activeCompanies: [],
  selectedCompany: null,
  
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
    sortField: 'name',
    sortDirection: 'asc',
    isActive: undefined
  },
  
  // UI states
  isDialogOpen: false,
  dialogMode: null,
  
  // Validation states
  isValidatingName: false,
  isValidatingTaxCode: false,
  isValidatingEmail: false
}; 