/**
 * Core Types cho Ecosystem SmartBox Application
 * Định nghĩa các types cơ bản được sử dụng trong toàn bộ ứng dụng
 */

// =============================================================================
// BASIC TYPES
// =============================================================================

/** ID duy nhất cho entities */
export type EntityId = string | number;

/** Trạng thái loading */
export type LoadingState = 'idle' | 'loading' | 'success' | 'error';

/** Kiểu dữ liệu cơ bản */
export type Primitive = string | number | boolean | null | undefined;

/** Object có thể có bất kỳ thuộc tính nào */
export type AnyObject = Record<string, unknown>;

/** Tùy chọn có thể null */
export type Nullable<T> = T | null;

/** Tùy chọn có thể undefined */
export type Optional<T> = T | undefined;

// =============================================================================
// API RESPONSE TYPES
// =============================================================================

/** Base response structure cho tất cả API calls */
export interface BaseApiResponse<TData = unknown> {
  /** Kết quả thành công hay thất bại */
  readonly success: boolean;
  /** Thông điệp từ server */
  readonly message: string;
  /** Dữ liệu trả về */
  readonly data: TData;
  /** Thời gian server xử lý */
  readonly timestamp: string;
  /** Request ID để tracking */
  readonly requestId?: string;
}

/** Error response structure */
export interface ApiErrorResponse {
  readonly success: false;
  readonly message: string;
  readonly error: {
    readonly code: string;
    readonly details: string;
    readonly field?: string;
    readonly validationErrors?: ValidationError[];
  };
  readonly timestamp: string;
  readonly requestId?: string;
}

/** Validation error cho form fields */
export interface ValidationError {
  readonly field: string;
  readonly message: string;
  readonly code: string;
  readonly value?: Primitive;
}

/** Paginated response */
export interface PaginatedResponse<TData> extends BaseApiResponse<TData[]> {
  readonly pagination: {
    readonly currentPage: number;
    readonly totalPages: number;
    readonly totalItems: number;
    readonly itemsPerPage: number;
    readonly hasNext: boolean;
    readonly hasPrevious: boolean;
  };
}

// =============================================================================
// ERROR TYPES
// =============================================================================

/** Các loại error trong ứng dụng */
export type ErrorType = 
  | 'NETWORK_ERROR'
  | 'VALIDATION_ERROR' 
  | 'AUTHENTICATION_ERROR'
  | 'AUTHORIZATION_ERROR'
  | 'SERVER_ERROR'
  | 'CLIENT_ERROR'
  | 'UNKNOWN_ERROR';

/** Chi tiết error */
export interface AppError {
  readonly type: ErrorType;
  readonly message: string;
  readonly code?: string;
  readonly details?: string;
  readonly timestamp: Date;
  readonly userMessage?: string;
  readonly technicalDetails?: AnyObject;
}

// =============================================================================
// UI STATE TYPES
// =============================================================================

/** Trạng thái của một operation */
export interface OperationState<TData = unknown, TError = AppError> {
  readonly data: Nullable<TData>;
  readonly loading: boolean;
  readonly error: Nullable<TError>;
  readonly lastUpdated: Nullable<Date>;
}

/** Trạng thái form */
export interface FormState<TFormData = AnyObject> {
  readonly data: TFormData;
  readonly isDirty: boolean;
  readonly isValid: boolean;
  readonly isSubmitting: boolean;
  readonly errors: Record<string, string[]>;
  readonly touched: Record<string, boolean>;
}

// =============================================================================
// TABLE & LIST TYPES  
// =============================================================================

/** Cấu hình sorting */
export interface SortConfig {
  readonly field: string;
  readonly direction: 'asc' | 'desc';
}

/** Cấu hình filtering */
export interface FilterConfig {
  readonly field: string;
  readonly value: Primitive;
  readonly operator: 'equals' | 'contains' | 'startsWith' | 'endsWith' | 'greaterThan' | 'lessThan';
}

/** Cấu hình pagination */
export interface PaginationConfig {
  readonly page: number;
  readonly size: number;
  readonly totalItems?: number;
}

/** Query parameters cho list operations */
export interface ListQueryParams {
  readonly search?: string;
  readonly sort?: SortConfig;
  readonly filters?: FilterConfig[];
  readonly pagination?: PaginationConfig;
}

// =============================================================================
// UTILITY TYPES
// =============================================================================

/** Làm tất cả properties thành readonly đệ quy */
export type DeepReadonly<T> = {
  readonly [P in keyof T]: T[P] extends (infer U)[]
    ? DeepReadonlyArray<U>
    : T[P] extends object
    ? DeepReadonly<T[P]>
    : T[P];
};

export interface DeepReadonlyArray<T> extends ReadonlyArray<DeepReadonly<T>> {}

/** Làm tất cả properties thành optional */
export type PartialDeep<T> = {
  [P in keyof T]?: T[P] extends (infer U)[]
    ? PartialDeep<U>[]
    : T[P] extends object
    ? PartialDeep<T[P]>
    : T[P];
};

/** Pick properties theo type */
export type PickByType<T, U> = {
  [K in keyof T as T[K] extends U ? K : never]: T[K];
};

/** Omit properties theo type */
export type OmitByType<T, U> = {
  [K in keyof T as T[K] extends U ? never : K]: T[K];
}; 