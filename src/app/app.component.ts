import { Component, ViewChild, ElementRef, OnInit, AfterViewInit } from '@angular/core';
import * as pbi from 'powerbi-client';
import { models } from 'powerbi-client';
import { HttpClient } from '@angular/common/http';
declare var powerbi: any;


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
 export class AppComponent implements OnInit  {

  constructor(private httpClient: HttpClient) { }

  @ViewChild('embeddedReport')
  embeddedReport!: ElementRef;
  config: any;
  screenHeight!: number;
  screenWidth!: number;

  ngOnInit() {
    this.screenHeight = (window.screen.height);
    this.screenWidth = (window.screen.width);    
    this.httpClient.get<any>("https://localhost:44337/EmbedInfo")
    .subscribe(config => {
      this.config = config;
      const model = pbi.models;
      const embedConfig = {
        type: 'report',
        tokenType: model.TokenType.Embed,
        accessToken: config.EmbedToken.Token,
        embedUrl: config.EmbedReport[0].EmbedUrl,
        permissions: model.Permissions.All,
        settings: {
          filterPaneEnabled: true,
          navContentPaneEnabled: true
        }
      };
      powerbi.embed(this.embeddedReport.nativeElement, embedConfig);
    });
  }
}