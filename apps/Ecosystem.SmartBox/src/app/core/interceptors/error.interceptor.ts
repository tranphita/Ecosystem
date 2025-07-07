/**
 * HTTP Error Interceptor
 * Tự động xử lý errors từ HTTP requests và integrate với ErrorHandlerService
 */

import { Injectable, inject } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpErrorResponse,
  HttpEventType
} from '@angular/common/http';
import { Observable, throwError, of, timer } from 'rxjs';
import { catchError, retry, delayWhen, tap } from 'rxjs/operators';

import { ErrorHandlerService, ErrorContext } from '../errors/error-handler.service';
import { LoadingService } from '../services/loading.service';

/**
 * Retry configuration cho different error types
 */
interface RetryConfig {
  readonly maxRetries: number;
  readonly retryDelay: number;
  readonly retryCondition: (error: HttpErrorResponse) => boolean;
}

/**
 * Request context để track request information
 */
interface RequestContext {
  readonly url: string;
  readonly method: string;
  readonly startTime: number;
  readonly retryCount: number;
}

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  private readonly errorHandler = inject(ErrorHandlerService);
  private readonly loadingService = inject(LoadingService);

  // Request tracking
  private readonly activeRequests = new Map<string, RequestContext>();

  // Retry configuration cho các loại error khác nhau
  private readonly retryConfigs: Record<number, RetryConfig> = {
    // Network errors - retry with exponential backoff
    0: {
      maxRetries: 3,
      retryDelay: 1000,
      retryCondition: () => true
    },
    // Server errors - retry a few times
    500: {
      maxRetries: 2,
      retryDelay: 2000,
      retryCondition: () => true
    },
    502: {
      maxRetries: 2,
      retryDelay: 2000,
      retryCondition: () => true
    },
    503: {
      maxRetries: 2,
      retryDelay: 3000,
      retryCondition: () => true
    },
    // Timeout errors
    408: {
      maxRetries: 2,
      retryDelay: 1500,
      retryCondition: () => true
    }
  };

  /**
   * Intercept HTTP requests và xử lý errors
   */
  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const requestId = this.generateRequestId();
    const context: RequestContext = {
      url: request.url,
      method: request.method,
      startTime: Date.now(),
      retryCount: 0
    };

    // Track active request
    this.activeRequests.set(requestId, context);

    // Add request headers if needed
    const enhancedRequest = this.enhanceRequest(request, requestId);

    return next.handle(enhancedRequest).pipe(
      // Tap to track request progress
      tap(event => {
        if (event.type === HttpEventType.Response) {
          this.handleSuccessfulResponse(requestId, context, event);
        }
      }),
      
      // Retry logic với exponential backoff
      retry({
        count: this.getMaxRetries(0), // Default retry for network errors
        delay: (error: HttpErrorResponse, retryCount: number) => {
          const retryConfig = this.getRetryConfig(error.status);
          
          if (!retryConfig || !retryConfig.retryCondition(error) || retryCount >= retryConfig.maxRetries) {
            throw error;
          }

          const delay = retryConfig.retryDelay * Math.pow(2, retryCount - 1); // Exponential backoff
          
          console.log(`🔄 Retrying request ${requestId} (attempt ${retryCount}/${retryConfig.maxRetries}) after ${delay}ms`);
          
          // Update retry count in context
          const updatedContext = { ...context, retryCount };
          this.activeRequests.set(requestId, updatedContext);
          
          return timer(delay);
        }
      }),

      // Error handling
      catchError((error: HttpErrorResponse) => {
        return this.handleError(error, requestId, context);
      }),

      // Cleanup trong mọi trường hợp
      tap({
        complete: () => this.cleanupRequest(requestId),
        error: () => this.cleanupRequest(requestId)
      })
    );
  }

  // =============================================================================
  // PRIVATE METHODS
  // =============================================================================

  private enhanceRequest(request: HttpRequest<unknown>, requestId: string): HttpRequest<unknown> {
    return request.clone({
      setHeaders: {
        'X-Request-ID': requestId,
        'X-Timestamp': new Date().toISOString(),
        // Add other common headers như correlation ID, user ID, etc.
      }
    });
  }

  private handleSuccessfulResponse(
    requestId: string, 
    context: RequestContext, 
    response: any
  ): void {
    const duration = Date.now() - context.startTime;
    
    console.log(`✅ Request ${requestId} completed successfully in ${duration}ms`);
    
    // Optional: Log performance metrics
    if (duration > 5000) { // Warn về slow requests
      console.warn(`⚠️ Slow request detected: ${context.method} ${context.url} took ${duration}ms`);
    }
  }

  private handleError(
    error: HttpErrorResponse,
    requestId: string,
    context: RequestContext
  ): Observable<never> {
    const duration = Date.now() - context.startTime;
    
    console.error(`❌ Request ${requestId} failed after ${duration}ms:`, error);

    // Build error context với request information
    const errorContext: Partial<ErrorContext> = {
      url: context.url,
      additionalData: {
        requestId,
        method: context.method,
        duration,
        retryCount: context.retryCount,
        requestHeaders: this.extractRequestHeaders(error.url),
        responseHeaders: error.headers ? this.extractResponseHeaders(error.headers) : {}
      }
    };

    // Xử lý specific error cases
    if (this.isAuthenticationError(error)) {
      this.handleAuthenticationError(error, errorContext);
    } else if (this.isNetworkError(error)) {
      this.handleNetworkError(error, errorContext);
    } else if (this.isServerError(error)) {
      this.handleServerError(error, errorContext);
    } else if (this.isClientError(error)) {
      this.handleClientError(error, errorContext);
    }

    // Process error through error handler service
    const appError = this.errorHandler.handleHttpError(error, errorContext);

    // Return error observable với enhanced error information
    return throwError(() => ({
      ...error,
      appError,
      requestId,
      context: errorContext
    }));
  }

  private handleAuthenticationError(
    error: HttpErrorResponse, 
    context: Partial<ErrorContext>
  ): void {
    console.log('🔐 Authentication error detected, clearing session...');
    // Clear authentication state, redirect to login, etc.
    // This could be handled by AuthService
  }

  private handleNetworkError(
    error: HttpErrorResponse, 
    context: Partial<ErrorContext>
  ): void {
    console.log('🌐 Network error detected, checking connectivity...');
    // Could implement network connectivity checking here
  }

  private handleServerError(
    error: HttpErrorResponse, 
    context: Partial<ErrorContext>
  ): void {
    console.log('🖥️ Server error detected, may need retry logic...');
    // Could implement circuit breaker pattern here
  }

  private handleClientError(
    error: HttpErrorResponse, 
    context: Partial<ErrorContext>
  ): void {
    console.log('📱 Client error detected, checking request validity...');
    // Could implement request validation or fallback logic
  }

  private getRetryConfig(status: number): RetryConfig | null {
    return this.retryConfigs[status] || null;
  }

  private getMaxRetries(status: number): number {
    const config = this.getRetryConfig(status);
    return config?.maxRetries || 0;
  }

  private isAuthenticationError(error: HttpErrorResponse): boolean {
    return error.status === 401;
  }

  private isNetworkError(error: HttpErrorResponse): boolean {
    return error.status === 0 || !navigator.onLine;
  }

  private isServerError(error: HttpErrorResponse): boolean {
    return error.status >= 500 && error.status < 600;
  }

  private isClientError(error: HttpErrorResponse): boolean {
    return error.status >= 400 && error.status < 500 && error.status !== 401;
  }

  private extractRequestHeaders(url?: string | null): Record<string, string> {
    // Extract relevant request headers for debugging
    // This would need access to original request, implementing basic version
    return {
      'Content-Type': 'application/json',
      'Accept': 'application/json'
      // Add other standard headers
    };
  }

  private extractResponseHeaders(headers: any): Record<string, string> {
    const result: Record<string, string> = {};
    if (headers && headers.keys) {
      headers.keys().forEach((key: string) => {
        result[key] = headers.get(key);
      });
    }
    return result;
  }

  private cleanupRequest(requestId: string): void {
    this.activeRequests.delete(requestId);
  }

  private generateRequestId(): string {
    return `req_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
  }
} 