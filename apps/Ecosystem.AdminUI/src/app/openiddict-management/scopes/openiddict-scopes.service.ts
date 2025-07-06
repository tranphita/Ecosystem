import { Injectable } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';
import {
  ScopeModel,
  CreateScopeModel,
  UpdateScopeModel,
  GetScopeListModel,
  ScopeListResultModel
} from './models/scope.model';

@Injectable({
  providedIn: 'root'
})
export class OpenIddictScopesService {
  private readonly apiName = 'Default';
  private readonly baseUrl = '/api/openiddict/scopes';

  constructor(private restService: RestService) {}

  getList(input: GetScopeListModel = {}): Observable<ScopeListResultModel> {
    return this.restService.request<void, ScopeListResultModel>({
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

  get(id: string): Observable<ScopeModel> {
    return this.restService.request<void, ScopeModel>({
      method: 'GET',
      url: `${this.baseUrl}/${id}`
    }, {
      apiName: this.apiName
    });
  }

  create(input: CreateScopeModel): Observable<ScopeModel> {
    return this.restService.request<CreateScopeModel, ScopeModel>({
      method: 'POST',
      url: this.baseUrl,
      body: input
    }, {
      apiName: this.apiName
    });
  }

  update(id: string, input: UpdateScopeModel): Observable<ScopeModel> {
    return this.restService.request<UpdateScopeModel, ScopeModel>({
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
