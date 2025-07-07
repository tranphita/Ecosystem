/**
 * Employee Domain Types
 * Định nghĩa tất cả types liên quan đến Employee management
 */

import { EntityId, BaseApiResponse, PaginatedResponse, ListQueryParams } from './common.types';

// =============================================================================
// EMPLOYEE ENTITY TYPES
// =============================================================================

/** Base Employee interface - readonly cho immutability */
export interface Employee {
  readonly id: EntityId;
  readonly name: string;
  readonly position: string;
  readonly email: string;
  readonly mobile: string;
  readonly dateOfJoining: Date;
  readonly salary: number;
  readonly projects: number;
  readonly imagePath: string;
  readonly department?: string;
  readonly status: EmployeeStatus;
  readonly createdAt: Date;
  readonly updatedAt: Date;
}

/** Employee status enum */
export type EmployeeStatus = 'active' | 'inactive' | 'terminated' | 'on_leave';

/** Employee for creation - omit readonly fields */
export interface CreateEmployeeDto {
  readonly name: string;
  readonly position: string;
  readonly email: string;
  readonly mobile: string;
  readonly dateOfJoining: Date;
  readonly salary: number;
  readonly projects?: number;
  readonly imagePath?: string;
  readonly department?: string;
  readonly status?: EmployeeStatus;
}

/** Employee for update - partial of create */
export interface UpdateEmployeeDto extends Partial<CreateEmployeeDto> {
  readonly id: EntityId;
}

/** Employee display model cho UI components */
export interface EmployeeDisplayModel {
  readonly id: EntityId;
  readonly fullName: string;
  readonly positionTitle: string;
  readonly contactEmail: string;
  readonly phoneNumber: string;
  readonly joinDate: string; // formatted date string
  readonly monthlySalary: string; // formatted currency
  readonly projectCount: number;
  readonly profileImage: string;
  readonly departmentName: string;
  readonly currentStatus: {
    readonly value: EmployeeStatus;
    readonly label: string;
    readonly color: 'primary' | 'accent' | 'warn';
  };
  readonly tenure: string; // e.g., "2 năm 3 tháng"
}

// =============================================================================
// EMPLOYEE TABLE TYPES
// =============================================================================

/** Column configuration cho Employee table */
export interface EmployeeTableColumn {
  readonly key: keyof Employee | 'actions';
  readonly label: string;
  readonly sortable: boolean;
  readonly filterable: boolean;
  readonly width?: string;
  readonly align?: 'left' | 'center' | 'right';
  readonly cellRenderer?: EmployeeCellRenderer;
}

/** Cell renderer types */
export type EmployeeCellRenderer = 
  | 'text'
  | 'currency'
  | 'date'
  | 'status'
  | 'avatar'
  | 'actions';

/** Employee query parameters */
export interface EmployeeQueryParams extends ListQueryParams {
  readonly departmentFilter?: string;
  readonly statusFilter?: EmployeeStatus[];
  readonly salaryRange?: {
    readonly min: number;
    readonly max: number;
  };
  readonly dateJoinedRange?: {
    readonly from: Date;
    readonly to: Date;
  };
}

// =============================================================================
// EMPLOYEE FORM TYPES
// =============================================================================

/** Employee form data structure */
export interface EmployeeFormData {
  readonly name: string;
  readonly position: string;
  readonly email: string;
  readonly mobile: string;
  readonly dateOfJoining: Date;
  readonly salary: number;
  readonly projects: number;
  readonly imagePath: string;
  readonly department: string;
  readonly status: EmployeeStatus;
}

/** Form validation rules */
export interface EmployeeFormValidation {
  readonly name: {
    readonly required: boolean;
    readonly minLength: number;
    readonly maxLength: number;
    readonly pattern?: RegExp;
  };
  readonly email: {
    readonly required: boolean;
    readonly emailFormat: boolean;
    readonly uniqueCheck: boolean;
  };
  readonly mobile: {
    readonly required: boolean;
    readonly pattern: RegExp;
    readonly minLength: number;
    readonly maxLength: number;
  };
  readonly salary: {
    readonly required: boolean;
    readonly min: number;
    readonly max: number;
  };
}

/** Form action types */
export type EmployeeFormAction = 'create' | 'update' | 'view' | 'delete';

/** Form state configuration */
export interface EmployeeFormConfig {
  readonly action: EmployeeFormAction;
  readonly title: string;
  readonly submitButtonText: string;
  readonly cancelButtonText: string;
  readonly showDeleteButton: boolean;
  readonly readOnlyMode: boolean;
}

// =============================================================================
// API RESPONSE TYPES
// =============================================================================

/** Employee API responses */
export type EmployeeResponse = BaseApiResponse<Employee>;
export type EmployeeListResponse = PaginatedResponse<Employee>;
export type CreateEmployeeResponse = BaseApiResponse<Employee>;
export type UpdateEmployeeResponse = BaseApiResponse<Employee>;
export type DeleteEmployeeResponse = BaseApiResponse<{ readonly id: EntityId }>;

// =============================================================================
// EMPLOYEE STATISTICS TYPES
// =============================================================================

/** Employee statistics */
export interface EmployeeStatistics {
  readonly totalEmployees: number;
  readonly activeEmployees: number;
  readonly departmentBreakdown: DepartmentStats[];
  readonly averageSalary: number;
  readonly averageTenure: number;
  readonly newHiresThisMonth: number;
  readonly turnoverRate: number;
}

export interface DepartmentStats {
  readonly department: string;
  readonly employeeCount: number;
  readonly averageSalary: number;
  readonly percentage: number;
}

// =============================================================================
// EMPLOYEE SERVICE TYPES
// =============================================================================

/** Employee service operation results */
export type EmployeeServiceResult<T> = Promise<{
  readonly success: boolean;
  readonly data?: T;
  readonly error?: string;
  readonly validationErrors?: Record<string, string[]>;
}>;

/** Bulk operations */
export interface BulkEmployeeOperation {
  readonly operation: 'delete' | 'update_status' | 'export';
  readonly employeeIds: EntityId[];
  readonly data?: Partial<Employee>;
}

export type BulkOperationResult = EmployeeServiceResult<{
  readonly successCount: number;
  readonly failureCount: number;
  readonly failedItems: Array<{
    readonly id: EntityId;
    readonly error: string;
  }>;
}>;

// =============================================================================
// TYPE GUARDS
// =============================================================================

/** Type guard để kiểm tra Employee object */
export function isEmployee(obj: unknown): obj is Employee {
  return (
    typeof obj === 'object' &&
    obj !== null &&
    'id' in obj &&
    'name' in obj &&
    'position' in obj &&
    'email' in obj &&
    'mobile' in obj
  );
}

/** Type guard để kiểm tra Employee array */
export function isEmployeeArray(obj: unknown): obj is Employee[] {
  return Array.isArray(obj) && obj.every(isEmployee);
}

// =============================================================================
// UTILITY TYPES
// =============================================================================

/** Employee keys để tạo type-safe property access */
export type EmployeeKeys = keyof Employee;

/** Searchable employee fields */
export type SearchableEmployeeFields = 'name' | 'position' | 'email' | 'department';

/** Sortable employee fields */
export type SortableEmployeeFields = 'name' | 'position' | 'dateOfJoining' | 'salary' | 'projects';

/** Employee field value type */
export type EmployeeFieldValue<K extends EmployeeKeys> = Employee[K]; 