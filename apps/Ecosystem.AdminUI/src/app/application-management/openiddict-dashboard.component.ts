import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil, filter } from 'rxjs/operators';

@Component({
  selector: 'app-openiddict-dashboard',
  templateUrl: './openiddict-dashboard.component.html',
  styleUrls: ['./openiddict-dashboard.component.scss']
})
export class OpenIddictDashboardComponent implements OnInit, OnDestroy {
  
  // Tab hiện tại
  currentTab: string = 'applications';
  
  // Subject để handle unsubscribe
  private destroy$ = new Subject<void>();
  
  // Các tab available
  tabs = [
    {
      key: 'applications',
      label: 'Applications',
      icon: 'fas fa-th-large'
    },
    {
      key: 'scopes',
      label: 'Scopes',
      icon: 'fas fa-key'
    }
  ];

  constructor(
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Check initial route data
    this.checkCurrentRoute();
    
    // Listen to router navigation events để detect route changes
    this.router.events
      .pipe(
        filter(event => event instanceof NavigationEnd),
        takeUntil(this.destroy$)
      )
      .subscribe(() => {
        this.checkCurrentRoute();
      });

    // Subscribe to route data changes
    this.route.data
      .pipe(takeUntil(this.destroy$))
      .subscribe(data => {
        if (data['tab']) {
          this.currentTab = data['tab'];
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private checkCurrentRoute(): void {
    const routeData = this.route.snapshot.data;
    const url = this.router.url;
    
    if (routeData['tab']) {
      this.currentTab = routeData['tab'];
    } else {
      // Default based on URL
      if (url.includes('/scopes')) {
        this.currentTab = 'scopes';
      } else {
        this.currentTab = 'applications';
      }
    }
  }

  /**
   * Chuyển tab
   */
  switchTab(tabKey: string): void {
    console.log('🔥 switchTab called with:', tabKey);
    
    try {
      // Navigate to specific route for each tab
      if (tabKey === 'applications') {
        console.log('🔥 Navigating to applications...');
        this.router.navigate(['/application-management/applications']).then(success => {
          console.log('🔥 Navigation success:', success);
        }).catch(error => {
          console.error('🔥 Navigation error:', error);
        });
      } else if (tabKey === 'scopes') {
        console.log('🔥 Navigating to scopes...');
        this.router.navigate(['/application-management/scopes']).then(success => {
          console.log('🔥 Navigation success:', success);
        }).catch(error => {
          console.error('🔥 Navigation error:', error);
        });
      } else {
        // Fallback to setting tab directly
        console.log('🔥 Setting tab directly:', tabKey);
        this.currentTab = tabKey;
      }
    } catch (error) {
      console.error('🔥 Error in switchTab:', error);
    }
  }

    /**
   * Kiểm tra tab có active không
   */
  isTabActive(tabKey: string): boolean {
    return this.currentTab === tabKey;
  }



 
}  