import { Component } from '@angular/core';
import { BaseComponent } from 'src/app/componetns/base.component';
import { ListItem } from 'src/app/componetns/list-item-component/list-item';

@Component({
  selector: 'gamestore-main',
  templateUrl: './main-page.component.html',
  styleUrls: ['./main-page.component.scss'],
})
export class MainPageComponent extends BaseComponent {
  mainListItems: ListItem[] = [
    {
      title: this.labels.gamesMenuItem,
      pageLink: this.links.get(this.pageRoutes.Games)
    },
    {
      title: this.labels.genresMenuItem,
      pageLink: this.links.get(this.pageRoutes.Genres)
    },
    {
      title: this.labels.platformsMenuItem,
      pageLink: this.links.get(this.pageRoutes.Platforms)
    },
    {
      title: this.labels.publishersMenuItem,
      pageLink: this.links.get(this.pageRoutes.Publishers)
    },
    {
      title: this.labels.historyMenuItem,
      pageLink: this.links.get(this.pageRoutes.History)
    },
    {
      title: this.labels.usersMenuItem,
      pageLink: this.links.get(this.pageRoutes.Users)
    },
    {
      title: this.labels.rolesMenuItem,
      pageLink: this.links.get(this.pageRoutes.Roles)
    },
    {
      title: this.labels.ordersMenuItem,
      pageLink: this.links.get(this.pageRoutes.Orders)
    }
  ] 
}
