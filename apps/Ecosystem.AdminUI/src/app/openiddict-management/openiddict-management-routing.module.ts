import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ApplicationListComponent } from './applications/application-list.component';
import { ScopesListComponent } from './scopes/scopes-list.component';

const routes: Routes = [
  { path: 'applications', component: ApplicationListComponent },
  { path: 'scopes', component: ScopesListComponent },
  { path: '', redirectTo: 'applications', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OpeniddictManagementRoutingModule {}
