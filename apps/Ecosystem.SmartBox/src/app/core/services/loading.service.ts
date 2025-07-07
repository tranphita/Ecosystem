/**
 * Loading Service
 * Quản lý loading states cho toàn bộ ứng dụng với type safety
 */

import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, combineLatest } from 'rxjs';
import { map } from 'rxjs/operators';

import { LoadingState } from '../types/common.types';

/**
 * Loading operation configuration
 */
export interface LoadingOperation {
  readonly id: string;
  readonly label: string;
  readonly startTime: Date;
  readonly timeout?: number;
}

/**
 * Global loading state
 */
export interface GlobalLoadingState {
  readonly isLoading: boolean;
  readonly activeOperations: LoadingOperation[];
  readonly totalOperations: number;
  readonly longestRunningOperation?: LoadingOperation;
}

@Injectable({
  providedIn: 'root'
})
export class LoadingService {
  // Private subjects
  private readonly globalLoadingSubject = new BehaviorSubject<boolean>(false);
  private readonly operationsSubject = new BehaviorSubject<Map<string, LoadingOperation>>(new Map());

  // Public observables
  public readonly isGlobalLoading$: Observable<boolean> = this.globalLoadingSubject.asObservable();
  public readonly activeOperations$: Observable<LoadingOperation[]> = this.operationsSubject.pipe(
    map(operations => Array.from(operations.values()))
  );

  // Combined loading state
  public readonly loadingState$: Observable<GlobalLoadingState> = combineLatest([
    this.isGlobalLoading$,
    this.activeOperations$
  ]).pipe(
    map(([isLoading, operations]) => ({
      isLoading,
      activeOperations: operations,
      totalOperations: operations.length,
      longestRunningOperation: this.findLongestRunningOperation(operations)
    }))
  );

  /**
   * Start a loading operation
   */
  startLoading(operationId: string, label: string, timeout?: number): void {
    const operation: LoadingOperation = {
      id: operationId,
      label,
      startTime: new Date(),
      timeout
    };

    const currentOperations = new Map(this.operationsSubject.value);
    currentOperations.set(operationId, operation);
    
    this.operationsSubject.next(currentOperations);
    this.updateGlobalLoadingState();

    // Set timeout if specified
    if (timeout) {
      setTimeout(() => {
        this.stopLoading(operationId);
        console.warn(`⏰ Loading operation "${label}" timed out after ${timeout}ms`);
      }, timeout);
    }
  }

  /**
   * Stop a loading operation
   */
  stopLoading(operationId: string): void {
    const currentOperations = new Map(this.operationsSubject.value);
    const operation = currentOperations.get(operationId);
    
    if (operation) {
      const duration = Date.now() - operation.startTime.getTime();
      console.log(`✅ Loading operation "${operation.label}" completed in ${duration}ms`);
      
      currentOperations.delete(operationId);
      this.operationsSubject.next(currentOperations);
      this.updateGlobalLoadingState();
    }
  }

  /**
   * Check if a specific operation is loading
   */
  isOperationLoading(operationId: string): Observable<boolean> {
    return this.operationsSubject.pipe(
      map(operations => operations.has(operationId))
    );
  }

  /**
   * Clear all loading operations
   */
  clearAllOperations(): void {
    this.operationsSubject.next(new Map());
    this.updateGlobalLoadingState();
  }

  /**
   * Get current loading state synchronously
   */
  getCurrentLoadingState(): GlobalLoadingState {
    const operations = Array.from(this.operationsSubject.value.values());
    return {
      isLoading: this.globalLoadingSubject.value,
      activeOperations: operations,
      totalOperations: operations.length,
      longestRunningOperation: this.findLongestRunningOperation(operations)
    };
  }

  // =============================================================================
  // PRIVATE METHODS
  // =============================================================================

  private updateGlobalLoadingState(): void {
    const hasActiveOperations = this.operationsSubject.value.size > 0;
    this.globalLoadingSubject.next(hasActiveOperations);
  }

  private findLongestRunningOperation(operations: LoadingOperation[]): LoadingOperation | undefined {
    if (operations.length === 0) {
      return undefined;
    }

    return operations.reduce((longest, current) => 
      current.startTime < longest.startTime ? current : longest
    );
  }
} 