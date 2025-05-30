import { ListItem } from "src/app/componetns/list-item-component/list-item";
import { Genre } from "src/app/models/genre.model";
import { Platform } from "src/app/models/platform.model";
import { Publisher } from "src/app/models/publisher.model";
import { SafeUrl } from "@angular/platform-browser";

export class GameInfoItem {
  title?: string;
  key?: string;
  price?: string;
  description?: string;
  imageUrl?:string;
  image?: SafeUrl;
  copyType?:string;
  commentCount?:string;
  gameSize?:string;
  releasedDate?:string;
  platforms?: Platform[];
  publisher?: Publisher;
  genres?: Genre[];
  name?: string;
  value?: string;
  nestedValues?: ListItem[];
  pageLink?: string;
}
