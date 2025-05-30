import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { switchMap, tap } from 'rxjs/operators';
import { BaseComponent } from 'src/app/componetns/base.component';
import { InputValidator } from 'src/app/configuration/input-validator';
import { Game } from 'src/app/models/game.model';
import { Genre } from 'src/app/models/genre.model';
import { Platform } from 'src/app/models/platform.model';
import { Publisher } from 'src/app/models/publisher.model';
import { GameService } from 'src/app/services/game.service';
import { GenreService } from 'src/app/services/genre.service';
import { PlatformService } from 'src/app/services/platform.service';
import { PublisherService } from 'src/app/services/publisher.service';

@Component({
  selector: 'gamestore-update-game',
  templateUrl: './update-game-page.component.html',
  styleUrls: ['./update-game-page.component.scss'],
})
export class UpdateGamePageComponent extends BaseComponent implements OnInit {
  form?: FormGroup;
  genreItems: string[] = [];
  platformItems: string[] = [];
  gamePageLink?: string;

  game?: Game;

  genres: Genre[] = [];
  publishers: { name: string; value: string }[] = [{ name: '-', value: '' }];
  platforms: Platform[] = [];

  gameGenres: Genre[] = [];
  gamePublisher?: Publisher;
  gamePlatforms: Platform[] = [];

  constructor(
    private gameService: GameService,
    private genreService: GenreService,
    private platformService: PlatformService,
    private publisherService: PublisherService,
    private route: ActivatedRoute,
    private builder: FormBuilder,
    private router: Router,
    private cd: ChangeDetectorRef
  ) {
    super();
  }

  ngOnInit(): void {
    this.getRouteParam(this.route, 'key')
      .pipe(
        switchMap((key) =>
          !!key?.length ? this.gameService.getGame(key) : of(undefined)
        ),
        tap((x) => (this.game = x)),
        switchMap((x) =>
          forkJoin({
            gameGenres: !!x?.key?.length
              ? this.genreService.getGenresByGameKey(x.key)
              : of([]),
            gamePlatforms: !!x?.key?.length
              ? this.platformService.getPlatformsByGameKey(x.key)
              : of([]),
            gamePublisher: !!x?.key?.length
              ? this.publisherService.getPublisherByGameKey(x.key)
              : of(undefined),
            genres: this.genreService.getGenres(),
            platforms: this.platformService.getPlatforms(),
            publishers: this.publisherService.getPublishers(),
          })
        )
      )
      .subscribe((x) => {
        this.platforms = x.platforms;
        x.publishers.forEach((publisher) =>
          this.publishers.push({
            name: publisher.companyName,
            value: publisher.id ?? '',
          })
        );
        this.genres = x.genres;
        this.gameGenres = x.gameGenres;
        this.gamePlatforms = x.gamePlatforms;
        this.gamePublisher = x.gamePublisher;
        this.createForm();
      });
  }

  getFormControl(name: string): FormControl {
    return this.form?.get(name) as FormControl;
  }

  getFormControlArray(name: string): FormControl[] {
    return (this.form?.get(name) as FormArray).controls.map(
      (x) => x as FormControl
    );
  }

  onImage(event: any): void {
    const reader = new FileReader();

    if (event.target.files && event.target.files.length && this.form) {
      const [file] = event.target.files;
      reader.readAsDataURL(file);

      reader.onload = () => {
        this.form!.patchValue({
          image: reader.result,
        });

        this.cd.markForCheck();
      };
    }
  }

  onSave(): void {
    const game: Game = {
      id: this.form!.value.id,
      name: this.form!.value.name,
      description: this.form!.value.description,
      key: this.form!.value.key,
      unitInStock: this.form!.value.unitInStock,
      price: this.form!.value.price,
      discontinued: this.form!.value.discontinued,
      imageUrl: this.form!.value.imageUrl,
      copyType: this.form!.value.copyType,
      releasedDate: this.form!.value.releasedDate,
      gameSize: this.form!.value.gameSize,
    };

    const selectedGenres = this.genres
      .filter((x, i) => !!this.form!.value.genres[i])
      .map((x) => x.id ?? '');
    const selectedPlatforms = this.platforms
      .filter((x, i) => !!this.form!.value.platforms[i])
      .map((x) => x.id ?? '');

    const selectedPublisher = this.form!.value.publisher;

    (!!game.id
      ? this.gameService.updateGame(
          game,
          selectedGenres,
          selectedPlatforms,
          selectedPublisher,
          this.form!.value.image
        )
      : this.gameService.addGame(
          game,
          selectedGenres,
          selectedPlatforms,
          selectedPublisher,
          this.form!.value.image
        )
    ).subscribe((_) =>
      this.router.navigateByUrl(
        !!game.id
          ? this.links.get(this.pageRoutes.Game) + `/${game.key}`
          : this.links.get(this.pageRoutes.Games) ?? ''
      )
    );
  }

  private createForm(): void {
    this.gamePageLink = !!this.game
      ? `${this.links.get(this.pageRoutes.Game)}/${this.game.key}`
      : undefined;

    this.form = this.builder.group({
      id: [this.game?.id ?? ''],
      name: [this.game?.name ?? '', Validators.required],
      key: [this.game?.key ?? ''],
      description: [this.game?.description ?? ''],
      unitInStock: [
        this.game?.unitInStock ?? '',
        [Validators.required, InputValidator.getNumberValidator()],
      ],
      price: [
        this.game?.price ?? '',
        [Validators.required, InputValidator.getNumberValidator()],
      ],
      discontinued: [
        this.game?.discontinued ?? '',
        [Validators.required, InputValidator.getNumberValidator()],
      ],
      image: [null],
      imageUrl: [this.game?.imageUrl ?? '', Validators.required],
      copyType: [this.game?.copyType ?? '', Validators.required],
      releasedDate: [
        this.game?.releasedDate ?? '',
        [Validators.required, InputValidator.getNumberValidator()],
      ],
      gameSize: [
        this.game?.gameSize ?? '',
        [Validators.required],
      ],
      publisher: [this.gamePublisher?.id ?? ''],
      genres: this.builder.array(
        this.genres.map((x) => this.gameGenres.some((z) => z.id === x.id))
      ),
      platforms: this.builder.array(
        this.platforms.map((x) => this.gamePlatforms.some((z) => z.id === x.id))
      ),
    });

    this.genreItems = this.genres.map((x) => x.name);
    this.platformItems = this.platforms.map((x) => x.type);
  }
}
