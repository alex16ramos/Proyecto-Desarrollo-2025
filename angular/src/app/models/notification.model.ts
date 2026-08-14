export interface AppNotificationDto {
  id: string;
  userId: string;
  title: string;
  message: string;
  type?: string;
  isRead: boolean;
  creationTime: string;
}

export interface CreateNotificationDto {
  userId: string;
  title: string;
  message: string;
  type?: string;
}
