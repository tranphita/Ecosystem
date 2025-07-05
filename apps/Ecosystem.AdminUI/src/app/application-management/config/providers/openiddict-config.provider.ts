import { APP_INITIALIZER } from '@angular/core';
import { RoutesService, eLayoutType } from '@abp/ng.core';

// OpenIddict Configuration Provider
export function provideOpenIddictConfig() {
  return [
    {
      provide: APP_INITIALIZER,
      useFactory: configureOpenIddictRoutes,
      deps: [RoutesService],
      multi: true,
    },
  ];
}

function configureOpenIddictRoutes(routesService: RoutesService) {
  return () => {
    routesService.add([
      {
        path: '/administration',
        name: 'AbpUiNavigation::Menu:Administration',
        iconClass: 'fas fa-wrench',
        order: 2,
        layout: eLayoutType.application,
      },
      {
        path: '/application-management',
        name: 'OpenIddict Management', 
        iconClass: 'fas fa-shield-alt',
        order: 100,
        layout: eLayoutType.application,
        parentName: 'AbpUiNavigation::Menu:Administration',
      },
      {
        path: '/application-management/applications',
        name: 'Applications',
        iconClass: 'fas fa-th-large',
        order: 1,
        layout: eLayoutType.application,
        parentName: 'OpenIddict Management',
      },
      {
        path: '/application-management/scopes',
        name: 'Scopes',
        iconClass: 'fas fa-key',
        order: 2,
        layout: eLayoutType.application,
        parentName: 'OpenIddict Management',
      },
    ]);
  };
}

export const OPENIDDICT_CONFIG_PROVIDERS = provideOpenIddictConfig();

// Localization texts cho OpenIddict
export const OPENIDDICT_LOCALIZATIONS = {
  en: {
    'Menu:OpenIddictManagement': 'OpenIddict Management',
    'Menu:Administration': 'Administration',
    'OpenIddict:Applications': 'Applications',
    'OpenIddict:Scopes': 'Scopes',
    'OpenIddict:Permissions': 'Permissions',
    'OpenIddict:ApplicationManagement': 'Application Management',
    'OpenIddict:ScopeManagement': 'Scope Management',
    'OpenIddict:NewApplication': 'New Application',
    'OpenIddict:NewScope': 'New Scope',
    'OpenIddict:EditApplication': 'Edit Application',
    'OpenIddict:EditScope': 'Edit Scope',
    'OpenIddict:DisplayName': 'Display Name',
    'OpenIddict:ClientId': 'Client ID',
    'OpenIddict:ClientSecret': 'Client Secret',
    'OpenIddict:ApplicationType': 'Application Type',
    'OpenIddict:ConsentType': 'Consent Type',
    'OpenIddict:GrantTypes': 'Grant Types',
    'OpenIddict:RedirectUris': 'Redirect URIs',
    'OpenIddict:PostLogoutRedirectUris': 'Post Logout Redirect URIs',
    'OpenIddict:Description': 'Description',
    'OpenIddict:Resources': 'Resources',
    'OpenIddict:ScopeName': 'Scope Name',
    'OpenIddict:CreationTime': 'Creation Time',
    'OpenIddict:Actions': 'Actions',
    'OpenIddict:Edit': 'Edit',
    'OpenIddict:Delete': 'Delete',
    'OpenIddict:Create': 'Create',
    'OpenIddict:Update': 'Update',
    'OpenIddict:Cancel': 'Cancel',
    'OpenIddict:Save': 'Save',
    'OpenIddict:Search': 'Search',
    'OpenIddict:Refresh': 'Refresh',
    'OpenIddict:NoData': 'No data available',
    'OpenIddict:ConfirmDelete': 'Are you sure you want to delete this item?',
    'OpenIddict:DeleteSuccess': 'Item deleted successfully',
    'OpenIddict:CreateSuccess': 'Item created successfully',
    'OpenIddict:UpdateSuccess': 'Item updated successfully',
    'OpenIddict:LoadingError': 'Error loading data',
    'OpenIddict:DeleteError': 'Error deleting item',
    'OpenIddict:CreateError': 'Error creating item',
    'OpenIddict:UpdateError': 'Error updating item',
  },
  vi: {
    'Menu:OpenIddictManagement': 'Quản lý OpenIddict',
    'Menu:Administration': 'Quản trị',
    'OpenIddict:Applications': 'Ứng dụng',
    'OpenIddict:Scopes': 'Phạm vi',
    'OpenIddict:Permissions': 'Quyền hạn',
    'OpenIddict:ApplicationManagement': 'Quản lý Ứng dụng',
    'OpenIddict:ScopeManagement': 'Quản lý Phạm vi',
    'OpenIddict:NewApplication': 'Ứng dụng mới',
    'OpenIddict:NewScope': 'Phạm vi mới',
    'OpenIddict:EditApplication': 'Chỉnh sửa Ứng dụng',
    'OpenIddict:EditScope': 'Chỉnh sửa Phạm vi',
    'OpenIddict:DisplayName': 'Tên hiển thị',
    'OpenIddict:ClientId': 'Client ID',
    'OpenIddict:ClientSecret': 'Client Secret',
    'OpenIddict:ApplicationType': 'Loại ứng dụng',
    'OpenIddict:ConsentType': 'Loại đồng ý',
    'OpenIddict:GrantTypes': 'Loại cấp phép',
    'OpenIddict:RedirectUris': 'Redirect URIs',
    'OpenIddict:PostLogoutRedirectUris': 'Post Logout URIs',
    'OpenIddict:Description': 'Mô tả',
    'OpenIddict:Resources': 'Tài nguyên',
    'OpenIddict:ScopeName': 'Tên phạm vi',
    'OpenIddict:CreationTime': 'Thời gian tạo',
    'OpenIddict:Actions': 'Thao tác',
    'OpenIddict:Edit': 'Chỉnh sửa',
    'OpenIddict:Delete': 'Xóa',
    'OpenIddict:Create': 'Tạo mới',
    'OpenIddict:Update': 'Cập nhật',
    'OpenIddict:Cancel': 'Hủy',
    'OpenIddict:Save': 'Lưu',
    'OpenIddict:Search': 'Tìm kiếm',
    'OpenIddict:Refresh': 'Làm mới',
    'OpenIddict:NoData': 'Không có dữ liệu',
    'OpenIddict:ConfirmDelete': 'Bạn có chắc chắn muốn xóa mục này?',
    'OpenIddict:DeleteSuccess': 'Xóa thành công',
    'OpenIddict:CreateSuccess': 'Tạo mới thành công',
    'OpenIddict:UpdateSuccess': 'Cập nhật thành công',
    'OpenIddict:LoadingError': 'Lỗi khi tải dữ liệu',
    'OpenIddict:DeleteError': 'Lỗi khi xóa',
    'OpenIddict:CreateError': 'Lỗi khi tạo mới',
    'OpenIddict:UpdateError': 'Lỗi khi cập nhật',
  }
}; 