import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class AppConfigService {
  private config: any;
  constructor(private http: HttpClient) {}

  loadConfig() {
    return this.http
      .get('/assets/appsettings.json')
      .toPromise()
      .then((cfg) => (this.config = cfg));
  }
  get oidcConfig() {
    return this.config?.oidc;
  }
}
