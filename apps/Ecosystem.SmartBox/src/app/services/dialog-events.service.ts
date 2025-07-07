import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DialogEventsService {
  private cancelDialogSource = new Subject<void>();
  cancelDialog$ = this.cancelDialogSource.asObservable();

  private submitDialogSource = new Subject<any>();
  submitDialog$ = this.submitDialogSource.asObservable();

  emitCancelDialog() {
    this.cancelDialogSource.next();
  }

  emitSubmitDialog(data: any) {
    this.submitDialogSource.next(data);
  }
}
