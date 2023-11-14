import {Injectable} from "@angular/core";
import {HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from "@angular/common/http";
import {catchError, Observable} from "rxjs";
import {ToastController} from "@ionic/angular";

@Injectable()
export class ErrorHttpInterceptor implements HttpInterceptor {

  constructor(private readonly toast: ToastController) {
  }

  private async showError(message: string) {
    return (await this.toast.create({
      message: message,
      duration: 5000,
      color: 'danger'
    })).present()
  }
  intercept(req: HttpRequest<any>, next: HttpHandler):
    Observable<HttpEvent<any>> {
    return next.handle(req).pipe(catchError(async e => {
      if (e instanceof HttpErrorResponse) {
        this.showError(e.statusText);
      }
      throw e;
    }));
  }
}
