import { Injectable } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';
import {
  ApplicationModel,
  CreateApplicationModel,
  UpdateApplicationModel,
  GetApplicationListModel,
  ApplicationListResultModel
} from './models/application.model';

@Injectable({
  providedIn: 'root'
})
export class OpenIddictApplicationsService {
  private readonly apiName = 'Default';
  private readonly baseUrl = '/api/openiddict/applications';

  constructor(private restService: RestService) {}

  getList(input: GetApplicationListModel = {}): Observable<ApplicationListResultModel> {
    return this.restService.request<void, ApplicationListResultModel>({
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

  get(id: string): Observable<ApplicationModel> {
    return this.restService.request<void, ApplicationModel>({
      method: 'GET',
      url: `${this.baseUrl}/${id}`
    }, {
      apiName: this.apiName
    });
  }

  create(input: CreateApplicationModel): Observable<ApplicationModel> {
    return this.restService.request<CreateApplicationModel, ApplicationModel>({
      method: 'POST',
      url: this.baseUrl,
      body: input
    }, {
      apiName: this.apiName
    });
  }

  update(id: string, input: UpdateApplicationModel): Observable<ApplicationModel> {
    return this.restService.request<UpdateApplicationModel, ApplicationModel>({
      method: 'PUT',
      url: `${this.baseUrl}/${id}`,
      body: input
    }, {
      apiName: this.apiName
    });
  }

  delete(id: string): Observable<void> {
    return this.restService.request<void, void>({
      method: 'DELETE',
      url: `${this.baseUrl}/${id}`
    }, {
      apiName: this.apiName
    });
  }
}
