import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ExperienceService } from '../services/experience.service';
import { DestinationService } from '../services/destination.service';
import { ExperienceDto, CreateUpdateExperienceDto } from '../models/experience.model';
import { DestinationDto } from '../models/destination.model';

@Component({
  selector: 'app-experiences',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './experiences.component.html',
  styleUrls: ['./experiences.component.scss'],
})
export class ExperiencesComponent implements OnInit {
  private readonly experienceService = inject(ExperienceService);
  private readonly destinationService = inject(DestinationService);

  experiences: ExperienceDto[] = [];
  filteredExperiences: ExperienceDto[] = [];
  destinations: DestinationDto[] = [];
  isLoading = true;
  showModal = false;
  editingId: string | null = null;

  searchTerm = '';
  ratingFilter = 0;

  newExperience: CreateUpdateExperienceDto = {
    destinationId: '',
    title: '',
    description: '',
    dateExperienced: new Date().toISOString().substring(0, 10),
    rating: 5,
    comments: '',
  };

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;
    this.experienceService.getUserExperiences().subscribe({
      next: (list) => {
        this.experiences = list || [];
        this.filterExperiences();
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });

    this.destinationService.getList({ skipCount: 0, maxResultCount: 100 }).subscribe({
      next: (res) => {
        this.destinations = res.items || [];
      },
    });
  }

  filterExperiences(): void {
    let result = [...this.experiences];

    if (this.searchTerm.trim()) {
      const term = this.searchTerm.toLowerCase();
      result = result.filter(
        (e) =>
          e.title.toLowerCase().includes(term) ||
          e.description.toLowerCase().includes(term) ||
          (e.destinationName && e.destinationName.toLowerCase().includes(term))
      );
    }

    if (this.ratingFilter > 0) {
      result = result.filter((e) => e.rating === this.ratingFilter);
    }

    this.filteredExperiences = result;
  }

  openModal(): void {
    this.editingId = null;
    this.newExperience = {
      destinationId: this.destinations.length > 0 ? this.destinations[0].id : '',
      title: '',
      description: '',
      dateExperienced: new Date().toISOString().substring(0, 10),
      rating: 5,
      comments: '',
    };
    this.showModal = true;
  }

  openEditModal(exp: ExperienceDto): void {
    this.editingId = exp.id;
    this.newExperience = {
      destinationId: exp.destinationId,
      title: exp.title,
      description: exp.description,
      dateExperienced: exp.dateExperienced ? exp.dateExperienced.substring(0, 10) : new Date().toISOString().substring(0, 10),
      rating: exp.rating,
      comments: exp.comments || '',
    };
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.editingId = null;
  }

  saveExperience(): void {
    if (!this.newExperience.destinationId || !this.newExperience.title || !this.newExperience.description) {
      return;
    }

    if (this.editingId) {
      this.experienceService.update(this.editingId, this.newExperience).subscribe({
        next: () => {
          this.closeModal();
          this.loadData();
        },
      });
    } else {
      this.experienceService.create(this.newExperience).subscribe({
        next: () => {
          this.closeModal();
          this.loadData();
        },
      });
    }
  }

  deleteExperience(id: string): void {
    if (confirm('¿Estás seguro de que deseas eliminar esta experiencia?')) {
      this.experienceService.delete(id).subscribe({
        next: () => {
          this.experiences = this.experiences.filter((e) => e.id !== id);
          this.filterExperiences();
        },
      });
    }
  }
}
