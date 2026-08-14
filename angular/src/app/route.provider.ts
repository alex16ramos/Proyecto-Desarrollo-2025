import { RoutesService, eLayoutType } from '@abp/ng.core';
import { inject, provideAppInitializer } from '@angular/core';

export const APP_ROUTE_PROVIDER = [
  provideAppInitializer(() => {
    configureRoutes();
  }),
];

function configureRoutes() {
  const routes = inject(RoutesService);
  routes.add([
    {
      path: '/',
      name: '::Menu:Home',
      iconClass: 'fas fa-home',
      order: 1,
      layout: eLayoutType.application,
    },
    {
      path: '/destinations',
      name: 'Destinos',
      iconClass: 'fas fa-map-marked-alt',
      order: 2,
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
