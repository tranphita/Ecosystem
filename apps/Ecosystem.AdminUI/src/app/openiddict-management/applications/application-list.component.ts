import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ToasterService } from '@abp/ng.theme.shared';
import { OpenIddictApplicationsService } from './openiddict-applications.service';
import { ApplicationModel, GetApplicationListModel, ApplicationListResultModel } from './models/application.model';

@Component({
  selector: 'app-application-list',
  templateUrl: './application-list.component.html',
  styleUrls: ['./application-list.component.scss']
})
export class ApplicationListComponent implements OnInit {
  // Danh sách applications
  applications: ApplicationModel[] = [];
  
  // Trạng thái loading
  isLoading = false;
  
  // Thông tin phân trang
  pageSize = 10;
  page = 1;
  totalCount = 0;
  
  // Form tìm kiếm
  searchForm: FormGroup;
  
  // Cấu hình hiển thị modal
  isModalOpen = false;
  selectedApplication: ApplicationModel | null = null;
  
  // Cấu hình modal xác nhận xóa
  isConfirmDeleteOpen = false;
  applicationToDelete: ApplicationModel | null = null;
  
  // Expose Math cho template
  Math = Math;
  
  constructor(
    private applicationService: OpenIddictApplicationsService,
    private fb: FormBuilder,
    private toasterService: ToasterService
  ) {
    // Khởi tạo form tìm kiếm
    this.searchForm = this.fb.group({
      filter: ['']
    });
  }

  ngOnInit(): void {
    this.loadApplications();
  }

  /**
   * Tải danh sách applications với phân trang
   */
  loadApplications(): void {
    this.isLoading = true;
    const request: GetApplicationListModel = {
      filter: this.searchForm.get('filter')?.value || '',
      skipCount: (this.page - 1) * this.pageSize,
      maxResultCount: this.pageSize
    };
    this.applicationService.getList(request).subscribe({
      next: (response: ApplicationListResultModel) => {
        this.applications = response.items;
        this.totalCount = response.totalCount;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading applications:', error);
        this.toasterService.error('Lỗi khi tải danh sách applications', 'Lỗi');
        this.isLoading = false;
      }
    });
  }

  /**
   * Tìm kiếm applications
   */
  onSearch(): void {
    this.page = 1;
    this.loadApplications();
  }

  /**
   * Xử lý thay đổi trang
   */
  onPageChange(page: number): void {
    this.page = page;
    this.loadApplications();
  }

  /**
   * Mở modal tạo mới application
   */
  openCreateModal(): void {
    this.selectedApplication = null;
    this.isModalOpen = true;
  }

  /**
   * Mở modal chỉnh sửa application
   */
  openEditModal(application: ApplicationModel): void {
    this.selectedApplication = application;
    this.isModalOpen = true;
  }

  /**
   * Đóng modal
   */
  closeModal(): void {
    this.isModalOpen = false;
    this.selectedApplication = null;
  }

  /**
   * Xử lý sau khi tạo/cập nhật thành công
   */
  onModalSave(): void {
    this.closeModal();
    this.loadApplications();
  }

  /**
   * Xóa application
   */
  deleteApplication(application: ApplicationModel): void {
    this.applicationToDelete = application;
    this.isConfirmDeleteOpen = true;
  }

  /**
   * Xác nhận xóa application
   */
  confirmDelete(): void {
    if (this.applicationToDelete) {
      this.applicationService.delete(this.applicationToDelete.id).subscribe({
        next: () => {
          this.loadApplications();
          this.toasterService.success('Xóa application thành công', 'Thành công');
          this.cancelDelete();
        },
        error: (error) => {
          this.toasterService.error('Lỗi khi xóa application', 'Lỗi');
          console.error('Error deleting application:', error);
          this.cancelDelete();
        }
      });
    }
  }

  /**
   * Hủy xóa application
   */
  cancelDelete(): void {
    this.isConfirmDeleteOpen = false;
    this.applicationToDelete = null;
  }

  /**
   * Lấy application type display name
   */
  getApplicationTypeDisplayName(type?: string): string {
    switch (type) {
      case 'confidential':
        return 'Confidential Application';
      case 'public':
        return 'Public Application';
      default:
        return type || 'Unknown';
    }
  }

  /**
   * Lấy consent type display name
   */
  getConsentTypeDisplayName(consentType?: string): string {
    switch (consentType) {
      case 'explicit':
        return 'Explicit';
      case 'external':
        return 'External';
      case 'implicit':
        return 'Implicit';
      case 'systematic':
        return 'Systematic';
      default:
        return consentType || 'Not specified';
    }
  }

  /**
   * Làm mới dữ liệu
   */
  refresh(): void {
    this.page = 1;
    this.searchForm.reset();
    this.loadApplications();
  }

  /**
   * TrackBy function để optimize performance của *ngFor
   */
  trackByFn(index: number, item: ApplicationModel): string {
    return item.id;
  }
}
