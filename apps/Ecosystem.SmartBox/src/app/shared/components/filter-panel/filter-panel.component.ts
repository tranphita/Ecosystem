import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { TranslateModule } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

/**
 * Reusable filter panel component
 */
export interface FilterField {
  key: string;
  label: string;
  type: 'text' | 'select' | 'date' | 'boolean';
  placeholder?: string;
  options?: FilterOption[];
  value?: any;
  width?: string;
}

export interface FilterOption {
  value: any;
  label: string;
  disabled?: boolean;
}

@Component({
  selector: 'app-filter-panel',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    TranslateModule
  ],
  templateUrl: './filter-panel.component.html',
  styleUrl: './filter-panel.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class FilterPanelComponent {
  @Input() fields: FilterField[] = [];
  @Input() showClearButton = true;
  @Input() showSearchButton = false;
  @Input() searchDebounceTime = 300;
  @Input() title = '';
  @Input() collapsible = false;
  @Input() collapsed = false;

  @Output() filterChange = new EventEmitter<{ [key: string]: any }>();
  @Output() search = new EventEmitter<void>();
  @Output() clear = new EventEmitter<void>();

  private searchSubject = new Subject<string>();

  constructor() {
    // Setup search debouncing
    this.searchSubject.pipe(
      debounceTime(this.searchDebounceTime),
      distinctUntilChanged()
    ).subscribe(() => {
      this.emitFilterChange();
    });
  }

  onFieldChange(field: FilterField, value: any): void {
    field.value = value;
    
    // For text fields, use debounced search
    if (field.type === 'text') {
      this.searchSubject.next(value);
    } else {
      // For other field types, emit immediately
      this.emitFilterChange();
    }
  }

  onSearch(): void {
    this.search.emit();
  }

  onClear(): void {
    // Reset all field values
    this.fields.forEach(field => {
      field.value = undefined;
    });
    
    this.clear.emit();
    this.emitFilterChange();
  }

  toggleCollapse(): void {
    this.collapsed = !this.collapsed;
  }

  private emitFilterChange(): void {
    const filters: { [key: string]: any } = {};
    
    this.fields.forEach(field => {
      if (field.value !== undefined && field.value !== null && field.value !== '') {
        filters[field.key] = field.value;
      }
    });

    this.filterChange.emit(filters);
  }

  getFieldValue(field: FilterField): any {
    return field.value;
  }

  getFieldWidth(field: FilterField): string {
    return field.width || '200px';
  }
} 