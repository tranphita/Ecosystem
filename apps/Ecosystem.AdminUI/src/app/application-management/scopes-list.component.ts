import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ToasterService } from '@abp/ng.theme.shared';
import { PagedResultDto } from '@abp/ng.core';
import { OpenIddictScopeDto, GetOpenIddictScopeListDto } from './application.dto';
import { OpenIddictScopesService } from './openiddict-scopes.service';

@Component({
  selector: 'app-scopes-list',
  templateUrl: './scopes-list.component.html',
  styleUrls: ['./scopes-list.component.scss']
})
export class ScopesListComponent implements OnInit {
  // Danh sách scopes
  scopes: OpenIddictScopeDto[] = [];
  
  // Trạng thái loading
  isLoading = false;
  
  // Thông tin phân trang
  pageSize = 10;
  page = 0;
  totalCount = 0;
  
  // Form tìm kiếm
  searchForm: FormGroup;
  
  // Cấu hình hiển thị modal
  isModalOpen = false;
  selectedScope: OpenIddictScopeDto | null = null;
  
  // Expose Math cho template
  Math = Math;
  
  constructor(
    private scopesService: OpenIddictScopesService,
    private fb: FormBuilder,
    private toasterService: ToasterService
  ) {
    // Khởi tạo form tìm kiếm
    this.searchForm = this.fb.group({
      filter: ['']
    });
  }

  ngOnInit(): void {
    console.log('🔑 ScopesListComponent ngOnInit');
    try {
      this.loadScopes();
    } catch (error) {
      console.error('🔑 Error in ScopesListComponent ngOnInit:', error);
    }
  }

  /**
   * Tải danh sách scopes với phân trang
   */
  loadScopes(): void {
    console.log('🔑 Loading scopes...');
    this.isLoading = true;
    
    const request: GetOpenIddictScopeListDto = {
      filter: this.searchForm.get('filter')?.value || '',
      skipCount: this.page * this.pageSize,
      maxResultCount: this.pageSize
    };

    console.log('🔑 Request:', request);

    this.scopesService.getList(request).subscribe({
      next: (response) => {
        console.log('🔑 Scopes loaded:', response);
        this.scopes = response.items;
        this.totalCount = response.totalCount;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('🔑 Error loading scopes:', error);
        this.toasterService.error('Lỗi khi tải danh sách scopes', 'Lỗi');
        this.isLoading = false;
      }
    });
  }

  /**
   * Tìm kiếm scopes
   */
  onSearch(): void {
    this.page = 0;
    this.loadScopes();
  }

  /**
   * Xử lý thay đổi trang
   */
  onPageChange(page: number): void {
    this.page = page;
    this.loadScopes();
  }

  /**
   * Mở modal tạo mới scope
   */
  openCreateModal(): void {
    this.selectedScope = null;
    this.isModalOpen = true;
  }

  /**
   * Mở modal chỉnh sửa scope
   */
  openEditModal(scope: OpenIddictScopeDto): void {
    this.selectedScope = scope;
    this.isModalOpen = true;
  }

  /**
   * Đóng modal
   */
  closeModal(): void {
    this.isModalOpen = false;
    this.selectedScope = null;
  }

  /**
   * Xử lý sau khi tạo/cập nhật thành công
   */
  onModalSave(): void {
    this.closeModal();
    this.loadScopes();
    this.toasterService.success(
      this.selectedScope ? 'Cập nhật scope thành công' : 'Tạo scope thành công',
      'Thành công'
    );
  }

  /**
   * Xóa scope
   */
  deleteScope(scope: OpenIddictScopeDto): void {
    const isConfirmed = confirm(`Bạn có chắc chắn muốn xóa scope "${scope.name}"?`);
    
    if (isConfirmed) {
      this.scopesService.delete(scope.id).subscribe({
        next: () => {
          this.loadScopes();
          this.toasterService.success('Xóa scope thành công', 'Thành công');
        },
        error: (error) => {
          this.toasterService.error('Lỗi khi xóa scope', 'Lỗi');
          console.error('Error deleting scope:', error);
        }
      });
    }
  }

  /**
   * Làm mới dữ liệu
   */
  refresh(): void {
    this.page = 0;
    this.searchForm.reset();
    this.loadScopes();
  }

  /**
   * TrackBy function để optimize performance của *ngFor
   */
  trackByFn(index: number, item: OpenIddictScopeDto): string {
    return item.id;
  }
} 