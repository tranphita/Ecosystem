import { Component, OnInit, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
})
export class AppComponent implements OnInit {
  title = 'Smartlog DRP';
  private authService = inject(AuthService);
  
  ngOnInit() {
    // AuthService sẽ tự động initialize auth state
    console.log('🚀 App started, auth service initialized');
    
    // Subscribe để theo dõi auth state changes
    this.authService.authState$.subscribe(state => {
      console.log('📊 Auth state changed:', state);
    });
  }
}
