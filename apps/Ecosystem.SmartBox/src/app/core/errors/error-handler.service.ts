/**
 * Global Error Handler Service
 * Xử lý tất cả errors trong ứng dụng một cách tập trung và thống nhất
 */

import { Injectable, ErrorHandler, inject, NgZone } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable } from 'rxjs';

import { 
  AppError, 
  ErrorType, 
  AnyObject,
  ApiErrorResponse 
} from '../types/common.types';

/**
 * Error notification configuration
 */
export interface ErrorNotificationConfig {
  readonly showToUser: boolean;
  readonly autoHide: boolean;
  readonly duration: number;
  readonly actionLabel?: string;
  readonly actionCallback?: () => void;
}

/**
 * Error context cho debugging
 */
export interface ErrorContext {
  readonly url?: string;
  readonly userId?: string;
  readonly userAgent?: string;
  readonly timestamp: Date;
  readonly sessionId?: string;
  readonly additionalData?: AnyObject;
}

/**
 * Processed error information
 */
export interface ProcessedError {
  readonly originalError: unknown;
  readonly appError: AppError;
  readonly context: ErrorContext;
  readonly shouldNotifyUser: boolean;
  readonly userMessage: string;
  readonly technicalMessage: string;
}

/**
 * Error statistics
 */
export interface ErrorStatistics {
  readonly totalErrors: number;
  readonly errorsByType: Record<ErrorType, number>;
  readonly recentErrors: ProcessedError[];
  readonly lastReset: Date;
}

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlerService implements ErrorHandler {
  private readonly snackBar = inject(MatSnackBar);
  private readonly router = inject(Router);
  private readonly ngZone = inject(NgZone);

  // Error state management
  private readonly currentErrorSubject = new BehaviorSubject<ProcessedError | null>(null);
  private readonly errorHistorySubject = new BehaviorSubject<ProcessedError[]>([]);
  private readonly statisticsSubject = new BehaviorSubject<ErrorStatistics>(this.getInitialStatistics());

  // Observables
  public readonly currentError$: Observable<ProcessedError | null> = this.currentErrorSubject.asObservable();
  public readonly errorHistory$: Observable<ProcessedError[]> = this.errorHistorySubject.asObservable();
  public readonly statistics$: Observable<ErrorStatistics> = this.statisticsSubject.asObservable();

  // Configuration
  private readonly defaultConfig: ErrorNotificationConfig = {
    showToUser: true,
    autoHide: true,
    duration: 5000
  };

  /**
   * Angular ErrorHandler implementation
   */
  handleError(error: unknown): void {
    console.error('💥 Global error caught:', error);
    
    this.ngZone.run(() => {
      const processedError = this.processError(error);
      this.handleProcessedError(processedError);
    });
  }

  /**
   * Xử lý HTTP errors
   */
  handleHttpError(error: HttpErrorResponse, context?: Partial<ErrorContext>): AppError {
    const processedError = this.processHttpError(error, context);
    this.handleProcessedError(processedError);
    return processedError.appError;
  }

  /**
   * Xử lý validation errors
   */
  handleValidationError(
    errors: Record<string, string[]>, 
    context?: Partial<ErrorContext>
  ): AppError {
    const processedError = this.processValidationError(errors, context);
    this.handleProcessedError(processedError);
    return processedError.appError;
  }

  /**
   * Xử lý business logic errors
   */
  handleBusinessError(
    message: string, 
    code?: string, 
    context?: Partial<ErrorContext>
  ): AppError {
    const processedError = this.processBusinessError(message, code, context);
    this.handleProcessedError(processedError);
    return processedError.appError;
  }

  /**
   * Show custom error message to user
   */
  showErrorToUser(
    message: string, 
    config: Partial<ErrorNotificationConfig> = {}
  ): void {
    const finalConfig = { ...this.defaultConfig, ...config };
    
    const snackBarRef = this.snackBar.open(
      message,
      finalConfig.actionLabel || 'Đóng',
      {
        duration: finalConfig.autoHide ? finalConfig.duration : undefined,
        panelClass: ['error-snackbar']
      }
    );

    if (finalConfig.actionCallback) {
      snackBarRef.onAction().subscribe(finalConfig.actionCallback);
    }
  }

  /**
   * Clear current error
   */
  clearCurrentError(): void {
    this.currentErrorSubject.next(null);
  }

  /**
   * Clear error history
   */
  clearErrorHistory(): void {
    this.errorHistorySubject.next([]);
    this.updateStatistics();
  }

  /**
   * Get current error statistics
   */
  getStatistics(): ErrorStatistics {
    return this.statisticsSubject.value;
  }

  // =============================================================================
  // PRIVATE METHODS
  // =============================================================================

  private processError(error: unknown, context?: Partial<ErrorContext>): ProcessedError {
    if (error instanceof HttpErrorResponse) {
      return this.processHttpError(error, context);
    }

    if (this.isValidationError(error)) {
      return this.processValidationError(error as Record<string, string[]>, context);
    }

    return this.processGenericError(error, context);
  }

  private processHttpError(
    error: HttpErrorResponse, 
    context?: Partial<ErrorContext>
  ): ProcessedError {
    let errorType: ErrorType;
    let userMessage: string;

    // Xác định error type dựa trên HTTP status
    switch (error.status) {
      case 0:
        errorType = 'NETWORK_ERROR';
        userMessage = 'Không thể kết nối đến server. Vui lòng kiểm tra kết nối mạng.';
        break;
      case 400:
        errorType = 'VALIDATION_ERROR';
        userMessage = 'Dữ liệu không hợp lệ. Vui lòng kiểm tra lại thông tin.';
        break;
      case 401:
        errorType = 'AUTHENTICATION_ERROR';
        userMessage = 'Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại.';
        this.router.navigate(['/authentication/login']);
        break;
      case 403:
        errorType = 'AUTHORIZATION_ERROR';
        userMessage = 'Bạn không có quyền thực hiện thao tác này.';
        break;
      case 404:
        errorType = 'CLIENT_ERROR';
        userMessage = 'Không tìm thấy tài nguyên yêu cầu.';
        break;
      case 422:
        errorType = 'VALIDATION_ERROR';
        userMessage = this.extractValidationMessage(error);
        break;
      case 500:
      case 502:
      case 503:
        errorType = 'SERVER_ERROR';
        userMessage = 'Có lỗi xảy ra trên server. Vui lòng thử lại sau.';
        break;
      default:
        errorType = 'UNKNOWN_ERROR';
        userMessage = 'Có lỗi không xác định xảy ra. Vui lòng thử lại.';
    }

    const appError: AppError = {
      type: errorType,
      message: error.message,
      code: error.status.toString(),
      details: error.error?.message || error.statusText,
      timestamp: new Date(),
      userMessage,
      technicalDetails: {
        url: error.url,
        status: error.status,
        statusText: error.statusText,
        headers: this.extractHeaders(error.headers)
      }
    };

    return {
      originalError: error,
      appError,
      context: this.buildErrorContext(context),
      shouldNotifyUser: errorType !== 'AUTHENTICATION_ERROR', // Don't notify for auth errors
      userMessage,
      technicalMessage: `HTTP ${error.status}: ${error.message}`
    };
  }

  private processValidationError(
    errors: Record<string, string[]>, 
    context?: Partial<ErrorContext>
  ): ProcessedError {
    const errorMessages = Object.entries(errors)
      .map(([field, messages]) => `${field}: ${messages.join(', ')}`)
      .join('; ');

    const appError: AppError = {
      type: 'VALIDATION_ERROR',
      message: 'Validation failed',
      details: errorMessages,
      timestamp: new Date(),
      userMessage: 'Dữ liệu không hợp lệ. Vui lòng kiểm tra lại các trường thông tin.',
      technicalDetails: { validationErrors: errors }
    };

    return {
      originalError: errors,
      appError,
      context: this.buildErrorContext(context),
      shouldNotifyUser: true,
      userMessage: appError.userMessage!,
      technicalMessage: errorMessages
    };
  }

  private processBusinessError(
    message: string, 
    code?: string, 
    context?: Partial<ErrorContext>
  ): ProcessedError {
    const appError: AppError = {
      type: 'CLIENT_ERROR',
      message,
      code,
      timestamp: new Date(),
      userMessage: message,
      details: 'Business logic error'
    };

    return {
      originalError: new Error(message),
      appError,
      context: this.buildErrorContext(context),
      shouldNotifyUser: true,
      userMessage: message,
      technicalMessage: message
    };
  }

  private processGenericError(error: unknown, context?: Partial<ErrorContext>): ProcessedError {
    const errorMessage = this.extractErrorMessage(error);
    
    const appError: AppError = {
      type: 'UNKNOWN_ERROR',
      message: errorMessage,
      timestamp: new Date(),
      userMessage: 'Có lỗi không xác định xảy ra. Vui lòng thử lại.',
      technicalDetails: {
        stack: error instanceof Error ? error.stack : undefined,
        name: error instanceof Error ? error.name : typeof error
      }
    };

    return {
      originalError: error,
      appError,
      context: this.buildErrorContext(context),
      shouldNotifyUser: true,
      userMessage: appError.userMessage!,
      technicalMessage: errorMessage
    };
  }

  private handleProcessedError(processedError: ProcessedError): void {
    // Update current error
    this.currentErrorSubject.next(processedError);

    // Add to history
    const currentHistory = this.errorHistorySubject.value;
    const updatedHistory = [processedError, ...currentHistory].slice(0, 100); // Keep last 100 errors
    this.errorHistorySubject.next(updatedHistory);

    // Update statistics
    this.updateStatistics();

    // Show notification to user if needed
    if (processedError.shouldNotifyUser) {
      this.showErrorToUser(processedError.userMessage);
    }

    // Log to console for debugging
    this.logError(processedError);

    // TODO: Send to error reporting service (e.g., Sentry)
    this.sendToErrorReporting(processedError);
  }

  private buildErrorContext(context?: Partial<ErrorContext>): ErrorContext {
    return {
      url: window.location.href,
      userAgent: navigator.userAgent,
      timestamp: new Date(),
      sessionId: this.generateSessionId(),
      ...context
    };
  }

  private extractErrorMessage(error: unknown): string {
    if (error instanceof Error) {
      return error.message;
    }
    if (typeof error === 'string') {
      return error;
    }
    if (error && typeof error === 'object' && 'message' in error) {
      return String((error as { message: unknown }).message);
    }
    return 'Unknown error occurred';
  }

  private extractValidationMessage(error: HttpErrorResponse): string {
    if (error.error && typeof error.error === 'object') {
      const apiError = error.error as ApiErrorResponse;
      if (apiError.error?.validationErrors?.length) {
        return apiError.error.validationErrors
          .map(ve => `${ve.field}: ${ve.message}`)
          .join(', ');
      }
      if (apiError.message) {
        return apiError.message;
      }
    }
    return 'Dữ liệu không hợp lệ';
  }

  private extractHeaders(headers: any): Record<string, string> {
    const result: Record<string, string> = {};
    if (headers && headers.keys) {
      headers.keys().forEach((key: string) => {
        result[key] = headers.get(key);
      });
    }
    return result;
  }

  private isValidationError(error: unknown): boolean {
    return (
      error !== null &&
      typeof error === 'object' &&
      !Array.isArray(error) &&
      Object.values(error).every(value => Array.isArray(value))
    );
  }

  private updateStatistics(): void {
    const errors = this.errorHistorySubject.value;
    const errorsByType: Record<ErrorType, number> = {
      'NETWORK_ERROR': 0,
      'VALIDATION_ERROR': 0,
      'AUTHENTICATION_ERROR': 0,
      'AUTHORIZATION_ERROR': 0,
      'SERVER_ERROR': 0,
      'CLIENT_ERROR': 0,
      'UNKNOWN_ERROR': 0
    };

    errors.forEach(error => {
      errorsByType[error.appError.type]++;
    });

    const statistics: ErrorStatistics = {
      totalErrors: errors.length,
      errorsByType,
      recentErrors: errors.slice(0, 10),
      lastReset: new Date()
    };

    this.statisticsSubject.next(statistics);
  }

  private logError(processedError: ProcessedError): void {
    const { appError, context, technicalMessage } = processedError;
    
    console.group(`🚨 ${appError.type} - ${appError.timestamp.toISOString()}`);
    console.error('User Message:', processedError.userMessage);
    console.error('Technical Message:', technicalMessage);
    console.error('Context:', context);
    console.error('Original Error:', processedError.originalError);
    if (appError.technicalDetails) {
      console.error('Technical Details:', appError.technicalDetails);
    }
    console.groupEnd();
  }

  private sendToErrorReporting(processedError: ProcessedError): void {
    // TODO: Implement integration with error reporting service
    // This could be Sentry, LogRocket, or custom error reporting API
    console.log('📊 Error would be sent to reporting service:', processedError);
  }

  private generateSessionId(): string {
    return Math.random().toString(36).substr(2, 9);
  }

  private getInitialStatistics(): ErrorStatistics {
    return {
      totalErrors: 0,
      errorsByType: {
        'NETWORK_ERROR': 0,
        'VALIDATION_ERROR': 0,
        'AUTHENTICATION_ERROR': 0,
        'AUTHORIZATION_ERROR': 0,
        'SERVER_ERROR': 0,
        'CLIENT_ERROR': 0,
        'UNKNOWN_ERROR': 0
      },
      recentErrors: [],
      lastReset: new Date()
    };
  }
} 