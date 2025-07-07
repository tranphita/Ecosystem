import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MaterialModule } from 'src/app/material.module';
import { Employee } from 'src/app/models/employee.model';
import { DialogEventsService } from 'src/app/services/dialog-events.service';

@Component({
    selector: 'input-employee',
    standalone: true,
    imports: [CommonModule, MaterialModule, FormsModule],
    template: `
    <form #userForm="ngForm">
      <div class="d-flex align-items-center m-b-16">
        <img class="mat-card-avatar" [src]="data.imagePath" class="rounded-circle" width="50" />
        <button mat-flat-button color="primary" class="m-l-16 input-file-button">
          <input type="file" (change)="onFileSelected($event)" #fileInput />
        </button>
      </div>
      <div class="row">
        <div class="col-lg-6">
          <mat-form-field appearance="outline" class="w-100">
            <input type="text" matInput required id="name" name="name" [(ngModel)]="data.Name" placeholder="Tên" />
          </mat-form-field>
        </div>
        <div class="col-lg-6">
          <mat-form-field appearance="outline" class="w-100">
            <input type="text" matInput required id="position" name="position" [(ngModel)]="data.Position"
              placeholder="Vị trí" />
          </mat-form-field>
        </div>
        <div class="col-lg-6">
          <mat-form-field appearance="outline" class="w-100">
            <input type="email" matInput required id="email" name="email" [(ngModel)]="data.Email"
              placeholder="Email" />
          </mat-form-field>
        </div>
        <div class="col-lg-6">
          <mat-form-field appearance="outline" class="w-100">
            <input type="tel" matInput required id="mobile" name="mobile" [(ngModel)]="data.Mobile"
              placeholder="Số điện thoại" />
          </mat-form-field>
        </div>
        <div class="col-lg-6">
          <mat-form-field appearance="outline" class="w-100">
            <input placeholder="Ngày vào làm" [matDatepicker]="picker" matInput required id="doj" name="doj"
              [(ngModel)]="data.DateOfJoining" />
            <mat-datepicker-toggle matIconSuffix [for]="picker"></mat-datepicker-toggle>
            <mat-datepicker #picker></mat-datepicker>
          </mat-form-field>
        </div>
        <div class="col-lg-6">
          <mat-form-field appearance="outline" class="w-100">
            <input type="number" matInput required id="salary" name="salary" [(ngModel)]="data.Salary"
              placeholder="Lương" />
          </mat-form-field>
        </div>
        <div class="col-lg-6">
          <mat-form-field appearance="outline" class="w-100">
            <input type="number" matInput required id="projects" name="projects" [(ngModel)]="data.Projects"
              placeholder="Số dự án" />
          </mat-form-field>
        </div>
      </div>      <div mat-dialog-actions>
        <button mat-flat-button color="primary" [disabled]="!userForm.valid" (click)="onSubmit()">
          {{data.action}}
        </button>
        <button mat-button color="warn" class="m-l-8" type="button" (click)="onCancel()">
          Hủy
        </button>
      </div>
    </form>
  `
})
export class EmployeeFormComponent {
    @Input() data!: Employee & { action: string };

    constructor(private dialogEventsService: DialogEventsService) { }

    onFileSelected(event: any): void {
        if (!event.target.files[0] || event.target.files[0].length === 0) {
            return;
        }
        const mimeType = event.target.files[0].type;
        if (mimeType.match(/image\/*/) == null) {
            return;
        }
        const reader = new FileReader();
        reader.readAsDataURL(event.target.files[0]);
        reader.onload = (_event) => {
            this.data.imagePath = reader.result as string;
        };
    }

    onCancel(): void {
        this.dialogEventsService.emitCancelDialog();
    }

    onSubmit(): void {
        if (this.data) {
            this.dialogEventsService.emitSubmitDialog(this.data);
        }
    }
}
