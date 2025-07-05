// OpenIddict Application Models
export interface OpenIddictApplicationDto {
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

export interface CreateOpenIddictApplicationDto {
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

export interface UpdateOpenIddictApplicationDto {
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

// OpenIddict Scope Models
export interface OpenIddictScopeDto {
  id: string;
  name: string;
  displayName?: string;
  displayNames?: { [key: string]: string };
  description?: string;
  descriptions?: { [key: string]: string };
  resources?: string[];
  creationTime: Date;
  lastModificationTime?: Date;
}

export interface CreateOpenIddictScopeDto {
  name: string;
  displayName?: string;
  displayNames?: { [key: string]: string };
  description?: string;
  descriptions?: { [key: string]: string };
  resources?: string[];
}

export interface UpdateOpenIddictScopeDto {
  displayName?: string;
  displayNames?: { [key: string]: string };
  description?: string;
  descriptions?: { [key: string]: string };
  resources?: string[];
}

// Common DTOs
export interface GetOpenIddictApplicationListDto {
  filter?: string;
  skipCount?: number;
  maxResultCount?: number;
}

export interface GetOpenIddictScopeListDto {
  filter?: string;
  skipCount?: number;
  maxResultCount?: number;
}

export interface OpenIddictApplicationListResultDto {
  items: OpenIddictApplicationDto[];
  totalCount: number;
}

export interface OpenIddictScopeListResultDto {
  items: OpenIddictScopeDto[];
  totalCount: number;
}

// Enums và Constants
export enum OpenIddictConstants {
  // Application Types
  ApplicationType_Web = 'web',
  ApplicationType_Native = 'native',
  ApplicationType_Hybrid = 'hybrid',
  
  // Grant Types
  GrantType_AuthorizationCode = 'authorization_code',
  GrantType_ClientCredentials = 'client_credentials',
  GrantType_RefreshToken = 'refresh_token',
  GrantType_Password = 'password',
  GrantType_Implicit = 'implicit',
  GrantType_DeviceCode = 'urn:ietf:params:oauth:grant-type:device_code',
  
  // Response Types
  ResponseType_Code = 'code',
  ResponseType_Token = 'token',
  ResponseType_IdToken = 'id_token',
  
  // Consent Types
  ConsentType_Explicit = 'explicit',
  ConsentType_External = 'external',
  ConsentType_Implicit = 'implicit',
  ConsentType_Systematic = 'systematic',
  
  // Scopes
  Scope_OpenId = 'openid',
  Scope_Profile = 'profile',
  Scope_Email = 'email',
  Scope_Address = 'address',
  Scope_Phone = 'phone',
  Scope_Roles = 'roles',
} 