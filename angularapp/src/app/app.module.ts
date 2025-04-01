import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing.module';
import { AngularMaterialModule } from './angular-material.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AuthInterceptor } from './helpers/auth.interceptor';
import { ClipboardModule } from '@angular/cdk/clipboard';

import { AppComponent } from './app.component';
import { LoginComponent } from './components/auth/login/login.component';
import { RegisterComponent } from './components/auth/register/register.component';
import { LogoutComponent } from './components/auth/logout.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { ShortUrlsComponent } from './components/short-urls/short-urls.component';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { ShortUrlInfoComponent } from './components/short-urls/short-url-info.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    NotFoundComponent,
    LogoutComponent,
    NavMenuComponent,
    ShortUrlsComponent,
    ShortUrlInfoComponent
  ],
  imports: [
    BrowserModule, 
    HttpClientModule, 
    ReactiveFormsModule,
    FormsModule,
    BrowserAnimationsModule,
    AngularMaterialModule,
    AppRoutingModule,
    ClipboardModule
  ],
  providers: [
    { 
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true 
    },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
