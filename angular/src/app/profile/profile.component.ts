import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AuthService } from '@abp/ng.core';
import { UserProfileService } from '../services/user-profile.service';
import { PublicUserProfileDto, UpdateUserPreferencesDto } from '../models/user-profile.model';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss'],
})
export class ProfileComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly profileService = inject(UserProfileService);
  private readonly authService = inject(AuthService);

  profile?: PublicUserProfileDto;
  isOwnProfile = true;
  isLoading = true;
  isSaving = false;

  preferences: UpdateUserPreferencesDto = {
    bio: '',
    avatarUrl: '',
    preferredLanguage: 'es',
  };

  get isAuthenticated(): boolean {
    return this.authService.isAuthenticated;
  }

  ngOnInit(): void {
    const userId = this.route.snapshot.paramMap.get('id');
    if (userId) {
      this.isOwnProfile = false;
      this.loadPublicProfile(userId);
    } else {
      this.isOwnProfile = true;
      this.loadMyProfile();
    }
  }

  loadMyProfile(): void {
    if (!this.isAuthenticated) {
      this.router.navigate(['/']);
      return;
    }

    this.isLoading = true;
    this.profileService.getMyProfile().subscribe({
      next: (data) => {
        this.profile = data;
        this.preferences = {
          bio: data.bio || '',
          avatarUrl: data.avatarUrl || '',
          preferredLanguage: data.preferredLanguage || 'es',
        };
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  loadPublicProfile(userId: string): void {
    this.isLoading = true;
    this.profileService.getPublicProfile(userId).subscribe({
      next: (data) => {
        this.profile = data;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  savePreferences(): void {
    this.isSaving = true;
    this.profileService.updatePreferences(this.preferences).subscribe({
      next: () => {
        this.isSaving = false;
        alert('Preferencias guardadas exitosamente.');
        if (this.profile) {
          this.profile.bio = this.preferences.bio;
          this.profile.avatarUrl = this.preferences.avatarUrl;
          this.profile.preferredLanguage = this.preferences.preferredLanguage;
        }
      },
      error: () => {
        this.isSaving = false;
      },
    });
  }

  deleteAccount(): void {
    if (confirm('¿ATENCIÓN: Estás seguro de que deseas eliminar permanentemente tu cuenta y todos tus datos (favoritos, experiencias, calificaciones)? Esta acción no se puede deshacer.')) {
      this.profileService.deleteAccount().subscribe({
        next: () => {
          alert('Tu cuenta ha sido eliminada con éxito.');
          this.authService.logout().subscribe(() => {
            this.router.navigate(['/']);
          });
        },
      });
    }
  }
}
