/**
 * Custom Validators
 * Tập hợp custom validators với strong typing và detailed error messages
 */

import { AbstractControl, ValidationErrors, ValidatorFn, AsyncValidatorFn } from '@angular/forms';
import { Observable, of, timer } from 'rxjs';
import { map, switchMap, catchError } from 'rxjs/operators';

// =============================================================================
// VALIDATION ERROR TYPES
// =============================================================================

/** Validation error với typed error information */
export interface TypedValidationError {
  readonly errorCode: string;
  readonly message: string;
  readonly actualValue?: unknown;
  readonly expectedValue?: unknown;
  readonly additionalInfo?: Record<string, unknown>;
}

/** Async validation result */
export interface AsyncValidationResult {
  readonly isValid: boolean;
  readonly error?: TypedValidationError;
}

// =============================================================================
// VALIDATION RESULT CREATORS
// =============================================================================

/** Tạo validation error với proper typing */
function createValidationError(
  errorCode: string,
  message: string,
  actualValue?: unknown,
  expectedValue?: unknown,
  additionalInfo?: Record<string, unknown>
): ValidationErrors {
  const error: TypedValidationError = {
    errorCode,
    message,
    actualValue,
    expectedValue,
    additionalInfo
  };
  return { [errorCode]: error };
}

/** Tạo async validation error */
function createAsyncValidationError(error: TypedValidationError): ValidationErrors {
  return { [error.errorCode]: error };
}

// =============================================================================
// STRING VALIDATORS
// =============================================================================

/**
 * Validator kiểm tra minimum length với custom message
 */
export function minLengthValidator(minLength: number, message?: string): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value || control.value.length >= minLength) {
      return null;
    }

    return createValidationError(
      'minLength',
      message || `Tối thiểu ${minLength} ký tự`,
      control.value?.length,
      minLength,
      { requiredLength: minLength, actualLength: control.value?.length || 0 }
    );
  };
}

/**
 * Validator kiểm tra maximum length với custom message
 */
export function maxLengthValidator(maxLength: number, message?: string): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value || control.value.length <= maxLength) {
      return null;
    }

    return createValidationError(
      'maxLength',
      message || `Tối đa ${maxLength} ký tự`,
      control.value.length,
      maxLength,
      { allowedLength: maxLength, actualLength: control.value.length }
    );
  };
}

/**
 * Validator kiểm tra pattern với custom message
 */
export function patternValidator(pattern: RegExp, message: string): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value || pattern.test(control.value)) {
      return null;
    }

    return createValidationError(
      'pattern',
      message,
      control.value,
      pattern.toString(),
      { pattern: pattern.source }
    );
  };
}

/**
 * Validator kiểm tra chỉ chứa chữ cái và space
 */
export function alphabeticValidator(message?: string): ValidatorFn {
  const pattern = /^[a-zA-ZÀ-ÿ\u0100-\u017F\u1EA0-\u1EF9\s]+$/;
  return patternValidator(
    pattern,
    message || 'Chỉ được chứa chữ cái và khoảng trắng'
  );
}

/**
 * Validator kiểm tra không chứa ký tự đặc biệt
 */
export function noSpecialCharactersValidator(message?: string): ValidatorFn {
  const pattern = /^[a-zA-Z0-9\s]+$/;
  return patternValidator(
    pattern,
    message || 'Không được chứa ký tự đặc biệt'
  );
}

// =============================================================================
// EMAIL & CONTACT VALIDATORS
// =============================================================================

/**
 * Enhanced email validator
 */
export function emailValidator(message?: string): ValidatorFn {
  const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
  return patternValidator(
    emailPattern,
    message || 'Email không hợp lệ'
  );
}

/**
 * Vietnamese phone number validator
 */
export function vietnamesePhoneValidator(message?: string): ValidatorFn {
  const phonePattern = /^(0|\+84)(3|5|7|8|9)[0-9]{8}$/;
  return patternValidator(
    phonePattern,
    message || 'Số điện thoại không hợp lệ (VD: 0987654321)'
  );
}

// =============================================================================
// NUMERIC VALIDATORS
// =============================================================================

/**
 * Validator kiểm tra giá trị minimum
 */
export function minValueValidator(minValue: number, message?: string): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value || Number(control.value) >= minValue) {
      return null;
    }

    return createValidationError(
      'minValue',
      message || `Giá trị tối thiểu là ${minValue}`,
      Number(control.value),
      minValue,
      { min: minValue, actual: Number(control.value) }
    );
  };
}

/**
 * Validator kiểm tra giá trị maximum
 */
export function maxValueValidator(maxValue: number, message?: string): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value || Number(control.value) <= maxValue) {
      return null;
    }

    return createValidationError(
      'maxValue',
      message || `Giá trị tối đa là ${maxValue}`,
      Number(control.value),
      maxValue,
      { max: maxValue, actual: Number(control.value) }
    );
  };
}

/**
 * Validator kiểm tra số nguyên dương
 */
export function positiveIntegerValidator(message?: string): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return null;
    }

    const value = Number(control.value);
    if (Number.isInteger(value) && value > 0) {
      return null;
    }

    return createValidationError(
      'positiveInteger',
      message || 'Phải là số nguyên dương',
      control.value,
      'positive integer'
    );
  };
}

/**
 * Validator kiểm tra range
 */
export function rangeValidator(min: number, max: number, message?: string): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return null;
    }

    const value = Number(control.value);
    if (value >= min && value <= max) {
      return null;
    }

    return createValidationError(
      'range',
      message || `Giá trị phải từ ${min} đến ${max}`,
      value,
      `${min}-${max}`,
      { min, max, actual: value }
    );
  };
}

// =============================================================================
// DATE VALIDATORS
// =============================================================================

