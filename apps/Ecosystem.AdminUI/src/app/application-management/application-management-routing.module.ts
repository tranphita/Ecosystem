import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { OpenIddictDashboardComponent } from './openiddict-dashboard.component';
import { OpenIddictPermissionGuard } from './config/guards/openiddict-permission.guard';
import { OPENIDDICT_PERMISSIONS } from './config/permissions/openiddict-permissions';

const routes: Routes = [
  {
    path: '',
    component: OpenIddictDashboardComponent,
    canActivate: [OpenIddictPermissionGuard],
    data: {
      title: '::Menu:OpenIddictManagement',
      requiredPermission: OPENIDDICT_PERMISSIONS.Applications.Default
    }
  },
  {
    path: 'applications',
    component: OpenIddictDashboardComponent,
    canActivate: [OpenIddictPermissionGuard],
    data: {
      title: 'Applications',
      tab: 'applications',
      requiredPermission: OPENIDDICT_PERMISSIONS.Applications.Default
    }
  },
  {
    path: 'scopes',
    component: OpenIddictDashboardComponent,
    canActivate: [OpenIddictPermissionGuard],
    data: {
      title: 'Scopes',
      tab: 'scopes',
      requiredPermission: OPENIDDICT_PERMISSIONS.Scopes.Default
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ApplicationManagementRoutingModule { } 