import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/auth/login/login.component';
import { RegisterComponent } from './components/auth/register/register.component';
import { AuthGuard } from './helpers/guards/auth.guard';
import { AdminGuard } from './helpers/guards/admin.guard';
import { LogoutComponent } from './components/auth/logout.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { ShortUrlsComponent } from './components/short-urls/short-urls.component';
import { ShortUrlInfoComponent } from './components/short-urls/short-url-info.component';

const routes: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'logout', component: LogoutComponent, canActivate: [AuthGuard] },
    { path: '', component: ShortUrlsComponent },
    { path: 'short-urls', component: ShortUrlsComponent },
    { path: 'short-url/:id/info', component: ShortUrlInfoComponent, canActivate: [AuthGuard] },
    { path: '**', component: NotFoundComponent },
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule {
}