import { Component, OnInit } from '@angular/core';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { gameCountSubject } from 'src/app/configuration/shared-info';
import { UserService } from 'src/app/services/user.service';
import { BaseComponent } from '../base.component';

@UntilDestroy()
@Component({
  selector: 'gamestore-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
})
export class HeaderComponent extends BaseComponent implements OnInit {
  gameCount: string | null = null;
  dropdownOpen = false; 

  locales = [
    { name: 'En', value: 'en' },
    { name: 'Ua', value: 'ua' },
  ];

  control = new FormControl(localStorage.getItem('locale') ?? 'en');

  constructor(private userService: UserService, private router: Router) {
    super();
  }

  toggleDropdown() {
    this.dropdownOpen = !this.dropdownOpen; 
  }

  get isAuth(): boolean {
    return this.userService.isAuth();
  }

  ngOnInit(): void {
    gameCountSubject
      .pipe(untilDestroyed(this))
      .subscribe((x) => (this.gameCount = x));

      this.control.valueChanges.pipe(untilDestroyed(this)).subscribe((x) => {
        localStorage.setItem('locale', x.toString());
        this.router.navigateByUrl('').then(() => window.location.reload());
  });
  }
}
