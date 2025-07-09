import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

/**
 * Service quản lý permissions cho ứng dụng
 */
export interface Permission {
  name: string;
  isGranted: boolean;
}

export interface PermissionGroup {
  name: string;
  displayName: string;
  permissions: Permission[];
}

@Injectable({
  providedIn: 'root'
})
export class PermissionService {
  private readonly apiUrl = `${environment.apiUrl}/api/smartbox/permissions`;
  private permissionsSubject = new BehaviorSubject<Permission[]>([]);
  
  public permissions$ = this.permissionsSubject.asObservable();

  constructor(private http: HttpClient) {
    this.loadCurrentUserPermissions();
  }

  /**
   * Load permissions của user hiện tại
   */
  private loadCurrentUserPermissions(): void {
    // TODO: Replace with actual API call
    // this.http.get<Permission[]>(`${this.apiUrl}/current-user`).subscribe(
    //   permissions => this.permissionsSubject.next(permissions)
    // );

    // Mock permissions for development
    const mockPermissions: Permission[] = [
      { name: 'SmartBox.Users.View', isGranted: true },
      { name: 'SmartBox.Users.Create', isGranted: true },
      { name: 'SmartBox.Users.Edit', isGranted: true },
      { name: 'SmartBox.Users.Delete', isGranted: false },
      { name: 'SmartBox.Users.ManageRoles', isGranted: true },
      { name: 'SmartBox.Roles.View', isGranted: true },
      { name: 'SmartBox.Roles.Create', isGranted: true },
      { name: 'SmartBox.Roles.Edit', isGranted: true },
      { name: 'SmartBox.Roles.Delete', isGranted: false },
      { name: 'SmartBox.Companies.View', isGranted: true },
      { name: 'SmartBox.Companies.Create', isGranted: true },
      { name: 'SmartBox.Companies.Edit', isGranted: true },
      { name: 'SmartBox.Companies.Delete', isGranted: false }
    ];
    
    this.permissionsSubject.next(mockPermissions);
  }

  /**
   * Kiểm tra có quyền cụ thể không
   */
  hasPermission(permissionName: string): Observable<boolean> {
    return this.permissions$.pipe(
      map(permissions => {
        const permission = permissions.find(p => p.name === permissionName);
        return permission?.isGranted || false;
      })
    );
  }

  /**
   * Kiểm tra có tất cả quyền trong danh sách không
   */
  hasAllPermissions(permissionNames: string[]): Observable<boolean> {
    return this.permissions$.pipe(
      map(permissions => {
        return permissionNames.every(name => {
          const permission = permissions.find(p => p.name === name);
          return permission?.isGranted || false;
        });
      })
    );
  }

  /**
   * Kiểm tra có ít nhất một quyền trong danh sách không
   */
  hasAnyPermission(permissionNames: string[]): Observable<boolean> {
    return this.permissions$.pipe(
      map(permissions => {
        return permissionNames.some(name => {
          const permission = permissions.find(p => p.name === name);
          return permission?.isGranted || false;
        });
      })
    );
  }

  /**
   * Sync method để kiểm tra quyền - dùng cho template
   */
  hasPermissionSync(permissionName: string): boolean {
    const permissions = this.permissionsSubject.value;
    const permission = permissions.find(p => p.name === permissionName);
    return permission?.isGranted || false;
  }

  /**
   * Get permissions by provider (role, user)
   */
  getPermissions(providerName: string, providerKey: string): Observable<PermissionGroup[]> {
    return this.http.get<PermissionGroup[]>(`${this.apiUrl}`, {
      params: {
        providerName,
        providerKey
      }
    });
  }

  /**
   * Update permissions
   */
  updatePermissions(
    providerName: string, 
    providerKey: string, 
    permissions: { name: string; isGranted: boolean }[]
  ): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}`, {
      providerName,
      providerKey,
      permissions
    });
  }

  /**
   * Refresh current user permissions
   */
  refreshPermissions(): void {
    this.loadCurrentUserPermissions();
  }

  /**
   * Constants cho permission names
   */
  static readonly PERMISSIONS = {
    // User permissions
    USERS_VIEW: 'SmartBox.Users.View',
    USERS_CREATE: 'SmartBox.Users.Create',
    USERS_EDIT: 'SmartBox.Users.Edit',
    USERS_DELETE: 'SmartBox.Users.Delete',
    USERS_MANAGE_ROLES: 'SmartBox.Users.ManageRoles',

    // Role permissions
    ROLES_VIEW: 'SmartBox.Roles.View',
    ROLES_CREATE: 'SmartBox.Roles.Create',
    ROLES_EDIT: 'SmartBox.Roles.Edit',
    ROLES_DELETE: 'SmartBox.Roles.Delete',

    // Company permissions
    COMPANIES_VIEW: 'SmartBox.Companies.View',
    COMPANIES_CREATE: 'SmartBox.Companies.Create',
    COMPANIES_EDIT: 'SmartBox.Companies.Edit',
    COMPANIES_DELETE: 'SmartBox.Companies.Delete'
  } as const;
} 