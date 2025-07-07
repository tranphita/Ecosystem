/**
 * Enhanced Table Component
 * Reusable table component với strong typing, error handling và comprehensive features
 */

import {
  Component,
  Input,
  Output,
  EventEmitter,
  ViewChild,
  AfterViewInit,
  OnInit,
  OnDestroy,
  ChangeDetectionStrategy,
  inject,
  TrackByFunction
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from 'src/app/material.module';
import { TablerIconsModule } from 'angular-tabler-icons';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort, Sort } from '@angular/material/sort';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { Subscription } from 'rxjs';

import { 
  SortConfig, 
  FilterConfig, 
  PaginationConfig,
  OperationState,
  Nullable 
} from '../../../core/types/common.types';
import { ErrorHandlerService } from '../../../core/errors/error-handler.service';

/**
 * Enhanced table column configuration với strong typing
 */
export interface TableColumn<TData = unknown> {
  readonly key: keyof TData | 'actions';
  readonly label: string;
  readonly sortable?: boolean;
  readonly filterable?: boolean;
  readonly width?: string;
  readonly align?: 'left' | 'center' | 'right';
  readonly cellType?: TableCellType;
  readonly cellRenderer?: (value: unknown, row: TData) => string;
  readonly visible?: boolean;
  // Legacy support cho existing code
  readonly cell?: (row: TData) => any;
}

/**
 * Table cell types cho different rendering
 */
export type TableCellType = 
  | 'text' 
  | 'number' 
  | 'currency' 
  | 'date' 
  | 'datetime'
  | 'boolean' 
  | 'avatar' 
  | 'status' 
  | 'actions'
  | 'custom';

/**
 * Table action configuration
 */
export interface TableAction<TData = unknown> {
  readonly id: string;
  readonly label: string;
  readonly icon: string;
  readonly color: 'primary' | 'accent' | 'warn';
  readonly visible: (row: TData) => boolean;
  readonly disabled: (row: TData) => boolean;
  readonly tooltip?: string;
}

/**
 * Table configuration
 */
export interface TableConfig {
  readonly showPagination: boolean;
  readonly showFilter: boolean;
  readonly showSort: boolean;
  readonly pageSize: number;
  readonly pageSizeOptions: number[];
  readonly stickyHeader: boolean;
  readonly multiSelect: boolean;
  readonly trackByField?: string;
}

