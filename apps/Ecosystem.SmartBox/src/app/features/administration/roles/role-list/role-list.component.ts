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
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
import { TablerIconsModule } from 'angular-tabler-icons';
import { Store } from '@ngrx/store';
import { Observable, Subject } from 'rxjs';
import { takeUntil, debounceTime, distinctUntilChanged } from 'rxjs/operators';

import { SmartBoxRoleDto } from '../../../../shared/models';
import * as RoleActions from '../store/role.actions';
import * as RoleSelectors from '../store/role.selectors';
import { RoleDialogComponent } from '../role-dialog/role-dialog.component';

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
    MatDialogModule,
    TranslateModule,
    TablerIconsModule
  ],
  templateUrl: './role-list.component.html',
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
  dialogState$ = this.store.select(RoleSelectors.selectRoleDialogState);

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
  showFilters = true;

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

  constructor(
    private store: Store,
    private dialog: MatDialog
  ) {
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

    // Subscribe to dialog state changes
    this.dialogState$.pipe(takeUntil(this.destroy$)).subscribe(dialogState => {
      if (dialogState.isOpen && !this.dialog.openDialogs.length && dialogState.mode) {
        this.openRoleDialog(dialogState.mode, dialogState.selectedRole || undefined);
      } else if (!dialogState.isOpen && this.dialog.openDialogs.length > 0) {
        this.dialog.closeAll();
      }
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

  /**
   * Mở dialog tạo/chỉnh sửa/xem role
   */
  private openRoleDialog(mode: 'create' | 'edit' | 'view', role?: SmartBoxRoleDto): void {
    const dialogRef = this.dialog.open(RoleDialogComponent, {
      width: '600px',
      maxWidth: '90vw',
      disableClose: true,
      data: { mode, role }
    });

    dialogRef.afterClosed().subscribe(() => {
      this.store.dispatch(RoleActions.closeRoleDialog());
    });
  }
} 