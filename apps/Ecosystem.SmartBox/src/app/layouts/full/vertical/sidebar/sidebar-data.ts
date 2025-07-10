import { NavItem } from './nav-item/nav-item';

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
    divider: true,
    navCap: 'Other',
  },
  {
    displayName: 'Menu Level',
    iconName: 'solar:align-horizontal-center-line-duotone',
    route: '/menu-level',
    children: [
      {
        displayName: 'Menu 1',
        iconName: 'solar:round-alt-arrow-right-line-duotone',
        route: '/menu-1',
        children: [
          {
            displayName: 'Menu 1',
            subItemIcon: true,
            iconName: 'solar:round-alt-arrow-right-line-duotone',
            route: '/menu-1',
          },

          {
            displayName: 'Menu 2',
            subItemIcon: true,
            iconName: 'solar:round-alt-arrow-right-line-duotone',
            route: '/menu-2',
          },
        ],
      },

      {
        displayName: 'Menu 2',
        iconName: 'solar:round-alt-arrow-right-line-duotone',
        route: '/menu-2',
      },
    ],
  },
  {
    displayName: 'Disabled',
    iconName: 'solar:bookmark-circle-line-duotone',
    route: '/disabled',
    disabled: true,
  },
  {
    displayName: 'Chip',
    iconName: 'solar:branching-paths-up-line-duotone',
    route: '/',
    chip: true,
    chipClass: 'bg-primary text-white',
    chipContent: '9',
  },
  {
    displayName: 'Outlined',
    iconName: 'solar:add-square-line-duotone',
    route: '/',
    chip: true,
    chipClass: 'b-1 border-primary text-primary',
    chipContent: 'outlined',
  },
  {
    displayName: 'External Link',
    iconName: 'solar:link-round-angle-bold-duotone',
    route: 'https://www.google.com/',
    external: true,
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
