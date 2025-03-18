export class TokenResponseModel {
    constructor(
        public AccessToken: string,
        public RefreshToken: string
    ){}
}