import { Injectable, inject } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';
import { DestinationDto } from '../models/destination.model';

@Injectable({
  providedIn: 'root',
})
export class FavoriteService {
  private readonly rest = inject(RestService);
  private readonly apiName = 'Default';

  toggleFavorite(destinationId: string): Observable<boolean> {
    return this.rest.request<unknown, boolean>({
      method: 'POST',
      url: `/api/app/favorite-destination/toggle-favorite/${destinationId}`,
    }, { apiName: this.apiName });
  }

  isFavorite(destinationId: string): Observable<boolean> {
    return this.rest.request<unknown, boolean>({
      method: 'GET',
      url: `/api/app/favorite-destination/is-favorite/${destinationId}`,
    }, { apiName: this.apiName });
  }

  getUserFavorites(): Observable<DestinationDto[]> {
    return this.rest.request<unknown, DestinationDto[]>({
      method: 'GET',
      url: '/api/app/favorite-destination/user-favorites',
    }, { apiName: this.apiName });
  }
}
