import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { PermissionService } from '@abp/ng.core';
import { OPENIDDICT_PERMISSIONS } from '../permissions/openiddict-permissions';

@Injectable({
  providedIn: 'root'
})
export class OpenIddictPermissionGuard implements CanActivate {

  constructor(private permissionService: PermissionService) { }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    const requiredPermission = route.data['requiredPermission'] || OPENIDDICT_PERMISSIONS.Applications.Default;
    return this.permissionService.getGrantedPolicy$(requiredPermission);
  }
} 