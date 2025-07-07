// filepath: d:\Work\starterkit\src\app\pages\system\employee\employee.component.ts
import {
  Component,
  Inject,
  Optional,
  ViewChild,
  AfterViewInit,
} from '@angular/core';
import {
  MatDialog,
  MatDialogRef,
  MAT_DIALOG_DATA,
} from '@angular/material/dialog';
import { CommonModule, DatePipe } from '@angular/common';
import { AppAddEmployeeComponent } from './add/add.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MaterialModule } from 'src/app/material.module';
import { TablerIconsModule } from 'angular-tabler-icons';
import { Sort } from '@angular/material/sort';
import {
  TableComponent,
  TableColumn,
} from 'src/app/components/shared/table/table.component';
import { provideNativeDateAdapter } from '@angular/material/core';
import { FormDialogComponent } from 'src/app/components/shared/dialog/form-dialog.component';
import { EmployeeFormComponent } from './employee-form.component';

export interface Employee {
  id: number;
  Name: string;
  Position: string;
  Email: string;
  Mobile: number;
  DateOfJoining: Date;
  Salary: number;
  Projects: number;
  imagePath: string;
}

const employees = [
  {
    id: 1,
    Name: 'Johnathan Deo',
    Position: 'Seo Expert',
    Email: 'r@gmail.com',
    Mobile: 9786838,
    DateOfJoining: new Date('01-2-2020'),
    Salary: 12000,
    Projects: 10,
    imagePath: 'assets/images/profile/user-2.jpg',
  },
  {
    id: 2,
    Name: 'Mark Zukerburg',
    Position: 'Web Developer',
    Email: 'mark@gmail.com',
    Mobile: 8786838,
    DateOfJoining: new Date('04-2-2020'),
    Salary: 12000,
    Projects: 10,
    imagePath: 'assets/images/profile/user-3.jpg',
  },
  {
    id: 3,
    Name: 'Sam smith',
    Position: 'Web Designer',
    Email: 'sam@gmail.com',
    Mobile: 7788838,
    DateOfJoining: new Date('02-2-2020'),
    Salary: 12000,
    Projects: 10,
    imagePath: 'assets/images/profile/user-4.jpg',
  },
  {
    id: 4,
    Name: 'John Deo',
    Position: 'Tester',
    Email: 'john@gmail.com',
    Mobile: 8786838,
    DateOfJoining: new Date('03-2-2020'),
    Salary: 12000,
    Projects: 11,
    imagePath: 'assets/images/profile/user-5.jpg',
  },
  {
    id: 5,
    Name: 'Genilia',
    Position: 'Actor',
    Email: 'genilia@gmail.com',
    Mobile: 8786838,
    DateOfJoining: new Date('05-2-2020'),
    Salary: 12000,
    Projects: 19,
    imagePath: 'assets/images/profile/user-6.jpg',
  },
  {
    id: 6,
    Name: 'Jack Sparrow',
    Position: 'Content Writer',
    Email: 'jac@gmail.com',
    Mobile: 8786838,
    DateOfJoining: new Date('05-21-2020'),
    Salary: 12000,
    Projects: 5,
    imagePath: 'assets/images/profile/user-7.jpg',
  },
  {
    id: 7,
    Name: 'Tom Cruise',
    Position: 'Actor',
    Email: 'tom@gmail.com',
    Mobile: 8786838,
    DateOfJoining: new Date('02-15-2019'),
    Salary: 12000,
    Projects: 9,
    imagePath: 'assets/images/profile/user-3.jpg',
  },
  {
    id: 8,
    Name: 'Hary Porter',
    Position: 'Actor',
    Email: 'hary@gmail.com',
    Mobile: 8786838,
    DateOfJoining: new Date('07-3-2019'),
    Salary: 12000,
    Projects: 7,
    imagePath: 'assets/images/profile/user-6.jpg',
  },
  {
    id: 9,
    Name: 'Kristen Ronaldo',
    Position: 'Player',
    Email: 'kristen@gmail.com',
    Mobile: 8786838,
    DateOfJoining: new Date('01-15-2019'),
    Salary: 12000,
    Projects: 1,
    imagePath: 'assets/images/profile/user-5.jpg',
  },
];

