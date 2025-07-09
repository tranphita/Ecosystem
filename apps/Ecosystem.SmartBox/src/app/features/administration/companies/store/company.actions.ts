import { createAction, props } from '@ngrx/store';
import { SmartBoxCompanyDto, CreateUpdateSmartBoxCompanyDto, GetSmartBoxCompaniesInput, PagedResultDto, ListResultDto } from '../../../../shared/models';

/**
 * Company Actions cho NgRx Store
 */

// === Load Companies Actions ===
export const loadCompanies = createAction(
  '[Company] Load Companies',
  props<{ input: GetSmartBoxCompaniesInput }>()
);

export const loadCompaniesSuccess = createAction(
  '[Company] Load Companies Success',
  props<{ result: PagedResultDto<SmartBoxCompanyDto> }>()
);

export const loadCompaniesFailure = createAction(
  '[Company] Load Companies Failure',
  props<{ error: string }>()
);

// === Load Active Companies Actions ===
export const loadActiveCompanies = createAction(
  '[Company] Load Active Companies'
);

export const loadActiveCompaniesSuccess = createAction(
  '[Company] Load Active Companies Success',
  props<{ companies: SmartBoxCompanyDto[] }>()
);

export const loadActiveCompaniesFailure = createAction(
  '[Company] Load Active Companies Failure',
  props<{ error: string }>()
);

// === Load Single Company Actions ===
export const loadCompany = createAction(
  '[Company] Load Company',
  props<{ id: string }>()
);

export const loadCompanySuccess = createAction(
  '[Company] Load Company Success',
  props<{ company: SmartBoxCompanyDto }>()
);

export const loadCompanyFailure = createAction(
  '[Company] Load Company Failure',
  props<{ error: string }>()
);

// === Create Company Actions ===
export const createCompany = createAction(
  '[Company] Create Company',
  props<{ input: CreateUpdateSmartBoxCompanyDto }>()
);

export const createCompanySuccess = createAction(
  '[Company] Create Company Success',
  props<{ company: SmartBoxCompanyDto }>()
);

export const createCompanyFailure = createAction(
  '[Company] Create Company Failure',
  props<{ error: string }>()
);

// === Update Company Actions ===
export const updateCompany = createAction(
  '[Company] Update Company',
  props<{ id: string; input: CreateUpdateSmartBoxCompanyDto }>()
);

export const updateCompanySuccess = createAction(
  '[Company] Update Company Success',
  props<{ company: SmartBoxCompanyDto }>()
);

export const updateCompanyFailure = createAction(
  '[Company] Update Company Failure',
  props<{ error: string }>()
);

// === Delete Company Actions ===
export const deleteCompany = createAction(
  '[Company] Delete Company',
  props<{ id: string }>()
);

export const deleteCompanySuccess = createAction(
  '[Company] Delete Company Success',
  props<{ id: string }>()
);

export const deleteCompanyFailure = createAction(
  '[Company] Delete Company Failure',
  props<{ error: string }>()
);

// === Validation Actions ===
export const validateCompanyName = createAction(
  '[Company] Validate Company Name',
  props<{ name: string; excludeId?: string }>()
);

export const validateCompanyNameSuccess = createAction(
  '[Company] Validate Company Name Success',
  props<{ exists: boolean }>()
);

export const validateCompanyNameFailure = createAction(
  '[Company] Validate Company Name Failure',
  props<{ error: string }>()
);

export const validateCompanyCode = createAction(
  '[Company] Validate Company Code',
  props<{ code: string; excludeId?: string }>()
);

export const validateCompanyCodeSuccess = createAction(
  '[Company] Validate Company Code Success',
  props<{ exists: boolean }>()
);

export const validateCompanyCodeFailure = createAction(
  '[Company] Validate Company Code Failure',
  props<{ error: string }>()
);

// === UI State Actions ===
export const openCompanyDialog = createAction(
  '[Company] Open Company Dialog',
  props<{ mode: 'create' | 'edit' | 'view'; company?: SmartBoxCompanyDto }>()
);

export const closeCompanyDialog = createAction(
  '[Company] Close Company Dialog'
);

// === Filter Actions ===
export const updateCompanyFilter = createAction(
  '[Company] Update Company Filter',
  props<{ filter: Partial<GetSmartBoxCompaniesInput> }>()
);

export const clearCompanyFilter = createAction(
  '[Company] Clear Company Filter'
);

// === Pagination Actions ===
export const setCompanyPage = createAction(
  '[Company] Set Company Page',
  props<{ page: number }>()
);

export const setCompanyPageSize = createAction(
  '[Company] Set Company Page Size',
  props<{ pageSize: number }>()
);

// === Clear State Actions ===
export const clearCompanyState = createAction(
  '[Company] Clear Company State'
);

export const clearSelectedCompany = createAction(
  '[Company] Clear Selected Company'
);

export const clearCompanyError = createAction(
  '[Company] Clear Company Error'
); 