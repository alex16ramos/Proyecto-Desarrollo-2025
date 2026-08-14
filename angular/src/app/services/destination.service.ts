import { Injectable, inject } from '@angular/core';
import { RestService, PagedResultDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import { Observable } from 'rxjs';
import { DestinationDto, CreateUpdateDestinationDto, CitySearchResultDto } from '../models/destination.model';

@Injectable({
  providedIn: 'root',
})
export class DestinationService {
  private readonly rest = inject(RestService);
  private readonly apiName = 'Default';

  getList(params: PagedAndSortedResultRequestDto): Observable<PagedResultDto<DestinationDto>> {
    return this.rest.request<unknown, PagedResultDto<DestinationDto>>({
      method: 'GET',
      url: '/api/app/destination',
      params: {
        skipCount: params.skipCount,
        maxResultCount: params.maxResultCount,
        sorting: params.sorting,
      },
    }, { apiName: this.apiName });
  }

  get(id: string): Observable<DestinationDto> {
    return this.rest.request<unknown, DestinationDto>({
      method: 'GET',
      url: `/api/app/destination/${id}`,
    }, { apiName: this.apiName });
  }

  create(input: CreateUpdateDestinationDto): Observable<DestinationDto> {
    return this.rest.request<CreateUpdateDestinationDto, DestinationDto>({
      method: 'POST',
      url: '/api/app/destination',
      body: input,
    }, { apiName: this.apiName });
  }

  update(id: string, input: CreateUpdateDestinationDto): Observable<DestinationDto> {
    return this.rest.request<CreateUpdateDestinationDto, DestinationDto>({
      method: 'PUT',
      url: `/api/app/destination/${id}`,
      body: input,
    }, { apiName: this.apiName });
  }

  delete(id: string): Observable<void> {
    return this.rest.request<unknown, void>({
      method: 'DELETE',
      url: `/api/app/destination/${id}`,
    }, { apiName: this.apiName });
  }

  searchCities(prefix: string): Observable<CitySearchResultDto[]> {
    return this.rest.request<unknown, CitySearchResultDto[]>({
      method: 'GET',
      url: '/api/app/destination/search-cities',
      params: { namePrefix: prefix },
    }, { apiName: this.apiName });
  }

  getPopularDestinations(count: number = 6): Observable<DestinationDto[]> {
    return this.rest.request<unknown, DestinationDto[]>({
      method: 'GET',
      url: '/api/app/destination/popular-destinations',
      params: { count },
    }, { apiName: this.apiName });
  }
}