/**
 * Validator kiểm tra ngày trong quá khứ
 */
export function pastDateValidator(message?: string): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return null;
    }

    const inputDate = new Date(control.value);
    const today = new Date();
    today.setHours(23, 59, 59, 999); // End of today

    if (inputDate <= today) {
      return null;
    }

    return createValidationError(
      'pastDate',
      message || 'Ngày phải trong quá khứ hoặc hôm nay',
      inputDate.toISOString(),
      today.toISOString()
    );
  };
}

/**
 * Validator kiểm tra ngày trong tương lai
 */
export function futureDateValidator(message?: string): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return null;
    }

    const inputDate = new Date(control.value);
    const today = new Date();
    today.setHours(0, 0, 0, 0); // Start of today

    if (inputDate >= today) {
      return null;
    }

    return createValidationError(
      'futureDate',
      message || 'Ngày phải trong tương lai hoặc hôm nay',
      inputDate.toISOString(),
      today.toISOString()
    );
  };
}

/**
 * Validator kiểm tra tuổi tối thiểu
 */
export function minimumAgeValidator(minimumAge: number, message?: string): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return null;
    }

    const birthDate = new Date(control.value);
    const today = new Date();
    const age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();
    
    // Adjust age if birthday hasn't occurred this year yet
    const actualAge = monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate()) 
      ? age - 1 
      : age;

    if (actualAge >= minimumAge) {
      return null;
    }

    return createValidationError(
      'minimumAge',
      message || `Tuổi tối thiểu là ${minimumAge}`,
      actualAge,
      minimumAge,
      { birthDate: birthDate.toISOString(), calculatedAge: actualAge }
    );
  };
}

// =============================================================================
// ASYNC VALIDATORS
// =============================================================================

/**
 * Async validator kiểm tra email unique
 */
export function uniqueEmailValidator(
  checkEmailFn: (email: string) => Observable<boolean>,
  message?: string
): AsyncValidatorFn {
  return (control: AbstractControl): Observable<ValidationErrors | null> => {
    if (!control.value) {
      return of(null);
    }

    return timer(300).pipe( // Debounce
      switchMap(() => checkEmailFn(control.value)),
      map(isUnique => {
        if (isUnique) {
          return null;
        }
        return createAsyncValidationError({
          errorCode: 'uniqueEmail',
          message: message || 'Email đã được sử dụng',
          actualValue: control.value
        });
      }),
      catchError(() => of(null)) // If check fails, allow validation to pass
    );
  };
}

/**
 * Async validator kiểm tra username unique
 */
export function uniqueUsernameValidator(
  checkUsernameFn: (username: string) => Observable<boolean>,
  message?: string
): AsyncValidatorFn {
  return (control: AbstractControl): Observable<ValidationErrors | null> => {
    if (!control.value) {
      return of(null);
    }

    return timer(300).pipe( // Debounce
      switchMap(() => checkUsernameFn(control.value)),
      map(isUnique => {
        if (isUnique) {
          return null;
        }
        return createAsyncValidationError({
          errorCode: 'uniqueUsername',
          message: message || 'Tên đăng nhập đã được sử dụng',
          actualValue: control.value
        });
      }),
      catchError(() => of(null)) // If check fails, allow validation to pass
    );
  };
}

// =============================================================================
// COMPLEX VALIDATORS
// =============================================================================

/**
 * Validator so sánh hai field (confirm password, etc.)
 */
export function matchFieldValidator(
  matchFieldName: string,
  message?: string
): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.parent) {
      return null;
    }

    const matchControl = control.parent.get(matchFieldName);
    if (!matchControl) {
      return null;
    }

    if (control.value === matchControl.value) {
      return null;
    }

    return createValidationError(
      'matchField',
      message || `Giá trị phải trùng với ${matchFieldName}`,
      control.value,
      matchControl.value,
      { matchFieldName }
    );
  };
}

/**
 * Password strength validator
 */
export function passwordStrengthValidator(message?: string): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return null;
    }

    const password = control.value;
    const hasUpperCase = /[A-Z]/.test(password);
    const hasLowerCase = /[a-z]/.test(password);
    const hasNumbers = /\d/.test(password);
    const hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]/.test(password);
    const hasMinLength = password.length >= 8;

    const missingRequirements: string[] = [];
    if (!hasUpperCase) missingRequirements.push('chữ hoa');
    if (!hasLowerCase) missingRequirements.push('chữ thường');
    if (!hasNumbers) missingRequirements.push('số');
    if (!hasSpecialChar) missingRequirements.push('ký tự đặc biệt');
    if (!hasMinLength) missingRequirements.push('tối thiểu 8 ký tự');

    if (missingRequirements.length === 0) {
      return null;
    }

    return createValidationError(
      'passwordStrength',
      message || `Mật khẩu phải có: ${missingRequirements.join(', ')}`,
      password,
      'strong password',
      {
        missingRequirements,
        hasUpperCase,
        hasLowerCase,
        hasNumbers,
        hasSpecialChar,
        hasMinLength
      }
    );
  };
}

// =============================================================================
// UTILITY FUNCTIONS
// =============================================================================

/**
 * Lấy error message từ validation errors
 */
export function getValidationErrorMessage(errors: ValidationErrors): string {
  const firstError = Object.values(errors)[0] as TypedValidationError;
  return firstError?.message || 'Dữ liệu không hợp lệ';
}

/**
 * Kiểm tra có validation error hay không
 */
export function hasValidationError(errors: ValidationErrors | null, errorCode: string): boolean {
  return errors?.[errorCode] !== undefined;
}

/**
 * Lấy chi tiết error cho debugging
 */
export function getValidationErrorDetails(errors: ValidationErrors): TypedValidationError[] {
  return Object.values(errors) as TypedValidationError[];
} 