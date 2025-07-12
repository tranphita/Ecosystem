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
import { MatDialog } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
import { TablerIconsModule } from 'angular-tabler-icons';
import { Store } from '@ngrx/store';
import { Observable, Subject } from 'rxjs';
import { takeUntil, debounceTime, distinctUntilChanged } from 'rxjs/operators';

import { SmartBoxUserDto } from '../../../../shared/models';
import * as UserActions from '../store/user.actions';
import * as UserSelectors from '../store/user.selectors';
import { UserDialogComponent, UserDialogData } from '../user-dialog/user-dialog.component';

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
    TranslateModule,
    TablerIconsModule
  ],
  templateUrl: './user-list.component.html',
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
  companies$ = this.store.select(UserSelectors.selectUniqueCompanies);

  // === UI Properties ===
  displayedColumns: string[] = [
    'fullName',
    'userName',
    'email',
    'company',
    'isActive',
    'actions'
  ];

  searchTerm = '';

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
    this.loadUsers();

    // Subscribe to filter changes
    this.filter$.pipe(takeUntil(this.destroy$)).subscribe(filter => {
      this.searchTerm = filter.searchTerm;
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
  onCreateUser(): void {
    const dialogData: UserDialogData = {
      mode: 'create'
    };

    const dialogRef = this.dialog.open(UserDialogComponent, {
      width: '800px',
      data: dialogData,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadUsers(); // Reload data after successful create
      }
    });
  }

  onViewUser(user: SmartBoxUserDto): void {
    const dialogData: UserDialogData = {
      mode: 'view',
      user
    };

    this.dialog.open(UserDialogComponent, {
      width: '800px',
      data: dialogData
    });
  }

  onEditUser(user: SmartBoxUserDto): void {
    const dialogData: UserDialogData = {
      mode: 'edit',
      user
    };

    const dialogRef = this.dialog.open(UserDialogComponent, {
      width: '800px',
      data: dialogData,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadUsers(); // Reload data after successful edit
      }
    });
  }

  onManageRoles(user: SmartBoxUserDto): void {
    this.store.dispatch(UserActions.openRoleDialog({
      user
    }));
  }

  onDeleteUser(user: SmartBoxUserDto): void {
    // TODO: Thêm confirmation dialog
    this.store.dispatch(UserActions.deleteUser({
      id: user.id
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