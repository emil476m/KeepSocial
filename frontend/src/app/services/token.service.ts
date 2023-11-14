import { Injectable } from "@angular/core";

@Injectable()
export class TokenService {
  private readonly storage: Storage = window.sessionStorage;

  setToken(token: string) {
    this.storage.setItem("token", token);
  }

  getToken() {
    return this.storage.getItem("token");
  }

  clearToken() {
    this.storage.removeItem("token");
  }
}
