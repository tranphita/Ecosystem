import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseApiService } from './base-api.service';
import { 
  SmartBoxRoleDto, 
  CreateUpdateSmartBoxRoleDto, 
  GetSmartBoxRolesInput, 
  PagedResultDto,
  ListResultDto 
} from '../../../shared/models';

/**
 * Role API service cho SmartBox
 */
@Injectable({
  providedIn: 'root'
})
export class RoleApiService extends BaseApiService {
  private readonly endpoint = '/smartbox/roles';

  /**
   * Lấy danh sách roles với phân trang và filter
   */
  getRoles(input: GetSmartBoxRolesInput): Observable<PagedResultDto<SmartBoxRoleDto>> {
    const params = this.buildParams(input);
    return this.get<PagedResultDto<SmartBoxRoleDto>>(this.endpoint, params);
  }

  /**
   * Lấy thông tin role theo ID
   */
  getRole(id: string): Observable<SmartBoxRoleDto> {
    return this.get<SmartBoxRoleDto>(`${this.endpoint}/${id}`);
  }

  /**
   * Tạo role mới
   */
  createRole(input: CreateUpdateSmartBoxRoleDto): Observable<SmartBoxRoleDto> {
    return this.post<SmartBoxRoleDto>(this.endpoint, input);
  }

  /**
   * Cập nhật thông tin role
   */
  updateRole(id: string, input: CreateUpdateSmartBoxRoleDto): Observable<SmartBoxRoleDto> {
    return this.put<SmartBoxRoleDto>(`${this.endpoint}/${id}`, input);
  }

  /**
   * Xóa role
   */
  deleteRole(id: string): Observable<void> {
    return this.delete<void>(`${this.endpoint}/${id}`);
  }

  /**
   * Lấy danh sách roles hoạt động cho dropdown
   */
  getActiveRoles(): Observable<ListResultDto<SmartBoxRoleDto>> {
    return this.get<ListResultDto<SmartBoxRoleDto>>(`${this.endpoint}/active`);
  }

  /**
   * Lấy roles của user
   */
  getRolesByUserId(userId: string): Observable<ListResultDto<SmartBoxRoleDto>> {
    return this.get<ListResultDto<SmartBoxRoleDto>>(`${this.endpoint}/by-user/${userId}`);
  }

  /**
   * Kiểm tra tên role đã tồn tại
   */
  isNameExist(name: string, excludeId?: string): Observable<boolean> {
    const params = this.buildParams({ name, excludeId });
    return this.get<boolean>(`${this.endpoint}/check-name`, params);
  }

  /**
   * Kiểm tra tên hiển thị đã tồn tại
   */
  isDisplayNameExist(displayName: string, excludeId?: string): Observable<boolean> {
    const params = this.buildParams({ displayName, excludeId });
    return this.get<boolean>(`${this.endpoint}/check-display-name`, params);
  }
} 