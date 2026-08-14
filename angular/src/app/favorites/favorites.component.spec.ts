import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FavoritesComponent } from './favorites.component';
import { FavoriteService } from '../services/favorite.service';
import { of } from 'rxjs';
import { RouterTestingModule } from '@angular/router/testing';

describe('FavoritesComponent', () => {
  let component: FavoritesComponent;
  let fixture: ComponentFixture<FavoritesComponent>;
  let favoriteServiceSpy: jasmine.SpyObj<FavoriteService>;

  beforeEach(async () => {
    favoriteServiceSpy = jasmine.createSpyObj('FavoriteService', [
      'getUserFavorites',
      'toggleFavorite',
    ]);

    favoriteServiceSpy.getUserFavorites.and.returnValue(
      of([
        {
          id: '1',
          name: 'Machu Picchu',
          country: 'Perú',
          city: 'Cusco',
          creationTime: new Date().toISOString(),
          latitude: '-13.1',
          longitude: '-72.5',
          isFavorite: true,
        },
      ])
    );

    await TestBed.configureTestingModule({
      imports: [FavoritesComponent, RouterTestingModule],
      providers: [{ provide: FavoriteService, useValue: favoriteServiceSpy }],
    }).compileComponents();

    fixture = TestBed.createComponent(FavoritesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create and load favorites', () => {
    expect(component).toBeTruthy();
    expect(component.favorites.length).toBe(1);
    expect(component.favorites[0].name).toBe('Machu Picchu');
  });

  it('should remove favorite and update list', () => {
    favoriteServiceSpy.toggleFavorite.and.returnValue(of(false));

    const mockEvent = new MouseEvent('click');
    spyOn(mockEvent, 'stopPropagation');

    component.removeFavorite(mockEvent, component.favorites[0]);
    expect(component.favorites.length).toBe(0);
  });
});
