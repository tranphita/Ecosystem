// OpenIddict Permissions

export const OPENIDDICT_PERMISSIONS = {
  // OpenIddict group permission
  OpenIddict: 'Administration.OpenIddict',
  
  // Applications permissions
  Applications: {
    Default: 'Administration.OpenIddict.Applications',
    Create: 'Administration.OpenIddict.Applications.Create',
    Edit: 'Administration.OpenIddict.Applications.Update',
    Delete: 'Administration.OpenIddict.Applications.Delete',
    ManagePermissions: 'Administration.OpenIddict.Applications.ManagePermissions',
  },
  
  // Scopes permissions
  Scopes: {
    Default: 'Administration.OpenIddict.Scopes',
    Create: 'Administration.OpenIddict.Scopes.Create',
    Edit: 'Administration.OpenIddict.Scopes.Update',
    Delete: 'Administration.OpenIddict.Scopes.Delete',
  },
  
  // Authorization permissions
  Authorizations: {
    Default: 'Administration.OpenIddict.Authorizations',
    Delete: 'Administration.OpenIddict.Authorizations.Delete',
  },
  
  // Token permissions
  Tokens: {
    Default: 'Administration.OpenIddict.Tokens',
    Delete: 'Administration.OpenIddict.Tokens.Delete',
  },
} as const;

export type OpenIddictPermissions = typeof OPENIDDICT_PERMISSIONS; 