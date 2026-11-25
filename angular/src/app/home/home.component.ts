import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '@abp/ng.core';
import { DestinationService } from '../services/destination.service';
import { FavoriteService } from '../services/favorite.service';
import { ExperienceService } from '../services/experience.service';
import { DestinationDto } from '../models/destination.model';
import { ExperienceDto } from '../models/experience.model';

@Component({
  selector: 'app-home',
  standalone: true,
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  imports: [CommonModule, FormsModule, RouterModule],
})
export class HomeComponent implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly destinationService = inject(DestinationService);
  private readonly favoriteService = inject(FavoriteService);
  private readonly experienceService = inject(ExperienceService);

  quickSearchTerm = '';
  popularDestinations: DestinationDto[] = [];
  recentExperiences: ExperienceDto[] = [];
  favoriteDestinations: DestinationDto[] = [];

  totalDestinationsCount = 0;
  totalFavoritesCount = 0;
  totalExperiencesCount = 0;
  isLoading = true;

  get hasLoggedIn(): boolean {
    return this.authService.isAuthenticated;
  }

  get userDisplayName(): string {
    // Tomar el nombre del usuario autenticado si existe
    return 'Viajero';
  }

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.isLoading = true;

    // 1. Cargar destinos populares y total de destinos
    this.destinationService.getList({ skipCount: 0, maxResultCount: 100 }).subscribe({
      next: (res) => {
        this.totalDestinationsCount = res.totalCount || res.items?.length || 0;
      },
      error: () => {},
    });

    this.destinationService.getPopularDestinations(4).subscribe({
      next: (list) => {
        this.popularDestinations = list || [];
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });

    // 2. Cargar favoritos si el usuario está autenticado
    if (this.hasLoggedIn) {
      this.favoriteService.getUserFavorites().subscribe({
        next: (favs) => {
          this.favoriteDestinations = favs || [];
          this.totalFavoritesCount = this.favoriteDestinations.length;
        },
        error: () => {},
      });

      this.experienceService.getUserExperiences().subscribe({
        next: (exps) => {
          this.recentExperiences = (exps || []).slice(0, 3);
          this.totalExperiencesCount = exps?.length || 0;
        },
        error: () => {},
      });
    }
  }

  onQuickSearch(): void {
    if (this.quickSearchTerm.trim()) {
      this.router.navigate(['/destinations'], {
        queryParams: { search: this.quickSearchTerm.trim() },
      });
    } else {
      this.router.navigate(['/destinations']);
    }
  }

  getImageUrl(dest?: DestinationDto): string {
    if (dest?.photoUrl && dest.photoUrl.trim() !== '') {
      return dest.photoUrl;
    }
    const keyword = encodeURIComponent(dest?.city || dest?.name || 'travel');
    return `https://loremflickr.com/800/500/${keyword},landscape,travel/all`;
  }

  login(): void {
    this.authService.navigateToLogin();
  }
}
