import { Injectable, inject } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { PermissionService } from '../../core/services/permission.service';

/**
 * Guard để kiểm tra quyền truy cập
 */
@Injectable({
  providedIn: 'root'
})
export class PermissionGuard implements CanActivate {
  private router = inject(Router);
  private permissionService = inject(PermissionService);

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    const requiredPermissions = (route.data as any)['permissions'] as string[];
    
    if (!requiredPermissions || requiredPermissions.length === 0) {
      return true;
    }

    // Kiểm tra quyền
    return this.permissionService.permissions$.pipe(
      map(() => {
        const hasPermission = requiredPermissions.some(permission => 
          this.permissionService.hasPermissionSync(permission)
        );

        if (!hasPermission) {
          // Chuyển hướng đến trang access denied
          this.router.navigate(['/access-denied']);
          return false;
        }

        return true;
      })
    );
  }
} 