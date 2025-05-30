import { Component, Input } from '@angular/core';
import { BaseComponent } from '../base.component';
import { ListGame } from './list-game';

@Component({
  selector: 'gamestore-list-game',
  templateUrl: './list-game.component.html',
  styleUrls: ['./list-game.component.scss']
})
export class ListGameComponent extends BaseComponent {
  @Input()
  gameItem!: ListGame;
}
