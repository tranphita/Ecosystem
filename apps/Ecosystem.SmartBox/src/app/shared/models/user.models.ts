import { BaseAuditedEntityDto, ExtensibleEntityDto, PagedAndSortedResultRequestDto } from './app-state.models';

// === Base Types ===
export interface ListResultDto<T> {
  items: T[];
}

export interface PagedResultDto<T> extends ListResultDto<T> {
  totalCount: number;
}

// === Company Models ===
export interface SmartBoxCompanyDto extends BaseAuditedEntityDto {
  name: string;
  code: string;
  description?: string;
  address?: string;
  phoneNumber?: string;
  email?: string;
  website?: string;
  taxCode?: string;
  isActive: boolean;
  displayOrder: number;
  logo?: string;
}

export interface CreateUpdateSmartBoxCompanyDto {
  name: string;
  code: string;
  description?: string;
  address?: string;
  phoneNumber?: string;
  email?: string;
  website?: string;
  taxCode?: string;
  isActive: boolean;
  displayOrder?: number;
  logo?: string;
}

export interface GetSmartBoxCompaniesInput extends PagedAndSortedResultRequestDto {
  filter?: string;
  isActive?: boolean;
}

// === Role Models ===
export interface SmartBoxRoleDto extends BaseAuditedEntityDto {
  name: string;
  displayName: string;
  description?: string;
  isSystem: boolean;
  isActive: boolean;
  displayOrder: number;
}

export interface CreateUpdateSmartBoxRoleDto {
  name: string;
  displayName: string;
  description?: string;
  isActive: boolean;
  displayOrder?: number;
}

export interface GetSmartBoxRolesInput extends PagedAndSortedResultRequestDto {
  filter?: string;
  isActive?: boolean;
  isSystem?: boolean;
}

// === User Models ===
export interface SmartBoxUserDto extends BaseAuditedEntityDto {
  userName: string;
  email: string;
  fullName?: string;
  phoneNumber?: string;
  dateOfBirth?: Date;
  gender?: number;
  avatar?: string;
  companyId?: string;
  company?: SmartBoxCompanyDto;
  position?: string;
  department?: string;
  employeeCode?: string;
  startDate?: Date;
  address?: string;
  notes?: string;
  isActive: boolean;
  lastLoginTime?: Date;
  roles: SmartBoxRoleDto[];
}

export interface CreateUpdateSmartBoxUserDto {
  userName: string;
  email: string;
  password?: string;
  fullName?: string;
  phoneNumber?: string;
  dateOfBirth?: Date;
  gender?: number;
  avatar?: string;
  companyId?: string;
  position?: string;
  department?: string;
  employeeCode?: string;
  startDate?: Date;
  address?: string;
  notes?: string;
  isActive: boolean;
  roleIds: string[];
}

export interface GetSmartBoxUsersInput extends PagedAndSortedResultRequestDto {
  filter?: string;
  isActive?: boolean;
  companyId?: string;
  roleId?: string;
  department?: string;
  position?: string;
}

export interface AssignRoleToUserInput {
  userId: string;
  roleIds: string[];
}

export interface ToggleUserActiveInput {
  userId: string;
  isActive: boolean;
}

// === Permission Models ===
export interface SmartBoxPermissionDto {
  name: string;
  displayName: string;
  parentName?: string;
  isGranted: boolean;
  group?: string;
}

export interface GetPermissionListResultDto {
  entityDisplayName: string;
  groups: PermissionGroupDto[];
}

export interface PermissionGroupDto {
  name: string;
  displayName: string;
  permissions: SmartBoxPermissionDto[];
}

export interface UpdatePermissionsDto {
  permissions: UpdatePermissionDto[];
}

export interface UpdatePermissionDto {
  name: string;
  isGranted: boolean;
} 