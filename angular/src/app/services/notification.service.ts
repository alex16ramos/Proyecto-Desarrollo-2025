import { Injectable, inject } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { AppNotificationDto, CreateNotificationDto } from '../models/notification.model';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private readonly rest = inject(RestService);
  private readonly apiName = 'Default';

  private readonly unreadCountSubject = new BehaviorSubject<number>(0);
  readonly unreadCount$ = this.unreadCountSubject.asObservable();

  getUserNotifications(): Observable<AppNotificationDto[]> {
    return this.rest.request<unknown, AppNotificationDto[]>({
      method: 'GET',
      url: '/api/app/notification/user-notifications',
    }, { apiName: this.apiName }).pipe(
      tap((list) => {
        const unread = (list || []).filter((n) => !n.isRead).length;
        this.unreadCountSubject.next(unread);
      })
    );
  }

  getUnreadCount(): Observable<number> {
    return this.rest.request<unknown, number>({
      method: 'GET',
      url: '/api/app/notification/unread-count',
    }, { apiName: this.apiName }).pipe(
      tap((count) => this.unreadCountSubject.next(count || 0))
    );
  }

  refreshUnreadCount(): void {
    this.getUnreadCount().subscribe({
      error: () => {}
    });
  }

  markAsRead(id: string): Observable<void> {
    return this.rest.request<unknown, void>({
      method: 'POST',
      url: `/api/app/notification/${id}/mark-as-read`,
    }, { apiName: this.apiName }).pipe(
      tap(() => {
        const current = this.unreadCountSubject.value;
        if (current > 0) {
          this.unreadCountSubject.next(current - 1);
        }
      })
    );
  }

  markAllAsRead(): Observable<void> {
    return this.rest.request<unknown, void>({
      method: 'POST',
      url: '/api/app/notification/mark-all-as-read',
    }, { apiName: this.apiName }).pipe(
      tap(() => this.unreadCountSubject.next(0))
    );
  }

  checkUpcomingEventsForFavorites(): Observable<void> {
    return this.rest.request<unknown, void>({
      method: 'POST',
      url: '/api/app/notification/check-upcoming-events-for-favorites',
    }, { apiName: this.apiName });
  }

  sendNotification(input: CreateNotificationDto): Observable<AppNotificationDto> {
    return this.rest.request<CreateNotificationDto, AppNotificationDto>({
      method: 'POST',
      url: '/api/app/notification/send-notification',
      body: input,
    }, { apiName: this.apiName });
  }
}
