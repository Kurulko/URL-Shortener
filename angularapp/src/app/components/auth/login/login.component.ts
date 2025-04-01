import { Component } from '@angular/core';
import { AuthComponent } from '../auth.component';
import { Observable } from "rxjs";
import { MatSnackBar  } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { LoginModel } from 'src/app/models/auth/login.model';
import { TokenManager } from 'src/app/helpers/managers/token-manager';
import { AuthResult } from 'src/app/models/auth/auth-result.model';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent extends AuthComponent {
  loginModel: LoginModel = <LoginModel>{};

  constructor(
    router: Router, 
    route: ActivatedRoute, 
    authService: AuthService, 
    tokenManager: TokenManager,
    snackBar: MatSnackBar) 
  {
    super(router, route, authService, tokenManager, snackBar);
  }
  
  getAuthResult() : Observable<AuthResult> {
    return this.authService.login(this.loginModel);
  }

  login(): void{
    this.pipeAuthResult()
    .subscribe((authResult: AuthResult) => {
      this.authResult = authResult;

      this.showSnackbar(authResult.message)
      this.tokenManager.setToken(authResult.token!);

      const redirectUrl = this.returnUrl || '/';
      this.router.navigateByUrl(redirectUrl);
    })
  }
}
