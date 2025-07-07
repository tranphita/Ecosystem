import { Injectable, signal } from '@angular/core';
import { Employee } from '../models/employee.model';

@Injectable({
  providedIn: 'root',
})
export class EmployeeService {
  // Sử dụng signal để quản lý danh sách employee
  private _employees = signal<Employee[]>([
    {
      id: 1,
      Name: 'Johnathan Deo',
      Position: 'Seo Expert',
      Email: 'r@gmail.com',
      Mobile: 9786838,
      DateOfJoining: new Date('2020-01-02'),
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
  ]);

  employees = this._employees.asReadonly();

  add(employee: Employee) {
    this._employees.update((list) => [
      ...list,
      { ...employee, id: this._generateId() },
    ]);
  }

  update(employee: Employee) {
    this._employees.update((list) =>
      list.map((e) => (e.id === employee.id ? { ...employee } : e))
    );
  }

  delete(id: number) {
    this._employees.update((list) => list.filter((e) => e.id !== id));
  }

  private _generateId(): number {
    const list = this._employees();
    return list.length ? Math.max(...list.map((e) => e.id)) + 1 : 1;
  }
}
