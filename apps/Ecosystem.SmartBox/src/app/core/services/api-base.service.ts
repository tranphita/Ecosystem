/**
 * Base API Service
 * Cung cấp foundation cho tất cả API services với strong typing và error handling
 */

import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, of } from 'rxjs';
import { map, catchError, retry, timeout, finalize } from 'rxjs/operators';

import { 
  BaseApiResponse, 
  PaginatedResponse, 
  ApiErrorResponse,
  ListQueryParams,
  EntityId 
} from '../types/common.types';
import { ErrorHandlerService } from '../errors/error-handler.service';
import { LoadingService } from './loading.service';

/**
 * HTTP request options với typing
 */
export interface ApiRequestOptions {
  readonly headers?: Record<string, string>;
  readonly params?: Record<string, string | number | boolean>;
  readonly timeout?: number;
  readonly retryCount?: number;
  readonly showLoading?: boolean;
  readonly loadingLabel?: string;
}

/**
 * API endpoint configuration
 */
export interface ApiEndpoint {
  readonly baseUrl: string;
  readonly version: string;
  readonly timeout: number;
  readonly retryCount: number;
}

/**
 * Upload progress information
 */
export interface UploadProgress {
  readonly loaded: number;
  readonly total: number;
  readonly percentage: number;
}

@Injectable({
  providedIn: 'root'
})
export class ApiBaseService {
  private readonly http = inject(HttpClient);
  private readonly errorHandler = inject(ErrorHandlerService);
  private readonly loadingService = inject(LoadingService);

  // Default configuration
  private readonly defaultEndpoint: ApiEndpoint = {
    baseUrl: '/api',
    version: 'v1',
    timeout: 30000, // 30 seconds
    retryCount: 2
  };

  // Request ID counter for tracking
  private requestIdCounter = 0;

  // =============================================================================
  // PUBLIC HTTP METHODS
  // =============================================================================

  /**
   * GET request với type safety
   */
  get<TResponse>(
    url: string, 
    options: ApiRequestOptions = {}
  ): Observable<BaseApiResponse<TResponse>> {
    return this.executeRequest<TResponse>('GET', url, undefined, options);
  }

  /**
   * POST request với type safety
   */
  post<TResponse, TRequest = unknown>(
    url: string, 
    data: TRequest, 
    options: ApiRequestOptions = {}
  ): Observable<BaseApiResponse<TResponse>> {
    return this.executeRequest<TResponse>('POST', url, data, options);
  }

  /**
   * PUT request với type safety
   */
  put<TResponse, TRequest = unknown>(
    url: string, 
    data: TRequest, 
    options: ApiRequestOptions = {}
  ): Observable<BaseApiResponse<TResponse>> {
    return this.executeRequest<TResponse>('PUT', url, data, options);
  }

  /**
   * PATCH request với type safety
   */
  patch<TResponse, TRequest = unknown>(
    url: string, 
    data: TRequest, 
    options: ApiRequestOptions = {}
  ): Observable<BaseApiResponse<TResponse>> {
    return this.executeRequest<TResponse>('PATCH', url, data, options);
  }

  /**
   * DELETE request với type safety
   */
  delete<TResponse = { id: EntityId }>(
    url: string, 
    options: ApiRequestOptions = {}
  ): Observable<BaseApiResponse<TResponse>> {
    return this.executeRequest<TResponse>('DELETE', url, undefined, options);
  }

  // =============================================================================
  // SPECIALIZED METHODS
  // =============================================================================

  /**
   * GET paginated list với type safety
   */
  getList<TData>(
    url: string, 
    queryParams?: ListQueryParams,
    options: ApiRequestOptions = {}
  ): Observable<PaginatedResponse<TData>> {
    const params = this.buildQueryParams(queryParams);
    const enhancedOptions = { ...options, params };

    return this.executeRequest<TData[]>('GET', url, undefined, enhancedOptions)
      .pipe(
        map(response => response as PaginatedResponse<TData>)
      );
  }

  /**
   * GET single item by ID với type safety
   */
  getById<TData>(
    url: string, 
    id: EntityId, 
    options: ApiRequestOptions = {}
  ): Observable<BaseApiResponse<TData>> {
    const fullUrl = `${url}/${id}`;
    return this.executeRequest<TData>('GET', fullUrl, undefined, options);
  }

