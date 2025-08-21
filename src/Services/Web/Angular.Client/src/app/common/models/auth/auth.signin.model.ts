export class SigninRequestModel {
    constructor(
        public Username:string,
        public Password: string,
        public DeviceId: string
    ) { }
}