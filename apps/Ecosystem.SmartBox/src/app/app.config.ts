import {
  ApplicationConfig,
  provideZoneChangeDetection,
  importProvidersFrom,
  ErrorHandler,
  isDevMode,
} from '@angular/core';
import {
  HttpClient,
  provideHttpClient,
  withInterceptorsFromDi,
  HTTP_INTERCEPTORS,
} from '@angular/common/http';
import { routes } from './app.routes';
import {
  provideRouter,
  withComponentInputBinding,
  withInMemoryScrolling,
} from '@angular/router';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideClientHydration } from '@angular/platform-browser';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

// NgRx imports
import { provideStore } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { provideStoreDevtools } from '@ngrx/store-devtools';

// Store imports
import { userReducer } from './features/administration/users/store/user.reducer';
import { roleReducer } from './features/administration/roles/store/role.reducer';
// import { companyReducer } from './features/administration/companies/store/company.reducer';
import { UserEffects } from './features/administration/users/store/user.effects';
import { RoleEffects } from './features/administration/roles/store/role.effects';
// import { CompanyEffects } from './features/administration/companies/store/company.effects';

// icons
import { TablerIconsModule } from 'angular-tabler-icons';
import * as TablerIcons from 'angular-tabler-icons/icons';

// perfect scrollbar
import { NgScrollbarModule } from 'ngx-scrollbar';
//Import all material modules
import { MaterialModule } from './material.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AuthModule, LogLevel } from 'angular-auth-oidc-client';

// Core services và error handling
import { ErrorHandlerService } from './core/errors/error-handler.service';
import { ErrorInterceptor } from './core/interceptors/error.interceptor';
import { AuthInterceptor } from './core/interceptors/auth.interceptor';

export function HttpLoaderFactory(http: HttpClient): any {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

export function getOidcConfig() {
  const cfg = (window as any)['appConfig']?.oidc;
  return {
    authority: cfg?.authority || 'https://localhost:7600',
    clientId: cfg?.clientId || 'SmartBox_Angular',
    redirectUrl: cfg?.redirectUrl || 'http://localhost:4300/callback',
    postLogoutRedirectUri: cfg?.postLogoutRedirectUri || 'http://localhost:4300',
    scope: cfg?.scope || 'openid profile email address phone offline_access EcosystemSmartBox',
    responseType: cfg?.responseType || 'code',
    useRefreshToken: cfg?.useRefreshToken ?? true,
    logLevel: LogLevel.Error,
    secureRoutes: ['https://localhost:7500/api']
  };
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(
      routes,
      withInMemoryScrolling({
        scrollPositionRestoration: 'enabled',
        anchorScrolling: 'enabled',
      }),
      withComponentInputBinding()
    ),
    provideHttpClient(withInterceptorsFromDi()),
    provideAnimationsAsync(),

    // NgRx Store Configuration
    provideStore({
      users: userReducer,
      roles: roleReducer
      // companies: companyReducer
    }),

    // NgRx Effects
    provideEffects([
      UserEffects,
      RoleEffects
      // CompanyEffects
    ]),

    // NgRx DevTools (chỉ trong development)
    provideStoreDevtools({
      maxAge: 25,
      logOnly: !isDevMode(),
      autoPause: true,
      trace: false,
      traceLimit: 75
    }),

    // Global error handling
    {
      provide: ErrorHandler,
      useClass: ErrorHandlerService
    },

    // HTTP Interceptors
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ErrorInterceptor,
      multi: true
    },

    importProvidersFrom(
      FormsModule,
      ReactiveFormsModule,
      MaterialModule,
      TablerIconsModule.pick(TablerIcons),
      NgScrollbarModule,
      TranslateModule.forRoot({
        loader: {
          provide: TranslateLoader,
          useFactory: HttpLoaderFactory,
          deps: [HttpClient],
        },
      }),
      AuthModule.forRoot({
        config: getOidcConfig(),
      })
    ),
  ],
};
