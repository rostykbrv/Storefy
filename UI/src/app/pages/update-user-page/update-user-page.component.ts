import { Component, OnInit } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { BaseComponent } from 'src/app/componetns/base.component';
import { Role } from 'src/app/models/role.model';
import { User } from 'src/app/models/user.model';
import { Notification } from 'src/app/models/notification.model';
import { RoleService } from 'src/app/services/role.service';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'gamestore-update-user',
  templateUrl: './update-user-page.component.html',
  styleUrls: ['./update-user-page.component.scss'],
})
export class UpdateUserPageComponent extends BaseComponent implements OnInit {
  form?: FormGroup;
  userPageLink?: string;
  roleItems: string[] = [];

  roles: Role[] = [];
  notifications: { name: string; value: string }[] = [{ name: '-', value: '' }];

  userNotification?: Notification;

  userRoles: Role[] = [];

  constructor(
    private userService: UserService,
    private roleService: RoleService,
    private route: ActivatedRoute,
    private builder: FormBuilder,
    private router: Router
  ) {
    super();
  }

  getFormControlArray(name: string): FormControl[] {
    return (this.form?.get(name) as FormArray).controls.map(
      (x) => x as FormControl
    );
  }

  ngOnInit(): void {
    this.getRouteParam(this.route, 'id')
      .pipe(
        switchMap((id) =>
          !!id?.length ? this.userService.getUser(id) : of(undefined)
        )
      )
      .pipe(
        switchMap((x) =>
          forkJoin({
            roles: this.roleService.getRoles(),
            userNotification: !!x?.id?.length
            ? this.userService.getUserNotification(x.id)
            : of(undefined),
            user: of(x),
            userRoles: !!x?.id?.length
              ? this.roleService.getUserRoles(x.id)
              : of([]),
            notifications: this.userService.getNotifications(),
          })
        )
      )
      .subscribe((x) => {
        this.roles = x.roles;
        x.notifications.forEach((notification) =>
        this.notifications.push({
          name: notification.type,
          value: notification.id ?? '',
        })
      );
        this.userRoles = x.userRoles;
        this.userNotification = x.userNotification;

        this.createForm(x.user);
      });
  }

  getFormControl(name: string): FormControl {
    return this.form?.get(name) as FormControl;
  }

  onSave(): void {
    const user: User = {
      id: this.form!.value.id,
      name: this.form!.value.name,
    };
    const selectedNotification = this.form!.value.notification;
    const selectedRoles = this.roles
      .filter((x, i) => !!this.form!.value.roles[i])
      .map((x) => x.id ?? '');

    (!!user.id
      ? this.userService.updateUser(user, selectedRoles, this.form!.value.password, selectedNotification)
      : this.userService.addUser(user, selectedRoles, this.form!.value.password, selectedNotification)
    ).subscribe((_) =>
      this.router.navigateByUrl(
        !!user.id
          ? this.links.get(this.pageRoutes.User) + `/${user.id}`
          : this.links.get(this.pageRoutes.Users) ?? ''
      )
    );
  }

  private createForm(user?: User): void {
    this.userPageLink = !!user
      ? `${this.links.get(this.pageRoutes.User)}/${user.id}`
      : undefined;

    this.form = this.builder.group({
      id: [user?.id ?? ''],
      name: [user?.name ?? '', Validators.required],
      password: ['', Validators.required],
      notification: [this.userNotification?.id ?? ''],
      roles: this.builder.array(
        this.roles.map((x) => this.userRoles.some((z) => z.id === x.id))
      ),
    });

    this.roleItems = this.roles.map((x) => x.name);
  }
}
