import { ActivatedRouteSnapshot, CanActivate, CanActivateChild, GuardResult, MaybeAsync, Router, RouterStateSnapshot } from "@angular/router";
import { Injectable } from "@angular/core";
import { AuthStateService } from "../../common/services/auth-state/auth-state.service";

@Injectable({
    providedIn: 'root'
})
export class AuthGuard implements CanActivate, CanActivateChild {
    private isAuthenticated = false;

    constructor(
        private authStateService: AuthStateService,
        private router: Router
    ) {
        this.authStateService.isAuthenticated$.subscribe(status => {
            this.isAuthenticated = status;
        });
    }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): MaybeAsync<GuardResult> {
        if (this.isAuthenticated) {
            return true;
        } else {
            const currentUrl = state.url;
            this.router.navigate(['/signin'], { queryParams: { returnurl: currentUrl } });

            return false;
        }
    }

    canActivateChild(childRoute: ActivatedRouteSnapshot, state: RouterStateSnapshot): MaybeAsync<GuardResult> {
        return this.canActivate(childRoute, state);
    }
}