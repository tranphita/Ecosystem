/**
 * Enhanced Employee Service Example
 * Demonstrates proper typing, error handling, và best practices
 */

import { Injectable, inject } from '@angular/core';
import { Observable, of, combineLatest } from 'rxjs';
import { map, catchError, tap, switchMap } from 'rxjs/operators';

import { ApiBaseService } from '../services/api-base.service';
import { ErrorHandlerService } from '../errors/error-handler.service';
import { 
  Employee, 
  CreateEmployeeDto, 
  UpdateEmployeeDto,
  EmployeeQueryParams,
  EmployeeResponse,
  EmployeeListResponse,
  CreateEmployeeResponse,
  UpdateEmployeeResponse,
  DeleteEmployeeResponse,
  EmployeeServiceResult,
  isEmployee,
  isEmployeeArray
} from '../types/employee.types';
import { 
  BaseApiResponse, 
  PaginatedResponse,
  EntityId,
  OperationState 
} from '../types/common.types';

@Injectable({
  providedIn: 'root'
})
export class EnhancedEmployeeService {
  private readonly apiService = inject(ApiBaseService);
  private readonly errorHandler = inject(ErrorHandlerService);

  private readonly baseUrl = 'employees';

  // =============================================================================
  // CRUD OPERATIONS với comprehensive typing
  // =============================================================================

  /**
   * Lấy danh sách employees với pagination và filtering
   */
  getEmployees(params?: EmployeeQueryParams): Observable<EmployeeListResponse> {
    return this.apiService.getList<Employee>(this.baseUrl, params, {
      loadingLabel: 'Đang tải danh sách nhân viên...'
    }).pipe(
      map(response => this.validateEmployeeListResponse(response)),
      catchError(error => this.handleServiceError('Lỗi khi tải danh sách nhân viên', error))
    );
  }

  /**
   * Lấy thông tin employee theo ID
   */
  getEmployeeById(id: EntityId): Observable<EmployeeResponse> {
    return this.apiService.getById<Employee>(this.baseUrl, id, {
      loadingLabel: 'Đang tải thông tin nhân viên...'
    }).pipe(
      map(response => this.validateEmployeeResponse(response)),
      catchError(error => this.handleServiceError(`Lỗi khi tải thông tin nhân viên ID ${id}`, error))
    );
  }

  /**
   * Tạo employee mới với comprehensive validation
   */
  createEmployee(employeeData: CreateEmployeeDto): Observable<CreateEmployeeResponse> {
    // Client-side validation trước khi gửi request
    const validationResult = this.validateCreateEmployeeData(employeeData);
    if (!validationResult.isValid) {
      return this.returnValidationError(validationResult.errors!);
    }

    return this.apiService.post<Employee, CreateEmployeeDto>(this.baseUrl, employeeData, {
      loadingLabel: 'Đang tạo nhân viên mới...'
    }).pipe(
      map(response => this.validateEmployeeResponse(response)),
      tap(response => this.logSuccessfulOperation('Tạo nhân viên thành công', response.data)),
      catchError(error => this.handleServiceError('Lỗi khi tạo nhân viên', error))
    );
  }

  /**
   * Cập nhật employee với partial updates
   */
  updateEmployee(id: EntityId, updateData: UpdateEmployeeDto): Observable<UpdateEmployeeResponse> {
    // Ensure ID is included in update data
    const completeUpdateData: UpdateEmployeeDto = { ...updateData, id };

    const validationResult = this.validateUpdateEmployeeData(completeUpdateData);
    if (!validationResult.isValid) {
      return this.returnValidationError(validationResult.errors!);
    }

    return this.apiService.put<Employee, UpdateEmployeeDto>(`${this.baseUrl}/${id}`, completeUpdateData, {
      loadingLabel: 'Đang cập nhật nhân viên...'
    }).pipe(
      map(response => this.validateEmployeeResponse(response)),
      tap(response => this.logSuccessfulOperation('Cập nhật nhân viên thành công', response.data)),
      catchError(error => this.handleServiceError(`Lỗi khi cập nhật nhân viên ID ${id}`, error))
    );
  }

