import { NavItem } from '../../vertical/sidebar/nav-item/nav-item';

export const navItems: NavItem[] = [
  {
    navCap: 'Home',
  },
  {
    displayName: 'Starter',
    iconName: 'solar:home-smile-line-duotone',
    route: '/starter',
  },
  {
    navCap: 'System',
  },
  {
    displayName: 'Role',
    iconName: 'solar:user-id-line-duotone',
    route: '/system/role',
  },
  {
    displayName: 'User',
    iconName: 'solar:users-group-rounded-line-duotone',
    route: '/system/user',
  },
  {
    displayName: 'Function group',
    iconName: 'solar:widget-6-line-duotone',
    route: '/system/function-group',
  },
  {
    displayName: 'Function',
    iconName: 'solar:widget-3-line-duotone',
    route: '/system/function',
  },
  {
    displayName: 'Session',
    iconName: 'solar:clock-circle-line-duotone',
    route: '/system/session',
  },
  {
    displayName: 'Employee',
    iconName: 'solar:users-group-rounded-line-duotone',
    route: '/system/employee',
  },
];
