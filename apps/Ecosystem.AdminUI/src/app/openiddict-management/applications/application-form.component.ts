import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { ToasterService } from '@abp/ng.theme.shared';
import {
  ApplicationModel,
  CreateApplicationModel,
  UpdateApplicationModel
} from './models/application.model';
import { OpenIddictApplicationsService } from './openiddict-applications.service';

// OpenIddict constants (moved from DTO)
export const OpenIddictConstants = {
  ApplicationType_Confidential: 'confidential',
  ApplicationType_Public: 'public',
  GrantType_AuthorizationCode: 'authorization_code',
  GrantType_ClientCredentials: 'client_credentials',
  GrantType_RefreshToken: 'refresh_token',
  GrantType_Password: 'password',
  GrantType_Implicit: 'implicit',
  GrantType_DeviceCode: 'urn:ietf:params:oauth:grant-type:device_code',
  ResponseType_Code: 'code',
  ResponseType_Token: 'token',
  ResponseType_IdToken: 'id_token',
  ConsentType_Explicit: 'explicit',
  ConsentType_External: 'external',
  ConsentType_Implicit: 'implicit',
  ConsentType_Systematic: 'systematic',
  Scope_OpenId: 'openid',
  Scope_Profile: 'profile',
  Scope_Email: 'email',
  Scope_Address: 'address',
  Scope_Phone: 'phone',
  Scope_Roles: 'roles',
};

@Component({
  selector: 'app-application-form',
  templateUrl: './application-form.component.html',
  styleUrls: ['./application-form.component.scss']
})
export class ApplicationFormComponent implements OnInit, OnChanges {
  @Input() application: ApplicationModel | null = null;
  @Input() isVisible = false;
  @Output() saved = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  form: FormGroup;
  isLoading = false;

  // OpenIddict Constants cho dropdowns
  applicationTypes = [
    { value: OpenIddictConstants.ApplicationType_Confidential, label: 'Confidential Application' },
    { value: OpenIddictConstants.ApplicationType_Public, label: 'Public Application' }
  ];

  consentTypes = [
    { value: OpenIddictConstants.ConsentType_Explicit, label: 'Explicit' },
    { value: OpenIddictConstants.ConsentType_External, label: 'External' },
    { value: OpenIddictConstants.ConsentType_Implicit, label: 'Implicit' },
    { value: OpenIddictConstants.ConsentType_Systematic, label: 'Systematic' }
  ];

  grantTypes = [
    { value: OpenIddictConstants.GrantType_AuthorizationCode, label: 'Authorization Code' },
    { value: OpenIddictConstants.GrantType_ClientCredentials, label: 'Client Credentials' },
    { value: OpenIddictConstants.GrantType_RefreshToken, label: 'Refresh Token' },
    { value: OpenIddictConstants.GrantType_Password, label: 'Password' },
    { value: OpenIddictConstants.GrantType_Implicit, label: 'Implicit' },
    { value: OpenIddictConstants.GrantType_DeviceCode, label: 'Device Code' }
  ];

  responseTypes = [
    { value: OpenIddictConstants.ResponseType_Code, label: 'Code' },
    { value: OpenIddictConstants.ResponseType_Token, label: 'Token' },
    { value: OpenIddictConstants.ResponseType_IdToken, label: 'ID Token' }
  ];

  scopes = [
    { value: OpenIddictConstants.Scope_OpenId, label: 'OpenID' },
    { value: OpenIddictConstants.Scope_Profile, label: 'Profile' },
    { value: OpenIddictConstants.Scope_Email, label: 'Email' },
    { value: OpenIddictConstants.Scope_Address, label: 'Address' },
    { value: OpenIddictConstants.Scope_Phone, label: 'Phone' },
    { value: OpenIddictConstants.Scope_Roles, label: 'Roles' }
  ];

  constructor(
    private fb: FormBuilder,
    private applicationService: OpenIddictApplicationsService,
    private toasterService: ToasterService
  ) {
    this.buildForm();
  }

