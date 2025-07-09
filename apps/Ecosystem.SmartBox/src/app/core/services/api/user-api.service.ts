import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseApiService } from './base-api.service';
import { 
  SmartBoxUserDto, 
  CreateUpdateSmartBoxUserDto, 
  GetSmartBoxUsersInput, 
  PagedResultDto 
} from '../../../shared/models';

/**
 * User API service cho SmartBox
 */
@Injectable({
  providedIn: 'root'
})
export class UserApiService extends BaseApiService {
  private readonly endpoint = '/smartbox/users';

  /**
   * Lấy danh sách users với phân trang và filter
   */
  getUsers(input: GetSmartBoxUsersInput): Observable<PagedResultDto<SmartBoxUserDto>> {
    const params = this.buildParams(input);
    return this.get<PagedResultDto<SmartBoxUserDto>>(this.endpoint, params);
  }

  /**
   * Lấy thông tin user theo ID
   */
  getUser(id: string): Observable<SmartBoxUserDto> {
    return this.get<SmartBoxUserDto>(`${this.endpoint}/${id}`);
  }

  /**
   * Lấy thông tin user hiện tại
   */
  getCurrentUser(): Observable<SmartBoxUserDto> {
    return this.get<SmartBoxUserDto>(`${this.endpoint}/current`);
  }

  /**
   * Cập nhật thông tin user
   */
  updateUser(id: string, input: CreateUpdateSmartBoxUserDto): Observable<SmartBoxUserDto> {
    return this.put<SmartBoxUserDto>(`${this.endpoint}/${id}`, input);
  }

  /**
   * Cập nhật thông tin user hiện tại
   */
  updateCurrentUser(input: CreateUpdateSmartBoxUserDto): Observable<SmartBoxUserDto> {
    return this.put<SmartBoxUserDto>(`${this.endpoint}/current`, input);
  }

  /**
   * Xóa user (soft delete)
   */
  deleteUser(id: string): Observable<void> {
    return this.delete<void>(`${this.endpoint}/${id}`);
  }

  /**
   * Kích hoạt/Vô hiệu hóa user
   */
  setActive(id: string, isActive: boolean): Observable<SmartBoxUserDto> {
    return this.put<SmartBoxUserDto>(`${this.endpoint}/${id}/set-active`, { isActive });
  }

  /**
   * Gán vai trò cho user
   */
  assignRolesToUser(userId: string, roleIds: string[]): Observable<void> {
    return this.post<void>(`${this.endpoint}/${userId}/assign-roles`, roleIds);
  }

  /**
   * Lấy users theo công ty
   */
  getUsersByCompany(companyId: string, input: GetSmartBoxUsersInput): Observable<PagedResultDto<SmartBoxUserDto>> {
    const params = this.buildParams(input);
    return this.get<PagedResultDto<SmartBoxUserDto>>(`${this.endpoint}/by-company/${companyId}`, params);
  }

  /**
   * Lấy users theo vai trò
   */
  getUsersByRole(roleId: string, input: GetSmartBoxUsersInput): Observable<PagedResultDto<SmartBoxUserDto>> {
    const params = this.buildParams(input);
    return this.get<PagedResultDto<SmartBoxUserDto>>(`${this.endpoint}/by-role/${roleId}`, params);
  }

  /**
   * Kiểm tra tên đăng nhập đã tồn tại
   */
  isUserNameExist(userName: string, excludeId?: string): Observable<boolean> {
    const params = this.buildParams({ userName, excludeId });
    return this.get<boolean>(`${this.endpoint}/check-username`, params);
  }

  /**
   * Kiểm tra email đã tồn tại
   */
  isEmailExist(email: string, excludeId?: string): Observable<boolean> {
    const params = this.buildParams({ email, excludeId });
    return this.get<boolean>(`${this.endpoint}/check-email`, params);
  }

  /**
   * Kiểm tra mã nhân viên đã tồn tại
   */
  isEmployeeCodeExist(employeeCode: string, excludeId?: string): Observable<boolean> {
    const params = this.buildParams({ employeeCode, excludeId });
    return this.get<boolean>(`${this.endpoint}/check-employee-code`, params);
  }

  /**
   * Đồng bộ user hiện tại từ AuthServer
   */
  syncCurrentUser(): Observable<SmartBoxUserDto> {
    return this.post<SmartBoxUserDto>(`${this.endpoint}/sync-current`, {});
  }

  /**
   * Cập nhật avatar cho user hiện tại
   */
  updateAvatar(avatar: string): Observable<SmartBoxUserDto> {
    return this.put<SmartBoxUserDto>(`${this.endpoint}/current/avatar`, avatar);
  }
} 