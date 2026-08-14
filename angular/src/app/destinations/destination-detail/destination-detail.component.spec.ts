import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DestinationDetailComponent } from './destination-detail.component';
import { DestinationService } from '../../services/destination.service';
import { DestinationRatingService } from '../../services/rating.service';
import { ExperienceService } from '../../services/experience.service';
import { FavoriteService } from '../../services/favorite.service';
import { AuthService } from '@abp/ng.core';
import { of } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';

describe('DestinationDetailComponent', () => {
  let component: DestinationDetailComponent;
  let fixture: ComponentFixture<DestinationDetailComponent>;
  let destinationServiceSpy: jasmine.SpyObj<DestinationService>;
  let ratingServiceSpy: jasmine.SpyObj<DestinationRatingService>;
  let experienceServiceSpy: jasmine.SpyObj<ExperienceService>;
  let favoriteServiceSpy: jasmine.SpyObj<FavoriteService>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;

  beforeEach(async () => {
    destinationServiceSpy = jasmine.createSpyObj('DestinationService', ['get']);
    ratingServiceSpy = jasmine.createSpyObj('DestinationRatingService', ['getSummary', 'getRatings', 'rateDestination']);
    experienceServiceSpy = jasmine.createSpyObj('ExperienceService', ['getListByDestination', 'create']);
    favoriteServiceSpy = jasmine.createSpyObj('FavoriteService', ['toggleFavorite']);
    authServiceSpy = jasmine.createSpyObj('AuthService', [], { isAuthenticated: true });

    destinationServiceSpy.get.and.returnValue(
      of({
        id: 'dest-1',
        name: 'Bariloche',
        country: 'Argentina',
        city: 'Bariloche',
        creationTime: new Date().toISOString(),
        latitude: '-41.1',
        longitude: '-71.3',
        description: 'Patagonia',
      })
    );

    ratingServiceSpy.getSummary.and.returnValue(
      of({
        destinationId: 'dest-1',
        averageScore: 4.5,
        totalRatings: 10,
        ratings: [],
      })
    );

    ratingServiceSpy.getRatings.and.returnValue(of([]));
    experienceServiceSpy.getListByDestination.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [DestinationDetailComponent, RouterTestingModule],
      providers: [
        { provide: DestinationService, useValue: destinationServiceSpy },
        { provide: DestinationRatingService, useValue: ratingServiceSpy },
        { provide: ExperienceService, useValue: experienceServiceSpy },
        { provide: FavoriteService, useValue: favoriteServiceSpy },
        { provide: AuthService, useValue: authServiceSpy },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: {
                get: () => 'dest-1',
              },
            },
          },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(DestinationDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create and load destination detail', () => {
    expect(component).toBeTruthy();
    expect(component.destination?.name).toBe('Bariloche');
    expect(component.ratingSummary?.averageScore).toBe(4.5);
  });

  it('should submit rating and refresh summary', () => {
    ratingServiceSpy.rateDestination.and.returnValue(of(undefined));
    component.userScore = 5;
    component.userComment = 'Espectacular lugar';

    component.submitRating();

    expect(ratingServiceSpy.rateDestination).toHaveBeenCalledWith({
      destinationId: 'dest-1',
      score: 5,
      comment: 'Espectacular lugar',
    });
    expect(component.userComment).toBe('');
  });
});