  /**
   * Upload file với progress tracking
   */
  uploadFile<TResponse>(
    url: string,
    file: File,
    additionalData?: Record<string, unknown>,
    options: ApiRequestOptions = {}
  ): Observable<BaseApiResponse<TResponse> | UploadProgress> {
    const requestId = this.generateRequestId();
    
    if (options.showLoading !== false) {
      this.loadingService.startLoading(
        `upload_${requestId}`, 
        options.loadingLabel || 'Đang tải file lên...'
      );
    }

    const formData = new FormData();
    formData.append('file', file);
    
    if (additionalData) {
      Object.entries(additionalData).forEach(([key, value]) => {
        formData.append(key, String(value));
      });
    }

    const httpOptions = {
      headers: this.buildHeaders(options.headers),
      reportProgress: true,
      observe: 'events' as const
    };

    return this.http.post<BaseApiResponse<TResponse>>(
      this.buildFullUrl(url), 
      formData, 
      httpOptions
    ).pipe(
      map(event => {
        if (event.type === 1 && event.total) { // UploadProgress event
          const progress: UploadProgress = {
            loaded: event.loaded,
            total: event.total,
            percentage: Math.round((event.loaded / event.total) * 100)
          };
          return progress;
        } else if (event.type === 4) { // Response event
          return event.body!;
        }
        throw new Error('Unknown event type');
      }),
      catchError(error => this.handleError(error, requestId)),
      finalize(() => {
        if (options.showLoading !== false) {
          this.loadingService.stopLoading(`upload_${requestId}`);
        }
      })
    );
  }

  /**
   * Download file
   */
  downloadFile(
    url: string,
    filename?: string,
    options: ApiRequestOptions = {}
  ): Observable<Blob> {
    const requestId = this.generateRequestId();
    
    if (options.showLoading !== false) {
      this.loadingService.startLoading(
        `download_${requestId}`, 
        options.loadingLabel || 'Đang tải file xuống...'
      );
    }

    const httpOptions = {
      headers: this.buildHeaders(options.headers),
      responseType: 'blob' as const,
      observe: 'body' as const
    };

    return this.http.get(this.buildFullUrl(url), httpOptions).pipe(
      map(blob => {
        if (filename) {
          this.saveFile(blob, filename);
        }
        return blob;
      }),
      catchError(error => this.handleError(error, requestId)),
      finalize(() => {
        if (options.showLoading !== false) {
          this.loadingService.stopLoading(`download_${requestId}`);
        }
      })
    );
  }

  // =============================================================================
  // BATCH OPERATIONS
  // =============================================================================

  /**
   * Batch create multiple items
   */
  batchCreate<TResponse, TRequest>(
    url: string,
    items: TRequest[],
    options: ApiRequestOptions = {}
  ): Observable<BaseApiResponse<TResponse[]>> {
    const batchData = { items };
    return this.post<TResponse[], typeof batchData>(`${url}/batch`, batchData, options);
  }

  /**
   * Batch update multiple items
   */
  batchUpdate<TResponse, TRequest>(
    url: string,
    items: TRequest[],
    options: ApiRequestOptions = {}
  ): Observable<BaseApiResponse<TResponse[]>> {
    const batchData = { items };
    return this.put<TResponse[], typeof batchData>(`${url}/batch`, batchData, options);
  }

  /**
   * Batch delete multiple items
   */
  batchDelete(
    url: string,
    ids: EntityId[],
    options: ApiRequestOptions = {}
  ): Observable<BaseApiResponse<{ deletedIds: EntityId[] }>> {
    const batchData = { ids };
    return this.post<{ deletedIds: EntityId[] }, typeof batchData>(`${url}/batch/delete`, batchData, options);
  }

  // =============================================================================
  // PRIVATE HELPER METHODS
  // =============================================================================

