import { Injectable, inject } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';
import { ExperienceDto, CreateUpdateExperienceDto } from '../models/experience.model';

@Injectable({
  providedIn: 'root',
})
export class ExperienceService {
  private readonly rest = inject(RestService);
  private readonly apiName = 'Default';

  getListByDestination(destinationId: string): Observable<ExperienceDto[]> {
    return this.rest.request<unknown, ExperienceDto[]>({
      method: 'GET',
      url: `/api/app/experience/by-destination/${destinationId}`,
    }, { apiName: this.apiName });
  }

  getUserExperiences(): Observable<ExperienceDto[]> {
    return this.rest.request<unknown, ExperienceDto[]>({
      method: 'GET',
      url: '/api/app/experience/user-experiences',
    }, { apiName: this.apiName });
  }

  create(input: CreateUpdateExperienceDto): Observable<ExperienceDto> {
    return this.rest.request<CreateUpdateExperienceDto, ExperienceDto>({
      method: 'POST',
      url: '/api/app/experience',
      body: input,
    }, { apiName: this.apiName });
  }

  update(id: string, input: CreateUpdateExperienceDto): Observable<ExperienceDto> {
    return this.rest.request<CreateUpdateExperienceDto, ExperienceDto>({
      method: 'PUT',
      url: `/api/app/experience/${id}`,
      body: input,
    }, { apiName: this.apiName });
  }

  delete(id: string): Observable<void> {
    return this.rest.request<unknown, void>({
      method: 'DELETE',
      url: `/api/app/experience/${id}`,
    }, { apiName: this.apiName });
  }
}