@Component({
  selector: 'app-table',
  standalone: true,
  imports: [CommonModule, MaterialModule, TablerIconsModule],
  templateUrl: './table.component.html',
  styleUrls: ['./table.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TableComponent<TData = unknown> implements AfterViewInit, OnInit, OnDestroy {
  private readonly errorHandler = inject(ErrorHandlerService);
  private readonly subscriptions = new Subscription();

  // Configuration inputs
  @Input() columns: TableColumn<TData>[] = [];
  @Input() actions: TableAction<TData>[] = [];
  @Input() config: TableConfig = this.getDefaultConfig();
  
  // Data inputs
  @Input() set data(value: TData[] | null) {
    try {
      this.dataSource.data = value || [];
      this.updateDataState(value);
    } catch (error) {
      this.errorHandler.handleBusinessError('Lỗi khi cập nhật dữ liệu table', 'TABLE_DATA_ERROR');
    }
  }
  
  @Input() set filterText(value: string | null) {
    try {
      const filterValue = value?.trim().toLowerCase() || '';
      this.dataSource.filter = filterValue;
      this.currentFilter = filterValue;
      this.filterChange.emit(filterValue);
    } catch (error) {
      this.errorHandler.handleBusinessError('Lỗi khi áp dụng filter', 'TABLE_FILTER_ERROR');
    }
  }

  @Input() loading = false;
  @Input() error: Nullable<string> = null;

  // Event outputs với proper typing
  @Output() readonly actionClick = new EventEmitter<{ action: string; row: TData }>();
  @Output() readonly sortChange = new EventEmitter<SortConfig>();
  @Output() readonly pageChange = new EventEmitter<PaginationConfig>();
  @Output() readonly filterChange = new EventEmitter<string>();
  @Output() readonly selectionChange = new EventEmitter<TData[]>();
  @Output() readonly dataStateChange = new EventEmitter<OperationState<TData[]>>();
  
  // Legacy support cho existing code
  @Output() readonly edit = new EventEmitter<TData>();
  @Output() readonly delete = new EventEmitter<TData>();
  @Output() readonly dataChanged = new EventEmitter<TData[]>();

  // Component state
  dataSource = new MatTableDataSource<TData>([]);
  selectedRows = new Set<TData>();
  currentFilter = '';
  
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  // Track by function cho performance
  trackByFn: TrackByFunction<TData> = (index: number, item: TData) => {
    const trackByField = this.config.trackByField;
    if (trackByField && item && typeof item === 'object') {
      return (item as any)[trackByField] || index;
    }
    return index;
  };

  ngOnInit(): void {
    try {
      this.setupDataSource();
    } catch (error) {
      this.errorHandler.handleBusinessError('Lỗi khởi tạo table', 'TABLE_INIT_ERROR');
    }
  }

  ngAfterViewInit(): void {
    try {
      this.setupSortingAndPagination();
    } catch (error) {
      this.errorHandler.handleBusinessError('Lỗi thiết lập table controls', 'TABLE_CONTROLS_ERROR');
    }
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  // =============================================================================
  // PUBLIC METHODS
  // =============================================================================

  /**
   * Get displayed columns với proper typing
   */
  get displayedColumns(): (keyof TData | 'actions' | 'select')[] {
    const columns: (keyof TData | 'actions' | 'select')[] = [];
    
    if (this.config.multiSelect) {
      columns.push('select');
    }
    
    columns.push(...this.columns.filter(col => col.visible !== false).map(col => col.key));
    
    if (this.actions.length > 0) {
      columns.push('actions');
    }
    
    return columns;
  }

  /**
   * Get cell value với proper typing và error handling
   */
  getCellValue(element: TData, columnKey: keyof TData | string): unknown {
    try {
      if (!element || columnKey === 'actions' || columnKey === 'select') {
        return '';
      }
      // Type assertion để handle cả string và symbol keys
      return (element as any)[columnKey];
    } catch (error) {
      this.errorHandler.handleBusinessError('Lỗi lấy giá trị cell', 'TABLE_CELL_ERROR');
      return '';
    }
  }

  /**
   * Handle action click với proper typing
   */
  onActionClick(actionId: string, row: TData): void {
    try {
      this.actionClick.emit({ action: actionId, row });
      
      // Emit legacy events cho backward compatibility
      if (actionId === 'edit') {
        this.edit.emit(row);
      } else if (actionId === 'delete') {
        this.delete.emit(row);
      }
    } catch (error) {
      this.errorHandler.handleBusinessError('Lỗi thực thi action', 'TABLE_ACTION_ERROR');
    }
  }

  /**
   * Handle row selection
   */
  onRowSelect(row: TData, event: Event): void {
    try {
      const isChecked = (event.target as HTMLInputElement).checked;
      
      if (isChecked) {
        this.selectedRows.add(row);
      } else {
        this.selectedRows.delete(row);
      }
      
      this.selectionChange.emit(Array.from(this.selectedRows));
    } catch (error) {
      this.errorHandler.handleBusinessError('Lỗi chọn dòng', 'TABLE_SELECTION_ERROR');
    }
  }

  /**
   * Handle select all
   */
  onSelectAll(event: Event): void {
    try {
      const isChecked = (event.target as HTMLInputElement).checked;
      
      if (isChecked) {
        this.dataSource.data.forEach(row => this.selectedRows.add(row));
      } else {
        this.selectedRows.clear();
      }
      
      this.selectionChange.emit(Array.from(this.selectedRows));
    } catch (error) {
      this.errorHandler.handleBusinessError('Lỗi chọn tất cả', 'TABLE_SELECT_ALL_ERROR');
    }
  }

  /**
   * Check if row is selected
   */
  isRowSelected(row: TData): boolean {
    return this.selectedRows.has(row);
  }

  /**
   * Check if all rows are selected
   */
  isAllSelected(): boolean {
    return this.dataSource.data.length > 0 && 
           this.dataSource.data.every(row => this.selectedRows.has(row));
  }

  /**
   * Check if some rows are selected (for indeterminate state)
   */
  isSomeSelected(): boolean {
    return this.selectedRows.size > 0 && !this.isAllSelected();
  }

  /**
   * Check if name field has avatar data structure
   */
  isNameWithAvatar(value: unknown): boolean {
    return typeof value === 'object' && value !== null && 
      'name' in (value as object) && 'position' in (value as object) && 'imagePath' in (value as object);
  }

  /**
   * Convert column key to string for template usage
   */
  getColumnKeyAsString(key: keyof TData | 'actions' | 'select'): string {
    return String(key);
  }

  /**
   * Get current page data
   */
  getCurrentPageData(): TData[] {
    try {
      if (this.dataSource.paginator) {
        const startIndex = this.dataSource.paginator.pageIndex * this.dataSource.paginator.pageSize;
        return this.dataSource.filteredData.slice(
          startIndex,
          startIndex + this.dataSource.paginator.pageSize
        );
      }
      return this.dataSource.filteredData;
    } catch (error) {
      this.errorHandler.handleBusinessError('Lỗi lấy dữ liệu trang hiện tại', 'TABLE_PAGE_DATA_ERROR');
      return [];
    }
  }

  // =============================================================================
  // PRIVATE METHODS
  // =============================================================================

  private getDefaultConfig(): TableConfig {
    return {
      showPagination: true,
      showFilter: true,
      showSort: true,
      pageSize: 10,
      pageSizeOptions: [5, 10, 25, 50],
      stickyHeader: false,
      multiSelect: false
    };
  }

  private setupDataSource(): void {
    // Setup custom filter predicate
    this.dataSource.filterPredicate = (data: TData, filter: string) => {
      try {
        if (!filter) return true;
        
        const searchableFields = this.columns
          .filter(col => col.filterable !== false && col.key !== 'actions')
          .map(col => col.key);

        return searchableFields.some(field => {
          const value = this.getCellValue(data, field);
          return value && String(value).toLowerCase().includes(filter);
        });
      } catch (error) {
        this.errorHandler.handleBusinessError('Lỗi filter predicate', 'TABLE_FILTER_PREDICATE_ERROR');
        return false;
      }
    };
  }

  private setupSortingAndPagination(): void {
    if (this.config.showSort && this.sort) {
      this.dataSource.sort = this.sort;
      
      // Listen to sort changes
      const sortSubscription = this.sort.sortChange.subscribe((sortEvent: Sort) => {
        try {
          const sortConfig: SortConfig = {
            field: sortEvent.active,
            direction: sortEvent.direction as 'asc' | 'desc'
          };
          this.sortChange.emit(sortConfig);
          this.dataChanged.emit(this.getCurrentPageData());
        } catch (error) {
          this.errorHandler.handleBusinessError('Lỗi xử lý sort change', 'TABLE_SORT_ERROR');
        }
      });
      this.subscriptions.add(sortSubscription);
    }

    if (this.config.showPagination && this.paginator) {
      this.dataSource.paginator = this.paginator;
      
      // Listen to page changes
      const pageSubscription = this.paginator.page.subscribe((pageEvent: PageEvent) => {
        try {
          const paginationConfig: PaginationConfig = {
            page: pageEvent.pageIndex,
            size: pageEvent.pageSize,
            totalItems: pageEvent.length
          };
          this.pageChange.emit(paginationConfig);
          this.dataChanged.emit(this.getCurrentPageData());
        } catch (error) {
          this.errorHandler.handleBusinessError('Lỗi xử lý page change', 'TABLE_PAGE_ERROR');
        }
      });
      this.subscriptions.add(pageSubscription);
    }
  }

  private updateDataState(data: TData[] | null): void {
    try {
      const dataState: OperationState<TData[]> = {
        data: data,
        loading: this.loading,
        error: this.error ? { 
          type: 'CLIENT_ERROR',
          message: this.error,
          timestamp: new Date()
        } : null,
        lastUpdated: new Date()
      };
      
      this.dataStateChange.emit(dataState);
    } catch (error) {
      this.errorHandler.handleBusinessError('Lỗi cập nhật data state', 'TABLE_DATA_STATE_ERROR');
    }
  }
}
