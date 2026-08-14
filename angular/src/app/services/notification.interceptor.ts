import { HttpInterceptorFn, HttpResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { tap } from 'rxjs';
import { NotificationService } from './notification.service';

export const notificationInterceptor: HttpInterceptorFn = (req, next) => {
  const notificationService = inject(NotificationService);

  return next(req).pipe(
    tap((event) => {
      if (event instanceof HttpResponse && event.status >= 200 && event.status < 300) {
        const url = req.url.toLowerCase();
        // Disparar refresco de campanita cuando se realizan acciones que generan notificaciones
        if (
          (req.method === 'POST' || req.method === 'PUT' || req.method === 'DELETE') &&
          (url.includes('favorite') ||
            url.includes('profile') ||
            url.includes('password') ||
            url.includes('destination') ||
            url.includes('rating') ||
            url.includes('experience'))
        ) {
          notificationService.refreshUnreadCount();
        }
      }
    })
  );
};
