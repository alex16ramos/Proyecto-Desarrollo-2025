import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FavoriteService } from '../services/favorite.service';
import { DestinationDto } from '../models/destination.model';

@Component({
  selector: 'app-favorites',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './favorites.component.html',
  styleUrls: ['./favorites.component.scss'],
})
export class FavoritesComponent implements OnInit {
  private readonly favoriteService = inject(FavoriteService);

  favorites: DestinationDto[] = [];
  isLoading = true;

  ngOnInit(): void {
    this.loadFavorites();
  }

  loadFavorites(): void {
    this.isLoading = true;
    this.favoriteService.getUserFavorites().subscribe({
      next: (list) => {
        this.favorites = list || [];
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  removeFavorite(event: Event, dest: DestinationDto): void {
    event.stopPropagation();
    this.favoriteService.toggleFavorite(dest.id).subscribe({
      next: () => {
        this.favorites = this.favorites.filter((f) => f.id !== dest.id);
      },
    });
  }
}
