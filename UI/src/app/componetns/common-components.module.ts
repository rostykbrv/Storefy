import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from '../app-routing.module';
import { DeleteWrapperComponent } from './delete-wrapper-component/delete-wrapper.component';
import { FormComponent } from './form-component/form.component';
import { InfoComponent } from './info-component/info.component';
import { InfoWrapperComponent } from './info-wrapper-component/info-wrapper.component';
import { ListComponent } from './list-component/list.component';
import { ListItemComponent } from './list-item-component/list-item.component';
import { TextInputComponent } from './text-input-component/text-input.component';
import { MatDialogModule } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';
import { MatRadioModule } from '@angular/material/radio';
import { LoaderComponent } from './loader-component/loader.component';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatSelectModule } from '@angular/material/select';
import { TextareaInputComponent } from './textarea-input-component/textarea-input.component';
import { SelectorInputComponent } from './selector-input-component/selector-input.component';
import { CheckboxesInputComponent } from './checkboxes-input-component/checkboxes-input.component';
import { NumberInputComponent } from './number-input-component/number-input.component';
import { RadioInputComponent } from './radio-input-component/radio-input.component';
import { ListGameComponent } from './list-game-component/list-game.component';

@NgModule({
  declarations: [
    ListItemComponent,
    ListComponent,
    InfoWrapperComponent,
    InfoComponent,
    FormComponent,
    TextInputComponent,
    DeleteWrapperComponent,
    LoaderComponent,
    TextareaInputComponent,
    SelectorInputComponent,
    CheckboxesInputComponent,
    NumberInputComponent,
    RadioInputComponent,
    ListGameComponent
  ],
  imports: [
    CommonModule,
    AppRoutingModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatProgressSpinnerModule,
    MatButtonModule,
    MatInputModule,
    MatListModule,
    MatIconModule,
    MatSelectModule,
    MatCheckboxModule,
    MatRadioModule
  ],
  exports: [
    ListItemComponent,
    ListComponent,
    ListGameComponent,
    InfoWrapperComponent,
    InfoComponent,
    FormComponent,
    TextInputComponent,
    TextareaInputComponent,
    DeleteWrapperComponent,
    SelectorInputComponent,
    CheckboxesInputComponent,
    NumberInputComponent,
    RadioInputComponent
  ],
})
export class CommonComponentsModule {}
