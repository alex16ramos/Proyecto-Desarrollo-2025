export interface ExperienceDto {
  id: string;
  destinationId: string;
  destinationName?: string;
  userId: string;
  userName?: string;
  title: string;
  description: string;
  dateExperienced: string;
  rating: number;
  comments?: string;
  creationTime: string;
}

export interface CreateUpdateExperienceDto {
  destinationId: string;
  title: string;
  description: string;
  dateExperienced: string;
  rating: number;
  comments?: string;
}
