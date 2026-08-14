import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AuthService } from '@abp/ng.core';
import { DestinationService } from '../../services/destination.service';
import { FavoriteService } from '../../services/favorite.service';
import { DestinationDto, CreateUpdateDestinationDto } from '../../models/destination.model';

@Component({
  selector: 'app-destination-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './destination-list.component.html',
  styleUrls: ['./destination-list.component.scss'],
})
export class DestinationListComponent implements OnInit {
  private readonly destinationService = inject(DestinationService);
  private readonly favoriteService = inject(FavoriteService);
  private readonly authService = inject(AuthService);

  destinations: DestinationDto[] = [];
  filteredDestinations: DestinationDto[] = [];
  popularDestinations: DestinationDto[] = [];
  countries: string[] = [];
  searchTerm = '';
  selectedCountry = '';
  sortBy = 'rating';
  isLoading = false;
  showCreateModal = false;

  newDestination: CreateUpdateDestinationDto = {
    name: '',
    country: '',
    city: '',
    photoUrl: '',
    latitude: '',
    longitude: '',
    description: '',
  };

  get isAuthenticated(): boolean {
    return this.authService.isAuthenticated;
  }

  ngOnInit(): void {
    this.loadDestinations();
    this.loadPopularDestinations();
  }

  loadDestinations(): void {
    this.isLoading = true;
    this.destinationService.getList({ skipCount: 0, maxResultCount: 100 }).subscribe({
      next: (result) => {
        this.destinations = result.items || [];
        this.countries = Array.from(new Set(this.destinations.map((d) => d.country).filter(Boolean))).sort();
        this.filterDestinations();
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  loadPopularDestinations(): void {
    this.destinationService.getPopularDestinations(4).subscribe({
      next: (list) => {
        this.popularDestinations = list || [];
      },
    });
  }

  filterDestinations(): void {
    let result = [...this.destinations];

    if (this.searchTerm.trim()) {
      const term = this.searchTerm.toLowerCase();
      result = result.filter(
        (d) =>
          d.name.toLowerCase().includes(term) ||
          d.city.toLowerCase().includes(term) ||
          d.country.toLowerCase().includes(term) ||
          (d.description && d.description.toLowerCase().includes(term))
      );
    }

    if (this.selectedCountry) {
      result = result.filter((d) => d.country === this.selectedCountry);
    }

    if (this.sortBy === 'rating') {
      result.sort((a, b) => (b.averageRating || 0) - (a.averageRating || 0));
    } else if (this.sortBy === 'name') {
      result.sort((a, b) => a.name.localeCompare(b.name));
    } else if (this.sortBy === 'country') {
      result.sort((a, b) => a.country.localeCompare(b.country));
    }

    this.filteredDestinations = result;
  }

  toggleFavorite(event: Event, destination: DestinationDto): void {
    event.stopPropagation();
    if (!this.isAuthenticated) {
      alert('Debe iniciar sesión para guardar favoritos.');
      return;
    }

    this.favoriteService.toggleFavorite(destination.id).subscribe({
      next: (isFav) => {
        destination.isFavorite = isFav;
      },
    });
  }

  openCreateModal(): void {
    this.newDestination = {
      name: '',
      country: '',
      city: '',
      photoUrl: '',
      latitude: '',
      longitude: '',
      description: '',
    };
    this.showCreateModal = true;
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
  }

  saveDestination(): void {
    if (!this.newDestination.name || !this.newDestination.country || !this.newDestination.city) {
      return;
    }

    this.destinationService.create(this.newDestination).subscribe({
      next: () => {
        this.closeCreateModal();
        this.loadDestinations();
      },
    });
  }
}
