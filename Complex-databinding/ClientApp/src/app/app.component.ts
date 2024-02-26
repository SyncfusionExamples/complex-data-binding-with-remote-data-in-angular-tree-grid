import { Component } from '@angular/core';
import { DataManager, Query, UrlAdaptor } from '@syncfusion/ej2-data';

@Component({
  selector: 'app-root',
  template: `
  <ejs-treegrid [dataSource]='data' [query]="query" [treeColumnIndex]='1' height='400' idMapping='TaskID' parentIdMapping='ParentValue' hasChildMapping='isParent' [allowPaging]="true" [pageSettings]="pagesettings" >
                <e-columns>
                  <e-column field='TaskID' headerText='Task ID' width='90' textAlign='Right'></e-column>
                  <e-column field='TaskName' headerText='Task Name' width='180'></e-column>
                  <e-column field='Duration' headerText='Duration' width='80' textAlign='Right'></e-column>
                  <e-column field='Tasks.Name' headerText='Name' width='100' textAlign='Right'></e-column>
                  </e-columns>
               </ejs-treegrid>
 `,

})
export class AppComponent {
  public data: DataManager = new DataManager({
    adaptor: new UrlAdaptor,
    url: "Home/Datasource",
  });
  public query?: Query = new Query().expand('Tasks');
  public pagesettings = { pageSize: 12, pageSizeMode: 'Root' };
}
