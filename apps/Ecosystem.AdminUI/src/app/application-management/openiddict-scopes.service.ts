import { Injectable } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';
import { 
  OpenIddictScopeDto, 
  CreateOpenIddictScopeDto, 
  UpdateOpenIddictScopeDto, 
  GetOpenIddictScopeListDto, 
  OpenIddictScopeListResultDto 
} from './application.dto';

@Injectable({
  providedIn: 'root'
})
export class OpenIddictScopesService {
  // API endpoint cho OpenIddict Scopes
  private readonly apiName = 'Default';
  private readonly baseUrl = '/api/openiddict/scopes';

  constructor(private restService: RestService) {}

  /**
   * Lấy danh sách scopes với phân trang và tìm kiếm
   */
  getList(input: GetOpenIddictScopeListDto = {}): Observable<OpenIddictScopeListResultDto> {
    return this.restService.request<void, OpenIddictScopeListResultDto>({
      method: 'GET',
      url: this.baseUrl,
      params: {
        filter: input.filter,
        skipCount: input.skipCount || 0,
        maxResultCount: input.maxResultCount || 10
      }
    }, {
      apiName: this.apiName
    });
  }

  /**
   * Lấy thông tin chi tiết một scope theo ID
   */
  get(id: string): Observable<OpenIddictScopeDto> {
    return this.restService.request<void, OpenIddictScopeDto>({
      method: 'GET',
      url: `${this.baseUrl}/${id}`
    }, {
      apiName: this.apiName
    });
  }

  /**
   * Tạo mới một scope
   */
  create(input: CreateOpenIddictScopeDto): Observable<OpenIddictScopeDto> {
    return this.restService.request<CreateOpenIddictScopeDto, OpenIddictScopeDto>({
      method: 'POST',
      url: this.baseUrl,
      body: input
    }, {
      apiName: this.apiName
    });
  }

  /**
   * Cập nhật thông tin scope
   */
  update(id: string, input: UpdateOpenIddictScopeDto): Observable<OpenIddictScopeDto> {
    return this.restService.request<UpdateOpenIddictScopeDto, OpenIddictScopeDto>({
      method: 'PUT',
      url: `${this.baseUrl}/${id}`,
      body: input
    }, {
      apiName: this.apiName
    });
  }

  /**
   * Xóa scope theo ID
   */
  delete(id: string): Observable<void> {
    return this.restService.request<void, void>({
      method: 'DELETE',
      url: `${this.baseUrl}/${id}`
    }, {
      apiName: this.apiName
    });
  }
} 