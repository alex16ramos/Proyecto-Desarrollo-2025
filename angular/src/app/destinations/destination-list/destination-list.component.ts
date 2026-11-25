import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { AuthService } from '@abp/ng.core';
import { DestinationService } from '../../services/destination.service';
import { FavoriteService } from '../../services/favorite.service';
import { DestinationDto, CreateUpdateDestinationDto, CityDto } from '../../models/destination.model';

@Component({
  selector: 'app-destination-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './destination-list.component.html',
  styleUrls: ['./destination-list.component.scss'],
})
export class DestinationListComponent implements OnInit, OnDestroy {
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly destinationService = inject(DestinationService);
  private readonly favoriteService = inject(FavoriteService);
  private readonly authService = inject(AuthService);

  destinations: DestinationDto[] = [];
  filteredDestinations: DestinationDto[] = [];
  popularDestinations: DestinationDto[] = [];
  citySuggestions: CityDto[] = [];
  allCountries: { name: string; code: string }[] = [
    { name: 'Alemania', code: 'DE' },
    { name: 'Andorra', code: 'AD' },
    { name: 'Arabia Saudita', code: 'SA' },
    { name: 'Argentina', code: 'AR' },
    { name: 'Australia', code: 'AU' },
    { name: 'Austria', code: 'AT' },
    { name: 'Bélgica', code: 'BE' },
    { name: 'Bolivia', code: 'BO' },
    { name: 'Brasil', code: 'BR' },
    { name: 'Canadá', code: 'CA' },
    { name: 'Chile', code: 'CL' },
    { name: 'China', code: 'CN' },
    { name: 'Colombia', code: 'CO' },
    { name: 'Corea del Sur', code: 'KR' },
    { name: 'Costa Rica', code: 'CR' },
    { name: 'Croacia', code: 'HR' },
    { name: 'Cuba', code: 'CU' },
    { name: 'Dinamarca', code: 'DK' },
    { name: 'Ecuador', code: 'EC' },
    { name: 'Egipto', code: 'EG' },
    { name: 'Emiratos Árabes Unidos', code: 'AE' },
    { name: 'España', code: 'ES' },
    { name: 'Estados Unidos', code: 'US' },
    { name: 'Filipinas', code: 'PH' },
    { name: 'Finlandia', code: 'FI' },
    { name: 'Francia', code: 'FR' },
    { name: 'Grecia', code: 'GR' },
    { name: 'Guatemala', code: 'GT' },
    { name: 'Honduras', code: 'HN' },
    { name: 'Hungría', code: 'HU' },
    { name: 'India', code: 'IN' },
    { name: 'Indonesia', code: 'ID' },
    { name: 'Irlanda', code: 'IE' },
    { name: 'Islandia', code: 'IS' },
    { name: 'Israel', code: 'IL' },
    { name: 'Italia', code: 'IT' },
    { name: 'Japón', code: 'JP' },
    { name: 'Marruecos', code: 'MA' },
    { name: 'México', code: 'MX' },
    { name: 'Noruega', code: 'NO' },
    { name: 'Nueva Zelanda', code: 'NZ' },
    { name: 'Países Bajos', code: 'NL' },
    { name: 'Panamá', code: 'PA' },
    { name: 'Paraguay', code: 'PY' },
    { name: 'Perú', code: 'PE' },
    { name: 'Polonia', code: 'PL' },
    { name: 'Portugal', code: 'PT' },
    { name: 'Reino Unido', code: 'GB' },
    { name: 'República Checa', code: 'CZ' },
    { name: 'República Dominicana', code: 'DO' },
    { name: 'Rusia', code: 'RU' },
    { name: 'Singapur', code: 'SG' },
    { name: 'Sudáfrica', code: 'ZA' },
    { name: 'Suecia', code: 'SE' },
    { name: 'Suiza', code: 'CH' },
    { name: 'Tailandia', code: 'TH' },
    { name: 'Turquía', code: 'TR' },
    { name: 'Uruguay', code: 'UY' },
    { name: 'Venezuela', code: 'VE' },
    { name: 'Vietnam', code: 'VN' },
  ];

  countries: string[] = [];
  searchTerm = '';
  selectedCountry = '';
  selectedCountryCode = '';
  selectedRegion = '';
  minPopulation: number = 0;
  sortBy = 'rating';
  isLoading = false;
  isSearchingCities = false;
  showCreateModal = false;
  showAdvancedFilters = false;

