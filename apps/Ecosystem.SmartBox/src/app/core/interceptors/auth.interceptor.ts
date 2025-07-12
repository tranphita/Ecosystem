/**
 * Auth Interceptor
 * Tự động thêm JWT token vào Authorization header cho tất cả API requests
 */

import { Injectable, inject } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { switchMap, take } from 'rxjs/operators';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { environment } from '../../../environments/environment';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private readonly oidcSecurityService = inject(OidcSecurityService);

  /**
   * Intercept HTTP requests và thêm Authorization token
   */
  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // Chỉ thêm token cho các API requests (không phải authentication endpoints)
    if (!this.shouldAddToken(request.url)) {
      return next.handle(request);
    }

    // Lấy access token từ OIDC service và xử lý async
    return this.oidcSecurityService.getAccessToken().pipe(
      take(1),
      switchMap((token: string) => {
        if (token) {
          // Clone request và thêm Authorization header
          const authenticatedRequest = request.clone({
            setHeaders: {
              Authorization: `Bearer ${token}`
            }
          });

          return next.handle(authenticatedRequest);
        }

        return next.handle(request);
      })
    );
  }

  /**
   * Kiểm tra xem có nên thêm token vào request này không
   */
  private shouldAddToken(url: string): boolean {
    // Không thêm token cho authentication endpoints
    const authUrls = [
      '/connect/token',
      '/connect/authorize',
      '/connect/userinfo',
      '/oauth',
      '/auth',
      environment.authUrl
    ];

    // Không thêm token cho external URLs
    if (url.startsWith('http') && !url.startsWith(environment.apiUrl)) {
      return false;
    }

    // Không thêm token cho auth endpoints
    if (authUrls.some(authUrl => url.includes(authUrl))) {
      return false;
    }

    // Thêm token cho tất cả API requests khác
    return url.includes(environment.apiUrl) || url.startsWith('/api');
  }
} 