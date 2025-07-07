import {
  Component,
  Input,
  Output,
  EventEmitter,
  ViewChild,
  AfterViewInit,
  OnInit,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from 'src/app/material.module';
import { TablerIconsModule } from 'angular-tabler-icons';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';

export interface TableColumn<T = any> {
  key: string;
  label: string;
  cell?: (row: T) => any;
  sortable?: boolean;
}

@Component({
  selector: 'app-table',
  standalone: true,
  imports: [CommonModule, MaterialModule, TablerIconsModule],
  templateUrl: './table.component.html',
  styleUrls: ['./table.component.scss'],
})
export class TableComponent<T = any> implements AfterViewInit, OnInit {
  @Input() columns: TableColumn<T>[] = [];
  @Input() set data(value: T[]) {
    this.dataSource.data = value || [];
  }
  @Input() set filterText(value: string) {
    console.log('Filter text set:', value);
    this.dataSource.filter = value?.trim().toLowerCase() || '';
    this.dataSource._updateChangeSubscription(); // force update (for debug)
  }
  @Output() edit = new EventEmitter<T>();
  @Output() delete = new EventEmitter<T>();
  @Output() dataChanged = new EventEmitter<T[]>();
  @Output() sortChange = new EventEmitter<any>();

  dataSource = new MatTableDataSource<T>([]);
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit() {
    this.dataSource.filterPredicate = (data: T, filter: string) => {
      return Object.values(data as any).some(
        (val) => typeof val === 'string' && val.toLowerCase().includes(filter)
      );
    };
    this.dataSource.filterPredicate = this.dataSource.filterPredicate;
    this.dataSource.connect().subscribe(() => {
      this.dataChanged.emit(this.getCurrentPageData());
    });
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
    if (this.sort) {
      this.sort.sortChange.subscribe((event) => {
        this.sortChange.emit(event);
        this.dataChanged.emit(this.getCurrentPageData());
      });
    }
    if (this.paginator) {
      this.paginator.page.subscribe(() => {
        this.dataChanged.emit(this.getCurrentPageData());
      });
    }
  }

  get displayedColumns() {
    return this.columns.map((col) => col.key).concat(['action']);
  }
  getCellValue(element: T, colKey: string) {
    return (element as any)[colKey];
  }
  
  isNameWithAvatar(value: any): boolean {
    return typeof value === 'object' && value !== null && 
      'name' in value && 'position' in value && 'imagePath' in value;
  }

  getCurrentPageData(): T[] {
    if (this.dataSource.paginator) {
      const startIndex =
        this.dataSource.paginator.pageIndex *
        this.dataSource.paginator.pageSize;
      return this.dataSource.filteredData.slice(
        startIndex,
        startIndex + this.dataSource.paginator.pageSize
      );
    }
    return this.dataSource.filteredData;
  }
}
