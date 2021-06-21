
import { Component, ViewChild, ElementRef, OnInit, AfterViewInit } from '@angular/core';
import * as pbi from 'powerbi-client';
import { HttpClient } from '@angular/common/http';
declare var powerbi: any;

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  @ViewChild('embeddedReport', { static: false })
  embeddedReport: ElementRef;
  config: any;
  screenHeight: number;

  constructor(private httpClient: HttpClient) { }

  ngOnInit() {
    this.screenHeight = (window.screen.height);

    this.httpClient.get<any>("/powerbi/getembedinfo")
      .subscribe(config => {
        this.config = config;
        const model = window['powerbi-client'].models;
        const embedConfig = {
          type: 'report',
          tokenType: model.TokenType.Embed,
          accessToken: config.embedParams.EmbedToken.Token,
          embedUrl: config.embedParams.EmbedReport[0].EmbedUrl,
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
