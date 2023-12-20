import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {environment} from "../../environments/environment";

export interface Credentials {
  email: string;
  password: string;
}

@Injectable()
export class AccountService
{
  constructor(private readonly http: HttpClient) { }
  login(value: Credentials)
  {
    return this.http.post<{token: string}>(environment.baseURL+'account/login', value);
  }


}
