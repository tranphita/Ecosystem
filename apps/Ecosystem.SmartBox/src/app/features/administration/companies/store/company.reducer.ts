import { createReducer, on } from '@ngrx/store';
import { CompanyState, initialCompanyState } from './company.state';
import * as CompanyActions from './company.actions';

/**
 * Company Reducer cho NgRx Store
 */
export const companyReducer = createReducer(
  initialCompanyState,

  // === Load Companies Actions ===
  on(CompanyActions.loadCompanies, (state, { input }) => ({
    ...state,
    isLoading: true,
    error: null,
    filter: {
      ...state.filter,
      ...input,
      searchTerm: input.filter || state.filter.searchTerm
    }
  })),

  on(CompanyActions.loadCompaniesSuccess, (state, { result }) => ({
    ...state,
    companies: result.items,
    totalCount: result.totalCount,
    totalPages: Math.ceil(result.totalCount / state.pageSize),
    isLoading: false,
    error: null
  })),

  on(CompanyActions.loadCompaniesFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // === Load Active Companies Actions ===
  on(CompanyActions.loadActiveCompanies, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(CompanyActions.loadActiveCompaniesSuccess, (state, { companies }) => ({
    ...state,
    activeCompanies: companies,
    isLoading: false,
    error: null
  })),

  on(CompanyActions.loadActiveCompaniesFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // === Load Single Company Actions ===
  on(CompanyActions.loadCompany, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(CompanyActions.loadCompanySuccess, (state, { company }) => ({
    ...state,
    selectedCompany: company,
    isLoading: false,
    error: null
  })),

  on(CompanyActions.loadCompanyFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // === Create Company Actions ===
  on(CompanyActions.createCompany, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(CompanyActions.createCompanySuccess, (state, { company }) => ({
    ...state,
    companies: [...state.companies, company],
    isLoading: false,
    error: null
  })),

  on(CompanyActions.createCompanyFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // === Update Company Actions ===
  on(CompanyActions.updateCompany, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(CompanyActions.updateCompanySuccess, (state, { company }) => ({
    ...state,
    companies: state.companies.map(c => c.id === company.id ? company : c),
    selectedCompany: state.selectedCompany?.id === company.id ? company : state.selectedCompany,
    isLoading: false,
    error: null
  })),

  on(CompanyActions.updateCompanyFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // === Delete Company Actions ===
  on(CompanyActions.deleteCompany, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(CompanyActions.deleteCompanySuccess, (state, { id }) => ({
    ...state,
    companies: state.companies.filter(c => c.id !== id),
    selectedCompany: state.selectedCompany?.id === id ? null : state.selectedCompany,
    isLoading: false,
    error: null
  })),

  on(CompanyActions.deleteCompanyFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // === Validation Actions ===
  on(CompanyActions.validateCompanyName, (state) => ({
    ...state,
    isValidatingName: true
  })),

  on(CompanyActions.validateCompanyNameSuccess, (state, { exists }) => ({
    ...state,
    isValidatingName: false
  })),

  on(CompanyActions.validateCompanyNameFailure, (state) => ({
    ...state,
    isValidatingName: false
  })),

  on(CompanyActions.validateCompanyCode, (state) => ({
    ...state,
    isValidatingTaxCode: true
  })),

  on(CompanyActions.validateCompanyCodeSuccess, (state, { exists }) => ({
    ...state,
    isValidatingTaxCode: false
  })),

  on(CompanyActions.validateCompanyCodeFailure, (state) => ({
    ...state,
    isValidatingTaxCode: false
  })),

  // === Dialog Management Actions ===
  on(CompanyActions.openCompanyDialog, (state, { mode, company }) => ({
    ...state,
    isDialogOpen: true,
    dialogMode: mode,
    selectedCompany: company || null
  })),

  on(CompanyActions.closeCompanyDialog, (state) => ({
    ...state,
    isDialogOpen: false,
    dialogMode: null,
    selectedCompany: null
  })),

  // === Filter & Pagination Actions ===
  on(CompanyActions.updateCompanyFilter, (state, { filter }) => ({
    ...state,
    filter: {
      ...state.filter,
      ...filter
    },
    currentPage: 1 // Reset về trang đầu khi filter
  })),

  on(CompanyActions.clearCompanyFilter, (state) => ({
    ...state,
    filter: {
      searchTerm: '',
      sortField: 'name',
      sortDirection: 'asc' as const,
      isActive: undefined
    },
    currentPage: 1
  })),

  on(CompanyActions.setCompanyPage, (state, { page }) => ({
    ...state,
    currentPage: page
  })),

  on(CompanyActions.setCompanyPageSize, (state, { pageSize }) => ({
    ...state,
    pageSize,
    totalPages: Math.ceil(state.totalCount / pageSize),
    currentPage: 1 // Reset về trang đầu khi thay đổi page size
  })),

  // === Clear Actions ===
  on(CompanyActions.clearCompanyError, (state) => ({
    ...state,
    error: null
  })),

  on(CompanyActions.clearSelectedCompany, (state) => ({
    ...state,
    selectedCompany: null
  }))
); 