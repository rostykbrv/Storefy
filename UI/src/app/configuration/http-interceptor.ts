import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { LoaderService } from '../componetns/loader-component/loader.service';

export const apiErrorTitle = 'Error during API call!';

@Injectable()
export class GlobalHttpInterceptorService implements HttpInterceptor {
  constructor(
    private toastr: ToastrService,
    private loaderService: LoaderService
  ) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const authKey = localStorage.getItem('authKey');
    if (!!authKey?.length) {
      request = request.clone({
        setHeaders: {
          Authorization: 'Bearer ' + authKey,
          'Content-Type': 'application/json',
        },
        url: request.url,
      });
    }

    return next
      .handle(request)
      .pipe(catchError((error) => this.handleError(error)));
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    this.loaderService.closeLoader();
    if (error.status === 0) {
      this.toastr.error('API is unavailable', apiErrorTitle);
    } else {
      this.toastr.error(
        `API '${error.url}' returned code ${error.status}${
          !!error.error ? `, error body was: '${error.error}'` : ''
        }`,
        apiErrorTitle
      );
    }

    throw new Error(apiErrorTitle);
  }
}
