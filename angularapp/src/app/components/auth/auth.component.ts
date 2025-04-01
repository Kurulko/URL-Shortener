import { ActivatedRoute, Router } from '@angular/router';
import { MatSnackBar  } from '@angular/material/snack-bar';
import { Observable } from 'rxjs';
import { AuthResult } from 'src/app/models/auth/auth-result.model';
import { TokenManager } from 'src/app/helpers/managers/token-manager';
import { AuthService } from 'src/app/services/auth.service';
import { BaseComponent } from 'src/app/base.component';

export abstract class AuthComponent extends BaseComponent {
  authResult?: AuthResult;
  returnUrl: string | null = null;

  constructor(protected router: Router, 
    route: ActivatedRoute, 
    protected authService: AuthService, 
    tokenManager: TokenManager,
    snackBar: MatSnackBar) 
  {
    super(tokenManager, snackBar);

    route.queryParams.subscribe(params => {
      this.returnUrl = params['returnUrl'] || null;
    });
  }

  abstract getAuthResult() : Observable<AuthResult>;

  pipeAuthResult() : Observable<AuthResult> {
    return this.getAuthResult().pipe(this.catchError());
  }    
}