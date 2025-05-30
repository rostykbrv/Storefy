import { HttpParams } from '@angular/common/http';
import { Component } from '@angular/core';
import { BehaviorSubject, Subject } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { BaseComponent } from 'src/app/componetns/base.component';
import { ListGame } from 'src/app/componetns/list-game-component/list-game';
import { ListItem } from 'src/app/componetns/list-item-component/list-item';
import { Platform } from 'src/app/models/platform.model';
import { User } from 'src/app/models/user.model';
import { GameService } from 'src/app/services/game.service';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'gamestore-games',
  templateUrl: './games-page.component.html',
  styleUrls: ['./games-page.component.scss'],
})
export class GamesPageComponent extends BaseComponent {
  gamesList: ListGame[] = [];
  pagesSubject = new BehaviorSubject<{ totalPages: number; page: number }>({
    totalPages: 1,
    page: 1,
  });

  gamesLoadSubject = new Subject<void>();

  currentUserRole?: string;
  addGameLink?: string;

  constructor(private gameService: GameService, private userService: UserService) {
    super();
  }

  ngOnInit(){
    this.currentUserRole = localStorage.getItem('userRole') || 'Guest';
    this.addGameLink = (this.currentUserRole === 'Administrator' || this.currentUserRole === 'Manager') 
    ? this.links.get(this.pageRoutes.AddGame) : undefined;
  };

  loadGames(filterParams: HttpParams): void {
    this.gameService
      .getGames(filterParams)
      .pipe(
        tap((x) => {
          this.pagesSubject.next({
            totalPages: !!x.totalPages ? x.totalPages : 1,
            page: !!x.currentPage ? x.currentPage : 1,
          });
        }),
        map((gamesInfo) =>
          gamesInfo.games.map((game) => {
            const gameItem: ListGame = { 
              imageUrl: game.imageUrl,
              title: game.name,
              gameType: game.copyType,
              price: game.price,
              pageLink: `${this.links.get(this.pageRoutes.Game)}/${game.key}`,
              updateLink: `${this.links.get(this.pageRoutes.UpdateGame)}/${game.key
                }`,
              deleteLink: `${this.links.get(this.pageRoutes.DeleteGame)}/${game.key
                }`,
            };

            return gameItem;
          })
        )
      )
      .subscribe((x) => this.onGamesLoaded(x), () => this.onGamesLoaded([]));
  }
  private onGamesLoaded(games: ListGame[]): void {
    this.gamesList = games ?? [];
    this.gamesLoadSubject.next();
  }
}
