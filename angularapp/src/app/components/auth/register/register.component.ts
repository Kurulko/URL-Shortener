import { Component, OnInit } from '@angular/core';import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl, Validators, AbstractControl, ValidationErrors, FormBuilder } from '@angular/forms';
import { AuthComponent } from '../auth.component';
import { Observable } from "rxjs";
import { MatSnackBar  } from '@angular/material/snack-bar';
import { AuthService } from 'src/app/services/auth.service';
import { TokenManager } from 'src/app/helpers/managers/token-manager';
import { RegisterModel } from 'src/app/models/auth/register.model';
import { AuthResult } from 'src/app/models/auth/auth-result.model';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent extends AuthComponent implements OnInit {
  maxBirthdayDate!: Date;

  constructor(
    router: Router, 
    route: ActivatedRoute, 
    authService: AuthService, 
    tokenManager: TokenManager,
    snackBar: MatSnackBar) 
  {
    super(router, route, authService, tokenManager, snackBar);
  }

  registerForm!: FormGroup;

  ngOnInit() {
    this.registerForm = new FormGroup({
      name: new FormControl(null, [ Validators.required, Validators.minLength(3)] ),
      password: new FormControl(null, [ Validators.required, Validators.minLength(8) ]),
      passwordConfirm: new FormControl(null, [ Validators.required, Validators.minLength(8) ]),
      rememberMe: new FormControl(false),
    }, { validators: this.passwordsMatchValidator });
  };

  passwordsMatchValidator(formGroup: AbstractControl): ValidationErrors | null {
    const password = formGroup.get('password')?.value;
    const passwordConfirm = formGroup.get('passwordConfirm')?.value;
   
    return (password && passwordConfirm && password === passwordConfirm) ? null : { notMatching: true };
  }
  
  getAuthResult() : Observable<AuthResult> {
    const registerModel: RegisterModel = this.registerForm.value;
    return this.authService.register(registerModel);
  }

  register(): void{
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