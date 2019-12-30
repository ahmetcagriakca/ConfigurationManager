import { Component } from '@angular/core';
import { ConfigurationService } from './shared/configuration.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'app';
  constructor( private configurationService: ConfigurationService) {
  }

  ngOnInit() {

	  //Get configuration from server environment variables:
	  console.log('configuration');
	  this.configurationService.load();
  }
}
