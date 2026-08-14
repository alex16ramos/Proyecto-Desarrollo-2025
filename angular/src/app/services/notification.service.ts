import { Injectable, inject } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';
import { AppNotificationDto, CreateNotificationDto } from '../models/notification.model';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private readonly rest = inject(RestService);
  private readonly apiName = 'Default';

  getUserNotifications(): Observable<AppNotificationDto[]> {
    return this.rest.request<unknown, AppNotificationDto[]>({
      method: 'GET',
      url: '/api/app/notification/user-notifications',
    }, { apiName: this.apiName });
  }

  getUnreadCount(): Observable<number> {
    return this.rest.request<unknown, number>({
      method: 'GET',
      url: '/api/app/notification/unread-count',
    }, { apiName: this.apiName });
  }

  markAsRead(id: string): Observable<void> {
    return this.rest.request<unknown, void>({
      method: 'POST',
      url: `/api/app/notification/${id}/mark-as-read`,
    }, { apiName: this.apiName });
  }

  markAllAsRead(): Observable<void> {
    return this.rest.request<unknown, void>({
      method: 'POST',
      url: '/api/app/notification/mark-all-as-read',
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
