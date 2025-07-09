 import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseApiService } from './base-api.service';
import { 
  CompanyDto, 
  CreateUpdateCompanyDto, 
  GetCompaniesInput, 
  PagedResultDto,
  ListResultDto 
} from '../../../shared/models';

/**
 * Company API service cho SmartBox
 */
@Injectable({
  providedIn: 'root'
})
export class CompanyApiService extends BaseApiService {
  private readonly endpoint = '/smartbox/companies';

  /**
   * Lấy danh sách companies với phân trang và filter
   */
  getCompanies(input: GetCompaniesInput): Observable<PagedResultDto<CompanyDto>> {
    const params = this.buildParams(input);
    return this.get<PagedResultDto<CompanyDto>>(this.endpoint, params);
  }

  /**
   * Lấy thông tin company theo ID
   */
  getCompany(id: string): Observable<CompanyDto> {
    return this.get<CompanyDto>(`${this.endpoint}/${id}`);
  }

  /**
   * Tạo company mới
   */
  createCompany(input: CreateUpdateCompanyDto): Observable<CompanyDto> {
    return this.post<CompanyDto>(this.endpoint, input);
  }

  /**
   * Cập nhật thông tin company
   */
  updateCompany(id: string, input: CreateUpdateCompanyDto): Observable<CompanyDto> {
    return this.put<CompanyDto>(`${this.endpoint}/${id}`, input);
  }

  /**
   * Xóa company
   */
  deleteCompany(id: string): Observable<void> {
    return this.delete<void>(`${this.endpoint}/${id}`);
  }

  /**
   * Lấy danh sách companies hoạt động cho dropdown
   */
  getActiveCompanies(): Observable<ListResultDto<CompanyDto>> {
    return this.get<ListResultDto<CompanyDto>>(`${this.endpoint}/active`);
  }

  /**
   * Kiểm tra tên company đã tồn tại
   */
  isNameExist(name: string, excludeId?: string): Observable<boolean> {
    const params = this.buildParams({ name, excludeId });
    return this.get<boolean>(`${this.endpoint}/check-name`, params);
  }

  /**
   * Kiểm tra mã số thuế đã tồn tại
   */
  isTaxCodeExist(taxCode: string, excludeId?: string): Observable<boolean> {
    const params = this.buildParams({ taxCode, excludeId });
    return this.get<boolean>(`${this.endpoint}/check-tax-code`, params);
  }

  /**
   * Kiểm tra email đã tồn tại
   */
  isEmailExist(email: string, excludeId?: string): Observable<boolean> {
    const params = this.buildParams({ email, excludeId });
    return this.get<boolean>(`${this.endpoint}/check-email`, params);
  }
} 