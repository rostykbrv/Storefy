import { Component, OnInit } from '@angular/core';
import { forkJoin, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import { BaseComponent } from 'src/app/componetns/base.component';
import { Game } from 'src/app/models/game.model';
import { OrderDetail } from 'src/app/models/order-detail.model';
import { GameService } from 'src/app/services/game.service';
import { OrderService } from 'src/app/services/order.service';

@Component({
  selector: 'gamestore-basket',
  templateUrl: './basket-page.component.html',
  styleUrls: ['./basket-page.component.scss'],
})
export class BasketPageComponent extends BaseComponent implements OnInit {
  orderInfoList?: { game: Game; details: OrderDetail; gamePageLink: string }[];
  subTotal: number = 0;
  taxes: number = 0;
  totalPrice: number = 0;

  constructor(
    private orderService: OrderService,
    private gameService: GameService
  ) {
    super();
  }

  ngOnInit(): void {
    this.refreshBasketItems();
  }

  refreshBasketItems(): void {
    this.orderService
      .getBasket()
      .pipe(
        switchMap((x) =>
          forkJoin(
            !!x.length
              ? x.map((z) =>
                  this.gameService.getGameById(z.productId).pipe(
                    map((y) => {
                      return { orderDetail: z, game: y };
                    })
                  )
                )
              : of([])
          )
        )
      )
      .subscribe((x) => {
        this.addDetailsInfo(x);

        this.subTotal = this.orderInfoList?.reduce((total, item) => total + ((item?.details?.price ?? 0) * (item?.details?.quantity ?? 0)), 0) || 0;
        this.taxes = this.subTotal * 0.025;
        this.totalPrice = this.taxes + this.subTotal;
      });
  }

  addDetailsInfo(details: { orderDetail: OrderDetail; game: Game }[]): void {
    this.orderInfoList = [];
    if (!details?.length) {
      return;
    }

    details.forEach((x) => {
      if(!x?.game?.key){
        return;
      }
      this.orderInfoList?.push({
        game: x.game,
        details: x.orderDetail,
        gamePageLink: `${this.links.get(this.pageRoutes.Game)}/${x.game.key}`,
      });
    });
  }

  cancelBuy(gameKey: string): void {
    this.orderService
      .cancelGameBuy(gameKey)
      .subscribe((_) => this.refreshBasketItems());
  }
}
