import {Injectable} from "@angular/core";

@Injectable({
  providedIn: 'root',
})
export class Globalstate
{
  ishuman = false;

  updatingWhatAccountItem: String|null = null;
}