  ngOnInit(): void {
    this.initializeForm();
    
    // Listen for application type changes to update client secret validation
    this.form.get('type')?.valueChanges.subscribe(type => {
      this.updateClientSecretValidation(type);
    });
    
    // Set initial validation based on current type
    this.updateClientSecretValidation(this.form.get('type')?.value);
  }

  ngOnChanges(changes: SimpleChanges): void {
    // When the application input changes, update the form
    if (changes['application'] && this.form) {
      this.initializeForm();
    }
  }

  private buildForm(): void {
    this.form = this.fb.group({
      clientId: ['', [Validators.required]],
      displayName: [''],
      clientSecret: [''],
      type: [OpenIddictConstants.ApplicationType_Public, [Validators.required]], // Changed to public as default
      consentType: [OpenIddictConstants.ConsentType_Explicit, [Validators.required]],
      clientUri: [''],
      logoUri: [''],
      grantTypes: this.fb.array([]),
      responseTypes: this.fb.array([]),
      scopes: this.fb.array([]),
      redirectUris: this.fb.array([]),
      postLogoutRedirectUris: this.fb.array([])
    });
  }
  
  private setDefaultGrantTypes(): void {
    // Add default grant types for new applications
    // This matches the curl example: ["authorization_code", "refresh_token"]
    this.grantTypesArray.push(this.fb.control(OpenIddictConstants.GrantType_AuthorizationCode));
    this.grantTypesArray.push(this.fb.control(OpenIddictConstants.GrantType_RefreshToken));
    
    // Add default response type: ["code"]
    this.responseTypesArray.push(this.fb.control(OpenIddictConstants.ResponseType_Code));
    
    // Add default scopes matching the curl example: ["openid", "profile", "email", "roles"]
    this.scopesArray.push(this.fb.control(OpenIddictConstants.Scope_OpenId));
    this.scopesArray.push(this.fb.control(OpenIddictConstants.Scope_Profile));
    this.scopesArray.push(this.fb.control(OpenIddictConstants.Scope_Email));
    this.scopesArray.push(this.fb.control(OpenIddictConstants.Scope_Roles));
  }

  private updateForm(): void {
    if (!this.application) return;
    
    this.form.patchValue({
      clientId: this.application.clientId,
      displayName: this.application.displayName,
      clientSecret: this.application.clientSecret,
      type: this.application.type,
      consentType: this.application.consentType,
      clientUri: this.application.clientUri,
      logoUri: this.application.logoUri
    });
    
    this.updateFormArray('grantTypes', this.application.grantTypes || []);
    this.updateFormArray('responseTypes', this.application.responseTypes || []);
    this.updateFormArray('scopes', this.application.scopes || []);
    this.updateFormArray('redirectUris', this.application.redirectUris || []);
    this.updateFormArray('postLogoutRedirectUris', this.application.postLogoutRedirectUris || []);
  }

  private updateFormArray(controlName: string, values: string[]): void {
    const formArray = this.form.get(controlName) as FormArray;
    formArray.clear();
    values.forEach(value => {
      formArray.push(this.fb.control(value));
    });
  }

  private updateClientSecretValidation(type: string): void {
    const clientSecretControl = this.form.get('clientSecret');
    if (type === OpenIddictConstants.ApplicationType_Confidential) {
      clientSecretControl?.setValidators([Validators.required]);
      clientSecretControl?.enable();
    } else {
      // For public applications, clear validators, clear value, and disable the field
      clientSecretControl?.clearValidators();
      clientSecretControl?.setValue('');
      clientSecretControl?.disable();
    }
    clientSecretControl?.updateValueAndValidity();
  }

  private initializeForm(): void {
    if (this.application) {
      this.updateForm();
    } else {
      // Reset form for new applications and set defaults
      this.resetFormForNewApplication();
    }
  }

