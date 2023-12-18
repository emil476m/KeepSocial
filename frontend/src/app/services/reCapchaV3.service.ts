import {ReCaptchaV3Service} from "ngx-captcha";
import {Injectable} from "@angular/core";
import {Globalstate} from "./states/globalstate";
import {HttpClient} from "@angular/common/http";
import {environment} from "../../environments/environment";
import {firstValueFrom} from "rxjs";
import {ResponsdtoModel} from "../models/responsdto.model";
import {FormControl} from "@angular/forms";

@Injectable()

export class ReCapchaV3Service
{
  recaptcha = new FormControl({token: "",})

  constructor(private reCaptchaV3service: ReCaptchaV3Service, private state: Globalstate, private http: HttpClient)
  {}



  execute(skey: string)
  {
    this.reCaptchaV3service.execute(skey,'login', async (token) => {
        this.recaptcha.setValue({token: token});
        const call = this.http.post(environment.baseURL + "ishuman", this.recaptcha.value)
        const response = await firstValueFrom<ResponsdtoModel>(call);
        this.state.ishuman = response.responseData.ishuman;
    }, {useGlobalDomain: false})
  }
}
