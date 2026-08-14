import { Injectable, inject } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';
import { DestinationRatingDto, DestinationRatingSummaryDto, RateDestinationDto } from '../models/rating.model';

@Injectable({
  providedIn: 'root',
})
export class DestinationRatingService {
  private readonly rest = inject(RestService);
  private readonly apiName = 'Default';

  rateDestination(input: RateDestinationDto): Observable<void> {
    return this.rest.request<RateDestinationDto, void>({
      method: 'POST',
      url: `/api/app/destination-rating/rate?destinationId=${input.destinationId}&score=${input.score}${input.comment ? `&comment=${encodeURIComponent(input.comment)}` : ''}`,
    }, { apiName: this.apiName });
  }

  getRatings(destinationId: string): Observable<DestinationRatingDto[]> {
    return this.rest.request<unknown, DestinationRatingDto[]>({
      method: 'GET',
      url: `/api/app/destination-rating/${destinationId}/ratings`,
    }, { apiName: this.apiName });
  }

  getSummary(destinationId: string): Observable<DestinationRatingSummaryDto> {
    return this.rest.request<unknown, DestinationRatingSummaryDto>({
      method: 'GET',
      url: `/api/app/destination-rating/${destinationId}/summary`,
    }, { apiName: this.apiName });
  }

  getMyRating(destinationId: string): Observable<DestinationRatingDto | null> {
    return this.rest.request<unknown, DestinationRatingDto | null>({
      method: 'GET',
      url: `/api/app/destination-rating/my-rating-for-destination?destinationId=${destinationId}`,
    }, { apiName: this.apiName });
  }

  updateMyRating(ratingId: string, score: number, comment?: string): Observable<void> {
    return this.rest.request<unknown, void>({
      method: 'PUT',
      url: `/api/app/destination-rating/my-rating?ratingId=${ratingId}&score=${score}${comment ? `&comment=${encodeURIComponent(comment)}` : ''}`,
    }, { apiName: this.apiName });
  }

  deleteMyRating(ratingId: string): Observable<void> {
    return this.rest.request<unknown, void>({
      method: 'DELETE',
      url: `/api/app/destination-rating/my-rating?ratingId=${ratingId}`,
    }, { apiName: this.apiName });
  }
}
