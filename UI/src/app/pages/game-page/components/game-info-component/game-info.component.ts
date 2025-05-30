import { Component, Input, Output } from '@angular/core';
import { EventEmitter } from '@angular/core';
import { BaseComponent } from 'src/app/componetns/base.component';
import { GameInfoItem } from './game-info-item';

@Component({
  selector: 'game-info-component',
  templateUrl: './game-info.component.html',
  styleUrls: ['./game-info.component.scss']
})
export class GameInfoComponent extends BaseComponent {
  @Input()
  gameDetails?: GameInfoItem;

  @Input() 
  canBuy: boolean = false;
  
  @Output() 
  buyEvent = new EventEmitter<void>();

  isEnlarged: boolean = false;
}
