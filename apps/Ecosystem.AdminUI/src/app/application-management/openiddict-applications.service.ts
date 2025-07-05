import { Injectable } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';
import { 
  OpenIddictApplicationDto, 
  CreateOpenIddictApplicationDto, 
  UpdateOpenIddictApplicationDto, 
  GetOpenIddictApplicationListDto, 
  OpenIddictApplicationListResultDto 
} from './application.dto';

@Injectable({
  providedIn: 'root'
})
export class OpenIddictApplicationsService {
  // API endpoint cho OpenIddict Applications
  private readonly apiName = 'Default';
  private readonly baseUrl = '/api/openiddict/applications';

  constructor(private restService: RestService) {}

  /**
   * Lấy danh sách applications với phân trang và tìm kiếm
   */
  getList(input: GetOpenIddictApplicationListDto = {}): Observable<OpenIddictApplicationListResultDto> {
    return this.restService.request<void, OpenIddictApplicationListResultDto>({
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
   * Lấy thông tin chi tiết một application theo ID
   */
  get(id: string): Observable<OpenIddictApplicationDto> {
    return this.restService.request<void, OpenIddictApplicationDto>({
      method: 'GET',
      url: `${this.baseUrl}/${id}`
    }, {
      apiName: this.apiName
    });
  }

  /**
   * Tạo mới một application
   */
  create(input: CreateOpenIddictApplicationDto): Observable<OpenIddictApplicationDto> {
    return this.restService.request<CreateOpenIddictApplicationDto, OpenIddictApplicationDto>({
      method: 'POST',
      url: this.baseUrl,
      body: input
    }, {
      apiName: this.apiName
    });
  }

  /**
   * Cập nhật thông tin application
   */
  update(id: string, input: UpdateOpenIddictApplicationDto): Observable<OpenIddictApplicationDto> {
    return this.restService.request<UpdateOpenIddictApplicationDto, OpenIddictApplicationDto>({
      method: 'PUT',
      url: `${this.baseUrl}/${id}`,
      body: input
    }, {
      apiName: this.apiName
    });
  }

  /**
   * Xóa application theo ID
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