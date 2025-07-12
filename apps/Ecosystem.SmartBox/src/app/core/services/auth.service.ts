/**
 * Auth Service
 * Quản lý authentication state và cung cấp helper methods
 */

import { Injectable, inject } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { OidcSecurityService, LoginResponse } from 'angular-auth-oidc-client';
import { Router } from '@angular/router';

export interface AuthState {
    isAuthenticated: boolean;
    token: string | null;
    user: any | null;
    isLoading: boolean;
}

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private readonly oidcSecurityService = inject(OidcSecurityService);
    private readonly router = inject(Router);

    private authStateSubject = new BehaviorSubject<AuthState>({
        isAuthenticated: false,
        token: null,
        user: null,
        isLoading: true
    });

    public authState$ = this.authStateSubject.asObservable();

    constructor() {
        this.initializeAuth();
    }

    /**
     * Khởi tạo authentication state
     */
    private initializeAuth(): void {
        // Kiểm tra auth state khi service khởi tạo
        this.oidcSecurityService.checkAuth().subscribe((loginResponse: LoginResponse) => {
            this.updateAuthState({
                isAuthenticated: loginResponse.isAuthenticated,
                token: loginResponse.isAuthenticated ? null : null, // Sẽ get riêng
                user: loginResponse.userData,
                isLoading: false
            });

            // Lấy token riêng
            if (loginResponse.isAuthenticated) {
                this.oidcSecurityService.getAccessToken().subscribe(token => {
                    this.updateAuthState({
                        token: token
                    });
                });
            }
        });
    }

    /**
     * Cập nhật auth state
     */
    private updateAuthState(update: Partial<AuthState>): void {
        const currentState = this.authStateSubject.value;
        const newState = { ...currentState, ...update };
        this.authStateSubject.next(newState);
    }

    /**
     * Đăng nhập
     */
    login(): void {
        console.log('🚀 Starting login process...');
        this.oidcSecurityService.authorize();
    }

    /**
     * Đăng xuất
     */
    logout(): void {
        console.log('👋 Starting logout process...');
        this.oidcSecurityService.logoff().subscribe(() => {
            this.updateAuthState({
                isAuthenticated: false,
                token: null,
                user: null,
                isLoading: false
            });
            this.router.navigate(['/authentication/login']);
        });
    }

    /**
     * Kiểm tra xem user có authenticated không
     */
    isAuthenticated(): Observable<boolean> {
        return this.authState$.pipe(map(state => state.isAuthenticated));
    }

    /**
     * Lấy current access token
     */
    getAccessToken(): Observable<string> {
        return this.oidcSecurityService.getAccessToken();
    }

    /**
     * Lấy user data
     */
    getUserData(): Observable<any> {
        return this.authState$.pipe(map(state => state.user));
    }

    /**
     * Force refresh token
     */
    refreshToken(): Observable<LoginResponse> {
        console.log('🔄 Refreshing token...');
        return this.oidcSecurityService.forceRefreshSession();
    }


} 