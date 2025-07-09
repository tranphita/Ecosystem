import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TranslateModule } from '@ngx-translate/core';

/**
 * Column definition interface
 */
export interface DataTableColumn {
  key: string;
  header: string;
  type: 'text' | 'date' | 'boolean' | 'chip' | 'avatar' | 'email' | 'custom';
  sortable?: boolean;
  width?: string;
  align?: 'left' | 'center' | 'right';
  // For chip type
  chipColorFn?: (value: any) => string;
  chipLabelFn?: (value: any) => string;
  // For boolean type
  trueLabel?: string;
  falseLabel?: string;
  // For avatar type
  avatarFallbackFn?: (item: any) => string;
}

/**
 * Action button interface
 */
export interface DataTableAction {
  key: string;
  label: string;
  icon: string;
  color?: 'primary' | 'accent' | 'warn';
  tooltip?: string;
  visibleFn?: (item: any) => boolean;
  disabledFn?: (item: any) => boolean;
  permissionRequired?: string;
}

/**
 * Pagination info interface
 */
export interface PaginationInfo {
  currentPage: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage?: boolean;
  hasPreviousPage?: boolean;
}

/**
 * Reusable data table component với hỗ trợ đầy đủ tính năng
 */
@Component({
  selector: 'app-data-table',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatPaginatorModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    MatTooltipModule,
    TranslateModule
  ],
  templateUrl: './data-table.component.html',
  styleUrl: './data-table.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DataTableComponent implements OnInit {
  @Input() data: any[] = [];
  @Input() columns: DataTableColumn[] = [];
  @Input() actions: DataTableAction[] = [];
  @Input() isLoading = false;
  @Input() error: string | null = null;
  @Input() pagination: PaginationInfo | null = null;
  @Input() trackByFn: (index: number, item: any) => any = (index, item) => item.id || index;
  @Input() emptyMessage = 'common.noDataFound';
  @Input() showPagination = true;
  @Input() stickyHeader = false;

  @Output() actionClick = new EventEmitter<{ action: string; item: any }>();
  @Output() pageChange = new EventEmitter<PageEvent>();
  @Output() sortChange = new EventEmitter<{ column: string; direction: 'asc' | 'desc' }>();
  @Output() retry = new EventEmitter<void>();

  displayedColumns: string[] = [];

  ngOnInit(): void {
    this.displayedColumns = this.columns.map(col => col.key);
    if (this.actions.length > 0) {
      this.displayedColumns.push('actions');
    }
  }

  onActionClick(action: string, item: any): void {
    this.actionClick.emit({ action, item });
  }

  onPageChange(event: PageEvent): void {
    this.pageChange.emit(event);
  }

  onRetry(): void {
    this.retry.emit();
  }

  isActionVisible(action: DataTableAction, item: any): boolean {
    return action.visibleFn ? action.visibleFn(item) : true;
  }

  isActionDisabled(action: DataTableAction, item: any): boolean {
    return action.disabledFn ? action.disabledFn(item) : false;
  }

  getColumnValue(item: any, column: DataTableColumn): any {
    const keys = column.key.split('.');
    let value = item;
    for (const key of keys) {
      value = value?.[key];
    }
    return value;
  }

  formatValue(value: any, column: DataTableColumn): string {
    if (value === null || value === undefined) {
      return '';
    }

    switch (column.type) {
      case 'date':
        return new Date(value).toLocaleDateString('vi-VN');
      case 'boolean':
        return value ? 'Có' : 'Không';
      default:
        return String(value);
    }
  }

  getInitials(name: string): string {
    if (!name) return '?';
    return name
      .split(' ')
      .map(word => word.charAt(0))
      .join('')
      .substring(0, 2)
      .toUpperCase();
  }

  getChipColor(value: any, key: string): string {
    if (key.includes('active') || key.includes('Active')) {
      return value ? 'primary' : 'warn';
    }
    if (key.includes('system') || key.includes('System')) {
      return value ? 'accent' : 'primary';
    }
    return 'primary';
  }
} 