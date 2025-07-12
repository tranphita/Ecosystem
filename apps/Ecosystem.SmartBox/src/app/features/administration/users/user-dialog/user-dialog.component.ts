import { Component, OnInit, OnDestroy, ChangeDetectionStrategy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { TranslateModule } from '@ngx-translate/core';
import { Store } from '@ngrx/store';
import { Subject, Observable } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { SmartBoxUserDto, CreateUpdateSmartBoxUserDto } from '../../../../shared/models';
import * as UserActions from '../store/user.actions';
import * as UserSelectors from '../store/user.selectors';

/**
 * Interface cho dialog data
 */
export interface UserDialogData {
  mode: 'create' | 'edit' | 'view';
  user?: SmartBoxUserDto;
}

/**
 * Component dialog để tạo/sửa/xem user
 */
@Component({
  selector: 'app-user-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    MatIconModule,
    MatProgressSpinnerModule,
    TranslateModule
  ],
  templateUrl: './user-dialog.component.html',
  styleUrls: ['./user-dialog.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserDialogComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  private fb = inject(FormBuilder);
  private store = inject(Store);
  private dialogRef = inject(MatDialogRef<UserDialogComponent>);
  private data = inject(MAT_DIALOG_DATA) as UserDialogData;

  // === Form ===
  userForm: FormGroup;
  mode = this.data.mode;
  user = this.data.user;

  // === Store Observables ===
  isLoading$ = this.store.select(UserSelectors.selectIsLoading);
  error$ = this.store.select(UserSelectors.selectUserError);
  companies$ = this.store.select(UserSelectors.selectUniqueCompanies);
  validationStates$ = this.store.select(UserSelectors.selectValidationStates);

  // === UI Properties ===
  get isCreateMode(): boolean {
    return this.mode === 'create';
  }

  get isEditMode(): boolean {
    return this.mode === 'edit';
  }

  get isViewMode(): boolean {
    return this.mode === 'view';
  }

  get dialogTitle(): string {
    switch (this.mode) {
      case 'create':
        return 'users.createUser';
      case 'edit':
        return 'users.editUser';
      case 'view':
        return 'users.viewUser';
      default:
        return 'users.user';
    }
  }

  constructor() {
    this.userForm = this.createForm();
  }

  ngOnInit(): void {
    // Cập nhật validation cho password dựa trên mode
    this.updatePasswordValidation();

    // Điền dữ liệu form nếu là edit/view mode
    if (this.user && (this.isEditMode || this.isViewMode)) {
      this.populateForm();
    }

    // Disable form nếu là view mode
    if (this.isViewMode) {
      this.userForm.disable();
    }

    // Subscribe to form changes để validate realtime
    this.setupFormValidation();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // === Form Creation ===
  private createForm(): FormGroup {
    return this.fb.group({
      fullName: ['', [Validators.required, Validators.maxLength(200)]],
      userName: ['', [Validators.required, Validators.maxLength(100), Validators.pattern(/^[a-zA-Z0-9._-]+$/)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(200)]],
      password: [''], // Sẽ được cập nhật validation trong updatePasswordValidation
      employeeCode: ['', [Validators.maxLength(50)]],
      phoneNumber: ['', [Validators.maxLength(20)]],
      dateOfBirth: [''],
      gender: [null],
      position: ['', [Validators.maxLength(100)]],
      department: ['', [Validators.maxLength(100)]],
      companyId: [''],
      startDate: [''],
      salary: [null, [Validators.min(0)]],
      address: ['', [Validators.maxLength(500)]],
      isActive: [true],
      notes: ['', [Validators.maxLength(1000)]]
    });
  }

  private updatePasswordValidation(): void {
    const passwordControl = this.userForm.get('password');
    if (passwordControl) {
      if (this.isCreateMode) {
        passwordControl.setValidators([Validators.required, Validators.minLength(6)]);
      } else {
        passwordControl.clearValidators();
      }
      passwordControl.updateValueAndValidity();
    }
  }

  private populateForm(): void {
    if (!this.user) return;

    this.userForm.patchValue({
      fullName: this.user.fullName || '',
      userName: this.user.userName || '',
      email: this.user.email || '',
      // Không populate password vì lý do bảo mật
      employeeCode: this.user.employeeCode || '',
      phoneNumber: this.user.phoneNumber || '',
      dateOfBirth: this.user.dateOfBirth || '',
      gender: this.user.gender || null,
      position: this.user.position || '',
      department: this.user.department || '',
      companyId: this.user.companyId || '',
      startDate: this.user.startDate || '',
      salary: this.user.salary || null,
      address: this.user.address || '',
      isActive: this.user.isActive ?? true,
      notes: this.user.notes || ''
    });
  }

  private setupFormValidation(): void {
    // Validate username uniqueness
    this.userForm.get('userName')?.valueChanges.pipe(
      takeUntil(this.destroy$)
    ).subscribe(userName => {
      if (userName && userName !== this.user?.userName) {
        this.store.dispatch(UserActions.validateUsername({
          userName,
          excludeId: this.user?.id
        }));
      }
    });

    // Validate email uniqueness
    this.userForm.get('email')?.valueChanges.pipe(
      takeUntil(this.destroy$)
    ).subscribe(email => {
      if (email && email !== this.user?.email) {
        this.store.dispatch(UserActions.validateEmail({
          email,
          excludeId: this.user?.id
        }));
      }
    });

    // Validate employee code uniqueness
    this.userForm.get('employeeCode')?.valueChanges.pipe(
      takeUntil(this.destroy$)
    ).subscribe(employeeCode => {
      if (employeeCode && employeeCode !== this.user?.employeeCode) {
        this.store.dispatch(UserActions.validateEmployeeCode({
          employeeCode,
          excludeId: this.user?.id
        }));
      }
    });
  }

  // === Form Actions ===
  onSubmit(): void {
    if (this.userForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    const formValue = this.userForm.value;
    const userInput: CreateUpdateSmartBoxUserDto = {
      fullName: formValue.fullName,
      userName: formValue.userName,
      email: formValue.email,
      password: this.isCreateMode ? formValue.password : undefined, // Chỉ gửi password khi tạo mới
      employeeCode: formValue.employeeCode,
      phoneNumber: formValue.phoneNumber,
      dateOfBirth: formValue.dateOfBirth || undefined,
      gender: formValue.gender || undefined,
      position: formValue.position,
      department: formValue.department,
      companyId: formValue.companyId,
      startDate: formValue.startDate || undefined,
      salary: formValue.salary || undefined,
      address: formValue.address,
      isActive: formValue.isActive,
      notes: formValue.notes,
      roleIds: this.user?.roles?.map(r => r.id) || [] // Giữ nguyên roles hiện tại hoặc rỗng nếu tạo mới
    };

    if (this.isCreateMode) {
      this.store.dispatch(UserActions.createUser({ input: userInput }));
    } else if (this.isEditMode && this.user) {
      this.store.dispatch(UserActions.updateUser({ 
        id: this.user.id, 
        input: userInput 
      }));
    }

    this.dialogRef.close(true);
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

  // === Utility Methods ===
  private markFormGroupTouched(): void {
    Object.keys(this.userForm.controls).forEach(key => {
      const control = this.userForm.get(key);
      control?.markAsTouched();
    });
  }

  getFieldError(fieldName: string): string {
    const control = this.userForm.get(fieldName);
    if (control && control.invalid && control.touched) {
      if (control.errors?.['required']) {
        return 'validation.required';
      }
      if (control.errors?.['email']) {
        return 'validation.email';
      }
      if (control.errors?.['maxlength']) {
        return 'validation.maxLength';
      }
      if (control.errors?.['minlength']) {
        return 'validation.minLength';
      }
      if (control.errors?.['pattern']) {
        return 'validation.pattern';
      }
      if (control.errors?.['min']) {
        return 'validation.min';
      }
    }
    return '';
  }

  isFieldInvalid(fieldName: string): boolean {
    const control = this.userForm.get(fieldName);
    return !!(control && control.invalid && control.touched);
  }
} 