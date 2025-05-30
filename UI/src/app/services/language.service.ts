import { Injectable } from '@angular/core';

@Injectable()
export class LanguageService {
  getSelectedLanguage(): string {
    return localStorage.getItem('locale') || 'en';
  }
}