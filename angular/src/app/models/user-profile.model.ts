import { ExperienceDto } from './experience.model';

export interface PublicUserProfileDto {
  id: string;
  userName: string;
  name?: string;
  surname?: string;
  email?: string;
  bio?: string;
  avatarUrl?: string;
  preferredLanguage?: string;
  experiencesCount: number;
  favoritesCount: number;
  recentExperiences: ExperienceDto[];
}

export interface UpdateUserPreferencesDto {
  bio?: string;
  avatarUrl?: string;
  preferredLanguage?: string;
}
