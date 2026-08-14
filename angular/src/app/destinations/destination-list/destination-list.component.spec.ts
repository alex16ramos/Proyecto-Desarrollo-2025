import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DestinationListComponent } from './destination-list.component';
import { DestinationService } from '../../services/destination.service';
import { FavoriteService } from '../../services/favorite.service';
import { AuthService } from '@abp/ng.core';
import { of } from 'rxjs';
import { RouterTestingModule } from '@angular/router/testing';

describe('DestinationListComponent', () => {
  let component: DestinationListComponent;
  let fixture: ComponentFixture<DestinationListComponent>;
  let destinationServiceSpy: jasmine.SpyObj<DestinationService>;
  let favoriteServiceSpy: jasmine.SpyObj<FavoriteService>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;

  beforeEach(async () => {
    destinationServiceSpy = jasmine.createSpyObj('DestinationService', ['getList', 'create']);
    favoriteServiceSpy = jasmine.createSpyObj('FavoriteService', ['toggleFavorite']);
    authServiceSpy = jasmine.createSpyObj('AuthService', [], { isAuthenticated: true });

    destinationServiceSpy.getList.and.returnValue(
      of({
        items: [
          {
            id: '1',
            name: 'Iguazú',
            country: 'Argentina',
            city: 'Puerto Iguazú',
            creationTime: new Date().toISOString(),
            latitude: '-25.6',
            longitude: '-54.4',
            averageRating: 5,
            totalRatings: 8,
            isFavorite: false,
          },
        ],
        totalCount: 1,
      })
    );

    await TestBed.configureTestingModule({
      imports: [DestinationListComponent, RouterTestingModule],
      providers: [
        { provide: DestinationService, useValue: destinationServiceSpy },
        { provide: FavoriteService, useValue: favoriteServiceSpy },
        { provide: AuthService, useValue: authServiceSpy },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(DestinationListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create and load destinations', () => {
    expect(component).toBeTruthy();
    expect(component.destinations.length).toBe(1);
    expect(component.filteredDestinations.length).toBe(1);
    expect(component.destinations[0].name).toBe('Iguazú');
  });

  it('should filter destinations by searchTerm', () => {
    component.searchTerm = 'Iguazú';
    component.filterDestinations();
    expect(component.filteredDestinations.length).toBe(1);

    component.searchTerm = 'NonExistentPlace';
    component.filterDestinations();
    expect(component.filteredDestinations.length).toBe(0);
  });

  it('should toggle favorite status', () => {
    favoriteServiceSpy.toggleFavorite.and.returnValue(of(true));
    const dest = component.destinations[0];

    const mockEvent = new MouseEvent('click');
    spyOn(mockEvent, 'stopPropagation');

    component.toggleFavorite(mockEvent, dest);
    expect(dest.isFavorite).toBeTrue();
  });
});
