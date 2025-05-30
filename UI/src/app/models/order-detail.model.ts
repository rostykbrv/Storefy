import { BaseModel } from './base.model';

export class OrderDetail extends BaseModel {
  productId!: string;

  price?: number;
  quantity?: number;
  discount?: number;
}
