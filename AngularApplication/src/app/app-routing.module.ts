import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {HomeComponent} from './home/home.component';
import {ProjectComponent} from './project/project.component';
import {PageNotFoundComponent} from './page-not-found/page-not-found.component';
import {AdComponent} from './ad/ad.component';
import {LoginComponent} from './login/login.component';
import {AuthGuard} from './auth.guard';
import {StorageComponent} from './storage/storage.component';

const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'home', component: HomeComponent, canActivate: [AuthGuard] },
  { path: 'project/:id', component: ProjectComponent, canActivate: [AuthGuard] },
  { path: 'ad/:id', component: AdComponent, canActivate: [AuthGuard] },
  { path: 'storage', component: StorageComponent, canActivate: [AuthGuard] },
  { path: '**', component: PageNotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
  providers: [AuthGuard]
})
export class AppRoutingModule { }
