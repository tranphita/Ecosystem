// === Base ABP Interfaces ===
export interface BaseAuditedEntityDto {
  id: string;
  creationTime: Date;
  creatorId?: string;
  lastModificationTime?: Date;
  lastModifierId?: string;
}

export interface ExtensibleEntityDto {
  extraProperties?: Record<string, any>;
}

export interface PagedAndSortedResultRequestDto {
  sorting?: string;
  skipCount?: number;
  maxResultCount?: number;
}

// === State Interfaces ===
export interface LoadingState {
  isLoading: boolean;
  error: string | null;
}

export interface PaginationState {
  currentPage: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface BaseFilterState {
  searchTerm: string;
  sortField: string;
  sortDirection: 'asc' | 'desc';
} 