import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, switchMap, tap } from 'rxjs/operators';
import { of } from 'rxjs';
import { CompanyApiService } from '../../../../core/services/api';
import * as CompanyActions from './company.actions';

/**
 * Company Effects cho NgRx Store
 */
@Injectable()
export class CompanyEffects {
  constructor(
    private actions$: Actions,
    private companyApiService: CompanyApiService
  ) {}

  // === Load Companies Effect ===
  loadCompanies$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CompanyActions.loadCompanies),
      switchMap(({ input }) =>
        this.companyApiService.getCompanies(input).pipe(
          map(result => CompanyActions.loadCompaniesSuccess({ result })),
          catchError(error => of(CompanyActions.loadCompaniesFailure({ error })))
        )
      )
    )
  );

  // === Load Active Companies Effect ===
  loadActiveCompanies$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CompanyActions.loadActiveCompanies),
      switchMap(() =>
        this.companyApiService.getActiveCompanies().pipe(
          map(result => CompanyActions.loadActiveCompaniesSuccess({ companies: result.items })),
          catchError(error => of(CompanyActions.loadActiveCompaniesFailure({ error })))
        )
      )
    )
  );

  // === Load Single Company Effect ===
  loadCompany$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CompanyActions.loadCompany),
      switchMap(({ id }) =>
        this.companyApiService.getCompany(id).pipe(
          map(company => CompanyActions.loadCompanySuccess({ company })),
          catchError(error => of(CompanyActions.loadCompanyFailure({ error })))
        )
      )
    )
  );

  // === Create Company Effect ===
  createCompany$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CompanyActions.createCompany),
      switchMap(({ input }) =>
        this.companyApiService.createCompany(input).pipe(
          map(company => CompanyActions.createCompanySuccess({ company })),
          catchError(error => of(CompanyActions.createCompanyFailure({ error })))
        )
      )
    )
  );

  // === Update Company Effect ===
  updateCompany$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CompanyActions.updateCompany),
      switchMap(({ id, input }) =>
        this.companyApiService.updateCompany(id, input).pipe(
          map(company => CompanyActions.updateCompanySuccess({ company })),
          catchError(error => of(CompanyActions.updateCompanyFailure({ error })))
        )
      )
    )
  );

  // === Delete Company Effect ===
  deleteCompany$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CompanyActions.deleteCompany),
      switchMap(({ id }) =>
        this.companyApiService.deleteCompany(id).pipe(
          map(() => CompanyActions.deleteCompanySuccess({ id })),
          catchError(error => of(CompanyActions.deleteCompanyFailure({ error })))
        )
      )
    )
  );

  // === Validation Effects ===
  validateCompanyName$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CompanyActions.validateCompanyName),
      switchMap(({ name, excludeId }) =>
        this.companyApiService.isNameExist(name, excludeId).pipe(
          map(exists => CompanyActions.validateCompanyNameSuccess({ exists })),
          catchError(error => of(CompanyActions.validateCompanyNameFailure({ error })))
        )
      )
    )
  );

  validateCompanyCode$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CompanyActions.validateCompanyCode),
      switchMap(({ code, excludeId }) =>
        this.companyApiService.isNameExist(code, excludeId).pipe(
          map(exists => CompanyActions.validateCompanyCodeSuccess({ exists: Boolean(exists) })),
          catchError(error => of(CompanyActions.validateCompanyCodeFailure({ error: String(error) })))
        )
      )
    )
  );

  // === Side Effects (không return action) ===
  
  // Reload companies sau khi create/update/delete thành công
  reloadCompaniesAfterChange$ = createEffect(() =>
    this.actions$.pipe(
      ofType(
        CompanyActions.createCompanySuccess,
        CompanyActions.updateCompanySuccess,
        CompanyActions.deleteCompanySuccess
      ),
      map(() => CompanyActions.loadCompanies({ 
        input: { 
          skipCount: 0, 
          maxResultCount: 10 
        } 
      }))
    )
  );

  // Reload active companies sau khi có thay đổi
  reloadActiveCompaniesAfterChange$ = createEffect(() =>
    this.actions$.pipe(
      ofType(
        CompanyActions.createCompanySuccess,
        CompanyActions.updateCompanySuccess,
        CompanyActions.deleteCompanySuccess
      ),
      map(() => CompanyActions.loadActiveCompanies())
    )
  );

  // Close dialog sau khi create/update thành công
  closeDialogAfterSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(
        CompanyActions.createCompanySuccess,
        CompanyActions.updateCompanySuccess
      ),
      map(() => CompanyActions.closeCompanyDialog())
    )
  );

  // Log errors
  logErrors$ = createEffect(() =>
    this.actions$.pipe(
      ofType(
        CompanyActions.loadCompaniesFailure,
        CompanyActions.loadActiveCompaniesFailure,
        CompanyActions.loadCompanyFailure,
        CompanyActions.createCompanyFailure,
        CompanyActions.updateCompanyFailure,
        CompanyActions.deleteCompanyFailure,
        CompanyActions.validateCompanyNameFailure,
        CompanyActions.validateCompanyCodeFailure
      ),
      tap(({ error }) => console.error('Company Effect Error:', error))
    ),
    { dispatch: false }
  );
} 