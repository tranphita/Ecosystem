import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { ToasterService } from '@abp/ng.theme.shared';
import { 
  OpenIddictApplicationDto, 
  CreateOpenIddictApplicationDto, 
  UpdateOpenIddictApplicationDto,
  OpenIddictConstants 
} from './application.dto';
import { OpenIddictApplicationsService } from './openiddict-applications.service';

@Component({
  selector: 'app-application-form',
  templateUrl: './application-form.component.html',
  styleUrls: ['./application-form.component.scss']
})
export class ApplicationFormComponent implements OnInit {
  @Input() application: OpenIddictApplicationDto | null = null;
  @Input() isVisible = false;
  @Output() saved = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  form: FormGroup;
  isLoading = false;
  
  // OpenIddict Constants cho dropdowns
  applicationTypes = [
    { value: OpenIddictConstants.ApplicationType_Web, label: 'Web Application' },
    { value: OpenIddictConstants.ApplicationType_Native, label: 'Native Application' },
    { value: OpenIddictConstants.ApplicationType_Hybrid, label: 'Hybrid Application' }
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
    if (this.application) {
      this.updateForm();
    }
  }

  private buildForm(): void {
    this.form = this.fb.group({
      // Basic Information
      clientId: ['', [Validators.required]],
      displayName: [''],
      clientSecret: [''],
      type: [OpenIddictConstants.ApplicationType_Web],
      consentType: [OpenIddictConstants.ConsentType_Explicit],
      clientUri: [''],
      logoUri: [''],

      // Grant Types
      grantTypes: this.fb.array([]),

      // Response Types
      responseTypes: this.fb.array([]),

      // Scopes
      scopes: this.fb.array([]),

      // URIs
      redirectUris: this.fb.array([]),
      postLogoutRedirectUris: this.fb.array([])
    });
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

    // Update arrays
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

  // Form Array Getters
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

  // Array Management Methods
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

  // Checkbox handlers
  onGrantTypeChange(grantType: string, checked: boolean): void {
    if (checked) {
      this.grantTypesArray.push(this.fb.control(grantType));
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

  // Utility methods
  isGrantTypeSelected(grantType: string): boolean {
    return this.grantTypesArray.controls.some(control => control.value === grantType);
  }

  isResponseTypeSelected(responseType: string): boolean {
    return this.responseTypesArray.controls.some(control => control.value === responseType);
  }

  isScopeSelected(scope: string): boolean {
    return this.scopesArray.controls.some(control => control.value === scope);
  }

  // Form submission
  onSave(): void {
    if (this.form.invalid) {
      return;
    }

    this.isLoading = true;
    const formValue = this.form.value;

    const dto = {
      clientId: formValue.clientId,
      displayName: formValue.displayName,
      clientSecret: formValue.clientSecret,
      type: formValue.type,
      consentType: formValue.consentType,
      clientUri: formValue.clientUri,
      logoUri: formValue.logoUri,
      grantTypes: formValue.grantTypes,
      responseTypes: formValue.responseTypes,
      scopes: formValue.scopes,
      redirectUris: formValue.redirectUris.filter((uri: string) => uri.trim()),
      postLogoutRedirectUris: formValue.postLogoutRedirectUris.filter((uri: string) => uri.trim())
    };

    const operation$ = this.application
      ? this.applicationService.update(this.application.id, dto as UpdateOpenIddictApplicationDto)
      : this.applicationService.create(dto as CreateOpenIddictApplicationDto);

    operation$.subscribe({
      next: () => {
        this.isLoading = false;
        this.saved.emit();
      },
      error: (error) => {
        this.isLoading = false;
        this.toasterService.error('Lỗi khi lưu application', 'Lỗi');
        console.error('Error saving application:', error);
      }
    });
  }

  onCancel(): void {
    this.cancelled.emit();
  }
} 