  /**
   * Xóa employee với confirmation
   */
  deleteEmployee(id: EntityId): Observable<DeleteEmployeeResponse> {
    return this.apiService.delete<{ id: EntityId }>(`${this.baseUrl}/${id}`, {
      loadingLabel: 'Đang xóa nhân viên...'
    }).pipe(
      tap(() => this.logSuccessfulOperation('Xóa nhân viên thành công', { id })),
      catchError(error => this.handleServiceError(`Lỗi khi xóa nhân viên ID ${id}`, error))
    );
  }

  // =============================================================================
  // BATCH OPERATIONS
  // =============================================================================

  /**
   * Batch create multiple employees
   */
  createMultipleEmployees(employees: CreateEmployeeDto[]): Observable<BaseApiResponse<Employee[]>> {
    // Validate all employees before sending
    const validationResults = employees.map(emp => this.validateCreateEmployeeData(emp));
    const invalidEmployees = validationResults.filter(result => !result.isValid);

    if (invalidEmployees.length > 0) {
      const combinedErrors = invalidEmployees.reduce((acc, result) => 
        ({ ...acc, ...result.errors }), {});
      return this.returnValidationError(combinedErrors);
    }

    return this.apiService.batchCreate<Employee, CreateEmployeeDto>(this.baseUrl, employees, {
      loadingLabel: `Đang tạo ${employees.length} nhân viên...`
    }).pipe(
      tap(response => this.logSuccessfulOperation('Tạo nhiều nhân viên thành công', response.data)),
      catchError(error => this.handleServiceError('Lỗi khi tạo nhiều nhân viên', error))
    );
  }

  /**
   * Batch delete multiple employees
   */
  deleteMultipleEmployees(ids: EntityId[]): Observable<BaseApiResponse<{ deletedIds: EntityId[] }>> {
    if (ids.length === 0) {
      return this.returnValidationError({ ids: ['Danh sách ID không được trống'] });
    }

    return this.apiService.batchDelete(this.baseUrl, ids, {
      loadingLabel: `Đang xóa ${ids.length} nhân viên...`
    }).pipe(
      tap(response => this.logSuccessfulOperation('Xóa nhiều nhân viên thành công', response.data)),
      catchError(error => this.handleServiceError('Lỗi khi xóa nhiều nhân viên', error))
    );
  }

  // =============================================================================
  // SPECIALIZED METHODS
  // =============================================================================

  /**
   * Search employees với advanced criteria
   */
  searchEmployees(searchTerm: string, filters?: Partial<EmployeeQueryParams>): Observable<Employee[]> {
    const queryParams: EmployeeQueryParams = {
      search: searchTerm,
      ...filters,
      pagination: { page: 0, size: 50 } // Limit search results
    };

    return this.getEmployees(queryParams).pipe(
      map(response => response.data),
      catchError(error => {
        this.errorHandler.handleBusinessError('Lỗi khi tìm kiếm nhân viên', 'EMPLOYEE_SEARCH_ERROR');
        return of([]);
      })
    );
  }

  /**
   * Get employee statistics
   */
  getEmployeeStatistics(): Observable<any> {
    return this.apiService.get<any>(`${this.baseUrl}/statistics`, {
      loadingLabel: 'Đang tải thống kê nhân viên...'
    }).pipe(
      map(response => response.data),
      catchError(error => this.handleServiceError('Lỗi khi tải thống kê nhân viên', error))
    );
  }

  /**
   * Export employees to file
   */
  exportEmployees(format: 'excel' | 'pdf' = 'excel'): Observable<Blob> {
    return this.apiService.downloadFile(`${this.baseUrl}/export?format=${format}`, `employees.${format}`, {
      loadingLabel: 'Đang xuất dữ liệu nhân viên...'
    }).pipe(
      catchError(error => this.handleServiceError('Lỗi khi xuất dữ liệu nhân viên', error))
    );
  }

  // =============================================================================
  // VALIDATION METHODS
  // =============================================================================

