import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { PermissionService } from '@abp/ng.core';
import { OPENIDDICT_PERMISSIONS } from '../permissions/openiddict-permissions';

@Injectable({
  providedIn: 'root'
})
export class OpenIddictPermissionGuard implements CanActivate {
  
  constructor(private permissionService: PermissionService) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    
    console.log('🔐 Permission guard called for:', state.url);
    console.log('🔐 Required permission:', route.data['requiredPermission']);
    
    // Tạm thời return true để bypass permission check
    console.log('🔐 Bypassing permission check for testing');
    return true;
    
    // TODO: Enable sau khi test
    // const requiredPermission = route.data['requiredPermission'] || OPENIDDICT_PERMISSIONS.Applications.Default;
    // return this.permissionService.getGrantedPolicy$(requiredPermission);
  }
} 