import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ToasterService } from '@abp/ng.theme.shared';
import { PagedResultDto } from '@abp/ng.core';
import { OpenIddictApplicationDto, GetOpenIddictApplicationListDto } from './application.dto';
import { OpenIddictApplicationsService } from './openiddict-applications.service';

@Component({
  selector: 'app-application-list',
  templateUrl: './application-list.component.html',
  styleUrls: ['./application-list.component.scss']
})
export class ApplicationListComponent implements OnInit {
  // Danh sách applications
  applications: OpenIddictApplicationDto[] = [];
  
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
  selectedApplication: OpenIddictApplicationDto | null = null;
  
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
    console.log('📱 ApplicationListComponent ngOnInit');
    try {
      this.loadApplications();
    } catch (error) {
      console.error('📱 Error in ApplicationListComponent ngOnInit:', error);
    }
  }

  /**
   * Tải danh sách applications với phân trang
   */
  loadApplications(): void {
    console.log('📱 Loading applications...');
    this.isLoading = true;
    
    const request: GetOpenIddictApplicationListDto = {
      filter: this.searchForm.get('filter')?.value || '',
      skipCount: this.page * this.pageSize,
      maxResultCount: this.pageSize
    };

    console.log('📱 Request:', request);

    this.applicationService.getList(request).subscribe({
      next: (response) => {
        console.log('📱 Applications loaded:', response);
        this.applications = response.items;
        this.totalCount = response.totalCount;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('📱 Error loading applications:', error);
        this.toasterService.error('Lỗi khi tải danh sách applications', 'Lỗi');
        this.isLoading = false;
      }
    });
  }

  /**
   * Tìm kiếm applications
   */
  onSearch(): void {
    this.page = 0;
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
  openEditModal(application: OpenIddictApplicationDto): void {
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
    this.toasterService.success(
      this.selectedApplication ? 'Cập nhật thành công' : 'Tạo mới thành công',
      'Thành công'
    );
  }

  /**
   * Xóa application
   */
  deleteApplication(application: OpenIddictApplicationDto): void {
    const isConfirmed = confirm(`Bạn có chắc chắn muốn xóa application "${application.displayName || application.clientId}"?`);
    
    if (isConfirmed) {
      this.applicationService.delete(application.id).subscribe({
        next: () => {
          this.loadApplications();
          this.toasterService.success('Xóa application thành công', 'Thành công');
        },
        error: (error) => {
          this.toasterService.error('Lỗi khi xóa application', 'Lỗi');
          console.error('Error deleting application:', error);
        }
      });
    }
  }

  /**
   * Lấy application type display name
   */
  getApplicationTypeDisplayName(type?: string): string {
    switch (type) {
      case 'web':
        return 'Web Application';
      case 'native':
        return 'Native Application';
      case 'hybrid':
        return 'Hybrid Application';
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
    this.page = 0;
    this.searchForm.reset();
    this.loadApplications();
  }

  /**
   * TrackBy function để optimize performance của *ngFor
   */
  trackByFn(index: number, item: OpenIddictApplicationDto): string {
    return item.id;
  }
} 