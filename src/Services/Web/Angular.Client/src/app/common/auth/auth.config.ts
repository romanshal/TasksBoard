import { AuthConfig } from "angular-oauth2-oidc";

export const authConfig: AuthConfig = {
    issuer: 'https://accounts.google.com',
    redirectUri: window.location.origin,
    clientId: '',
    responseType: 'token',
    scope: 'openid profile email',
    showDebugInformation: true,
}