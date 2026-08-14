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

export interface CityDto {
  name: string;
  country: string;
  countryCode: string;
  region?: string;
  latitude?: string;
  longitude?: string;
}

export interface CitySearchResultDto {
  cities: CityDto[];
}

export interface DestinationEventDto {
  id: string;
  name: string;
  description?: string;
  eventDate?: string;
  venueName?: string;
  city?: string;
  country?: string;
  url?: string;
  imageUrl?: string;
  category?: string;
}
