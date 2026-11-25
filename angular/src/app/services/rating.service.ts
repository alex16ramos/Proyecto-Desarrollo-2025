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
    return this.rest.request<unknown, void>({
      method: 'POST',
      url: `/api/app/destination-rating/rate-destination/${input.destinationId}`,
      params: {
        score: input.score,
        comment: input.comment,
      },
    }, { apiName: this.apiName });
  }

  getRatings(destinationId: string): Observable<DestinationRatingDto[]> {
    return this.rest.request<unknown, DestinationRatingDto[]>({
      method: 'GET',
      url: `/api/app/destination-rating/ratings-for-destination/${destinationId}`,
    }, { apiName: this.apiName });
  }

  getSummary(destinationId: string): Observable<DestinationRatingSummaryDto> {
    return this.rest.request<unknown, DestinationRatingSummaryDto>({
      method: 'GET',
      url: `/api/app/destination-rating/destination-rating-summary/${destinationId}`,
    }, { apiName: this.apiName });
  }

  getMyRating(destinationId: string): Observable<DestinationRatingDto | null> {
    return this.rest.request<unknown, DestinationRatingDto | null>({
      method: 'GET',
      url: `/api/app/destination-rating/my-rating-for-destination/${destinationId}`,
    }, { apiName: this.apiName });
  }

  updateMyRating(ratingId: string, score: number, comment?: string): Observable<void> {
    return this.rest.request<unknown, void>({
      method: 'PUT',
      url: `/api/app/destination-rating/my-rating/${ratingId}`,
      params: {
        score,
        comment,
      },
    }, { apiName: this.apiName });
  }

  deleteMyRating(ratingId: string): Observable<void> {
    return this.rest.request<unknown, void>({
      method: 'DELETE',
      url: `/api/app/destination-rating/my-rating/${ratingId}`,
    }, { apiName: this.apiName });
  }
}
