import { Component, Inject, OnInit, ChangeDetectionStrategy, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { TranslateModule } from '@ngx-translate/core';
import { Store } from '@ngrx/store';
import { Subject } from 'rxjs';
import { takeUntil, filter } from 'rxjs/operators';
import { SmartBoxRoleDto, CreateUpdateSmartBoxRoleDto } from '../../../../shared/models';
import * as RoleActions from '../store/role.actions';
import * as RoleSelectors from '../store/role.selectors';

/**
 * Interface cho data được truyền vào dialog
 */
export interface RoleDialogData {
  mode: 'create' | 'edit' | 'view';
  role?: SmartBoxRoleDto;
}

/**
 * Dialog component để tạo/chỉnh sửa/xem role
 */
@Component({
  selector: 'app-role-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatCheckboxModule,
    MatIconModule,
    MatProgressSpinnerModule,
    TranslateModule
  ],
  templateUrl: './role-dialog.component.html',
  styleUrl: './role-dialog.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RoleDialogComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  roleForm!: FormGroup;
  isSubmitting$ = this.store.select(RoleSelectors.selectIsRoleLoading);

  constructor(
    private fb: FormBuilder,
    private store: Store,
    private dialogRef: MatDialogRef<RoleDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: RoleDialogData
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    
    // Subscribe to role creation/update success để reset form state
    this.store.select(RoleSelectors.selectIsRoleDialogOpen)
      .pipe(
        takeUntil(this.destroy$),
        filter(isOpen => !isOpen)
      )
      .subscribe(() => {
        // Dialog đã được đóng từ store, component sẽ tự đóng
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Khởi tạo form
   */
  private initializeForm(): void {
    const role = this.data.role;
    
    this.roleForm = this.fb.group({
      displayName: [role?.displayName || '', [Validators.required, Validators.maxLength(100)]],
      name: [role?.name || '', [Validators.required, Validators.maxLength(50), Validators.pattern(/^[a-zA-Z0-9_.-]+$/)]],
      description: [role?.description || '', [Validators.maxLength(500)]],
      isActive: [role?.isActive ?? true],
      displayOrder: [role?.displayOrder || 0, [Validators.min(0)]]
    });

    // Disable form nếu là view mode
    if (this.data.mode === 'view') {
      this.roleForm.disable();
    }

    // Disable name field nếu là system role trong edit mode
    if (this.data.mode === 'edit' && role?.isSystem) {
      this.roleForm.get('name')?.disable();
    }
  }

  /**
   * Lấy title cho dialog
   */
  get dialogTitle(): string {
    switch (this.data.mode) {
      case 'create':
        return 'roles.createRole';
      case 'edit':
        return 'roles.editRole';
      case 'view':
        return 'roles.viewRole';
      default:
        return 'roles.role';
    }
  }

  /**
   * Kiểm tra xem có phải read-only mode không
   */
  get isReadOnly(): boolean {
    return this.data.mode === 'view';
  }

  /**
   * Kiểm tra xem có phải create mode không
   */
  get isCreateMode(): boolean {
    return this.data.mode === 'create';
  }

  /**
   * Kiểm tra xem có phải edit mode không
   */
  get isEditMode(): boolean {
    return this.data.mode === 'edit';
  }

  /**
   * Xử lý submit form
   */
  onSubmit(): void {
    if (this.roleForm.invalid || this.isReadOnly) {
      return;
    }

    const formValue = this.roleForm.value;
    
    const input: CreateUpdateSmartBoxRoleDto = {
      displayName: formValue.displayName,
      name: formValue.name,
      description: formValue.description,
      isActive: formValue.isActive,
      displayOrder: formValue.displayOrder
    };

    if (this.isCreateMode) {
      this.store.dispatch(RoleActions.createRole({ input }));
    } else if (this.isEditMode && this.data.role) {
      this.store.dispatch(RoleActions.updateRole({ 
        id: this.data.role.id, 
        input 
      }));
    }
  }

  /**
   * Đóng dialog
   */
  onCancel(): void {
    this.dialogRef.close();
  }

  /**
   * Kiểm tra field có lỗi không
   */
  hasError(fieldName: string, errorType?: string): boolean {
    const field = this.roleForm.get(fieldName);
    if (!field) return false;
    
    const hasError = field.invalid && (field.dirty || field.touched);
    if (errorType) {
      return hasError && field.hasError(errorType);
    }
    return hasError;
  }

  /**
   * Lấy error message cho field
   */
  getErrorMessage(fieldName: string): string {
    const field = this.roleForm.get(fieldName);
    if (!field || !field.errors) return '';

    const errors = field.errors;
    
    if (errors['required']) {
      return `roles.validation.${fieldName}.required`;
    }
    if (errors['maxlength']) {
      return `roles.validation.${fieldName}.maxlength`;
    }
    if (errors['pattern']) {
      return `roles.validation.${fieldName}.pattern`;
    }
    if (errors['min']) {
      return `roles.validation.${fieldName}.min`;
    }

    return `roles.validation.${fieldName}.invalid`;
  }

  /**
   * Auto-generate name từ display name (chỉ trong create mode)
   */
  onDisplayNameChange(): void {
    if (this.isCreateMode) {
      const displayName = this.roleForm.get('displayName')?.value || '';
      const autoName = this.generateNameFromDisplayName(displayName);
      this.roleForm.get('name')?.setValue(autoName);
    }
  }

  /**
   * Generate name từ display name
   */
  private generateNameFromDisplayName(displayName: string): string {
    return displayName
      .toLowerCase()
      .replace(/[^a-zA-Z0-9\s]/g, '') // Xóa ký tự đặc biệt
      .replace(/\s+/g, '_') // Thay space bằng underscore
      .substring(0, 50); // Giới hạn độ dài
  }
} 