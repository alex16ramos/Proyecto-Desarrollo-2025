import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { NotificationService } from '../services/notification.service';
import { AppNotificationDto } from '../models/notification.model';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.scss'],
})
export class NotificationsComponent implements OnInit {
  private readonly notificationService = inject(NotificationService);

  notifications: AppNotificationDto[] = [];
  isLoading = true;

  ngOnInit(): void {
    this.loadNotifications();
  }

  loadNotifications(): void {
    this.isLoading = true;
    this.notificationService.getUserNotifications().subscribe({
      next: (list) => {
        this.notifications = list || [];
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  markAsRead(item: AppNotificationDto): void {
    if (item.isRead) return;

    this.notificationService.markAsRead(item.id).subscribe({
      next: () => {
        item.isRead = true;
      },
    });
  }

  markAllAsRead(): void {
    this.notificationService.markAllAsRead().subscribe({
      next: () => {
        this.notifications.forEach((n) => (n.isRead = true));
      },
    });
  }
}
