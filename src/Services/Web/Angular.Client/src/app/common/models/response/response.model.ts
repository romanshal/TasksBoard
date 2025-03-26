export class Response {
    public IsError: boolean | undefined;
    public Description: string | undefined;
}

export class ResultResponse<T> extends Response {
    public Result: T | undefined;
}