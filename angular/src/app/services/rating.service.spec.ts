import { TestBed } from '@angular/core/testing';
import { RestService } from '@abp/ng.core';
import { of } from 'rxjs';
import { DestinationRatingService } from './rating.service';
import { DestinationRatingSummaryDto } from '../models/rating.model';

describe('DestinationRatingService', () => {
  let service: DestinationRatingService;
  let restServiceSpy: jasmine.SpyObj<RestService>;

  beforeEach(() => {
    restServiceSpy = jasmine.createSpyObj('RestService', ['request']);

    TestBed.configureTestingModule({
      providers: [
        DestinationRatingService,
        { provide: RestService, useValue: restServiceSpy },
      ],
    });

    service = TestBed.inject(DestinationRatingService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get rating summary', (done) => {
    const mockSummary: DestinationRatingSummaryDto = {
      destinationId: 'dest-1',
      averageScore: 4.8,
      totalRatings: 12,
      ratings: [],
    };

    restServiceSpy.request.and.returnValue(of(mockSummary));

    service.getSummary('dest-1').subscribe((summary) => {
      expect(summary.averageScore).toBe(4.8);
      expect(summary.totalRatings).toBe(12);
      done();
    });
  });
});
