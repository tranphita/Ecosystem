import { Component, OnInit, ChangeDetectionStrategy, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatCardModule } from '@angular/material/card';
import { TranslateModule } from '@ngx-translate/core';
import { Store } from '@ngrx/store';
import { Observable, Subject } from 'rxjs';
import { takeUntil, debounceTime, distinctUntilChanged } from 'rxjs/operators';

import { SmartBoxRoleDto } from '../../../../shared/models';
import * as RoleActions from '../store/role.actions';
import * as RoleSelectors from '../store/role.selectors';

/**
 * Component hiển thị danh sách roles với tính năng filter, sort và pagination
 */
@Component({
  selector: 'app-role-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatSelectModule,
    MatFormFieldModule,
    MatPaginatorModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    MatTooltipModule,
    MatCardModule,
    TranslateModule
  ],
  templateUrl: './role-list.component.html',
  styleUrl: './role-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RoleListComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  private searchSubject = new Subject<string>();

  // === Store Observables ===
  roles$ = this.store.select(RoleSelectors.selectFilteredRoles);
  isLoading$ = this.store.select(RoleSelectors.selectIsRoleLoading);
  error$ = this.store.select(RoleSelectors.selectRoleError);
  pagination$ = this.store.select(RoleSelectors.selectRolePaginationInfo);
  filter$ = this.store.select(RoleSelectors.selectRoleFilter);
  roleStats$ = this.store.select(RoleSelectors.selectRoleStats);

  // === UI Properties ===
  displayedColumns: string[] = [
    'displayName',
    'name', 
    'description',
    'isSystem',
    'isActive',
    'displayOrder',
    'actions'
  ];

  searchTerm = '';
  selectedActiveFilter: boolean | undefined = undefined;
  selectedSystemFilter: boolean | undefined = undefined;

  // === Filter Options ===
  activeFilterOptions = [
    { value: undefined, label: 'roles.filter.allStatuses' },
    { value: true, label: 'roles.filter.active' },
    { value: false, label: 'roles.filter.inactive' }
  ];

  systemFilterOptions = [
    { value: undefined, label: 'roles.filter.allTypes' },
    { value: true, label: 'roles.filter.systemRoles' },
    { value: false, label: 'roles.filter.customRoles' }
  ];

  constructor(private store: Store) {
    // Setup search debouncing
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(searchTerm => {
      this.onSearchChange(searchTerm);
    });
  }

  ngOnInit(): void {
    // Load initial data
    this.loadRoles();
    this.loadActiveRoles();

    // Subscribe to filter changes
    this.filter$.pipe(takeUntil(this.destroy$)).subscribe(filter => {
      this.searchTerm = filter.searchTerm;
      this.selectedActiveFilter = filter.isActive;
      this.selectedSystemFilter = filter.isSystem;
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // === Data Loading ===
  private loadRoles(): void {
    this.store.dispatch(RoleActions.loadRoles({
      input: {
        skipCount: 0,
        maxResultCount: 10
      }
    }));
  }

  private loadActiveRoles(): void {
    this.store.dispatch(RoleActions.loadActiveRoles());
  }

  // === Search & Filter Methods ===
  onSearchInput(searchTerm: string): void {
    this.searchSubject.next(searchTerm);
  }

  private onSearchChange(searchTerm: string): void {
    this.store.dispatch(RoleActions.updateRoleFilter({
      filter: { filter: searchTerm }
    }));
  }

  onActiveFilterChange(isActive: boolean | undefined): void {
    this.store.dispatch(RoleActions.updateRoleFilter({
      filter: { isActive }
    }));
  }

  onSystemFilterChange(isSystem: boolean | undefined): void {
    this.store.dispatch(RoleActions.updateRoleFilter({
      filter: { isSystem }
    }));
  }

  onClearFilters(): void {
    this.searchTerm = '';
    this.selectedActiveFilter = undefined;
    this.selectedSystemFilter = undefined;
    this.store.dispatch(RoleActions.clearRoleFilter());
  }

  // === Pagination ===
  onPageChange(event: PageEvent): void {
    this.store.dispatch(RoleActions.setRolePage({
      page: event.pageIndex + 1
    }));
    
    // PageSize change is handled separately if needed
    this.store.dispatch(RoleActions.setRolePageSize({
      pageSize: event.pageSize
    }));
  }

  // === Row Actions ===
  onCreateRole(): void {
    this.store.dispatch(RoleActions.openRoleDialog({
      mode: 'create'
    }));
  }

  onViewRole(role: SmartBoxRoleDto): void {
    this.store.dispatch(RoleActions.openRoleDialog({
      mode: 'view',
      role
    }));
  }

  onEditRole(role: SmartBoxRoleDto): void {
    this.store.dispatch(RoleActions.openRoleDialog({
      mode: 'edit',
      role
    }));
  }

  onDeleteRole(role: SmartBoxRoleDto): void {
    if (role.isSystem) {
      // Không cho phép xóa system roles
      return;
    }

    // TODO: Show confirmation dialog
    const confirmed = confirm(`Bạn có chắc muốn xóa vai trò "${role.displayName}"?`);
    if (confirmed) {
      this.store.dispatch(RoleActions.deleteRole({ id: role.id }));
    }
  }

  // === Utility Methods ===
  trackByRoleId(index: number, role: SmartBoxRoleDto): string {
    return role.id;
  }

  getStatusChipColor(isActive: boolean): string {
    return isActive ? 'primary' : 'warn';
  }

  getTypeChipColor(isSystem: boolean): string {
    return isSystem ? 'accent' : 'primary';
  }

  canEditRole(role: SmartBoxRoleDto): boolean {
    // Có thể edit tất cả roles
    return true;
  }

  canDeleteRole(role: SmartBoxRoleDto): boolean {
    // Không cho phép xóa system roles
    return !role.isSystem;
  }

  // === Error Handling ===
  onRetry(): void {
    this.loadRoles();
  }

  clearError(): void {
    this.store.dispatch(RoleActions.clearRoleError());
  }
} 