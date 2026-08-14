import { Injectable, inject } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';
import { PublicUserProfileDto, UpdateUserPreferencesDto } from '../models/user-profile.model';

@Injectable({
  providedIn: 'root',
})
export class UserProfileService {
  private readonly rest = inject(RestService);
  private readonly apiName = 'Default';

  getPublicProfile(userId: string): Observable<PublicUserProfileDto> {
    return this.rest.request<unknown, PublicUserProfileDto>({
      method: 'GET',
      url: `/api/app/user-profile/${userId}/public-profile`,
    }, { apiName: this.apiName });
  }

  getMyProfile(): Observable<PublicUserProfileDto> {
    return this.rest.request<unknown, PublicUserProfileDto>({
      method: 'GET',
      url: '/api/app/user-profile/my-profile',
    }, { apiName: this.apiName });
  }

  updatePreferences(input: UpdateUserPreferencesDto): Observable<void> {
    return this.rest.request<UpdateUserPreferencesDto, void>({
      method: 'PUT',
      url: '/api/app/user-profile/preferences',
      body: input,
    }, { apiName: this.apiName });
  }

  deleteAccount(): Observable<void> {
    return this.rest.request<unknown, void>({
      method: 'DELETE',
      url: '/api/app/user-profile/account',
    }, { apiName: this.apiName });
  }
}
