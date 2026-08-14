import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { AuthService } from '@abp/ng.core';
import { DestinationService } from '../../services/destination.service';
import { DestinationRatingService } from '../../services/rating.service';
import { ExperienceService } from '../../services/experience.service';
import { FavoriteService } from '../../services/favorite.service';
import { DestinationDto } from '../../models/destination.model';
import { DestinationRatingDto, DestinationRatingSummaryDto } from '../../models/rating.model';
import { ExperienceDto, CreateUpdateExperienceDto } from '../../models/experience.model';

@Component({
  selector: 'app-destination-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './destination-detail.component.html',
  styleUrls: ['./destination-detail.component.scss'],
})
export class DestinationDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly destinationService = inject(DestinationService);
  private readonly ratingService = inject(DestinationRatingService);
  private readonly experienceService = inject(ExperienceService);
  private readonly favoriteService = inject(FavoriteService);
  private readonly authService = inject(AuthService);

  destinationId = '';
  destination?: DestinationDto;
  ratingSummary?: DestinationRatingSummaryDto;
  ratings: DestinationRatingDto[] = [];
  experiences: ExperienceDto[] = [];
  myRating: DestinationRatingDto | null = null;

  isLoading = true;
  userScore = 5;
  userComment = '';
  isSubmittingRating = false;
  isEditingRating = false;

  showExperienceModal = false;
  newExperience: CreateUpdateExperienceDto = {
    destinationId: '',
    title: '',
    description: '',
    dateExperienced: new Date().toISOString().substring(0, 10),
    rating: 5,
    comments: '',
  };

  get isAuthenticated(): boolean {
    return this.authService.isAuthenticated;
  }

  ngOnInit(): void {
    this.destinationId = this.route.snapshot.paramMap.get('id') || '';
    if (this.destinationId) {
      this.loadAllData();
    }
  }

  loadAllData(): void {
    this.isLoading = true;
    this.destinationService.get(this.destinationId).subscribe({
      next: (dest) => {
        this.destination = dest;
        this.loadRatings();
        this.loadExperiences();
        this.loadMyRating();
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  loadRatings(): void {
    this.ratingService.getSummary(this.destinationId).subscribe({
      next: (summary) => {
        this.ratingSummary = summary;
      },
    });

    this.ratingService.getRatings(this.destinationId).subscribe({
      next: (list) => {
        this.ratings = list || [];
      },
    });
  }

  loadMyRating(): void {
    if (!this.isAuthenticated) return;
    this.ratingService.getMyRating(this.destinationId).subscribe({
      next: (rating) => {
        this.myRating = rating;
        if (rating) {
          this.userScore = rating.score;
          this.userComment = rating.comment || '';
        }
      },
    });
  }

  loadExperiences(): void {
    this.experienceService.getListByDestination(this.destinationId).subscribe({
      next: (list) => {
        this.experiences = list || [];
      },
    });
  }

  toggleFavorite(): void {
    if (!this.destination) return;
    if (!this.isAuthenticated) {
      alert('Debe iniciar sesión para guardar favoritos.');
      return;
    }

    this.favoriteService.toggleFavorite(this.destination.id).subscribe({
      next: (isFav) => {
        if (this.destination) {
          this.destination.isFavorite = isFav;
        }
      },
    });
  }

  startEditRating(): void {
    if (this.myRating) {
      this.userScore = this.myRating.score;
      this.userComment = this.myRating.comment || '';
      this.isEditingRating = true;
    }
  }

  cancelEditRating(): void {
    this.isEditingRating = false;
  }

  submitRating(): void {
    if (!this.isAuthenticated) {
      alert('Debe iniciar sesión para calificar.');
      return;
    }

    this.isSubmittingRating = true;

    if (this.myRating && this.isEditingRating) {
      this.ratingService
        .updateMyRating(this.myRating.id, this.userScore, this.userComment)
        .subscribe({
          next: () => {
            this.isSubmittingRating = false;
            this.isEditingRating = false;
            this.loadRatings();
            this.loadMyRating();
          },
          error: () => {
            this.isSubmittingRating = false;
          },
        });
    } else {
      this.ratingService
        .rateDestination({
          destinationId: this.destinationId,
          score: this.userScore,
          comment: this.userComment,
        })
        .subscribe({
          next: () => {
            this.userComment = '';
            this.isSubmittingRating = false;
            this.loadRatings();
            this.loadMyRating();
          },
          error: () => {
            this.isSubmittingRating = false;
          },
        });
    }
  }

  deleteRating(): void {
    if (!this.myRating) return;
    if (!confirm('¿Seguro que deseas eliminar tu calificación?')) return;

    this.ratingService.deleteMyRating(this.myRating.id).subscribe({
      next: () => {
        this.myRating = null;
        this.userScore = 5;
        this.userComment = '';
        this.isEditingRating = false;
        this.loadRatings();
      },
    });
  }

  openExperienceModal(): void {
    if (!this.isAuthenticated) {
      alert('Debe iniciar sesión para compartir experiencias.');
      return;
    }

    this.newExperience = {
      destinationId: this.destinationId,
      title: '',
      description: '',
      dateExperienced: new Date().toISOString().substring(0, 10),
      rating: 5,
      comments: '',
    };
    this.showExperienceModal = true;
  }

  closeExperienceModal(): void {
    this.showExperienceModal = false;
  }

  saveExperience(): void {
    if (!this.newExperience.title || !this.newExperience.description) return;

    this.experienceService.create(this.newExperience).subscribe({
      next: () => {
        this.closeExperienceModal();
        this.loadExperiences();
      },
    });
  }
}
