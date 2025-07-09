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

import { SmartBoxUserDto } from '../../../../shared/models';
import * as UserActions from '../store/user.actions';
import * as UserSelectors from '../store/user.selectors';

/**
 * Component hiển thị danh sách users với tính năng filter, sort và pagination
 */
@Component({
  selector: 'app-user-list',
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
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserListComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  private searchSubject = new Subject<string>();

  // === Store Observables ===
  users$ = this.store.select(UserSelectors.selectUsers);
  isLoading$ = this.store.select(UserSelectors.selectIsLoading);
  error$ = this.store.select(UserSelectors.selectUserError);
  pagination$ = this.store.select(UserSelectors.selectPaginationInfo);
  filter$ = this.store.select(UserSelectors.selectFilter);
  userStats$ = this.store.select(UserSelectors.selectUserStats);

  // === UI Properties ===
  displayedColumns: string[] = [
    'avatar',
    'fullName',
    'userName',
    'email',
    'company',
    'position',
    'isActive',
    'actions'
  ];

  searchTerm = '';
  selectedActiveFilter: boolean | undefined = undefined;
  selectedCompanyFilter: string | undefined = undefined;

  // === Filter Options ===
  activeFilterOptions = [
    { value: undefined, label: 'users.filter.allStatuses' },
    { value: true, label: 'users.filter.active' },
    { value: false, label: 'users.filter.inactive' }
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
    this.loadUsers();

    // Subscribe to filter changes
    this.filter$.pipe(takeUntil(this.destroy$)).subscribe(filter => {
      this.searchTerm = filter.searchTerm;
      this.selectedActiveFilter = filter.isActive;
      this.selectedCompanyFilter = filter.companyId;
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // === Data Loading ===
  private loadUsers(): void {
    this.store.dispatch(UserActions.loadUsers({
      input: {
        skipCount: 0,
        maxResultCount: 10
      }
    }));
  }

  // === Search & Filter Methods ===
  onSearchInput(searchTerm: string): void {
    this.searchSubject.next(searchTerm);
  }

  private onSearchChange(searchTerm: string): void {
    this.store.dispatch(UserActions.updateFilter({
      filter: { filter: searchTerm }
    }));
  }

  onActiveFilterChange(isActive: boolean | undefined): void {
    this.store.dispatch(UserActions.updateFilter({
      filter: { isActive }
    }));
  }

  onCompanyFilterChange(companyId: string | undefined): void {
    this.store.dispatch(UserActions.updateFilter({
      filter: { companyId }
    }));
  }

  onClearFilters(): void {
    this.searchTerm = '';
    this.selectedActiveFilter = undefined;
    this.selectedCompanyFilter = undefined;
    this.store.dispatch(UserActions.clearFilter());
  }

  // === Pagination ===
  onPageChange(event: PageEvent): void {
    this.store.dispatch(UserActions.setPage({
      page: event.pageIndex + 1
    }));
    
    this.store.dispatch(UserActions.setPageSize({
      pageSize: event.pageSize
    }));
  }

  // === Row Actions ===
  onViewUser(user: SmartBoxUserDto): void {
    this.store.dispatch(UserActions.openUserDialog({
      mode: 'view',
      user
    }));
  }

  onEditUser(user: SmartBoxUserDto): void {
    this.store.dispatch(UserActions.openUserDialog({
      mode: 'edit',
      user
    }));
  }

  onManageRoles(user: SmartBoxUserDto): void {
    this.store.dispatch(UserActions.openRoleDialog({
      user
    }));
  }

  async onToggleActive(user: SmartBoxUserDto): Promise<void> {
    this.store.dispatch(UserActions.toggleUserActive({
      id: user.id,
      isActive: !user.isActive
    }));
  }

  // === Utility Methods ===
  trackByUserId(index: number, user: SmartBoxUserDto): string {
    return user.id;
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

  getStatusChipColor(isActive: boolean): string {
    return isActive ? 'primary' : 'warn';
  }

  // === Permissions ===
  hasCreatePermission = true;
  hasEditPermission = true;
  hasDeletePermission = true;
  hasManageRolesPermission = true;

  // === Error Handling ===
  onRetry(): void {
    this.loadUsers();
  }

  clearError(): void {
    this.store.dispatch(UserActions.clearError());
  }
} 