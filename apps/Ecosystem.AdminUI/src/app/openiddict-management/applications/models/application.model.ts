// OpenIddict Application Models (moved from application.dto.ts)
export interface ApplicationModel {
  id: string;
  clientId: string;
  clientSecret?: string;
  consentType?: string;
  displayName?: string;
  displayNames?: { [key: string]: string };
  type?: string;
  clientUri?: string;
  logoUri?: string;
  permissions?: string[];
  postLogoutRedirectUris?: string[];
  redirectUris?: string[];
  requirements?: string[];
  grantTypes?: string[];
  responseTypes?: string[];
  scopes?: string[];
  creationTime: Date;
  lastModificationTime?: Date;
}

export interface CreateApplicationModel {
  clientId: string;
  clientSecret?: string;
  consentType?: string;
  displayName?: string;
  displayNames?: { [key: string]: string };
  type?: string;
  clientUri?: string;
  logoUri?: string;
  permissions?: string[];
  postLogoutRedirectUris?: string[];
  redirectUris?: string[];
  requirements?: string[];
  grantTypes?: string[];
  responseTypes?: string[];
  scopes?: string[];
}

export interface UpdateApplicationModel {
  clientSecret?: string;
  consentType?: string;
  displayName?: string;
  displayNames?: { [key: string]: string };
  type?: string;
  clientUri?: string;
  logoUri?: string;
  permissions?: string[];
  postLogoutRedirectUris?: string[];
  redirectUris?: string[];
  requirements?: string[];
  grantTypes?: string[];
  responseTypes?: string[];
  scopes?: string[];
}

export interface GetApplicationListModel {
  filter?: string;
  skipCount?: number;
  maxResultCount?: number;
}

export interface ApplicationListResultModel {
  items: ApplicationModel[];
  totalCount: number;
}