  private resetFormForNewApplication(): void {
    this.form.reset({
      clientId: '',
      displayName: '',
      clientSecret: '',
      type: OpenIddictConstants.ApplicationType_Public, // Changed to public to match curl example
      consentType: OpenIddictConstants.ConsentType_Explicit,
      clientUri: '',
      logoUri: ''
    });
    
    // Clear all arrays
    this.grantTypesArray.clear();
    this.responseTypesArray.clear();
    this.scopesArray.clear();
    this.redirectUrisArray.clear();
    this.postLogoutRedirectUrisArray.clear();
    
    // Set default grant types for new applications
    this.setDefaultGrantTypes();
  }

  get grantTypesArray(): FormArray {
    return this.form.get('grantTypes') as FormArray;
  }
  get responseTypesArray(): FormArray {
    return this.form.get('responseTypes') as FormArray;
  }
  get scopesArray(): FormArray {
    return this.form.get('scopes') as FormArray;
  }
  get redirectUrisArray(): FormArray {
    return this.form.get('redirectUris') as FormArray;
  }
  get postLogoutRedirectUrisArray(): FormArray {
    return this.form.get('postLogoutRedirectUris') as FormArray;
  }

  addRedirectUri(): void {
    this.redirectUrisArray.push(this.fb.control(''));
  }
  removeRedirectUri(index: number): void {
    this.redirectUrisArray.removeAt(index);
  }
  addPostLogoutUri(): void {
    this.postLogoutRedirectUrisArray.push(this.fb.control(''));
  }
  removePostLogoutUri(index: number): void {
    this.postLogoutRedirectUrisArray.removeAt(index);
  }

  onGrantTypeChange(grantType: string, checked: boolean): void {
    if (checked) {
      this.grantTypesArray.push(this.fb.control(grantType));
      
      // Auto-add recommended response types and scopes based on grant type
      if (grantType === OpenIddictConstants.GrantType_AuthorizationCode) {
        // Add 'code' response type if not already present
        if (!this.isResponseTypeSelected(OpenIddictConstants.ResponseType_Code)) {
          this.responseTypesArray.push(this.fb.control(OpenIddictConstants.ResponseType_Code));
        }
        
        // Add 'openid' scope if not already present
        if (!this.isScopeSelected(OpenIddictConstants.Scope_OpenId)) {
          this.scopesArray.push(this.fb.control(OpenIddictConstants.Scope_OpenId));
        }
        
        // Add default redirect URIs if none exist
        if (this.redirectUrisArray.length === 0) {
          this.addRedirectUri();
          this.addRedirectUri(); // Add two empty slots to match curl example
        }
        
        // Add default post logout redirect URIs if none exist
        if (this.postLogoutRedirectUrisArray.length === 0) {
          this.addPostLogoutUri();
          this.addPostLogoutUri(); // Add two empty slots to match curl example
        }
      }
    } else {
      const index = this.grantTypesArray.controls.findIndex(control => control.value === grantType);
      if (index !== -1) {
        this.grantTypesArray.removeAt(index);
      }
    }
  }
  onResponseTypeChange(responseType: string, checked: boolean): void {
    if (checked) {
      this.responseTypesArray.push(this.fb.control(responseType));
    } else {
      const index = this.responseTypesArray.controls.findIndex(control => control.value === responseType);
      if (index !== -1) {
        this.responseTypesArray.removeAt(index);
      }
    }
  }
  onScopeChange(scope: string, checked: boolean): void {
    if (checked) {
      this.scopesArray.push(this.fb.control(scope));
    } else {
      const index = this.scopesArray.controls.findIndex(control => control.value === scope);
      if (index !== -1) {
        this.scopesArray.removeAt(index);
      }
    }
  }

  isGrantTypeSelected(grantType: string): boolean {
    return this.grantTypesArray.controls.some(control => control.value === grantType);
  }
  isResponseTypeSelected(responseType: string): boolean {
    return this.responseTypesArray.controls.some(control => control.value === responseType);
  }
  isScopeSelected(scope: string): boolean {
    return this.scopesArray.controls.some(control => control.value === scope);
  }

