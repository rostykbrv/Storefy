import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { LoaderService } from '../componetns/loader-component/loader.service';
import { appConfiguration } from '../configuration/configuration-resolver';
import { User } from '../models/user.model';
import { Notification } from '../models/notification.model';
import { BaseService } from './base.service';
import { jwtDecode } from "jwt-decode";

@Injectable()
export class UserService extends BaseService {
  constructor(http: HttpClient, loaderService: LoaderService) {
    super(http, loaderService);
  }

  getUser(id: string): Observable<User> {
    return this.get<User>(
      appConfiguration.userApiUrl.replace(environment.routeIdIdentifier, id)
    );
  }

  getUsers(): Observable<User[]> {
    return this.get<User[]>(appConfiguration.usersApiUrl);
  }

  getNotifications(): Observable<Notification[]>{
    return this.get<Notification[]>(appConfiguration.getNotificationUsersApiUrl);
  }

  getUserNotification(id: string):Observable<Notification>{
    return this.get<Notification>(appConfiguration.getUserNotificationApiUrl
      .replace(environment.routeIdIdentifier, id));
  }

  addUser(user: User, roles: string[], password: string, notification:Notification): Observable<any> {
    return this.post(appConfiguration.addUserApiUrl, { user, roles, password, notification });
  }

  login(model: {
    login: string;
    password: string;
    internalAuth: boolean;
  }): Observable<{ token: string }> {
    return this.post<any, { token: string }>(appConfiguration.loginApiUrl, {
      model,
    }).pipe(
      tap((x) => {
        const token = x.token;
        this.clearAuth();
        if (!token?.length) {
          return;
        }

        localStorage.setItem('authKey', token);

        const decodedToken = jwtDecode(token);
        const role = (decodedToken as any)['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
        localStorage.setItem('userRole', role)
      })
    );
  }

  updateUser(user: User, roles: string[], password: string, notification: Notification): Observable<any> {
    return this.put(appConfiguration.updateUserApiUrl, {
      user,
      roles,
      notification,
      password,
    });
  }

  deleteUser(id: string): Observable<any> {
    return this.delete(
      appConfiguration.deleteUserApiUrl.replace(
        environment.routeIdIdentifier,
        id
      ),
      {}
    );
  }

  checkAccess(targetPage: string, targetId?: string): Observable<boolean> {
    return this.post(appConfiguration.checkAccessApiUrl, {
      targetPage,
      targetId,
    });
  }

  clearAuth(): void {
    localStorage.removeItem('authKey');
    localStorage.removeItem('userRole');
  }

  isAuth(): boolean {
    return !!localStorage.getItem('authKey')?.length;
  }
}
