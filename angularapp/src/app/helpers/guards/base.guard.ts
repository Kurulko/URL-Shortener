import { ActivatedRouteSnapshot, Router, RouterStateSnapshot, UrlTree } from "@angular/router";
import { Observable } from "rxjs";
import { TokenManager } from "../managers/token-manager";

export abstract class BaseGuard {
    constructor(protected tokenManager: TokenManager, protected router: Router) {
    }

    abstract canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree | Observable<boolean | UrlTree> | Promise<boolean | UrlTree>;
}