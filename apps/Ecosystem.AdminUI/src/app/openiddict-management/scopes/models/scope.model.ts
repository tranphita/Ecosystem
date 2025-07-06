// OpenIddict Scope Models (moved from application.dto.ts)
export interface ScopeModel {
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

export interface CreateScopeModel {
  name: string;
  displayName?: string;
  displayNames?: { [key: string]: string };
  description?: string;
  descriptions?: { [key: string]: string };
  resources?: string[];
}

export interface UpdateScopeModel {
  displayName?: string;
  displayNames?: { [key: string]: string };
  description?: string;
  descriptions?: { [key: string]: string };
  resources?: string[];
}

export interface GetScopeListModel {
  filter?: string;
  skipCount?: number;
  maxResultCount?: number;
}

export interface ScopeListResultModel {
  items: ScopeModel[];
  totalCount: number;
}
