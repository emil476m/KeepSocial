import {Injectable} from "@angular/core";
import {PostModel} from "../../models/PostModel";

@Injectable({
  providedIn: 'root',
})
export class Globalstate
{
  ishuman = false;
  posts : PostModel[] = [];

  updatingWhatAccountItem: String|null = null;
}
