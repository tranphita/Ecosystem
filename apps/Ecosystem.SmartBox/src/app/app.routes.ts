import { Routes } from '@angular/router';
import { FullComponent } from './layouts/full/full.component';
import { AppErrorComponent } from './pages/error/error.component';
import { AppMaintenanceComponent } from './pages/maintenance/maintenance.component';

export const routes: Routes = [
  {
    path: '',
    component: FullComponent,
    children: [
      {
        path: '',
        loadChildren: () =>
          import('./pages/pages.routes').then((m) => m.PagesRoutes),
      },
    ],
  },
  {
    path: 'error',
    component: AppErrorComponent,
  },
  {
    path: 'maintenance',
    component: AppMaintenanceComponent,
  },
  {
    path: '**',
    redirectTo: 'error',
  },
];
