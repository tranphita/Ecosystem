import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ToasterService } from '@abp/ng.theme.shared';
import { ScopeModel, GetScopeListModel, ScopeListResultModel } from './models/scope.model';
import { OpenIddictScopesService } from './openiddict-scopes.service';

@Component({
    selector: 'app-scopes-list',
    templateUrl: './scopes-list.component.html',
    styleUrls: ['./scopes-list.component.scss']
})
export class ScopesListComponent implements OnInit {
    scopes: ScopeModel[] = [];
    isLoading = false;
    pageSize = 10;
    page = 1;
    totalCount = 0;
    searchForm: FormGroup;
    isModalOpen = false;
    selectedScope: ScopeModel | null = null;
    isConfirmDeleteOpen = false;
    scopeToDelete: ScopeModel | null = null;
    Math = Math;

    constructor(
        private scopesService: OpenIddictScopesService,
        private fb: FormBuilder,
        private toasterService: ToasterService
    ) {
        this.searchForm = this.fb.group({
            filter: ['']
        });
    }

    ngOnInit(): void {
        this.loadScopes();
    }

    loadScopes(): void {
        this.isLoading = true;
        const request: GetScopeListModel = {
            filter: this.searchForm.get('filter')?.value || '',
            skipCount: (this.page - 1) * this.pageSize,
            maxResultCount: this.pageSize
        };
        this.scopesService.getList(request).subscribe({
            next: (response: ScopeListResultModel) => {
                this.scopes = response.items;
                this.totalCount = response.totalCount;
                this.isLoading = false;
            },
            error: (error) => {
                console.error('Error loading scopes:', error);
                this.toasterService.error('Lỗi khi tải danh sách scopes', 'Lỗi');
                this.isLoading = false;
            }
        });
    }

    onSearch(): void {
        this.page = 1;
        this.loadScopes();
    }

    onPageChange(page: number): void {
        this.page = page;
        this.loadScopes();
    }

    openCreateModal(): void {
        this.selectedScope = null;
        this.isModalOpen = true;
    }

    openEditModal(scope: ScopeModel): void {
        this.selectedScope = scope;
        this.isModalOpen = true;
    }

    closeModal(): void {
        this.isModalOpen = false;
        this.selectedScope = null;
    }

    onModalSave(): void {
        this.closeModal();
        this.loadScopes();
    }

    deleteScope(scope: ScopeModel): void {
        this.scopeToDelete = scope;
        this.isConfirmDeleteOpen = true;
    }

    confirmDelete(): void {
        if (!this.scopeToDelete) return;
        this.scopesService.delete(this.scopeToDelete.id).subscribe({
            next: () => {
                this.loadScopes();
                this.toasterService.success('Xóa scope thành công', 'Thành công');
                this.isConfirmDeleteOpen = false;
                this.scopeToDelete = null;
            },
            error: (error) => {
                this.toasterService.error('Lỗi khi xóa scope', 'Lỗi');
                this.isConfirmDeleteOpen = false;
                this.scopeToDelete = null;
                console.error('Error deleting scope:', error);
            }
        });
    }

    cancelDelete(): void {
        this.isConfirmDeleteOpen = false;
        this.scopeToDelete = null;
    }

    refresh(): void {
        this.page = 1;
        this.searchForm.reset();
        this.loadScopes();
    }

    trackByFn(index: number, item: ScopeModel): string {
        return item.id;
    }
}