  /**
   * Check if the form is ready for submission with detailed feedback
   */
  isFormReady(): boolean {
    if (this.form.invalid) {
      return false;
    }
    
    const formValue = this.form.value;
    
    // Check grant types
    if (!formValue.grantTypes || formValue.grantTypes.length === 0) {
      return false;
    }
    
    // Check authorization_code specific requirements
    const hasAuthorizationCode = formValue.grantTypes.includes(OpenIddictConstants.GrantType_AuthorizationCode);
    if (hasAuthorizationCode) {
      const redirectUris = formValue.redirectUris.filter((uri: string) => uri && uri.trim());
      if (redirectUris.length === 0) {
        return false;
      }
    }
    
    return true;
  }

  /**
   * Get form validation summary for user feedback
   */
  getValidationSummary(): string[] {
    const issues: string[] = [];
    
    if (this.form.get('clientId')?.invalid) {
      issues.push('Client ID is required');
    }
    
    if (this.form.get('type')?.invalid) {
      issues.push('Application Type is required');
    }
    
    if (this.form.get('consentType')?.invalid) {
      issues.push('Consent Type is required');
    }
    
    if (this.form.get('type')?.value === OpenIddictConstants.ApplicationType_Confidential && 
        this.form.get('clientSecret')?.invalid) {
      issues.push('Client Secret is required for confidential applications');
    }
    
    if (this.grantTypesArray.length === 0) {
      issues.push('At least one grant type must be selected');
    }
    
    const formValue = this.form.value;
    const hasAuthorizationCode = formValue.grantTypes?.includes(OpenIddictConstants.GrantType_AuthorizationCode);
    if (hasAuthorizationCode) {
      const redirectUris = formValue.redirectUris?.filter((uri: string) => uri && uri.trim()) || [];
      if (redirectUris.length === 0) {
        issues.push('Redirect URIs are required for Authorization Code grant type');
      }
    }
    
    return issues;
  }

