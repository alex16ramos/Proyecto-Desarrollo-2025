export interface DestinationDto {
  id: string;
  name: string;
  country: string;
  city: string;
  photoUrl?: string;
  creationTime: string;
  latitude: string;
  longitude: string;
  description?: string;
  averageRating?: number;
  totalRatings?: number;
  isFavorite?: boolean;
}

export interface CreateUpdateDestinationDto {
  name: string;
  country: string;
  city: string;
  photoUrl?: string;
  latitude: string;
  longitude: string;
  description?: string;
}

export interface CitySearchResultDto {
  cityName: string;
  country: string;
  latitude: number;
  longitude: number;
}
