export class SignupRequestModel {
    constructor(
        public Username:string,
        public Email:string,
        public Password: string,
        public DeviceId: string
    ) { }
}