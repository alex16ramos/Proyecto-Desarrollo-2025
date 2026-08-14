import { TestBed } from '@angular/core/testing';
import { RestService } from '@abp/ng.core';
import { of } from 'rxjs';
import { FavoriteService } from './favorite.service';
import { DestinationDto } from '../models/destination.model';

describe('FavoriteService', () => {
  let service: FavoriteService;
  let restServiceSpy: jasmine.SpyObj<RestService>;

  beforeEach(() => {
    restServiceSpy = jasmine.createSpyObj('RestService', ['request']);

    TestBed.configureTestingModule({
      providers: [
        FavoriteService,
        { provide: RestService, useValue: restServiceSpy },
      ],
    });

    service = TestBed.inject(FavoriteService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should toggle favorite status', (done) => {
    restServiceSpy.request.and.returnValue(of(true));

    service.toggleFavorite('dest-1').subscribe((result) => {
      expect(result).toBeTrue();
      done();
    });
  });

  it('should get user favorites', (done) => {
    const mockFavorites: DestinationDto[] = [
      {
        id: '1',
        name: 'Machu Picchu',
        country: 'Peru',
        city: 'Cusco',
        creationTime: new Date().toISOString(),
        latitude: '-13.1',
        longitude: '-72.5',
      },
    ];

    restServiceSpy.request.and.returnValue(of(mockFavorites));

    service.getUserFavorites().subscribe((favs) => {
      expect(favs.length).toBe(1);
      expect(favs[0].name).toBe('Machu Picchu');
      done();
    });
  });
});
