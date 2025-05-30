import { NgModule } from '@angular/core';
import { MainPageComponent } from './main-page.component';
import { CommonModule } from '@angular/common';
import { CommonComponentsModule } from 'src/app/componetns/common-components.module';

@NgModule({
  declarations: [MainPageComponent],
  imports: [CommonModule, CommonComponentsModule],
})
export class MainPageModule {}
