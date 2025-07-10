import { Routes } from '@angular/router';
import { autoLoginPartialRoutesGuard } from 'angular-auth-oidc-client';

export const AdministrationRoutes: Routes = [
  {
    path: '',
    redirectTo: 'users',
    pathMatch: 'full',
  },
  {
    path: 'users',
    loadComponent: () => 
      import('./users/user-list/user-list.component').then(m => m.UserListComponent),
    canActivate: [autoLoginPartialRoutesGuard],
    data: {
      title: 'User Management',
      urls: [
        { title: 'Administration', url: '/administration' },
        { title: 'Users' },
      ],
    },
  },
  {
    path: 'roles',
    loadComponent: () => 
      import('./roles/role-list/role-list.component').then(m => m.RoleListComponent),
    canActivate: [autoLoginPartialRoutesGuard],
    data: {
      title: 'Role Management',
      urls: [
        { title: 'Administration', url: '/administration' },
        { title: 'Roles' },
      ],
    },
  },
]; 