  private validateCreateEmployeeData(data: CreateEmployeeDto): { isValid: boolean; errors?: Record<string, string[]> } {
    const errors: Record<string, string[]> = {};

    // Required field validation
    if (!data['name']?.trim()) {
      errors['name'] = ['Tên nhân viên là bắt buộc'];
    } else if (data['name'].length < 2) {
      errors['name'] = ['Tên nhân viên phải có ít nhất 2 ký tự'];
    }

    if (!data['email']?.trim()) {
      errors['email'] = ['Email là bắt buộc'];
    } else if (!this.isValidEmail(data['email'])) {
      errors['email'] = ['Email không hợp lệ'];
    }

    if (!data['position']?.trim()) {
      errors['position'] = ['Vị trí công việc là bắt buộc'];
    }

    if (!data['mobile']?.trim()) {
      errors['mobile'] = ['Số điện thoại là bắt buộc'];
    } else if (!this.isValidVietnamesePhone(data['mobile'])) {
      errors['mobile'] = ['Số điện thoại không hợp lệ'];
    }

    // Business logic validation
    if (data['salary'] !== undefined && data['salary'] < 0) {
      errors['salary'] = ['Lương không thể âm'];
    }

    if (data['projects'] !== undefined && data['projects'] < 0) {
      errors['projects'] = ['Số dự án không thể âm'];
    }

    return {
      isValid: Object.keys(errors).length === 0,
      errors: Object.keys(errors).length > 0 ? errors : undefined
    };
  }

  private validateUpdateEmployeeData(data: UpdateEmployeeDto): { isValid: boolean; errors?: Record<string, string[]> } {
    const errors: Record<string, string[]> = {};

    // ID is required for updates
    if (!data['id']) {
      errors['id'] = ['ID nhân viên là bắt buộc cho việc cập nhật'];
    }

    // Optional field validation (only validate if provided)
    if (data['name'] !== undefined && !data['name']?.trim()) {
      errors['name'] = ['Tên nhân viên không được trống'];
    }

    if (data['email'] !== undefined && data['email'] && !this.isValidEmail(data['email'])) {
      errors['email'] = ['Email không hợp lệ'];
    }

    if (data['mobile'] !== undefined && data['mobile'] && !this.isValidVietnamesePhone(data['mobile'])) {
      errors['mobile'] = ['Số điện thoại không hợp lệ'];
    }

    if (data['salary'] !== undefined && data['salary'] < 0) {
      errors['salary'] = ['Lương không thể âm'];
    }

    return {
      isValid: Object.keys(errors).length === 0,
      errors: Object.keys(errors).length > 0 ? errors : undefined
    };
  }

  // =============================================================================
  // RESPONSE VALIDATION
  // =============================================================================

  private validateEmployeeResponse(response: BaseApiResponse<Employee>): EmployeeResponse {
    if (!response.success || !response.data) {
      throw new Error('Invalid employee response format');
    }

    if (!isEmployee(response.data)) {
      throw new Error('Response data is not a valid Employee object');
    }

    return response as EmployeeResponse;
  }

  private validateEmployeeListResponse(response: BaseApiResponse<Employee[]>): EmployeeListResponse {
    if (!response.success || !Array.isArray(response.data)) {
      throw new Error('Invalid employee list response format');
    }

    if (!isEmployeeArray(response.data)) {
      throw new Error('Response data contains invalid Employee objects');
    }

    return response as EmployeeListResponse;
  }

  // =============================================================================
  // HELPER METHODS
  // =============================================================================

  private isValidEmail(email: string): boolean {
    const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    return emailRegex.test(email);
  }

  private isValidVietnamesePhone(phone: string): boolean {
    const phoneRegex = /^(0|\+84)(3|5|7|8|9)[0-9]{8}$/;
    return phoneRegex.test(phone);
  }

  private handleServiceError(userMessage: string, error: unknown): Observable<never> {
    this.errorHandler.handleBusinessError(userMessage, 'EMPLOYEE_SERVICE_ERROR');
    throw error;
  }

  private returnValidationError(errors: Record<string, string[]>): Observable<never> {
    this.errorHandler.handleValidationError(errors);
    throw new Error('Validation failed');
  }

  private logSuccessfulOperation(message: string, data?: unknown): void {
    console.log(`✅ ${message}:`, data);
  }
} 