  private readonly searchSubject = new Subject<string>();
  private searchSubscription?: Subscription;

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
    const searchParam = this.route.snapshot.queryParams['search'];
    if (searchParam) {
      this.searchTerm = searchParam;
    }
    this.setupDebouncedSearch();
    this.loadDestinations();
    this.loadPopularDestinations();
  }

  ngOnDestroy(): void {
    this.searchSubscription?.unsubscribe();
  }

  setupDebouncedSearch(): void {
    // Debounce reactivo de 300ms para búsqueda ágil en vivo
    this.searchSubscription = this.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((term) => {
        this.filterDestinations();
        if (term.trim().length >= 2) {
          this.searchExternalCities(term);
        } else {
          this.citySuggestions = [];
        }
      });
  }

  onSearchInput(value: string): void {
    this.searchTerm = value;
    this.searchSubject.next(value);
  }

  loadDestinations(): void {
    this.isLoading = true;
    this.destinationService.getList({ skipCount: 0, maxResultCount: 100 }).subscribe({
      next: (result) => {
        this.destinations = result.items || [];
        
        // Unir los países predefinidos con los países que ya existen en la base de datos de destinos
        const existingCountries = this.destinations.map((d) => d.country).filter(Boolean);
        const presetNames = this.allCountries.map((c) => c.name);
        const combined = Array.from(new Set([...presetNames, ...existingCountries]));
        
        this.countries = combined.sort((a, b) => a.localeCompare(b, 'es', { sensitivity: 'base' }));
        this.filterDestinations();
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  onCountryChange(): void {
    const found = this.allCountries.find(
      (c) => c.name.toLowerCase() === this.selectedCountry.toLowerCase()
    );
    this.selectedCountryCode = found ? found.code : '';
    this.filterDestinations();
    if (this.searchTerm.trim().length >= 2) {
      this.searchExternalCities(this.searchTerm);
    }
  }

  onFilterChange(): void {
    this.filterDestinations();
    if (this.searchTerm.trim().length >= 2) {
      this.searchExternalCities(this.searchTerm);
    }
  }

  searchExternalCities(term: string): void {
    this.isSearchingCities = true;
    const filters = {
      countryCode: this.selectedCountryCode ? this.selectedCountryCode : undefined,
      region: this.selectedRegion ? this.selectedRegion : undefined,
      minPopulation: this.minPopulation > 0 ? this.minPopulation : undefined,
    };

    this.destinationService.searchCities(term, filters).subscribe({
      next: (result) => {
        this.citySuggestions = result?.cities || [];
        this.isSearchingCities = false;
      },
      error: () => {
        this.citySuggestions = [];
        this.isSearchingCities = false;
      },
    });
  }

  toggleAdvancedFilters(): void {
    this.showAdvancedFilters = !this.showAdvancedFilters;
  }

  resetFilters(): void {
    this.searchTerm = '';
    this.selectedCountry = '';
    this.selectedCountryCode = '';
    this.selectedRegion = '';
    this.minPopulation = 0;
    this.sortBy = 'rating';
    this.citySuggestions = [];
    this.filterDestinations();
  }

  selectCitySuggestion(city: CityDto): void {
    // 1. Si el destino ya existe en la base de datos, navegar directamente a su detalle
    const existing = this.destinations.find(
      (d) =>
        d.name.toLowerCase() === city.name.toLowerCase() ||
        (d.city.toLowerCase() === city.name.toLowerCase() && d.country.toLowerCase() === city.country.toLowerCase())
    );

    if (existing) {
      this.router.navigate(['/destinations', existing.id]);
      return;
    }

    // 2. Si es una ciudad nueva de GeoDB, crearla automáticamente con su información geográfica
    const autoDesc = city.region
      ? `Destino turístico en ${city.region}, ${city.country}. Explora sus atracciones locales, historia y cultura.`
      : `Destino turístico en ${city.country}. Explora sus atracciones locales, historia y cultura.`;

    const newDest: CreateUpdateDestinationDto = {
      name: city.name,
      city: city.name,
      country: city.country,
      latitude: city.latitude || '',
      longitude: city.longitude || '',
      photoUrl: `https://images.unsplash.com/photo-1488646953014-85cb44e25828?auto=format&fit=crop&w=800&q=80`,
      description: autoDesc,
    };

    this.isLoading = true;
    this.destinationService.create(newDest).subscribe({
      next: (created) => {
        this.isLoading = false;
        this.router.navigate(['/destinations', created.id]);
      },
      error: () => {
        this.isLoading = false;
        this.searchTerm = city.name;
        this.filterDestinations();
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

  getImageUrl(dest: DestinationDto): string {
    if (dest.photoUrl && dest.photoUrl.trim() !== '') {
      return dest.photoUrl;
    }
    const keyword = encodeURIComponent(dest.city || dest.name || 'travel');
    return `https://loremflickr.com/600/400/${keyword},landscape,travel/all`;
  }

  openCreateModal(): void {
    if (!this.newDestination.photoUrl) {
      this.newDestination = {
        name: '',
        country: '',
        city: '',
        photoUrl: '',
        latitude: '',
        longitude: '',
        description: '',
      };
    }
    this.showCreateModal = true;
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
  }

  saveDestination(): void {
    if (!this.newDestination.name || !this.newDestination.country || !this.newDestination.city) {
      return;
    }

    if (!this.newDestination.photoUrl) {
      this.newDestination.photoUrl = `https://loremflickr.com/800/600/${encodeURIComponent(this.newDestination.city)},city,travel/all`;
    }

    this.destinationService.create(this.newDestination).subscribe({
      next: () => {
        this.closeCreateModal();
        this.loadDestinations();
      },
    });
  }
}