  onSave(): void {
    if (this.form.invalid) {
      // Mark all fields as touched to show validation errors
      Object.keys(this.form.controls).forEach(key => {
        this.form.get(key)?.markAsTouched();
      });
      
      const validationIssues = this.getValidationSummary();
      if (validationIssues.length > 0) {
        this.toasterService.error(
          `Vui lòng kiểm tra lại thông tin form:<br>• ${validationIssues.join('<br>• ')}`, 
          'Lỗi validation'
        );
      } else {
        this.toasterService.error('Vui lòng kiểm tra lại thông tin form', 'Lỗi validation');
      }
      return;
    }

    // Additional validation using the new methods
    if (!this.isFormReady()) {
      const validationIssues = this.getValidationSummary();
      this.toasterService.error(
        `Vui lòng hoàn thiện thông tin form:<br>• ${validationIssues.join('<br>• ')}`, 
        'Lỗi validation'
      );
      return;
    }

    // Validate required fields for OpenIddict
    const formValue = this.form.value;
    if (!formValue.clientId) {
      this.toasterService.error('Client ID là bắt buộc', 'Lỗi validation');
      return;
    }
    if (!formValue.type) {
      this.toasterService.error('Application Type là bắt buộc', 'Lỗi validation');
      return;
    }
    if (!formValue.consentType) {
      this.toasterService.error('Consent Type là bắt buộc', 'Lỗi validation');
      return;
    }

    // For confidential applications, client secret is required
    if (formValue.type === OpenIddictConstants.ApplicationType_Confidential && !formValue.clientSecret) {
      this.toasterService.error('Client Secret là bắt buộc cho Confidential Application', 'Lỗi validation');
      return;
    }

    // Validate at least one grant type is selected
    if (!formValue.grantTypes || formValue.grantTypes.length === 0) {
      this.toasterService.error('Vui lòng chọn ít nhất một Grant Type', 'Lỗi validation');
      return;
    }

    // Validate authorization_code specific requirements
    const hasAuthorizationCode = formValue.grantTypes.includes(OpenIddictConstants.GrantType_AuthorizationCode);
    if (hasAuthorizationCode) {
      // Check if redirectUris is required and provided
      const redirectUris = formValue.redirectUris.filter((uri: string) => uri && uri.trim());
      if (redirectUris.length === 0) {
        this.toasterService.error('Redirect URIs là bắt buộc khi sử dụng Authorization Code grant type', 'Lỗi validation');
        return;
      }

      // Validate redirect URIs format
      for (const uri of redirectUris) {
        try {
          new URL(uri);
        } catch {
          this.toasterService.error(`Redirect URI không hợp lệ: ${uri}`, 'Lỗi validation');
          return;
        }
      }

      // Validate post logout redirect URIs format (if provided)
      const postLogoutUris = formValue.postLogoutRedirectUris.filter((uri: string) => uri && uri.trim());
      if (postLogoutUris.length > 0) {
        for (const uri of postLogoutUris) {
          try {
            new URL(uri);
          } catch {
            this.toasterService.error(`Post Logout Redirect URI không hợp lệ: ${uri}`, 'Lỗi validation');
            return;
          }
        }
      }
      
      // Recommend 'code' response type for authorization_code grant
      if (!formValue.responseTypes.includes(OpenIddictConstants.ResponseType_Code)) {
        this.toasterService.warn('Khuyến nghị sử dụng "Code" response type với Authorization Code grant type', 'Gợi ý');
      }
      
      // Recommend 'openid' scope for authorization_code grant
      if (!formValue.scopes.includes(OpenIddictConstants.Scope_OpenId)) {
        this.toasterService.warn('Khuyến nghị sử dụng "OpenID" scope với Authorization Code grant type', 'Gợi ý');
      }
    }

    // Validate other grant types if they require specific settings
    if (formValue.grantTypes.includes(OpenIddictConstants.GrantType_Implicit)) {
      // Implicit grant should have token or id_token response type
      if (!formValue.responseTypes.includes(OpenIddictConstants.ResponseType_Token) && 
          !formValue.responseTypes.includes(OpenIddictConstants.ResponseType_IdToken)) {
        this.toasterService.warn('Implicit grant type thường sử dụng "Token" hoặc "ID Token" response type', 'Gợi ý');
      }
    }

    this.isLoading = true;
    
    // Build the DTO according to ABP OpenIddict format
    const dto = {
      clientId: formValue.clientId,
      displayName: formValue.displayName || null,
      // Only include clientSecret for confidential applications
      ...(formValue.type === OpenIddictConstants.ApplicationType_Confidential && formValue.clientSecret 
          ? { clientSecret: formValue.clientSecret } 
          : {}),
      type: formValue.type,
      consentType: formValue.consentType,
      clientUri: formValue.clientUri || null,
      logoUri: formValue.logoUri || null,
      grantTypes: formValue.grantTypes || [],
      responseTypes: formValue.responseTypes || [],
      scopes: formValue.scopes || [],
      redirectUris: formValue.redirectUris.filter((uri: string) => uri && uri.trim()),
      postLogoutRedirectUris: formValue.postLogoutRedirectUris.filter((uri: string) => uri && uri.trim()),
      // Add OpenIddict specific permissions based on grant types
      permissions: this.generatePermissions(formValue.grantTypes || [], formValue.responseTypes || [])
    };
    
    // Log the payload for debugging (matches the curl example format)
    console.log('OpenIddict Application Payload:', JSON.stringify(dto, null, 2));
    
    // Additional logging to verify the payload structure
    console.log('Payload validation:');
    console.log('- Client ID:', dto.clientId);
    console.log('- Type:', dto.type);
    console.log('- Grant Types:', dto.grantTypes);
    console.log('- Response Types:', dto.responseTypes);
    console.log('- Scopes:', dto.scopes);
    console.log('- Redirect URIs:', dto.redirectUris);
    console.log('- Post Logout URIs:', dto.postLogoutRedirectUris);
    console.log('- Permissions:', dto.permissions);
    
    const operation$ = this.application
      ? this.applicationService.update(this.application.id, dto as UpdateApplicationModel)
      : this.applicationService.create(dto as CreateApplicationModel);
    
    operation$.subscribe({
      next: () => {
        this.isLoading = false;
        this.toasterService.success('Lưu application thành công', 'Thành công');
        this.saved.emit();
      },
      error: (error) => {
        this.isLoading = false;
        let errorMessage = 'Lỗi không xác định';
        
        if (error.error?.error?.message) {
          errorMessage = error.error.error.message;
        } else if (error.error?.message) {
          errorMessage = error.error.message;
        } else if (error.message) {
          errorMessage = error.message;
        }
        
        // Check for specific OpenIddict validation errors
        if (errorMessage.includes('Only confidential or public applications are supported')) {
          errorMessage = 'Chỉ hỗ trợ Confidential hoặc Public applications. Vui lòng chọn đúng Application Type.';
        } else if (errorMessage.includes('A client secret cannot be associated with a public application')) {
          errorMessage = 'Không thể gán Client Secret cho Public Application. Vui lòng xóa Client Secret hoặc chọn Confidential Application.';
        }
        
        this.toasterService.error(errorMessage, 'Lỗi');
        console.error('Error saving application:', error);
      }
    });
  }

