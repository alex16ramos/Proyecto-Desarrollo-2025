import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AuthService } from '@abp/ng.core';
import { UserProfileService } from '../services/user-profile.service';
import {
  PublicUserProfileDto,
  UpdateUserPreferencesDto,
  UpdateProfileBasicDto,
  ChangePasswordDto,
} from '../models/user-profile.model';

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
  isSavingBasic = false;
  isChangingPassword = false;

  // Feedback notifications
  successMessage = '';
  errorMessage = '';
  pwdSuccessMessage = '';
  pwdErrorMessage = '';

  preferences: UpdateUserPreferencesDto = {
    bio: '',
    avatarUrl: '',
    preferredLanguage: 'es',
  };

  basicInfo: UpdateProfileBasicDto = {
    userName: '',
    email: '',
    name: '',
    surname: '',
  };

  passwordForm: ChangePasswordDto = {
    currentPassword: '',
    newPassword: '',
  };
  confirmPassword = '';
  showPasswordModal = false;

  get isAuthenticated(): boolean {
    return this.authService.isAuthenticated;
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const userId = params.get('id');
      if (userId) {
        this.isOwnProfile = false;
        this.loadPublicProfile(userId);
      } else {
        this.isOwnProfile = true;
        this.loadMyProfile();
      }
    });
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
        this.basicInfo = {
          userName: data.userName || '',
          email: data.email || '',
          name: data.name || '',
          surname: data.surname || '',
        };
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

  saveBasicAndPreferences(): void {
    this.isSaving = true;
    this.successMessage = '';
    this.errorMessage = '';

    // 1. Guardar preferencias (Bio, Avatar, Idioma)
    this.profileService.updatePreferences(this.preferences).subscribe({
      next: () => {
        // 2. Guardar datos básicos (Nombre, Apellido, Email, UserName)
        this.profileService.updateBasicProfile(this.basicInfo).subscribe({
          next: () => {
            this.isSaving = false;
            this.successMessage = '¡Tu perfil ha sido actualizado con éxito!';
            if (this.profile) {
              this.profile.name = this.basicInfo.name;
              this.profile.surname = this.basicInfo.surname;
              this.profile.email = this.basicInfo.email;
              this.profile.bio = this.preferences.bio;
              this.profile.avatarUrl = this.preferences.avatarUrl;
              this.profile.preferredLanguage = this.preferences.preferredLanguage;
            }
            setTimeout(() => (this.successMessage = ''), 4000);
          },
          error: (err) => {
            this.isSaving = false;
            this.errorMessage = err?.error?.error?.message || 'Error al actualizar los datos personales.';
          },
        });
      },
      error: (err) => {
        this.isSaving = false;
        this.errorMessage = err?.error?.error?.message || 'Error al actualizar las preferencias.';
      },
    });
  }

  openPasswordModal(): void {
    this.passwordForm = { currentPassword: '', newPassword: '' };
    this.confirmPassword = '';
    this.pwdSuccessMessage = '';
    this.pwdErrorMessage = '';
    this.showPasswordModal = true;
  }

  closePasswordModal(): void {
    this.showPasswordModal = false;
  }

  submitChangePassword(): void {
    this.pwdSuccessMessage = '';
    this.pwdErrorMessage = '';

    if (!this.passwordForm.currentPassword) {
      this.pwdErrorMessage = 'Debes ingresar tu contraseña actual.';
      return;
    }

    if (!this.passwordForm.newPassword || this.passwordForm.newPassword.length < 6) {
      this.pwdErrorMessage = 'La nueva contraseña debe contener al menos 6 caracteres.';
      return;
    }

    if (this.passwordForm.newPassword !== this.confirmPassword) {
      this.pwdErrorMessage = 'Las contraseñas no coinciden.';
      return;
    }

    this.isChangingPassword = true;
    this.profileService.changePassword(this.passwordForm).subscribe({
      next: () => {
        this.isChangingPassword = false;
        this.pwdSuccessMessage = '¡Contraseña actualizada correctamente!';
        setTimeout(() => {
          this.closePasswordModal();
        }, 1500);
      },
      error: (err) => {
        this.isChangingPassword = false;
        this.pwdErrorMessage =
          err?.error?.error?.message || 'Error al cambiar la contraseña. Verifica tu clave actual.';
      },
    });
  }

  deleteAccount(): void {
    if (
      confirm(
        '¿ATENCIÓN: Estás completamente seguro de que deseas eliminar permanentemente tu cuenta y todos tus datos (favoritos, experiencias, calificaciones)? Esta acción es irreversible.'
      )
    ) {
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

