import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { BaseComponent } from 'src/app/base.component';
import { TokenManager } from 'src/app/helpers/managers/token-manager';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent extends BaseComponent {
  constructor(
    private authService: AuthService, 
    private router: Router,
    tokenManager: TokenManager,
    snackBar: MatSnackBar) 
  {
    super(tokenManager, snackBar);
  }

  onLogout(): void {
    this.authService.logout();
    this.tokenManager.logout();
    this.router.navigate(["/login"]);
  }
}