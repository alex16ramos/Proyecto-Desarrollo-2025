import { authGuard, permissionGuard } from '@abp/ng.core';
import { Routes } from '@angular/router';

export const APP_ROUTES: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => import('./home/home.component').then(c => c.HomeComponent),
  },
  {
    path: 'destinations',
    loadComponent: () =>
      import('./destinations/destination-list/destination-list.component').then(
        c => c.DestinationListComponent
      ),
  },
  {
    path: 'destinations/:id',
    loadComponent: () =>
      import('./destinations/destination-detail/destination-detail.component').then(
        c => c.DestinationDetailComponent
      ),
  },
  {
    path: 'favorites',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./favorites/favorites.component').then(c => c.FavoritesComponent),
  },
  {
    path: 'experiences',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./experiences/experiences.component').then(c => c.ExperiencesComponent),
  },
  {
    path: 'notifications',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./notifications/notifications.component').then(c => c.NotificationsComponent),
  },
  {
    path: 'profile',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./profile/profile.component').then(c => c.ProfileComponent),
  },
  {
    path: 'profile/:id',
    loadComponent: () =>
      import('./profile/profile.component').then(c => c.ProfileComponent),
  },
  {
    path: 'account',
    loadChildren: () => import('@abp/ng.account').then(c => c.createRoutes()),
  },
  {
    path: 'identity',
    loadChildren: () => import('@abp/ng.identity').then(c => c.createRoutes()),
  },
  {
    path: 'setting-management',
    loadChildren: () => import('@abp/ng.setting-management').then(c => c.createRoutes()),
  },
];
