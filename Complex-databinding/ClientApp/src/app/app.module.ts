import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { GridModule } from '@syncfusion/ej2-angular-grids';
import { TreeGridModule, PageService } from '@syncfusion/ej2-angular-treegrid';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    GridModule, TreeGridModule 
  ],
  providers: [PageService],
  bootstrap: [AppComponent]
})
export class AppModule { }
