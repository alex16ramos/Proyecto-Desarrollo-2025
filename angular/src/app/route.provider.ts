import { RoutesService, eLayoutType } from '@abp/ng.core';
import { NavItemsService } from '@abp/ng.theme.shared';
import { inject, provideAppInitializer } from '@angular/core';
import { NotificationNavComponent } from './notifications/notification-nav/notification-nav.component';

export const APP_ROUTE_PROVIDER = [
  provideAppInitializer(() => {
    configureRoutes();
    configureNavItems();
  }),
];

function configureNavItems() {
  const navItems = inject(NavItemsService);
  navItems.addItems([
    {
      id: 'NotificationNav',
      order: 1,
      component: NotificationNavComponent,
    },
  ]);
}

function configureRoutes() {
  const routes = inject(RoutesService);
  routes.add([
    {
      path: '/destinations',
      name: 'Destinos',
      iconClass: 'fas fa-map-marked-alt',
      order: 1,
      layout: eLayoutType.application,
    },
    {
      path: '/favorites',
      name: 'Mis Favoritos',
      iconClass: 'fas fa-heart',
      order: 3,
      layout: eLayoutType.application,
      requiredPolicy: 'FAFS.Favorites',
    },
    {
      path: '/experiences',
      name: 'Mis Experiencias',
      iconClass: 'fas fa-compass',
      order: 4,
      layout: eLayoutType.application,
      requiredPolicy: 'FAFS.Experiences',
    },
    {
      path: '/notifications',
      name: 'Notificaciones',
      iconClass: 'fas fa-bell',
      order: 5,
      layout: eLayoutType.application,
    },
    {
      path: '/profile',
      name: 'Mi Perfil',
      iconClass: 'fas fa-user-circle',
      order: 6,
      layout: eLayoutType.application,
    },
  ]);
}
