import { Component, OnInit } from '@angular/core';
import {CrosspService} from '../services/crossp.service';
import {Router} from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  public username: string;
  public password: string;

  constructor(protected crosspService: CrosspService,
              protected router: Router) { }

  ngOnInit(): void {
  }

  public login(): void {
    this.crosspService.login(this.username, this.password).subscribe(result => {
      if (result) {
        this.router.navigate(['/home']);
      }
    });
  }
}
