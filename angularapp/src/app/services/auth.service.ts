import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { map } from 'rxjs/operators';
import { RegisterModel } from '../models/auth/register.model';
import { AuthResult } from '../models/auth/auth-result.model';
import { AuthModel } from '../models/auth/auth.model';
import { LoginModel } from '../models/auth/login.model';
import { TokenViewModel } from '../models/helpers/token.view-model';
import { TokenModel } from '../models/helpers/token.model';
import { BaseService } from './base.service';

@Injectable({
    providedIn: 'root'
})
export class AuthService extends BaseService {
    constructor(httpClient: HttpClient) {
        super(httpClient, 'auth');
    }
    
    private account(path:string, authModel: AuthModel): Observable<AuthResult> {
        var observableAuthViewResult = this.webClient.post<AuthViewResult>(path, authModel);
        return observableAuthViewResult
            .pipe(map((authViewResult:AuthViewResult) => toAuthResult(authViewResult)));
    }

    login(loginModel: LoginModel): Observable<AuthResult> {
        return this.account('login', loginModel);
    }

    register(registerModel: RegisterModel): Observable<AuthResult> {
        return this.account('register', registerModel);
    }

    token(): Observable<TokenModel> {
        var observableTokenViewModel =  this.webClient.get<TokenViewModel>('token');
        return observableTokenViewModel
            .pipe(map((tokenViewModel:TokenViewModel) => toTokenModel(tokenViewModel)!))
    }

    logout(): Observable<Object> {
        return this.webClient.post('logout');
    }
}

interface AuthViewResult {
    success: boolean;
    message: string;
    token: TokenViewModel|null;
}

function toAuthResult(authViewResult: AuthViewResult) : AuthResult {
    const authResult = <AuthResult>{};

    authResult.success = authViewResult.success;
    authResult.message = authViewResult.message;
    authResult.token = toTokenModel(authViewResult.token);

    return authResult;
}

function toTokenModel(tokenViewModel: TokenViewModel|null) : TokenModel|null {
    if(tokenViewModel == null)
        return null;
    
    const tokenModel = <TokenModel>{};

    tokenModel.tokenStr = tokenViewModel.tokenStr;
    tokenModel.roles = tokenViewModel.roles;

    const today = new Date();
    today.setDate(today.getDate() + tokenViewModel.expirationDays);
    tokenModel.expirationDate = today;

    return tokenModel;
}