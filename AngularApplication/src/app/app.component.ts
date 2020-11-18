import { Component } from '@angular/core';
import {CrosspService} from './services/crossp.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'AngularApplication';

  constructor(public crosspService: CrosspService
  ) {

  }

  public logout(): void {
    this.crosspService.logout();
  }
}
