import { Component } from '@angular/core';
import {CrosspService} from './services/crossp.service';
import {Router} from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'AngularApplication';

  constructor(public crosspService: CrosspService, protected router: Router
  ) {
  }

  public logout(): void {
    this.crosspService.logout();
    this.router.navigate(['/home']);
  }
}
