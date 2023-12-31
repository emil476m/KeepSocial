import {Injectable} from "@angular/core";
import {PostModel} from "../../models/PostModel";
import {CommentModel} from "../../models/CommentModel";

@Injectable({
  providedIn: 'root',
})
export class Globalstate
{
  ishuman = false;
  posts : PostModel[] = [];
  followedPosts: PostModel[] = [];
  currentPost : PostModel|any = {};
  currentComment : CommentModel|any = {}
  comments : CommentModel[] = [];
  updatingWhatAccountItem: String|null = null;
  currentUserName: string | null = null;
  profileId: number|null = null;
}
