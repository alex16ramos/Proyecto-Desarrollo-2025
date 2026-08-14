export interface DestinationRatingDto {
  id: string;
  userId: string;
  userName?: string;
  destinationId: string;
  score: number;
  comment?: string;
  creationTime: string;
}

export interface RateDestinationDto {
  destinationId: string;
  score: number;
  comment?: string;
}

export interface DestinationRatingSummaryDto {
  destinationId: string;
  averageScore: number;
  totalRatings: number;
  ratings: DestinationRatingDto[];
}
