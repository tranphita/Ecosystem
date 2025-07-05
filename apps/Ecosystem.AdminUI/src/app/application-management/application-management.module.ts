import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { ApplicationManagementRoutingModule } from './application-management-routing.module';
import { OpenIddictDashboardComponent } from './openiddict-dashboard.component';
import { ApplicationListComponent } from './application-list.component';
import { ScopesListComponent } from './scopes-list.component';
import { ApplicationFormComponent } from './application-form.component';
import { ScopeFormComponent } from './scope-form.component';
import { VALIDATION_BLUEPRINTS } from '@ngx-validate/core';

@NgModule({
  declarations: [
    OpenIddictDashboardComponent,
    ApplicationListComponent,
    ScopesListComponent,
    ApplicationFormComponent,
    ScopeFormComponent
  ],
  imports: [
    SharedModule,
    ApplicationManagementRoutingModule
  ],
  providers: [
    {
      provide: VALIDATION_BLUEPRINTS,
      useValue: {}
    }
  ]
})
export class ApplicationManagementModule { } 