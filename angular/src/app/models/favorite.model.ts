import { DestinationDto } from './destination.model';

export interface FavoriteDestinationDto {
  id: string;
  userId: string;
  destinationId: string;
  creationTime: string;
  destination?: DestinationDto;
}
