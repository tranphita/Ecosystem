import { Component, OnInit, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { OidcSecurityService } from 'angular-auth-oidc-client';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
})
export class AppComponent implements OnInit {
  title = 'Smartlog DRP';
  private oidcSecurityService = inject(OidcSecurityService);
  ngOnInit() {
    this.oidcSecurityService.checkAuth().subscribe();
  }
}
