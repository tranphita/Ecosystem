import { Routes } from '@angular/router';
import { StarterComponent } from './starter/starter.component';
import { autoLoginPartialRoutesGuard } from 'angular-auth-oidc-client';
import { FunctionGroupComponent } from './system/function-group/function-group.component';
import { FunctionComponent } from './system/function/function.component';
import { SessionComponent } from './system/session/session.component';
import { SystemComponent } from './system/system.component';
import { AppEmployeeComponent } from './system/employee/employee.component';

export const PagesRoutes: Routes = [
  {
    path: '',
    redirectTo: 'starter',
    pathMatch: 'full',
  },
  {
    path: 'starter',
    component: StarterComponent,
    canActivate: [autoLoginPartialRoutesGuard],
    data: {
      title: 'Starter',
      urls: [
        { title: 'Dashboard', url: '/starter' },
        { title: 'Starter' },
      ],
    },
  },
  {
    path: 'administration',
    loadChildren: () =>
      import('../features/administration/administration.routes').then((m) => m.AdministrationRoutes),
    canActivate: [autoLoginPartialRoutesGuard],
  },
  {
    path: 'system',
    component: SystemComponent,
    canActivate: [autoLoginPartialRoutesGuard],
    children: [
      { path: 'function-group', component: FunctionGroupComponent },
      { path: 'function', component: FunctionComponent },
      { path: 'session', component: SessionComponent },
      { path: 'employee', component: AppEmployeeComponent },
      { path: '', redirectTo: 'function-group', pathMatch: 'full' },
    ],
  },
];