@Component({
  templateUrl: './employee.component.html',
  standalone: true,
  imports: [
    CommonModule,
    MaterialModule,
    FormsModule,
    ReactiveFormsModule,
    TablerIconsModule,
    TableComponent,
  ],
  providers: [DatePipe],
})
export class AppEmployeeComponent {
  @ViewChild(TableComponent) tableComponent!: TableComponent<Employee>;

  searchText: any;
  employeeData = employees;
  filterText = '';

  columns: TableColumn<Employee>[] = [
    { key: 'id', label: '#' },
    {
      key: 'name',
      label: 'Name',
      sortable: true,
      cell: (row: Employee) => {
        // Custom rendering cho cột Name với avatar và thông tin vị trí
        return {
          name: row.Name,
          position: row.Position,
          imagePath: row.imagePath,
        };
      },
    },
    { key: 'Email', label: 'Email', sortable: true },
    { key: 'Mobile', label: 'Mobile', sortable: true },
    {
      key: 'DateOfJoining',
      label: 'Date of Joining',
      sortable: true,
      cell: (row: Employee) =>
        this.datePipe.transform(row.DateOfJoining, 'fullDate'),
    },
    { key: 'Salary', label: 'Salary', sortable: true },
    { key: 'Projects', label: 'Projects', sortable: true },
  ];

  constructor(public dialog: MatDialog, public datePipe: DatePipe) { }

  applyFilter(filterValue: string): void {
    this.filterText = filterValue;
  }

  handleSortChange(event: Sort): void {
    console.log('Sort changed:', event);
  }

  handleEdit(employee: Employee): void {
    this.openDialog('Update', employee);
  }

  handleDelete(employee: Employee): void {
    this.openDialog('Delete', employee);
  }

  openDialog(action: string, obj: any): void {
    if (action === 'Add') {
      obj = {
        Name: '',
        Position: '',
        Email: '',
        Mobile: '',
        DateOfJoining: new Date(),
        Salary: 0,
        Projects: 0,
        imagePath: 'assets/images/profile/user-1.jpg',
      };
    }

    const dialogData = {
      action: action,
      title:
        action === 'Add'
          ? 'Thêm nhân viên mới'
          : action === 'Update'
            ? 'Cập nhật nhân viên'
            : 'Xóa nhân viên',
      data: { ...obj, action },
      template: EmployeeFormComponent,
    };

    const dialogRef = this.dialog.open(FormDialogComponent, {
      data: dialogData,
      width: '600px',
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result && result.event) {
        if (result.event === 'Add') {
          this.addRowData(result.data);
        } else if (result.event === 'Update') {
          this.updateRowData(result.data);
        } else if (result.event === 'Delete') {
          this.deleteRowData(result.data);
        }
      }
    });
  }
  addRowData(row_obj: Employee): void {
    const newEmployee = {
      id: this.employeeData.length + 1,
      Name: row_obj.Name,
      Position: row_obj.Position,
      Email: row_obj.Email,
      Mobile: row_obj.Mobile,
      DateOfJoining: row_obj.DateOfJoining || new Date(),
      Salary: row_obj.Salary || 0,
      Projects: row_obj.Projects || 0,
      imagePath: row_obj.imagePath || 'assets/images/profile/user-1.jpg',
    };

    // Thêm dữ liệu vào đầu mảng
    this.employeeData = [newEmployee, ...this.employeeData];

    // Mở thông báo thêm thành công
    setTimeout(() => {
      this.dialog.open(AppAddEmployeeComponent);
    }, 100);
  }

  updateRowData(row_obj: Employee): void {
    this.employeeData = this.employeeData.map((emp) => {
      if (emp.id === row_obj.id) {
        return {
          ...emp,
          Name: row_obj.Name,
          Position: row_obj.Position,
          Email: row_obj.Email,
          Mobile: row_obj.Mobile,
          DateOfJoining: row_obj.DateOfJoining,
          Salary: row_obj.Salary,
          Projects: row_obj.Projects,
          imagePath: row_obj.imagePath,
        };
      }
      return emp;
    });
  }

  deleteRowData(row_obj: Employee): void {
    this.employeeData = this.employeeData.filter(
      (emp) => emp.id !== row_obj.id
    );
  }
}