import { TestBed } from '@angular/core/testing';
import { RestService } from '@abp/ng.core';
import { of } from 'rxjs';
import { DestinationService } from './destination.service';
import { DestinationDto } from '../models/destination.model';

describe('DestinationService', () => {
  let service: DestinationService;
  let restServiceSpy: jasmine.SpyObj<RestService>;

  beforeEach(() => {
    restServiceSpy = jasmine.createSpyObj('RestService', ['request']);

    TestBed.configureTestingModule({
      providers: [
        DestinationService,
        { provide: RestService, useValue: restServiceSpy },
      ],
    });

    service = TestBed.inject(DestinationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get destination list', (done) => {
    const mockDestinations: DestinationDto[] = [
      {
        id: '1',
        name: 'Cataratas',
        country: 'Argentina',
        city: 'Puerto Iguazu',
        creationTime: new Date().toISOString(),
        latitude: '-25.6',
        longitude: '-54.4',
        averageRating: 5,
        totalRatings: 10,
        isFavorite: false,
      },
    ];

    restServiceSpy.request.and.returnValue(
      of({ items: mockDestinations, totalCount: 1 })
    );

    service.getList({ skipCount: 0, maxResultCount: 10 }).subscribe((result) => {
      expect(result.items.length).toBe(1);
      expect(result.items[0].name).toBe('Cataratas');
      expect(restServiceSpy.request).toHaveBeenCalled();
      done();
    });
  });

  it('should get destination by id', (done) => {
    const mockDestination: DestinationDto = {
      id: '123',
      name: 'Bariloche',
      country: 'Argentina',
      city: 'Bariloche',
      creationTime: new Date().toISOString(),
      latitude: '-41.1',
      longitude: '-71.3',
    };

    restServiceSpy.request.and.returnValue(of(mockDestination));

    service.get('123').subscribe((result) => {
      expect(result.id).toBe('123');
      expect(result.name).toBe('Bariloche');
      done();
    });
  });
});
