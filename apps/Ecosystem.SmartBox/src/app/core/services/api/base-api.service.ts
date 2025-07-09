import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { PagedResultDto, ListResultDto } from '../../../shared/models';

/**
 * Base API service với các method chung
 */
@Injectable({
  providedIn: 'root'
})
export class BaseApiService {
  protected readonly baseUrl = environment.apiUrl;

  constructor(protected http: HttpClient) {}

  /**
   * GET request với generic type
   */
  protected get<T>(url: string, params?: HttpParams): Observable<T> {
    return this.http.get<T>(`${this.baseUrl}${url}`, { params })
      .pipe(catchError(this.handleError));
  }

  /**
   * POST request với generic type
   */
  protected post<T>(url: string, body: any): Observable<T> {
    return this.http.post<T>(`${this.baseUrl}${url}`, body)
      .pipe(catchError(this.handleError));
  }

  /**
   * PUT request với generic type
   */
  protected put<T>(url: string, body: any): Observable<T> {
    return this.http.put<T>(`${this.baseUrl}${url}`, body)
      .pipe(catchError(this.handleError));
  }

  /**
   * DELETE request
   */
  protected delete<T>(url: string): Observable<T> {
    return this.http.delete<T>(`${this.baseUrl}${url}`)
      .pipe(catchError(this.handleError));
  }

  /**
   * Build HTTP params từ object
   */
  protected buildParams(obj: any): HttpParams {
    let params = new HttpParams();
    
    Object.keys(obj).forEach(key => {
      const value = obj[key];
      if (value !== null && value !== undefined && value !== '') {
        params = params.set(key, value.toString());
      }
    });
    
    return params;
  }

  /**
   * Error handler chung
   */
  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'Đã xảy ra lỗi không xác định';
    
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = error.error.message;
    } else {
      // Server-side error
      switch (error.status) {
        case 400:
          errorMessage = 'Yêu cầu không hợp lệ';
          break;
        case 401:
          errorMessage = 'Không có quyền truy cập';
          break;
        case 403:
          errorMessage = 'Không đủ quyền thực hiện';
          break;
        case 404:
          errorMessage = 'Không tìm thấy tài nguyên';
          break;
        case 409:
          errorMessage = 'Dữ liệu đã tồn tại';
          break;
        case 500:
          errorMessage = 'Lỗi server nội bộ';
          break;
        default:
          errorMessage = error.error?.message || `Lỗi ${error.status}: ${error.statusText}`;
      }
    }
    
    console.error('API Error:', error);
    return throwError(() => errorMessage);
  }
} 