import { BaseModel } from './base.model';

export class Game extends BaseModel {
  key!: string;
  name!: string;
  description?: string;
  imageUrl?: string;
  copyType?: string;
  releasedDate?: string;
  gameSize?: string;

  price?: number;
  unitInStock?: number;
  discontinued?: number;
}
