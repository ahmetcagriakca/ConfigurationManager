import { Injectable } from '@angular/core';

import 'rxjs/Rx';
import 'rxjs/add/observable/throw';
import 'rxjs/add/operator/map';
import { HttpClient } from '@angular/common/http';
import { Subject } from 'rxjs';
import { StorageService } from './storage.service';
import { IConfiguration } from './configuration.model';


@Injectable()
export class ConfigurationService {
  serverSettings: IConfiguration = {
    baseUrl: ""
  };
  // observable that is fired when settings are loaded from server
  public isReady: boolean = false;

  private settingsLoadedSource = new Subject();
  settingsLoaded$ = this.settingsLoadedSource.asObservable();
  constructor(private http: HttpClient, private storageService: StorageService) { }

  load() {
    if (this.storageService.retrieve('baseUrl') !== '' && this.storageService.retrieve('baseUrl') !== undefined) {
      this.isReady = true;
      let serverConf: IConfiguration = { baseUrl: this.storageService.retrieve('baseUrl') };
      this.serverSettings = serverConf;
      this.settingsLoadedSource.next();
      this.getConfigurations();
    }
    else {
      this.getConfigurations();
    }
  }

  private getConfigurations() {
    const baseURI = document.baseURI.endsWith('/') ? document.baseURI : `${document.baseURI}/`;
    let url = `${baseURI}Home/Configuration`;
    this.http.get(url).subscribe((response: IConfiguration) => {
      console.log('server settings loaded');
      this.serverSettings = response;
      console.log(this.serverSettings);
      this.storageService.store('baseUrl', this.serverSettings.baseUrl);
      this.isReady = true;
      let serverConf: IConfiguration = { baseUrl: this.storageService.retrieve('baseUrl') };
      this.serverSettings = serverConf;
      this.settingsLoadedSource.next();
    });
  }
}
