import { Directive, Input, TemplateRef, ViewContainerRef, OnInit, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { PermissionService } from '../../core/services/permission.service';

/**
 * Directive để ẩn/hiện element dựa trên permission
 * Usage: *hasPermission="'SmartBox.Users.View'"
 * Usage: *hasPermission="['SmartBox.Users.View', 'SmartBox.Users.Edit']" [requireAll]="true"
 */
@Directive({
  selector: '[hasPermission]',
  standalone: true
})
export class HasPermissionDirective implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  private hasView = false;

  @Input() set hasPermission(permissions: string | string[]) {
    this.permissions = Array.isArray(permissions) ? permissions : [permissions];
    this.updateView();
  }

  @Input() requireAll = false; // true = cần tất cả quyền, false = cần ít nhất 1 quyền
  @Input() hideIfNoPermission = true; // true = ẩn element, false = disable element

  private permissions: string[] = [];

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private permissionService: PermissionService
  ) {}

  ngOnInit(): void {
    // Subscribe to permission changes
    this.permissionService.permissions$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.updateView();
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private updateView(): void {
    if (this.permissions.length === 0) {
      this.showView();
      return;
    }

    const hasPermission = this.requireAll
      ? this.checkAllPermissions()
      : this.checkAnyPermission();

    if (hasPermission) {
      this.showView();
    } else {
      this.hideView();
    }
  }

  private checkAllPermissions(): boolean {
    return this.permissions.every(permission => 
      this.permissionService.hasPermissionSync(permission)
    );
  }

  private checkAnyPermission(): boolean {
    return this.permissions.some(permission => 
      this.permissionService.hasPermissionSync(permission)
    );
  }

  private showView(): void {
    if (!this.hasView) {
      this.viewContainer.createEmbeddedView(this.templateRef);
      this.hasView = true;
    }
  }

  private hideView(): void {
    if (this.hasView) {
      this.viewContainer.clear();
      this.hasView = false;
    }
  }
} 