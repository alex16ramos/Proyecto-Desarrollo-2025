import { Component, OnInit, OnDestroy, inject, HostListener, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '@abp/ng.core';
import { Subscription } from 'rxjs';
import { NotificationService } from '../../services/notification.service';
import { AppNotificationDto } from '../../models/notification.model';

@Component({
  selector: 'app-notification-nav',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './notification-nav.component.html',
  styleUrls: ['./notification-nav.component.scss'],
})
export class NotificationNavComponent implements OnInit, OnDestroy {
  private readonly notificationService = inject(NotificationService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly elementRef = inject(ElementRef);

  unreadCount = 0;
  notifications: AppNotificationDto[] = [];
  isOpen = false;
  isLoading = false;

  private unreadSub?: Subscription;

  get isAuthenticated(): boolean {
    return this.authService.isAuthenticated;
  }

  ngOnInit(): void {
    if (this.isAuthenticated) {
      this.unreadSub = this.notificationService.unreadCount$.subscribe((count) => {
        this.unreadCount = count;
      });
      // Verificar eventos de favoritos al inicializar y refrescar contador
      this.notificationService.checkUpcomingEventsForFavorites().subscribe({
        next: () => this.notificationService.refreshUnreadCount(),
        error: () => this.notificationService.refreshUnreadCount()
      });
    }
  }

  ngOnDestroy(): void {
    this.unreadSub?.unsubscribe();
  }

  toggleDropdown(event: Event): void {
    event.stopPropagation();
    if (!this.isAuthenticated) {
      this.authService.navigateToLogin();
      return;
    }

    this.isOpen = !this.isOpen;
    if (this.isOpen) {
      this.loadRecentNotifications();
    }
  }

  loadRecentNotifications(): void {
    this.isLoading = true;
    this.notificationService.getUserNotifications().subscribe({
      next: (list) => {
        this.notifications = (list || []).slice(0, 5);
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  markAsRead(event: Event, item: AppNotificationDto): void {
    event.stopPropagation();
    if (item.isRead) return;

    this.notificationService.markAsRead(item.id).subscribe({
      next: () => {
        item.isRead = true;
      },
    });
  }

  markAllAsRead(event: Event): void {
    event.stopPropagation();
    this.notificationService.markAllAsRead().subscribe({
      next: () => {
        this.notifications.forEach((n) => (n.isRead = true));
      },
    });
  }

  goToAllNotifications(): void {
    this.isOpen = false;
    this.router.navigate(['/notifications']);
  }

  onLogout(): void {
    this.authService.logout().subscribe();
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.isOpen = false;
    }
  }
}
