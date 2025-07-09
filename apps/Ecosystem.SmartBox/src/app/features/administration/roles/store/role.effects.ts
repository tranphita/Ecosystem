import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, switchMap, tap } from 'rxjs/operators';
import { of } from 'rxjs';
import { RoleApiService } from '../../../../core/services/api';
import * as RoleActions from './role.actions';

/**
 * Role Effects cho NgRx Store
 */
@Injectable()
export class RoleEffects {
  constructor(
    private actions$: Actions,
    private roleApiService: RoleApiService
  ) {}

  // === Load Roles Effect ===
  loadRoles$ = createEffect(() =>
    this.actions$.pipe(
      ofType(RoleActions.loadRoles),
      switchMap(({ input }) =>
        this.roleApiService.getRoles(input).pipe(
          map(result => RoleActions.loadRolesSuccess({ result })),
          catchError(error => of(RoleActions.loadRolesFailure({ error })))
        )
      )
    )
  );

  // === Load Active Roles Effect ===
  loadActiveRoles$ = createEffect(() =>
    this.actions$.pipe(
      ofType(RoleActions.loadActiveRoles),
      switchMap(() =>
        this.roleApiService.getActiveRoles().pipe(
          map(result => RoleActions.loadActiveRolesSuccess({ roles: result.items })),
          catchError(error => of(RoleActions.loadActiveRolesFailure({ error })))
        )
      )
    )
  );

  // === Load Single Role Effect ===
  loadRole$ = createEffect(() =>
    this.actions$.pipe(
      ofType(RoleActions.loadRole),
      switchMap(({ id }) =>
        this.roleApiService.getRole(id).pipe(
          map(role => RoleActions.loadRoleSuccess({ role })),
          catchError(error => of(RoleActions.loadRoleFailure({ error })))
        )
      )
    )
  );

  // === Create Role Effect ===
  createRole$ = createEffect(() =>
    this.actions$.pipe(
      ofType(RoleActions.createRole),
      switchMap(({ input }) =>
        this.roleApiService.createRole(input).pipe(
          map(role => RoleActions.createRoleSuccess({ role })),
          catchError(error => of(RoleActions.createRoleFailure({ error })))
        )
      )
    )
  );

  // === Update Role Effect ===
  updateRole$ = createEffect(() =>
    this.actions$.pipe(
      ofType(RoleActions.updateRole),
      switchMap(({ id, input }) =>
        this.roleApiService.updateRole(id, input).pipe(
          map(role => RoleActions.updateRoleSuccess({ role })),
          catchError(error => of(RoleActions.updateRoleFailure({ error })))
        )
      )
    )
  );

  // === Delete Role Effect ===
  deleteRole$ = createEffect(() =>
    this.actions$.pipe(
      ofType(RoleActions.deleteRole),
      switchMap(({ id }) =>
        this.roleApiService.deleteRole(id).pipe(
          map(() => RoleActions.deleteRoleSuccess({ id })),
          catchError(error => of(RoleActions.deleteRoleFailure({ error })))
        )
      )
    )
  );

  // === Load Roles by User Effect ===
  loadRolesByUser$ = createEffect(() =>
    this.actions$.pipe(
      ofType(RoleActions.loadRolesByUser),
      switchMap(({ userId }) =>
        this.roleApiService.getRolesByUserId(userId).pipe(
          map(result => RoleActions.loadRolesByUserSuccess({ userId, roles: result.items })),
          catchError(error => of(RoleActions.loadRolesByUserFailure({ error })))
        )
      )
    )
  );

  // === Validation Effects ===
  validateRoleName$ = createEffect(() =>
    this.actions$.pipe(
      ofType(RoleActions.validateRoleName),
      switchMap(({ name, excludeId }) =>
        this.roleApiService.isNameExist(name, excludeId).pipe(
          map(exists => RoleActions.validateRoleNameSuccess({ exists })),
          catchError(error => of(RoleActions.validateRoleNameFailure({ error })))
        )
      )
    )
  );

  validateDisplayName$ = createEffect(() =>
    this.actions$.pipe(
      ofType(RoleActions.validateDisplayName),
      switchMap(({ displayName, excludeId }) =>
        this.roleApiService.isDisplayNameExist(displayName, excludeId).pipe(
          map(exists => RoleActions.validateDisplayNameSuccess({ exists })),
          catchError(error => of(RoleActions.validateDisplayNameFailure({ error })))
        )
      )
    )
  );

  // === Side Effects (không return action) ===
  
  // Reload roles sau khi create/update/delete thành công
  reloadRolesAfterChange$ = createEffect(() =>
    this.actions$.pipe(
      ofType(
        RoleActions.createRoleSuccess,
        RoleActions.updateRoleSuccess,
        RoleActions.deleteRoleSuccess
      ),
      map(() => RoleActions.loadRoles({ 
        input: { 
          skipCount: 0, 
          maxResultCount: 10 
        } 
      }))
    )
  );

  // Reload active roles sau khi có thay đổi
  reloadActiveRolesAfterChange$ = createEffect(() =>
    this.actions$.pipe(
      ofType(
        RoleActions.createRoleSuccess,
        RoleActions.updateRoleSuccess,
        RoleActions.deleteRoleSuccess
      ),
      map(() => RoleActions.loadActiveRoles())
    )
  );

  // Close dialog sau khi create/update thành công
  closeDialogAfterSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(
        RoleActions.createRoleSuccess,
        RoleActions.updateRoleSuccess
      ),
      map(() => RoleActions.closeRoleDialog())
    )
  );

  // Log errors
  logErrors$ = createEffect(() =>
    this.actions$.pipe(
      ofType(
        RoleActions.loadRolesFailure,
        RoleActions.loadActiveRolesFailure,
        RoleActions.loadRoleFailure,
        RoleActions.createRoleFailure,
        RoleActions.updateRoleFailure,
        RoleActions.deleteRoleFailure,
        RoleActions.loadRolesByUserFailure,
        RoleActions.validateRoleNameFailure,
        RoleActions.validateDisplayNameFailure
      ),
      tap(({ error }) => console.error('Role Effect Error:', error))
    ),
    { dispatch: false }
  );
} 