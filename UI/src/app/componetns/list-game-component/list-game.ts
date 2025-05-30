import { Platform } from "src/app/models/platform.model";
import { SafeUrl } from "@angular/platform-browser";

export class ListGame{
    title!: string;
    imageUrl?: string;
    gameType?: string;
    pageLink?: string;
    updateLink?: string;
    deleteLink?: string;
    price?: number;
    image?: SafeUrl;
    description?: string;
    platform?: Platform[];
}