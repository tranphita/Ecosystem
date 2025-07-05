// OpenIddict Permissions

export const OPENIDDICT_PERMISSIONS = {
  // OpenIddict group permission
  OpenIddict: 'OpenIddict',
  
  // Applications permissions
  Applications: {
    Default: 'OpenIddict.Applications',
    Create: 'OpenIddict.Applications.Create',
    Edit: 'OpenIddict.Applications.Edit',
    Delete: 'OpenIddict.Applications.Delete',
    ManagePermissions: 'OpenIddict.Applications.ManagePermissions',
  },
  
  // Scopes permissions
  Scopes: {
    Default: 'OpenIddict.Scopes',
    Create: 'OpenIddict.Scopes.Create',
    Edit: 'OpenIddict.Scopes.Edit',
    Delete: 'OpenIddict.Scopes.Delete',
  },
  
  // Authorization permissions
  Authorizations: {
    Default: 'OpenIddict.Authorizations',
    Delete: 'OpenIddict.Authorizations.Delete',
  },
  
  // Token permissions
  Tokens: {
    Default: 'OpenIddict.Tokens',
    Delete: 'OpenIddict.Tokens.Delete',
  },
} as const;

export type OpenIddictPermissions = typeof OPENIDDICT_PERMISSIONS; 