  private executeRequest<TResponse>(
    method: 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE',
    url: string,
    data?: unknown,
    options: ApiRequestOptions = {}
  ): Observable<BaseApiResponse<TResponse>> {
    const requestId = this.generateRequestId();
    const fullUrl = this.buildFullUrl(url);
    
    // Start loading indicator
    if (options.showLoading !== false) {
      this.loadingService.startLoading(
        `request_${requestId}`, 
        options.loadingLabel || 'Đang xử lý...',
        options.timeout
      );
    }

    const httpOptions = {
      headers: this.buildHeaders(options.headers),
      params: this.buildHttpParams(options.params)
    };

    let request$: Observable<BaseApiResponse<TResponse>>;

    switch (method) {
      case 'GET':
        request$ = this.http.get<BaseApiResponse<TResponse>>(fullUrl, httpOptions);
        break;
      case 'POST':
        request$ = this.http.post<BaseApiResponse<TResponse>>(fullUrl, data, httpOptions);
        break;
      case 'PUT':
        request$ = this.http.put<BaseApiResponse<TResponse>>(fullUrl, data, httpOptions);
        break;
      case 'PATCH':
        request$ = this.http.patch<BaseApiResponse<TResponse>>(fullUrl, data, httpOptions);
        break;
      case 'DELETE':
        request$ = this.http.delete<BaseApiResponse<TResponse>>(fullUrl, httpOptions);
        break;
      default:
        throw new Error(`Unsupported HTTP method: ${method}`);
    }

    return request$.pipe(
      timeout(options.timeout || this.defaultEndpoint.timeout),
      retry(options.retryCount || this.defaultEndpoint.retryCount),
      map(response => this.validateResponse(response)),
      catchError(error => this.handleError(error, requestId)),
      finalize(() => {
        if (options.showLoading !== false) {
          this.loadingService.stopLoading(`request_${requestId}`);
        }
      })
    );
  }

  private buildFullUrl(endpoint: string): string {
    const { baseUrl, version } = this.defaultEndpoint;
    const cleanEndpoint = endpoint.startsWith('/') ? endpoint.slice(1) : endpoint;
    return `${baseUrl}/${version}/${cleanEndpoint}`;
  }

  private buildHeaders(customHeaders?: Record<string, string>): HttpHeaders {
    let headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/json',
      'X-Requested-With': 'XMLHttpRequest'
    });

    if (customHeaders) {
      Object.entries(customHeaders).forEach(([key, value]) => {
        headers = headers.set(key, value);
      });
    }

    return headers;
  }

  private buildHttpParams(params?: Record<string, string | number | boolean>): HttpParams {
    let httpParams = new HttpParams();

    if (params) {
      Object.entries(params).forEach(([key, value]) => {
        if (value !== null && value !== undefined) {
          httpParams = httpParams.set(key, String(value));
        }
      });
    }

    return httpParams;
  }

  private buildQueryParams(queryParams?: ListQueryParams): Record<string, string | number | boolean> {
    const params: Record<string, string | number | boolean> = {};

    if (queryParams?.search) {
      params['search'] = queryParams.search;
    }

    if (queryParams?.sort) {
      params['sortField'] = queryParams.sort.field;
      params['sortDirection'] = queryParams.sort.direction;
    }

    if (queryParams?.pagination) {
      params['page'] = queryParams.pagination.page;
      params['size'] = queryParams.pagination.size;
    }

    if (queryParams?.filters) {
      queryParams.filters.forEach((filter, index) => {
        params[`filter[${index}].field`] = filter.field;
        params[`filter[${index}].value`] = String(filter.value);
        params[`filter[${index}].operator`] = filter.operator;
      });
    }

    return params;
  }

  private validateResponse<T>(response: BaseApiResponse<T>): BaseApiResponse<T> {
    if (!response || typeof response !== 'object') {
      throw new Error('Invalid response format');
    }

    if (!response.success) {
      const errorResponse = response as unknown as ApiErrorResponse;
      throw new HttpErrorResponse({
        error: errorResponse,
        status: 400,
        statusText: 'Validation Error'
      });
    }

    return response;
  }

  private handleError(error: unknown, requestId: string): Observable<never> {
    console.error(`API request ${requestId} failed:`, error);
    
    // Let the error interceptor handle the error
    return throwError(() => error);
  }

  private saveFile(blob: Blob, filename: string): void {
    try {
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = filename;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (error) {
      this.errorHandler.handleBusinessError('Lỗi khi lưu file', 'FILE_SAVE_ERROR');
    }
  }

  private generateRequestId(): string {
    return `api_${Date.now()}_${++this.requestIdCounter}`;
  }
} 