import { Component, Inject, Optional, Type, ViewContainerRef, ElementRef, ComponentRef, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MaterialModule } from 'src/app/material.module';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { DatePipe } from '@angular/common';
import { provideNativeDateAdapter } from '@angular/material/core';
import { EmployeeFormComponent } from 'src/app/pages/system/employee/employee-form.component';
import { DialogEventsService } from 'src/app/services/dialog-events.service';
import { Subscription } from 'rxjs';
import { AppAddEmployeeComponent } from 'src/app/pages/system/employee/add/add.component';

export interface DialogData<T = any> {
  action: 'Add' | 'Update' | 'Delete';
  title?: string;
  data: T;
  template: Type<any>;
}

@Component({
  selector: 'app-form-dialog',
  imports: [CommonModule, MaterialModule, FormsModule, ReactiveFormsModule],
  templateUrl: './form-dialog.component.html',
  providers: [DatePipe, provideNativeDateAdapter()],
  standalone: true
})
export class FormDialogComponent implements OnInit, OnDestroy {
  private cancelSubscription: Subscription | undefined;
  private submitSubscription: Subscription | undefined;
  action: string;
  local_data: any;
  customTemplate: Type<any>;
  
  constructor(
    public dialogRef: MatDialogRef<FormDialogComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: DialogData,
    private dialogEventsService: DialogEventsService,
    private dialog: MatDialog
  ) {
    this.local_data = { ...data.data };
    this.action = data.action;
    this.customTemplate = data.template;
  }

  ngOnInit(): void {
    this.cancelSubscription = this.dialogEventsService.cancelDialog$.subscribe(() => {
      this.closeDialog();
    });
    this.submitSubscription = this.dialogEventsService.submitDialog$.subscribe((employee) => {
      this.dialogRef.close({ event: this.action, data: employee });
    });
  }
  
  ngOnDestroy(): void {
    if (this.cancelSubscription) {
      this.cancelSubscription.unsubscribe();
    }
    if (this.submitSubscription) {
      this.submitSubscription.unsubscribe();
    }
  }

  doAction(): void {
    this.dialogRef.close({ event: this.action, data: this.local_data });
  }
  closeDialog(): void {
    this.dialogRef.close({ event: 'cancel' });
  }

  onSubmit = (employee: any) => {
    this.dialogRef.close({ event: this.action, data: employee });
  }
}
