import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ApplicationListComponent } from './applications/application-list.component';
import { ApplicationFormComponent } from './applications/application-form.component';
import { ScopesListComponent } from './scopes/scopes-list.component';
import { ScopeFormComponent } from './scopes/scope-form.component';
import { OpeniddictManagementRoutingModule } from './openiddict-management-routing.module';

@NgModule({
  declarations: [
    ApplicationListComponent,
    ApplicationFormComponent,
    ScopesListComponent,
    ScopeFormComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    OpeniddictManagementRoutingModule
  ],
  exports: [
    ApplicationListComponent,
    ApplicationFormComponent,
    ScopesListComponent,
    ScopeFormComponent
  ]
})
export class OpeniddictManagementModule {}
