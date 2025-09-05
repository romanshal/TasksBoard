import { catchError, map, Observable, throwError } from "rxjs";

export class Response {
    public isError: boolean | undefined;
    public description: string | undefined;
}

export class ResultResponse<T> extends Response {
    public result: T | undefined;
}

type ApiResponse<T> = Response | ResultResponse<T>;

export function unwrapResponse<T>() {
  return (source: Observable<ApiResponse<T>>): Observable<T | void> =>
    source.pipe(
      map((res: ApiResponse<T>) => {
        if (res.isError) {
          throw new Error(res.description || 'Unknown server error');
        }

        // Если это ResultResponse<T> — вернём Result
        if ('result' in res) {
          return (res as ResultResponse<T>).result as T;
        }

        // Если это просто Response — вернём undefined
        return undefined;
      }),
      catchError(err => {
        return throwError(() =>
          err instanceof Error ? err : new Error('Unexpected error')
        );
      })
    );
}