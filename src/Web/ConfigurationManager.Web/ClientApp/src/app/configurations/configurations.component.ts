import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ConfigurationService } from '../shared/configuration.service';

@Component({
	selector: 'configurations-component',
	templateUrl: './configurations.component.html'
})
export class ConfigurationsComponent {
	public configurations: ApplicationConfiguration[];
	baseUrl: string;
	searchText: string;

	constructor(public http: HttpClient, public _configurationService: ConfigurationService) {
		if (this._configurationService.isReady) {
			this.baseUrl = this._configurationService.serverSettings.baseUrl;
		}
		else {
			this._configurationService.settingsLoaded$.subscribe(x => {
				this.baseUrl = this._configurationService.serverSettings.baseUrl
				if (this.baseUrl) {
					this.loadData()
				}
			});
		}
		if (this.baseUrl) {
			this.loadData()
		}
	}
	loadData() {
		this.addValue = null;
		this.http.get(this.baseUrl + 'api/ConfigurationManagement/').subscribe(result => {
			this.configurations = (<any>result).resultValue;
		}, error =>{ 
			console.error(error)
			alert(error.error.error)
		});
	}

	public enableEdit = true;
	public enableAdd = true;

	addValue: ApplicationConfiguration;
	showcreate: boolean = false;
	delete(item) {
		this.enableAdd = true;
		this.enableEdit = true;
		if (confirm("Are you sure you want to delete this item?") == true) {
			var url = this.baseUrl + 'api/ConfigurationManagement/DeleteConfiguration?id=' + item.id ;
			this.http.delete(url).subscribe(result => {
				this.loadData();
			}, error =>{ 
				console.error(error)
				alert(error.error.error)
			});
		}
	}
	add() {
		if (!this.addValue) {
			this.addValue = {
				id: 0,
				applicationName: "",
				name: "",
				value: "",
				type: "",
				isActive: true
			}
			this.configurations.unshift(this.addValue);
			this.enableAdd = false;
			this.enableEdit = true;
		}
	}
	update(item) {
		this.enableAdd = true;
		this.enableEdit = false;
	}
	save(item) {
		// console.log("save",i)
		// this.indexVal = i;
		if (item.id == 0) {
			var url = this.baseUrl + 'api/ConfigurationManagement/AddConfiguration?name=' + item.name + '&value=' + item.value + '&type=' + item.type + '&applicationName=' + item.applicationName;
			this.http.post(url, null).subscribe(result => {
				this.loadData();
				this.enableAdd = true;
				this.enableEdit = true;
			}, error =>{ 
				console.error(error)
				alert(error.error.error)
			});
		}
		else {
			var url = this.baseUrl + 'api/ConfigurationManagement/UpdateConfiguration?id=' + item.id + '&name=' + item.name + '&value=' + item.value + '&type=' + item.type + '&applicationName=' + item.applicationName+'&isActive='+item.isActive;//(item.isActive==true?"true":"false");
			this.http.put(url, null).subscribe(result => {
				this.loadData();
				this.enableAdd = true;
				this.enableEdit = true;
			}, error =>{ 
				console.error(error)
				alert(error.error.error)
			});
		}

	}
	cancel() {
		this.loadData();
		this.enableAdd = true;
		this.enableEdit = true;
	}
}

interface ApplicationConfiguration {
	id: number;
	name: string;
	type: string;
	value: string;
	applicationName: string;
	isActive: boolean;
}
