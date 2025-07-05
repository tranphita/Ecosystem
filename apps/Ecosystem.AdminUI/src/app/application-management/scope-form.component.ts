import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToasterService } from '@abp/ng.theme.shared';
import { 
  OpenIddictScopeDto, 
  CreateOpenIddictScopeDto, 
  UpdateOpenIddictScopeDto 
} from './application.dto';
import { OpenIddictScopesService } from './openiddict-scopes.service';

@Component({
  selector: 'app-scope-form',
  templateUrl: './scope-form.component.html',
  styleUrls: ['./scope-form.component.scss']
})
export class ScopeFormComponent implements OnInit {
  @Input() scope: OpenIddictScopeDto | null = null;
  @Input() isVisible = false;
  @Output() saved = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  form: FormGroup;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private scopesService: OpenIddictScopesService,
    private toasterService: ToasterService
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      displayName: [''],
      description: [''],
      resources: [''] // Sẽ convert từ string sang array
    });
  }

  ngOnInit(): void {
    if (this.scope) {
      this.form.patchValue({
        name: this.scope.name,
        displayName: this.scope.displayName || '',
        description: this.scope.description || '',
        resources: this.scope.resources?.join(', ') || ''
      });
    }
  }

  onSave(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    const formValue = this.form.value;

    // Convert resources string to array
    const resources = formValue.resources 
      ? formValue.resources.split(',').map((r: string) => r.trim()).filter((r: string) => r)
      : [];

    if (this.scope) {
      // Update existing scope
      const updateDto: UpdateOpenIddictScopeDto = {
        displayName: formValue.displayName || null,
        description: formValue.description || null,
        resources: resources.length > 0 ? resources : null
      };

      this.scopesService.update(this.scope.id, updateDto).subscribe({
        next: () => {
          this.toasterService.success('Cập nhật scope thành công', 'Thành công');
          this.saved.emit();
          this.isLoading = false;
        },
        error: (error) => {
          this.toasterService.error('Lỗi khi cập nhật scope', 'Lỗi');
          this.isLoading = false;
          console.error('Error updating scope:', error);
        }
      });
    } else {
      // Create new scope
      const createDto: CreateOpenIddictScopeDto = {
        name: formValue.name,
        displayName: formValue.displayName || null,
        description: formValue.description || null,
        resources: resources.length > 0 ? resources : null
      };

      this.scopesService.create(createDto).subscribe({
        next: () => {
          this.toasterService.success('Tạo scope thành công', 'Thành công');
          this.saved.emit();
          this.isLoading = false;
        },
        error: (error) => {
          this.toasterService.error('Lỗi khi tạo scope', 'Lỗi');
          this.isLoading = false;
          console.error('Error creating scope:', error);
        }
      });
    }
  }

  onCancel(): void {
    this.cancelled.emit();
  }
} 