import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { ConfigurationService } from './shared/configuration.service';
import { StorageService } from './shared/storage.service';
import { ConfigurationsComponent } from './configurations/configurations.component';
import { HashLocationStrategy, LocationStrategy } from '@angular/common';
import { SearchFilterPipe } from './shared/searchfilter';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    ConfigurationsComponent,
	SearchFilterPipe,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: ConfigurationsComponent, pathMatch: 'full' },
      { path: 'configurations', component: ConfigurationsComponent },
    ])
  ],
  providers: [
	  ConfigurationService,
	  StorageService,
	  { provide: LocationStrategy, useClass: HashLocationStrategy },
	],
  bootstrap: [AppComponent]
})
export class AppModule { }
