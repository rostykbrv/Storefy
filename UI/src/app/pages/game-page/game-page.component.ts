import {
  AfterViewInit,
  Component,
  ElementRef,
  OnInit,
  ViewChild,
} from '@angular/core';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { switchMap, tap } from 'rxjs/operators';
import { BaseComponent } from 'src/app/componetns/base.component';
import { InfoItem } from 'src/app/componetns/info-component/info-item';
import { Game } from 'src/app/models/game.model';
import { Genre } from 'src/app/models/genre.model';
import { Platform } from 'src/app/models/platform.model';
import { Publisher } from 'src/app/models/publisher.model';
import { GameService } from 'src/app/services/game.service';
import { GenreService } from 'src/app/services/genre.service';
import { OrderService } from 'src/app/services/order.service';
import { PlatformService } from 'src/app/services/platform.service';
import { PublisherService } from 'src/app/services/publisher.service';
import { UserService } from 'src/app/services/user.service';
import { GameInfoItem } from './components/game-info-component/game-info-item';
import { CommentService } from 'src/app/services/comment.service';

@Component({
  selector: 'gamestore-game',
  templateUrl: './game-page.component.html',
  styleUrls: ['./game-page.component.scss'],
})
export class GamePageComponent
  extends BaseComponent
  implements OnInit, AfterViewInit
{
  private file?: Blob;
  private image?: Blob;

  @ViewChild('download')
  downloadLink!: ElementRef;

  gameValue?: Game;
  commentCount: number = 0;

  canSeeComments = false;
  canBuy = false;
  canUpdate = false;
  canDelete = false;

  imageUrl?: SafeUrl;

  gameInfo: GameInfoItem = {};
  gameInfoList: GameInfoItem[] = [];
  
  
  constructor(
    private gameService: GameService,
    private commentService: CommentService,
    private genreService: GenreService,
    private platformService: PlatformService,
    private publisherService: PublisherService,
    private orderService: OrderService,
    private route: ActivatedRoute,
    private userService: UserService,
    private router: Router,
    private sanitizer: DomSanitizer
  ) {
    super();
  }

  get deleteGameLink(): string | undefined  {
    if (this.canDelete){
      return `${this.links.get(this.pageRoutes.DeleteGame)}/${
        this.gameValue?.key
      }`;
    }

    return;
  }

  get updateGameLink(): string | undefined{
    if (this.canUpdate){
      return `${this.links.get(this.pageRoutes.UpdateGame)}/${
        this.gameValue?.key
      }`;
    }

    return;
  }

  get game(): Game | undefined {
    return this.gameValue;
  }

  set game(value: Game | undefined) {
    this.gameValue = value;
    this.gameInfoList = [];
    if (!value) {
      return;
    }

    this.gameInfoList.push(
      {
        title: value.name,
        price: value.price?.toString(),
      },
      {
        name: this.labels.gameNameLabel,
        value: value.name,
      },
      {
        name: this.labels.gameKeyLabel,
        value: value.key,
      },
      {
        name: this.labels.gameDescriptionLabel,
        value: value.description ?? '',
      },
      {
        name: this.labels.gamePriceLabel,
        value: (value.price ?? 0).toString(),
      },
      {
        name: this.labels.gameDiscontinuedLabel,
        value: (value.discontinued ?? 0).toString(),
      },
      {
        name: this.labels.gameUnitInStockLabel,
        value: (value.unitInStock ?? 0).toString(),
      }
    );
  }

  ngOnInit(): void {
    this.getRouteParam(this.route, 'key')
      .pipe(
        switchMap((key) => this.gameService.getGame(key)),
        tap((x) => {
          this.game = x;
          this.gameInfo.name = x.name;
          this.gameInfo.price = x.price?.toString();
          this.gameInfo.key = x.key;
          this.gameInfo.description = x.description;
          this.gameInfo.copyType = x.copyType;
          this.gameInfo.gameSize = x.gameSize;
          this.gameInfo.imageUrl = x.imageUrl;
          this.gameInfo.releasedDate = x.releasedDate;
          this.commentService.getCommentsByGameKey(x.key)
            .subscribe(comments => {
          this.commentCount = comments.length;
          this.gameInfo.commentCount = this.commentCount.toString();
      });
        }),
        switchMap((x) =>
          forkJoin({
            genres: this.genreService.getGenresByGameKey(x.key),
            platforms: this.platformService.getPlatformsByGameKey(x.key),
            file: this.gameService.getGameFile(x.key),
            image: this.gameService.getGameImage(x.key),
            publisher: this.publisherService.getPublisherByGameKey(x.key),
            canSeeComments: this.userService.checkAccess('Comments', x.key),
            canBuy: this.userService.checkAccess('Buy', x.key),
            canUpdate: this.userService.checkAccess('UpdateGame', x.key),
            canDelete: this.userService.checkAccess('DeleteGame', x.key),
          })
        )
      )
      .subscribe((x) => {
        this.addPlatformsInfo(x.platforms);
        this.addGenresInfo(x.genres);
        this.file = x.file;
        this.image = x.image;
        this.addDownloadFile();
        this.addPublisherInfo(x.publisher);
        this.canSeeComments = x.canSeeComments;
        this.canUpdate = x.canUpdate;
        this.canBuy = x.canBuy;
        this.canDelete = x.canDelete;
        this.addImage();
      });
  }

  ngAfterViewInit(): void {
    this.addDownloadFile();
  }

  addDownloadFile(): void {
    if (!!this.file && !!this.downloadLink) {
      const downloadURL = window.URL.createObjectURL(this.file);
      (this.downloadLink as any)._elementRef.nativeElement.href = downloadURL;
    }
  }

  addImage(): void {
    if (!!this.image) {
      const imageUrl = this.sanitizer.bypassSecurityTrustUrl(
        URL.createObjectURL(this.image)
      );
      this.imageUrl = imageUrl;
    }
    this.gameInfo.image = this.imageUrl;
  }

  addPlatformsInfo(platforms: Platform[]): void {
    if (!platforms?.length) {
      return;
    }
  
    this.gameInfo.platforms = platforms.map(platform => ({
      ...platform,
      pageLink: `${this.links.get(this.pageRoutes.Platform)}/${platform.id}`,
    }));
  }

  addPublisherInfo(publisher: Publisher): void {
    if (!publisher) {
      return;
    }

    this.gameInfo.publisher = {
      ...publisher,
      pageLink: !publisher?.id?.length
        ? undefined
        : `${this.links.get(this.pageRoutes.Publisher)}/${publisher.companyName}`,
    }
  }

  addGenresInfo(genres: Genre[]): void {
    if (!genres?.length) {
      return;
    }
  
    this.gameInfo.genres = genres.map(genre => ({
      ...genre,
      pageLink: `${this.links.get(this.pageRoutes.Genre)}/${genre.id}`,
    }));
  }

  buy(): void {
    this.orderService
      .buyGame(this.game!.key)
      .subscribe((_) =>
        this.router.navigateByUrl(this.links.get(this.pageRoutes.Basket)!)
      );
  }
}
