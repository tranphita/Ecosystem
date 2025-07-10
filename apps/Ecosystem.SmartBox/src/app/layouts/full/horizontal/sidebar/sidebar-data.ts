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
    navCap: 'Administration',
  },
  {
    displayName: 'Users',
    iconName: 'solar:users-group-rounded-line-duotone',
    route: '/administration/users',
  },
  {
    displayName: 'Roles',
    iconName: 'solar:user-id-line-duotone',
    route: '/administration/roles',
  },
  {
    navCap: 'System',
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