  onCancel(): void {
    this.cancelled.emit();
  }

  /**
   * Generate OpenIddict permissions based on grant types and response types
   * This matches the format expected by the ABP OpenIddict backend
   */
  private generatePermissions(grantTypes: string[], responseTypes: string[]): string[] {
    const permissions: string[] = [];

    // Basic endpoint permissions based on grant types
    if (grantTypes.includes(OpenIddictConstants.GrantType_AuthorizationCode) || 
        grantTypes.includes(OpenIddictConstants.GrantType_Implicit)) {
      permissions.push('ept:authorization');
    }

    if (grantTypes.includes(OpenIddictConstants.GrantType_AuthorizationCode) || 
        grantTypes.includes(OpenIddictConstants.GrantType_ClientCredentials) ||
        grantTypes.includes(OpenIddictConstants.GrantType_RefreshToken) ||
        grantTypes.includes(OpenIddictConstants.GrantType_Password)) {
      permissions.push('ept:token');
    }

    // Logout permission for authorization code flow
    if (grantTypes.includes(OpenIddictConstants.GrantType_AuthorizationCode)) {
      permissions.push('ept:logout');
    }

    // Response type permissions (standard OpenIddict format)
    if (responseTypes.includes(OpenIddictConstants.ResponseType_Code)) {
      permissions.push('rst:code');
    }
    if (responseTypes.includes(OpenIddictConstants.ResponseType_Token)) {
      permissions.push('rst:token');
    }
    if (responseTypes.includes(OpenIddictConstants.ResponseType_IdToken)) {
      permissions.push('rst:id_token');
    }

    // Grant type permissions (standard OpenIddict format)
    grantTypes.forEach(grantType => {
      switch (grantType) {
        case OpenIddictConstants.GrantType_AuthorizationCode:
          permissions.push('gt:authorization_code');
          break;
        case OpenIddictConstants.GrantType_ClientCredentials:
          permissions.push('gt:client_credentials');
          break;
        case OpenIddictConstants.GrantType_RefreshToken:
          permissions.push('gt:refresh_token');
          break;
        case OpenIddictConstants.GrantType_Password:
          permissions.push('gt:password');
          break;
        case OpenIddictConstants.GrantType_Implicit:
          permissions.push('gt:implicit');
          break;
        case OpenIddictConstants.GrantType_DeviceCode:
          permissions.push('gt:urn:ietf:params:oauth:grant-type:device_code');
          break;
      }
    });

    return permissions;
  }
}
